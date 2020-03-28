namespace YetAnotherPrunePhysics
{
    public class YAPP : PartModule
    {
        [UI_Toggle(affectSymCounterparts = UI_Scene.All),
            KSPField(guiName = "YAPP Enabled", isPersistant = true, guiActive = true, guiActiveEditor = true)]
        public bool yappEnabled = false;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            bool flag = CanPrune();

            var field = Fields[nameof(yappEnabled)];
            field.guiActive = flag;
            field.guiActiveEditor = flag;

            if (flag && HighLogic.LoadedSceneIsFlight)
            {
                if (yappEnabled)
                {
                    part.PhysicsSignificance = (int)Part.PhysicalSignificance.NONE;
                }
            }
        }

        private bool CanPrune()
        {
            return part
                && !part.sameVesselCollision
                && !part.isVesselEVA
                && WhiteList.IsInWhiteList(part.partInfo);
        }
    }
}
