using System;
using System.Collections.Generic;
using AutumnMooncat.SpireCore;
using AutumnMooncat.SpireCore.ExternalAPI.Kokoro;
using AutumnMooncat.SpireCore.Features;
using AutumnMooncat.SpireCore.Util;
using Nickel;

namespace AutumnMooncat.SpireCore.Util;

public class KokoroUtils : IRegisterable
{
    public static IKokoroApi.IV2.IActionCostsApi.IResource ChargeResource { get; set; }
    
    public static void Register(IModHelper helper)
    {
        ChargeResource = new ActionRequiredChargeResource();
        MainModFile.Kokoro().ActionCosts.RegisterResourceCostIcon(ChargeResource, ChargeStatus.Entry.Configuration.Definition.icon, CommonIcons.Find("charge5"));
    }

    public class ActionRequiredChargeResource : IKokoroApi.IV2.IActionCostsApi.IResource, ITooltipHelper
    {
        public static string ID => nameof(ActionRequiredChargeResource);
        public string ResourceKey => MainModFile.MakeID("actioncost.resource.charge");
        
        public int GetCurrentResourceAmount(State state, Combat combat)
        {
            return ChargeStatus.EffectiveCharge(state, combat, state.ship);
        }

        public void Pay(State state, Combat combat, int amount) { }

        public IReadOnlyList<Tooltip> GetTooltips(State state, Combat combat, int amount)
        {
            return [ITooltipHelper.MakeTooltip("resource", ID, ChargeStatus.Entry.Configuration.Definition.icon, Colors.action, null, new {Amount = amount})];
        }
    }

    public class CompoundStatusInfoRenderer(IKokoroApi.IV2.IStatusRenderingApi.IBarStatusInfoRenderer barRenderer, IKokoroApi.IV2.IStatusRenderingApi.ITextStatusInfoRenderer textRenderer, Func<IKokoroApi.IV2.IStatusRenderingApi.IStatusInfoRenderer.IRenderArgs.IBuilder, int> thresholdFunc) : IKokoroApi.IV2.IStatusRenderingApi.IStatusInfoRenderer
    {
        public IKokoroApi.IV2.IStatusRenderingApi.IBarStatusInfoRenderer BarPart => barRenderer;
        public IKokoroApi.IV2.IStatusRenderingApi.ITextStatusInfoRenderer TextPart => textRenderer;
        public int Render(IKokoroApi.IV2.IStatusRenderingApi.IStatusInfoRenderer.IRenderArgs args)
        {
            var width = 0;
            var newArgs = args.CopyToBuilder();
            var threshold = thresholdFunc(newArgs);

            width += barRenderer.Render(newArgs);
            if (newArgs.Amount > threshold)
            {
                newArgs.Position += new Vec(width, 0);
                textRenderer.Text = "+" + (newArgs.Amount - threshold);
                width += textRenderer.Render(newArgs);
            }
            return width;
        }
    }
}