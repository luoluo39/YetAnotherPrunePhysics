using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace YetAnotherPrunePhysics
{
    public class CollisionManagerOverride : CollisionManager
    {
        private static readonly PropertyInfo instanceProperty = typeof(CollisionManager).GetProperty(nameof(Instance));

        private static readonly FieldInfo requireUpdateField =
            typeof(CollisionManager).GetField("requireUpdate", BindingFlags.Instance | BindingFlags.NonPublic);

        private bool RequireUpdate
        {
            get => (bool)requireUpdateField.GetValue(this);
            set => requireUpdateField.SetValue(this, value);
        }

        //private Queue<(Collider, Collider, bool)> collisionQueue = new Queue<(Collider, Collider, bool)>();
        private readonly int maxUpdatePerFrame = 10000;
        private Coroutine currentCoroutine;

        private static void SetInstance(CollisionManager value)
        {
            instanceProperty.SetValue(null, value);
        }

        private void Awake()
        {
            if (Instance && !(Instance is CollisionManagerOverride))
            {
                Instance.enabled = false;
                Destroy(Instance);
            }
            SetInstance(this);
            //StartCoroutine(SetCollisionCoroutine());
        }

        // Token: 0x06004094 RID: 16532 RVA: 0x0002CF14 File Offset: 0x0002B114
        private void FixedUpdate()
        {
            if (FlightGlobals.ready && RequireUpdate)
            {
                if (currentCoroutine != null)
                    StopCoroutine(currentCoroutine);


                //collisionQueue.Clear();
                currentCoroutine = StartCoroutine(UpdatePartCollisionIgnores());
                RequireUpdate = false;
            }
        }

        //private IEnumerator SetCollisionCoroutine()
        //{
        //    while (this)
        //    {
        //        for (int i = Math.Min(maxUpdatePerFrame, collisionQueue.Count); i >= 0; i--)
        //        {
        //            var group = collisionQueue.Dequeue();
        //            Physics.IgnoreCollision(group.Item1, group.Item2, group.Item3);
        //        }
        //        yield return null;
        //    }
        //}

        private List<VesselColliderList> GetAllVesselColliders()
        {
            var list = vesselsList;
            list.Clear();
            var hasEva = false;
            var count = FlightGlobals.VesselsLoaded.Count;
            while (count-- > 0)
            {
                if (FlightGlobals.VesselsLoaded[count].isEVA)
                {
                    hasEva = true;
                    break;
                }
            }

            var i = 0;
            var count2 = FlightGlobals.Vessels.Count;
            while (i < count2)
            {
                var vessel = FlightGlobals.Vessels[i];
                var vesselColliderList = new VesselColliderList(vessel.id);
                var j = 0;
                var count3 = vessel.parts.Count;
                while (j < count3)
                {
                    var list2 = new List<Collider>();
                    var part = vessel.parts[j];
                    if (part.physicalSignificance != Part.PhysicalSignificance.NONE)
                    {
                        var partColliderList = new PartColliderList(
                            part.persistentId,
                            part.sameVesselCollision,
                            part.physicalSignificance == Part.PhysicalSignificance.NONE,
                            part.rb);
                        var componentsInChildren = part.partTransform.GetComponentsInChildren<Collider>(hasEva);
                        if (componentsInChildren != null)
                        {
                            var num = componentsInChildren.Length;
                            for (var k = 0; k < num; k++)
                            {
                                var collider = componentsInChildren[k];

                                if (!collider.gameObject.activeInHierarchy)
                                {
                                    if (collider.CompareTag("Ladder") || collider.CompareTag("Airlock"))
                                    {
                                        list2.Add(collider);
                                    }
                                }
                                else if (collider.enabled)
                                {
                                    list2.Add(collider);
                                }
                                else if (collider.CompareTag("Ladder") || collider.CompareTag("Airlock"))
                                {
                                    list2.Add(collider);
                                }
                            }
                        }
                        partColliderList.colliders = list2;
                        vesselColliderList.colliderList.Add(partColliderList);
                    }
                    j++;
                }
                list.Add(vesselColliderList);
                i++;
            }
            return list;
        }

        private IEnumerator UpdatePartCollisionIgnores()
        {
            var allVesselColliders = GetAllVesselColliders();
            var count = allVesselColliders.Count;
            var counter = maxUpdatePerFrame;
            var performanceCounter = 0UL;
            for (int i = 0; i < count; i++)
            {
                var count2 = allVesselColliders.Count;
                for (int j = i; j < count2; j++)
                {
                    var colliderList = allVesselColliders[i].colliderList;
                    var colliderList2 = allVesselColliders[j].colliderList;
                    var sameVessel = i == j;
                    var count3 = colliderList.Count;
                    for (int k = 0; k < count3; k++)
                    {
                        int count4 = colliderList2.Count;
                        for (int l = (sameVessel ? k + 1 : 0); l < count4; l++)
                        {
                            int count5 = colliderList[k].colliders.Count;
                            for (int m = 0; m < count5; m++)
                            {
                                bool ignore = sameVessel;
                                if (ignore
                                    && colliderList2[l].sameVesselCollision
                                    && colliderList[k].sameVesselCollision)
                                {
                                    ignore = false;
                                }

                                ignore |= colliderList[k].rbId == colliderList2[l].rbId || (colliderList[k].isPhysicsless && colliderList2[l].isPhysicsless);

                                var collider = colliderList[k].colliders[m];
                                int count6 = colliderList2[l].colliders.Count;
                                for (int n = 0; n < count6; n++)
                                {
                                    if (counter == 0)
                                    {
                                        yield return null;
                                        counter = maxUpdatePerFrame;
                                    }
                                    Physics.IgnoreCollision(collider, colliderList2[l].colliders[n], ignore);
                                    performanceCounter++;
                                    counter--;
                                }
                            }
                        }
                    }
                }
            }
            //Debug.Log($"yapp: queue count: {collisionQueue.Count}");
            Debug.Log($"yapp: finished Updating part collision ignores, {performanceCounter}");
            allVesselColliders.Clear();
        }

        // Token: 0x040041FB RID: 16891
        private static List<VesselColliderList> vesselsList = new List<VesselColliderList>(32);

        // Token: 0x020006E1 RID: 1761
        private class VesselColliderList
        {
            public VesselColliderList(Guid vId)
            {
                colliderList = new List<PartColliderList>();
                vesselId = vId;
            }

            public Guid vesselId;
            public List<PartColliderList> colliderList;
        }

        // Token: 0x020006E2 RID: 1762
        private class PartColliderList
        {
            public PartColliderList(uint persistentId, bool sameVslCollision, bool isPhysicsless, Rigidbody rb)
            {
                colliders = new List<Collider>();
                partPersistentId = persistentId;
                sameVesselCollision = sameVslCollision;
                this.isPhysicsless = isPhysicsless;
                rbId = rb ? rb.GetInstanceID() : 0;
            }

            public uint partPersistentId;
            public bool isPhysicsless;
            public bool sameVesselCollision;
            public int rbId;
            public List<Collider> colliders;
        }
    }

}
