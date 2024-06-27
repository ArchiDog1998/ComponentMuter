using Grasshopper.Kernel;
using Grasshopper;
using SimpleGrasshopper.Attributes;

namespace ComponentMuter;
internal static partial class Data
{
    [Setting, Config("Component Muter")]
    private static readonly bool _Enable = true;

    [Shortcut(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M | System.Windows.Forms.Keys.Shift)]
    [Config("Toggle Mute")]
    public static object Toggle 
    {
        get => false;
        set
        {
            var doc = Instances.ActiveCanvas.Document;
            if (doc == null) return;

            if (!Enable) return;

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
        }
    }
}
