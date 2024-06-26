using Grasshopper.Kernel;
using System.Runtime.CompilerServices;

namespace ComponentMuter;
internal static  class ComponentExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsMute(this IGH_Component owner) => owner.OnPingDocument()?.ValueTable.GetValue(nameof(IsMute) + owner.InstanceGuid, false) ?? false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void SetMute(this IGH_Component owner, bool value) => owner.OnPingDocument()?.ValueTable.SetValue(nameof(IsMute) + owner.InstanceGuid, value);

}
