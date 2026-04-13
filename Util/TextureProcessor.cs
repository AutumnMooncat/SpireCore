using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nickel;

namespace AutumnMooncat.Spirecore.Util;

public class TextureProcessor
{
    private static Microsoft.Xna.Framework.Color ConvertColor(Color color)
    {
        return new Microsoft.Xna.Framework.Color((float)color.r, (float)color.g, (float)color.b, (float)color.a);
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