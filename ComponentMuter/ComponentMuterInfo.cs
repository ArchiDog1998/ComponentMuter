using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using HarmonyLib;
using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace ComponentMuter;
public class ComponentMuterInfo : GH_AssemblyInfo
{
    public override string Name => "ComponentMuter";

    //Return a 24x24 pixel bitmap to represent this GHA library.
    public override Bitmap Icon => null!;

    //Return a short string describing the purpose of this GHA library.
    public override string Description => "";

    public override Guid Id => new ("3181bf96-debc-4700-bce9-d0482f253ee9");

    //Return a string identifying you or your company.
    public override string AuthorName => "秋水";

    //Return a string representing your preferred contact details.
    public override string AuthorContact => "1123993881@qq.com";
}

partial class SimpleAssemblyPriority
{
    internal static Harmony? _harmony;
    protected override void DoWithEditor(GH_DocumentEditor editor)
    {
        _harmony = new Harmony("Grasshopper.ComponentMuter");
        _harmony.PatchAll();

        CustomShortcuts[Keys.M] = () =>
        {
            var doc = Instances.ActiveCanvas.Document;
            if (doc == null) return;

            bool any = false;
            foreach (var obj in doc.SelectedObjects())
            {
                if (obj is not IGH_Component component) continue;
                component.SetMute(!component.IsMute());
                component.ExpireSolution(false);
                any = true;
            }

            if (any)
            {
                doc.NewSolution(false);
            }
        };

        base.DoWithEditor(editor);
    }
}
