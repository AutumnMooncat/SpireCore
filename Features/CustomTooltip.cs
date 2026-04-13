using System;
using System.Linq;
using System.Text;

namespace AutumnMooncat.SpireCore.Features;

/// <summary>
/// A tooltip, mimicking one that would be used for a game glossary.
/// </summary>
/// <param name="key">The key used for deduplicating tooltips - if two tooltips have the same key, only the first one will be rendered.</param>
public class CustomTooltip(string key) : TTGlossary(key)
{
	/// <summary>The icons to show next to the title. If a title is not provided, this icon will not show.</summary>
	public Records.TexturePayload[] IconData = null;

	/// <summary>The color for the icon. If provided, the icon's texture is multiplied by this color.</summary>
	public Color? IconColor = null;

	/// <summary>The color for the title text.</summary>
	public Color? TitleColor = null;

	/// <summary>The title. If not provided, the icon (<see cref="Icon"/>) will not show.</summary>
	public string Title = null;

	/// <summary>The description.</summary>
	public string Description = null;

	/// <summary>Whether the title should be converted to all uppercase characters. Defaults to <c>true</c>.</summary>
	/// <remarks>When this is <c>true</c>, color tags will not work, as these need to be lowercase.</remarks>
	public bool UppercaseTitle = true;

	public int TitleOffset = 5;

	/// <inheritdoc/>
	public override Rect Render(G g, bool dontDraw)
	{
		var sb = new StringBuilder();
		if (!string.IsNullOrEmpty(Title))
		{
			int num = TitleOffset;
			if (DB.currentLocale != null && DB.currentLocale.isHighRes)
			{
				num = Convert.ToInt32(MathF.Ceiling(num * 6f / 5f));
			}
			
			for (int i = 0; i < num; i++)
			{
				sb.Append(' ');
			}

			if (TitleColor is not null)
				sb.Append($"<c={TitleColor.Value.ToString()}>");
			sb.Append(UppercaseTitle ? Title.ToUpper() : Title);
			if (TitleColor is not null)
				sb.Append("</c>");
		}

		if (!string.IsNullOrEmpty(Description))
		{
			if (IconData is not null || !string.IsNullOrEmpty(Title))
				sb.Append('\n');

			var args = vals?.Select(object (v) => "<c=boldPink>{0}</c>".FF(v.ToString() ?? "")).ToArray() ?? [];
			sb.Append(string.Format(Description, args));
		}

		var rect = Draw.Text(sb.ToString(), 0, 0, color: Colors.textMain, maxWidth: 100, dontDraw: true);
		if (!dontDraw)
		{
			var xy = g.Push(null, rect).rect.xy;
			if (IconData != null)
			{
				foreach (var tpl in IconData)
				{
					Draw.Sprite(tpl.spr, xy.x - 1 + tpl.x, xy.y + 2 + tpl.y, tpl.flipX, tpl.flipY, color: tpl.color);
				}
			}
			Draw.Text(sb.ToString(), xy.x, xy.y + 4, color: Colors.textMain, maxWidth: 100);
			g.Pop();
		}

		return rect;
	}
}
    
