using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel.Attributes;
using HarmonyLib;
using System.Drawing;

namespace ComponentMuter.Patch;

[HarmonyPatch(typeof(GH_ComponentAttributes))]
internal class ComponentAttributePatch
{
    [HarmonyPatch("Render")]
    static void Postfix(GH_ComponentAttributes __instance, GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
    {
        if (!Data.Enable) return;

        var component = __instance.Owner;
        if (!component.IsMute()) return;

        if (channel != GH_CanvasChannel.Wires) return;

        if (!ComponentPatch._recordedDict.TryGetValue(component, out var list)) return;

        foreach (var pair in list) 
        {
            var input = component.Params.Input[pair.From];
            var output = component.Params.Output[pair.To];

            canvas.Painter.DrawConnection(output.Attributes.OutputGrip, input.Attributes.InputGrip, GH_WireDirection.left, GH_WireDirection.right, __instance.Selected, __instance.Selected, GH_WireType.generic);
        }
    }
}
