using System;
using UnityEngine;

namespace TMPro
{
	[Serializable]
	public class TMP_Sprite : TMP_TextElement
	{
		public string name;

		public int hashCode;

		public Vector2 pivot;

		public float scale;

		public Sprite sprite;
	}
}
