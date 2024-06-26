using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using HarmonyLib;
namespace ComponentMuter.Patch;

[HarmonyPatch(typeof(GH_CapsuleRenderEngine))]
internal class CapsuleRenderEnginePatch
{
    [HarmonyPatch(nameof(GH_CapsuleRenderEngine.GetImpliedPalette))]
    static bool Prefix(ref GH_Palette __result, IGH_ActiveObject obj)
    {
        if (obj is not IGH_Component component) return true;
        if (!component.IsMute()) return true;

        __result = GH_Palette.Transparent;
        return false;
    }
}
