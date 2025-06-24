using System;

namespace TMPro
{
	[Serializable]
	public class TMP_Glyph : TMP_TextElement
	{
		public static TMP_Glyph Clone(TMP_Glyph source)
		{
			TMP_Glyph tMP_Glyph = new TMP_Glyph();
			tMP_Glyph.id = source.id;
			tMP_Glyph.x = source.x;
			tMP_Glyph.y = source.y;
			tMP_Glyph.width = source.width;
			tMP_Glyph.height = source.height;
			tMP_Glyph.xOffset = source.xOffset;
			tMP_Glyph.yOffset = source.yOffset;
			tMP_Glyph.xAdvance = source.xAdvance;
			return tMP_Glyph;
		}
	}
}
