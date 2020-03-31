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
        private void Awake()
        {
            GameEvents.onPartWillDie.Add(OnPartDie);
        }

        private void OnPartDie(Part data)
        {
            foreach (var part in data.children)
            {
                ProcessPart(part);
            }

            void ProcessPart(Part part)
            {
                var module = part.FindModuleImplementing<YAPP>();
                if (module && module.yappEnabled)
                {
                   part. PromoteToPhysicalPart();
                }
            }
        }
    }
}