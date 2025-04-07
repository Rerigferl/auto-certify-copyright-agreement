using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
internal static class AutoCertifyCopyrightAgreement
{
    private const string ID = "numeira.auto-certify-copyright-agreement";

    static AutoCertifyCopyrightAgreement()
    {
        var vrcCopyrightAgreement = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.DefinedTypes).FirstOrDefault(x => x.FullName is "VRC.SDKBase.VRCCopyrightAgreement");
        if (vrcCopyrightAgreement == null)
        {
            Debug.LogWarning($"[{ID}] Type `VRC.SDKBase.VRCCopyrightAgreement` is missing, skip processing.");
            return;
        }

        var method = vrcCopyrightAgreement.GetMethod("HasAgreement", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        if (method == null)
            return;

        var harmony = new Harmony(ID);
        harmony.Patch(method, prefix: new HarmonyMethod(typeof(AutoCertifyCopyrightAgreement), nameof(Prefix)));

        AssemblyReloadEvents.beforeAssemblyReload += () => { harmony.UnpatchAll(ID); };
    }

    public static bool Prefix(ref Task<bool> __result)
    {
        __result = Task.FromResult(true);
        return false;
    }
}
