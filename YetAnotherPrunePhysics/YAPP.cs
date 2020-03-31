namespace YetAnotherPrunePhysics
{
    public class YAPP : PartModule
    {
        [UI_Toggle(affectSymCounterparts = UI_Scene.All),
            KSPField(guiName = "#loc_yapp_button", isPersistant = true, guiActive = true, guiActiveEditor = true)]
        public bool yappEnabled = false;

        public override void OnInitialize()
        {
            base.OnInitialize();
            bool flag = CanPrune();

            var field = Fields[nameof(yappEnabled)];
            field.guiActive = flag;
            field.guiActiveEditor = flag;

            if (flag && HighLogic.LoadedSceneIsFlight)
            {
                field.guiName = KSP.Localization.Localizer.Format("#yapp_button_delayed");
                if (yappEnabled)
                {
                    part.PhysicsSignificance = (int)Part.PhysicalSignificance.NONE;
                }
            }
        }

        private bool CanPrune()
        {
            return part
                && !part.isVesselEVA
                && WhiteList.IsInWhiteList(part.partInfo);
        }
    }
}
