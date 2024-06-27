using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ComponentMuter.Patch;

[HarmonyPatch(typeof(GH_Component))]
internal class ComponentPatch
{
    internal static readonly Dictionary<IGH_Component, List<SearchPair>> _recordedDict = [];
    static readonly List<Type> _replacedType = [];

    [HarmonyPatch("ComputeData")]
    static void Prefix(GH_Component __instance)
    {
        if (!Data.Enable) return;
        if (!__instance.IsMute()) return;

        var type = __instance.GetType();

        if (!_replacedType.Contains(type))
        {
            var method = GetSolveInstance(type);
            SimpleAssemblyPriority._harmony?.Patch(method, new HarmonyMethod(SolveInstance));
            _replacedType.Add(type);
        }

        if (!_recordedDict.TryGetValue(__instance, out var list))
        {
            _recordedDict[__instance] = list = [];
        }

        for (int j = 0; j < __instance.Params.Output.Count; j++)
        {
            var output = __instance.Params.Output[j];

            for (int i = 0; i < __instance.Params.Input.Count; i++)
            {
                var input = __instance.Params.Input[i];

                if (input.ComponentGuid != output.ComponentGuid) continue;
                if (input.Access != output.Access) continue;
                list.Add(new(i, j, input.Type, input.Access));

                break;
            }
        }
    }

    static MethodInfo? GetSolveInstance(Type? type)
    {
        if (type == null) return null;
        var methods = type.GetRuntimeMethods().Where(m => m.Name == "SolveInstance" && m.DeclaringType == type);
        if (methods.Any()) return methods.First();
        return GetSolveInstance(type.BaseType);
    }

    static bool SolveInstance(GH_Component __instance, object[] __args)
    {
        IGH_DataAccess DA = (__args[0] as IGH_DataAccess)!;

        if (!__instance.IsMute()) return true;

        if (!_recordedDict.TryGetValue(__instance, out var list)) return true;

        foreach (var pair in list)
        {
            var method = (pair.Access switch
            {
                GH_ParamAccess.list => GetMethod(DA, nameof(DA.GetDataList)),
                GH_ParamAccess.tree => GetMethod(DA, nameof(DA.GetDataTree)),
                _ => GetMethod(DA, nameof(DA.GetData)),
            }).MakeGenericMethod(pair.Type);

            object[] pms = [pair.From, null!];
            if (!(bool)method.Invoke(DA, pms)!) continue;

            var value = pms[1];

            switch (pair.Access)
            {
                case GH_ParamAccess.list:
                    DA.SetDataList(pair.To, value as IEnumerable);
                    break;

                case GH_ParamAccess.tree:
                    DA.SetDataTree(pair.To, value as IGH_DataTree);
                    break;

                default:
                    DA.SetData(pair.To, value);
                    break;
            }
        }

        return false;

        static MethodInfo GetMethod(IGH_DataAccess DA, string name)
        {
            return DA.GetType().GetRuntimeMethods().First(m =>
            {
                if (m.Name != name) return false;
                var pms = m.GetParameters();
                if (pms.Length != 2) return false;
                if (pms[0].ParameterType != typeof(int)) return false;
                return true;
            });
        }
    }
}

internal readonly record struct SearchPair(int From, int To, Type Type, GH_ParamAccess Access)
{

}