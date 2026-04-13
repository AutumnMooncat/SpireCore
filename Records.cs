namespace AutumnMooncat.SpireCore;

public class Records
{
    public record RenderPayload
    {
        public Spr? spr;
        public int? amount;
        public int? xHint;
        public Color? color;
        public int width = 8;
        public int dx;
        public int dy;
        public bool flipX;
        public bool flipY;
    }

    public record TexturePayload
    {
        public Spr spr;
        public int x;
        public int y;
        public Color? color;
        public bool flipX;
        public bool flipY;
    }
}