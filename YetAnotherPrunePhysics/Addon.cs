using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace YetAnotherPrunePhysics
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public sealed class Addon : MonoBehaviour
    {
        private static bool _loaded = false;
        private void Awake()
        {
            if (!_loaded)
            {

                WhiteList.ReadWhiteList();
                _loaded = true;
            }
            GameEvents.onPartWillDie.Add(OnPartDie);

            if (CollisionManager.Instance && !(CollisionManager.Instance is CollisionManagerOverride))
            {
                CollisionManager.Instance.gameObject.AddComponent<CollisionManagerOverride>();
            }
        }

        private void OnDestroy()
        {
            GameEvents.onPartWillDie.Remove(OnPartDie);
        }

        private void OnPartDie(Part data)
        {
            foreach (var part in data.children)
            {
                var module = part.FindModuleImplementing<YAPP>();
                if (module && module.yappEnabled && part.physicalSignificance == Part.PhysicalSignificance.NONE)
                {
                    part.PromoteToPhysicalPart();
                }
            }
        }
    }
}