using System;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nickel;

namespace AutumnMooncat.SpireCore.Util;

public class TextureProcessor
{
    public delegate void RenderJob();
    public static event RenderJob Jobs;

    public static void OnInit()
    {
        MainModFile.Log("Running TextureProcessor jobs...");
        var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        Jobs?.Invoke();
        var time = (DateTimeOffset.Now.ToUnixTimeMilliseconds() - now)/1000f;
        MainModFile.Log("Processed {} jobs in {}s", Jobs?.GetInvocationList().Length ?? 0, time);
    }
    
    private static Microsoft.Xna.Framework.Color ConvertColor(Color color)
    {
        return new Microsoft.Xna.Framework.Color((float)color.r, (float)color.g, (float)color.b, (float)color.a);
    }

    public static ISpriteEntry MakeSprite(int width, int height, Action<Vec> actions)
    {
        return MainModFile.GetHelper().Content.Sprites.RegisterSprite(() => Make(width, height, actions));
    }

    public static Texture2D Make(int width, int height, Action<Vec> actions)
    {
        var origSb = MG.inst.sb;
        var origTarget = MG.inst.renderTarget;
        var sb = new SpriteBatch(MG.inst.GraphicsDevice);
        var target = new RenderTarget2D(MG.inst.GraphicsDevice, width, height);
        MG.inst.sb = sb;
        MG.inst.GraphicsDevice.SetRenderTarget(target);
        MG.inst.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Transparent);
        sb.Begin();
        actions(new Vec(width, height));
        sb.End();
        MG.inst.sb = origSb;
        MG.inst.GraphicsDevice.SetRenderTarget(origTarget);
        return target;
    }
    
    public static ISpriteEntry Combine(int width, int height, params Records.TexturePayload[] textures)
    {
        MainModFile.Instance.Logger.LogInformation($"{nameof(TextureProcessor)}.{nameof(Combine)} called");
        var sb = new SpriteBatch(MG.inst.GraphicsDevice);
        var target = new RenderTarget2D(MG.inst.GraphicsDevice, width, height);
        var origTarget = MG.inst.renderTarget;
        MG.inst.GraphicsDevice.SetRenderTarget(target);
        sb.Begin();
        foreach (var tpl in textures)
        {
            var tex = MainModFile.Instance.Helper.Content.Sprites.LookupBySpr(tpl.spr)?.ObtainTexture();
            if (tex != null)
            {
                MainModFile.Instance.Logger.LogInformation($"Got spr {tpl.spr} texture, drawing");
                var col = Microsoft.Xna.Framework.Color.White;
                if (tpl.color.HasValue)
                {
                    col = ConvertColor(tpl.color.Value);
                }
                sb.Draw(tex, new Vector2(tpl.x, tpl.y), col);
            }
        }
        sb.End();
        MG.inst.GraphicsDevice.SetRenderTarget(origTarget);
        return MainModFile.Instance.Helper.Content.Sprites.RegisterSprite(() => target);
    }
}