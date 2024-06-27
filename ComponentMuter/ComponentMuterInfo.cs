using Grasshopper.GUI;
using Grasshopper.Kernel;
using HarmonyLib;
using System;
using System.Drawing;

namespace ComponentMuter;
public class ComponentMuterInfo : GH_AssemblyInfo
{
    public override string Name => "Component Muter";

    //Return a 24x24 pixel bitmap to represent this GHA library.
    public override Bitmap Icon => null!;

    //Return a short string describing the purpose of this GHA library.
    public override string Description => "Mute the component as you want.";

    public override Guid Id => new ("3181bf96-debc-4700-bce9-d0482f253ee9");

    //Return a string identifying you or your company.
    public override string AuthorName => "秋水";

    //Return a string representing your preferred contact details.
    public override string AuthorContact => "1123993881@qq.com";

    public override string Version => typeof(ComponentMuterInfo).Assembly.GetName().Version?.ToString() ?? "unknown";
}

partial class SimpleAssemblyPriority
{
    protected override int? MenuIndex => 4;

    protected override int InsertIndex => 10;

    internal static Harmony? _harmony;
    protected override void DoWithEditor(GH_DocumentEditor editor)
    {
        _harmony = new Harmony("Grasshopper.ComponentMuter");
        _harmony.PatchAll();

        CustomShortcuts[System.Windows.Forms.Keys.M] = () => Data.Toggle = false;

        base.DoWithEditor(editor);
    }
}
