using HarmonyLib;

namespace PoiStorageRescue
{
    public class Main : IModApi
    {
        private static Harmony? _harmony;

        public void InitMod(Mod modInstance)
        {
            _harmony ??= new Harmony("GluckHouse.PoiStorageRescue");
            _harmony.PatchAll();
            Log.Out("[PoiStorageRescue] Initialized.");
        }
    }
}
