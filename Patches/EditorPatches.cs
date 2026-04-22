using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace AutumnMooncat.SpireCore.Patches;

[HarmonyPatch]
public class EditorPatches
{
    public static void OnNewRun(G g)
    {
        g.state.ChangeRoute(() => new NewRunOptions());
    }

    public static void NRG(G g)
    {
        if (g.state.route is Combat c)
        {
            c.QueueImmediate(new AEnergy(){changeAmount = 1});
        }
    }

    public static void Draw(G g)
    {
        if (g.state.route is Combat c)
        {
            c.QueueImmediate(new ADrawCard(){count = 1});
        }
    }

    public static void Discard(G g)
    {
        if (g.state.route is Combat c)
        {
            c.QueueImmediate(new ADiscard());
        }
    }
    
    [HarmonyPatch(typeof(Editor), nameof(Editor.PanelTools))]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> input, ILGenerator generator)
    {
        var codes = input.ToList();
        var newRun = AccessTools.Method(typeof(EditorPatches), nameof(OnNewRun));
        var newRunJump = generator.DefineLabel();
        var nrg = AccessTools.Method(typeof(EditorPatches), nameof(NRG));
        var nrgJump = generator.DefineLabel();
        var draw = AccessTools.Method(typeof(EditorPatches), nameof(Draw));
        var drawJump = generator.DefineLabel();
        var discard = AccessTools.Method(typeof(EditorPatches), nameof(Discard));
        var discardJump = generator.DefineLabel();
        var inserted = false;
        CodeInstruction button = codes.Find(c => c.opcode == OpCodes.Call && c.operand is MethodInfo { Name: "Button" });
        CodeInstruction sameLine = codes.Find(c => c.opcode == OpCodes.Call && c.operand is MethodInfo { Name: "SameLine" });
        for (var i = 0; i < codes.Count; i++)
        {
            if (!inserted && codes[i + 1] == button)
            {
                inserted = true;
                
                yield return new CodeInstruction(OpCodes.Ldstr, "New Run");
                yield return button;
                yield return new CodeInstruction(OpCodes.Brfalse, newRunJump);
                yield return new CodeInstruction(OpCodes.Ldarg_1);
                yield return new CodeInstruction(OpCodes.Call, newRun);

                yield return sameLine.Clone().WithLabels(newRunJump);
                yield return new CodeInstruction(OpCodes.Ldstr, "Gain Energy");
                yield return button;
                yield return new CodeInstruction(OpCodes.Brfalse, nrgJump);
                yield return new CodeInstruction(OpCodes.Ldarg_1);
                yield return new CodeInstruction(OpCodes.Call, nrg);
                
                yield return sameLine.Clone().WithLabels(nrgJump);
                yield return new CodeInstruction(OpCodes.Ldstr, "Draw Card");
                yield return button;
                yield return new CodeInstruction(OpCodes.Brfalse, drawJump);
                yield return new CodeInstruction(OpCodes.Ldarg_1);
                yield return new CodeInstruction(OpCodes.Call, draw);
                
                yield return sameLine.Clone().WithLabels(drawJump);
                yield return new CodeInstruction(OpCodes.Ldstr, "Discard Hand");
                yield return button;
                yield return new CodeInstruction(OpCodes.Brfalse, discardJump);
                yield return new CodeInstruction(OpCodes.Ldarg_1);
                yield return new CodeInstruction(OpCodes.Call, discard);
                codes[i].labels.Add(discardJump);
            }
            yield return codes[i];
        }
    }
}