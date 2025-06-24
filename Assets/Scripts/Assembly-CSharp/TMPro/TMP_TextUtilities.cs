using UnityEngine;

namespace TMPro
{
	public static class TMP_TextUtilities
	{
		private struct LineSegment
		{
			public Vector3 Point1;

			public Vector3 Point2;

			public LineSegment(Vector3 p1, Vector3 p2)
			{
				Point1 = p1;
				Point2 = p2;
			}
		}

		private static Vector3[] m_rectWorldCorners = new Vector3[4];

		private const string k_lookupStringL = "-------------------------------- !-#$%&-()*+,-./0123456789:;<=>?@abcdefghijklmnopqrstuvwxyz[-]^_`abcdefghijklmnopqrstuvwxyz{|}~-";

		private const string k_lookupStringU = "-------------------------------- !-#$%&-()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[-]^_`ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~-";

		public static CaretInfo GetCursorInsertionIndex(TMP_Text textComponent, Vector3 position, Camera camera)
		{
			int num = FindNearestCharacter(textComponent, position, camera, false);
			RectTransform rectTransform = textComponent.rectTransform;
			ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			TMP_CharacterInfo tMP_CharacterInfo = textComponent.textInfo.characterInfo[num];
			Vector3 vector = rectTransform.TransformPoint(tMP_CharacterInfo.bottomLeft);
			Vector3 vector2 = rectTransform.TransformPoint(tMP_CharacterInfo.topRight);
			float num2 = (position.x - vector.x) / (vector2.x - vector.x);
			if (num2 < 0.5f)
			{
				return new CaretInfo(num, CaretPosition.Left);
			}
			return new CaretInfo(num, CaretPosition.Right);
		}

		public static int GetCursorIndexFromPosition(TMP_Text textComponent, Vector3 position, Camera camera)
		{
			int num = FindNearestCharacter(textComponent, position, camera, false);
			RectTransform rectTransform = textComponent.rectTransform;
			ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			TMP_CharacterInfo tMP_CharacterInfo = textComponent.textInfo.characterInfo[num];
			Vector3 vector = rectTransform.TransformPoint(tMP_CharacterInfo.bottomLeft);
			Vector3 vector2 = rectTransform.TransformPoint(tMP_CharacterInfo.topRight);
			float num2 = (position.x - vector.x) / (vector2.x - vector.x);
			if (num2 < 0.5f)
			{
				return num;
			}
			return num + 1;
		}

		public static bool IsIntersectingRectTransform(RectTransform rectTransform, Vector3 position, Camera camera)
		{
			ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			rectTransform.GetWorldCorners(m_rectWorldCorners);
			if (PointIntersectRectangle(position, m_rectWorldCorners[0], m_rectWorldCorners[1], m_rectWorldCorners[2], m_rectWorldCorners[3]))
			{
				return true;
			}
			return false;
		}

		public static int FindIntersectingCharacter(TextMeshProUGUI text, Vector3 position, Camera camera, bool visibleOnly)
		{
			RectTransform rectTransform = text.rectTransform;
			ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			for (int i = 0; i < text.textInfo.characterCount; i++)
			{
				TMP_CharacterInfo tMP_CharacterInfo = text.textInfo.characterInfo[i];
				if (!visibleOnly || tMP_CharacterInfo.isVisible)
				{
					Vector3 a = rectTransform.TransformPoint(tMP_CharacterInfo.bottomLeft);
					Vector3 b = rectTransform.TransformPoint(new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.topRight.y, 0f));
					Vector3 c = rectTransform.TransformPoint(tMP_CharacterInfo.topRight);
					Vector3 d = rectTransform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.bottomLeft.y, 0f));
					if (PointIntersectRectangle(position, a, b, c, d))
					{
						return i;
					}
				}
			}
			return -1;
		}

		public static int FindIntersectingCharacter(TextMeshPro text, Vector3 position, Camera camera, bool visibleOnly)
		{
			Transform transform = text.transform;
			ScreenPointToWorldPointInRectangle(transform, position, camera, out position);
			for (int i = 0; i < text.textInfo.characterCount; i++)
			{
				TMP_CharacterInfo tMP_CharacterInfo = text.textInfo.characterInfo[i];
				if (!visibleOnly || tMP_CharacterInfo.isVisible)
				{
					Vector3 a = transform.TransformPoint(tMP_CharacterInfo.bottomLeft);
					Vector3 b = transform.TransformPoint(new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.topRight.y, 0f));
					Vector3 c = transform.TransformPoint(tMP_CharacterInfo.topRight);
					Vector3 d = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.bottomLeft.y, 0f));
					if (PointIntersectRectangle(position, a, b, c, d))
					{
						return i;
					}
				}
			}
			return -1;
		}

		public static int FindNearestCharacter(TMP_Text text, Vector3 position, Camera camera, bool visibleOnly)
		{
			RectTransform rectTransform = text.rectTransform;
			float num = float.PositiveInfinity;
			int result = 0;
			ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			for (int i = 0; i < text.textInfo.characterCount; i++)
			{
				TMP_CharacterInfo tMP_CharacterInfo = text.textInfo.characterInfo[i];
				if (!visibleOnly || tMP_CharacterInfo.isVisible)
				{
					Vector3 vector = rectTransform.TransformPoint(tMP_CharacterInfo.bottomLeft);
					Vector3 vector2 = rectTransform.TransformPoint(new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.topRight.y, 0f));
					Vector3 vector3 = rectTransform.TransformPoint(tMP_CharacterInfo.topRight);
					Vector3 vector4 = rectTransform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.bottomLeft.y, 0f));
					if (PointIntersectRectangle(position, vector, vector2, vector3, vector4))
					{
						return i;
					}
					float num2 = DistanceToLine(vector, vector2, position);
					float num3 = DistanceToLine(vector2, vector3, position);
					float num4 = DistanceToLine(vector3, vector4, position);
					float num5 = DistanceToLine(vector4, vector, position);
					float num6 = ((!(num2 < num3)) ? num3 : num2);
					num6 = ((!(num6 < num4)) ? num4 : num6);
					num6 = ((!(num6 < num5)) ? num5 : num6);
					if (num > num6)
					{
						num = num6;
						result = i;
					}
				}
			}
			return result;
		}

		public static int FindNearestCharacter(TextMeshProUGUI text, Vector3 position, Camera camera, bool visibleOnly)
		{
			RectTransform rectTransform = text.rectTransform;
			float num = float.PositiveInfinity;
			int result = 0;
			ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			for (int i = 0; i < text.textInfo.characterCount; i++)
			{
				TMP_CharacterInfo tMP_CharacterInfo = text.textInfo.characterInfo[i];
				if (!visibleOnly || tMP_CharacterInfo.isVisible)
				{
					Vector3 vector = rectTransform.TransformPoint(tMP_CharacterInfo.bottomLeft);
					Vector3 vector2 = rectTransform.TransformPoint(new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.topRight.y, 0f));
					Vector3 vector3 = rectTransform.TransformPoint(tMP_CharacterInfo.topRight);
					Vector3 vector4 = rectTransform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.bottomLeft.y, 0f));
					if (PointIntersectRectangle(position, vector, vector2, vector3, vector4))
					{
						return i;
					}
					float num2 = DistanceToLine(vector, vector2, position);
					float num3 = DistanceToLine(vector2, vector3, position);
					float num4 = DistanceToLine(vector3, vector4, position);
					float num5 = DistanceToLine(vector4, vector, position);
					float num6 = ((!(num2 < num3)) ? num3 : num2);
					num6 = ((!(num6 < num4)) ? num4 : num6);
					num6 = ((!(num6 < num5)) ? num5 : num6);
					if (num > num6)
					{
						num = num6;
						result = i;
					}
				}
			}
			return result;
		}

		public static int FindNearestCharacter(TextMeshPro text, Vector3 position, Camera camera, bool visibleOnly)
		{
			Transform transform = text.transform;
			float num = float.PositiveInfinity;
			int result = 0;
			ScreenPointToWorldPointInRectangle(transform, position, camera, out position);
			for (int i = 0; i < text.textInfo.characterCount; i++)
			{
				TMP_CharacterInfo tMP_CharacterInfo = text.textInfo.characterInfo[i];
				if (!visibleOnly || tMP_CharacterInfo.isVisible)
				{
					Vector3 vector = transform.TransformPoint(tMP_CharacterInfo.bottomLeft);
					Vector3 vector2 = transform.TransformPoint(new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.topRight.y, 0f));
					Vector3 vector3 = transform.TransformPoint(tMP_CharacterInfo.topRight);
					Vector3 vector4 = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.bottomLeft.y, 0f));
					if (PointIntersectRectangle(position, vector, vector2, vector3, vector4))
					{
						return i;
					}
					float num2 = DistanceToLine(vector, vector2, position);
					float num3 = DistanceToLine(vector2, vector3, position);
					float num4 = DistanceToLine(vector3, vector4, position);
					float num5 = DistanceToLine(vector4, vector, position);
					float num6 = ((!(num2 < num3)) ? num3 : num2);
					num6 = ((!(num6 < num4)) ? num4 : num6);
					num6 = ((!(num6 < num5)) ? num5 : num6);
					if (num > num6)
					{
						num = num6;
						result = i;
					}
				}
			}
			return result;
		}

		public static int FindIntersectingWord(TextMeshProUGUI text, Vector3 position, Camera camera)
		{
			RectTransform rectTransform = text.rectTransform;
			ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			for (int i = 0; i < text.textInfo.wordCount; i++)
			{
				TMP_WordInfo tMP_WordInfo = text.textInfo.wordInfo[i];
				bool flag = false;
				Vector3 a = Vector3.zero;
				Vector3 b = Vector3.zero;
				Vector3 zero = Vector3.zero;
				Vector3 zero2 = Vector3.zero;
				float num = float.NegativeInfinity;
				float num2 = float.PositiveInfinity;
				for (int j = 0; j < tMP_WordInfo.characterCount; j++)
				{
					int num3 = tMP_WordInfo.firstCharacterIndex + j;
					TMP_CharacterInfo tMP_CharacterInfo = text.textInfo.characterInfo[num3];
					int lineNumber = tMP_CharacterInfo.lineNumber;
					bool flag2 = ((num3 <= text.maxVisibleCharacters && tMP_CharacterInfo.lineNumber <= text.maxVisibleLines && (text.OverflowMode != TextOverflowModes.Page || tMP_CharacterInfo.pageNumber + 1 == text.pageToDisplay)) ? true : false);
					num = Mathf.Max(num, tMP_CharacterInfo.ascender);
					num2 = Mathf.Min(num2, tMP_CharacterInfo.descender);
					if (!flag && flag2)
					{
						flag = true;
						a = new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.descender, 0f);
						b = new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.ascender, 0f);
						if (tMP_WordInfo.characterCount == 1)
						{
							flag = false;
							zero = new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f);
							zero2 = new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f);
							a = rectTransform.TransformPoint(new Vector3(a.x, num2, 0f));
							b = rectTransform.TransformPoint(new Vector3(b.x, num, 0f));
							zero2 = rectTransform.TransformPoint(new Vector3(zero2.x, num, 0f));
							zero = rectTransform.TransformPoint(new Vector3(zero.x, num2, 0f));
							if (PointIntersectRectangle(position, a, b, zero2, zero))
							{
								return i;
							}
						}
					}
					if (flag && j == tMP_WordInfo.characterCount - 1)
					{
						flag = false;
						zero = new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f);
						zero2 = new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f);
						a = rectTransform.TransformPoint(new Vector3(a.x, num2, 0f));
						b = rectTransform.TransformPoint(new Vector3(b.x, num, 0f));
						zero2 = rectTransform.TransformPoint(new Vector3(zero2.x, num, 0f));
						zero = rectTransform.TransformPoint(new Vector3(zero.x, num2, 0f));
						if (PointIntersectRectangle(position, a, b, zero2, zero))
						{
							return i;
						}
					}
					else if (flag && lineNumber != text.textInfo.characterInfo[num3 + 1].lineNumber)
					{
						flag = false;
						zero = new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f);
						zero2 = new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f);
						a = rectTransform.TransformPoint(new Vector3(a.x, num2, 0f));
						b = rectTransform.TransformPoint(new Vector3(b.x, num, 0f));
						zero2 = rectTransform.TransformPoint(new Vector3(zero2.x, num, 0f));
						zero = rectTransform.TransformPoint(new Vector3(zero.x, num2, 0f));
						num = float.NegativeInfinity;
						num2 = float.PositiveInfinity;
						if (PointIntersectRectangle(position, a, b, zero2, zero))
						{
							return i;
						}
					}
				}
			}
			return -1;
		}

		public static int FindIntersectingWord(TextMeshPro text, Vector3 position, Camera camera)
		{
			Transform transform = text.transform;
			ScreenPointToWorldPointInRectangle(transform, position, camera, out position);
			for (int i = 0; i < text.textInfo.wordCount; i++)
			{
				TMP_WordInfo tMP_WordInfo = text.textInfo.wordInfo[i];
				bool flag = false;
				Vector3 a = Vector3.zero;
				Vector3 b = Vector3.zero;
				Vector3 zero = Vector3.zero;
				Vector3 zero2 = Vector3.zero;
				float num = float.NegativeInfinity;
				float num2 = float.PositiveInfinity;
				for (int j = 0; j < tMP_WordInfo.characterCount; j++)
				{
					int num3 = tMP_WordInfo.firstCharacterIndex + j;
					TMP_CharacterInfo tMP_CharacterInfo = text.textInfo.characterInfo[num3];
					int lineNumber = tMP_CharacterInfo.lineNumber;
					bool flag2 = ((num3 <= text.maxVisibleCharacters && tMP_CharacterInfo.lineNumber <= text.maxVisibleLines && (text.OverflowMode != TextOverflowModes.Page || tMP_CharacterInfo.pageNumber + 1 == text.pageToDisplay)) ? true : false);
					num = Mathf.Max(num, tMP_CharacterInfo.ascender);
					num2 = Mathf.Min(num2, tMP_CharacterInfo.descender);
					if (!flag && flag2)
					{
						flag = true;
						a = new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.descender, 0f);
						b = new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.ascender, 0f);
						if (tMP_WordInfo.characterCount == 1)
						{
							flag = false;
							zero = new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f);
							zero2 = new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f);
							a = transform.TransformPoint(new Vector3(a.x, num2, 0f));
							b = transform.TransformPoint(new Vector3(b.x, num, 0f));
							zero2 = transform.TransformPoint(new Vector3(zero2.x, num, 0f));
							zero = transform.TransformPoint(new Vector3(zero.x, num2, 0f));
							if (PointIntersectRectangle(position, a, b, zero2, zero))
							{
								return i;
							}
						}
					}
					if (flag && j == tMP_WordInfo.characterCount - 1)
					{
						flag = false;
						zero = new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f);
						zero2 = new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f);
						a = transform.TransformPoint(new Vector3(a.x, num2, 0f));
						b = transform.TransformPoint(new Vector3(b.x, num, 0f));
						zero2 = transform.TransformPoint(new Vector3(zero2.x, num, 0f));
						zero = transform.TransformPoint(new Vector3(zero.x, num2, 0f));
						if (PointIntersectRectangle(position, a, b, zero2, zero))
						{
							return i;
						}
					}
					else if (flag && lineNumber != text.textInfo.characterInfo[num3 + 1].lineNumber)
					{
						flag = false;
						zero = new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f);
						zero2 = new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f);
						a = transform.TransformPoint(new Vector3(a.x, num2, 0f));
						b = transform.TransformPoint(new Vector3(b.x, num, 0f));
						zero2 = transform.TransformPoint(new Vector3(zero2.x, num, 0f));
						zero = transform.TransformPoint(new Vector3(zero.x, num2, 0f));
						num = float.NegativeInfinity;
						num2 = float.PositiveInfinity;
						if (PointIntersectRectangle(position, a, b, zero2, zero))
						{
							return i;
						}
					}
				}
			}
			return -1;
		}

		public static int FindNearestWord(TextMeshProUGUI text, Vector3 position, Camera camera)
		{
			RectTransform rectTransform = text.rectTransform;
			float num = float.PositiveInfinity;
			int result = 0;
			ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			for (int i = 0; i < text.textInfo.wordCount; i++)
			{
				TMP_WordInfo tMP_WordInfo = text.textInfo.wordInfo[i];
				bool flag = false;
				Vector3 vector = Vector3.zero;
				Vector3 vector2 = Vector3.zero;
				Vector3 vector3 = Vector3.zero;
				Vector3 vector4 = Vector3.zero;
				for (int j = 0; j < tMP_WordInfo.characterCount; j++)
				{
					int num2 = tMP_WordInfo.firstCharacterIndex + j;
					TMP_CharacterInfo tMP_CharacterInfo = text.textInfo.characterInfo[num2];
					int lineNumber = tMP_CharacterInfo.lineNumber;
					bool flag2 = ((num2 <= text.maxVisibleCharacters && tMP_CharacterInfo.lineNumber <= text.maxVisibleLines && (text.OverflowMode != TextOverflowModes.Page || tMP_CharacterInfo.pageNumber + 1 == text.pageToDisplay)) ? true : false);
					if (!flag && flag2)
					{
						flag = true;
						vector = rectTransform.TransformPoint(new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.descender, 0f));
						vector2 = rectTransform.TransformPoint(new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.ascender, 0f));
						if (tMP_WordInfo.characterCount == 1)
						{
							flag = false;
							vector3 = rectTransform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
							vector4 = rectTransform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
							if (PointIntersectRectangle(position, vector, vector2, vector4, vector3))
							{
								return i;
							}
						}
					}
					if (flag && j == tMP_WordInfo.characterCount - 1)
					{
						flag = false;
						vector3 = rectTransform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
						vector4 = rectTransform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
						if (PointIntersectRectangle(position, vector, vector2, vector4, vector3))
						{
							return i;
						}
					}
					else if (flag && lineNumber != text.textInfo.characterInfo[num2 + 1].lineNumber)
					{
						flag = false;
						vector3 = rectTransform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
						vector4 = rectTransform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
						if (PointIntersectRectangle(position, vector, vector2, vector4, vector3))
						{
							return i;
						}
					}
				}
				float num3 = DistanceToLine(vector, vector2, position);
				float num4 = DistanceToLine(vector2, vector4, position);
				float num5 = DistanceToLine(vector4, vector3, position);
				float num6 = DistanceToLine(vector3, vector, position);
				float num7 = ((!(num3 < num4)) ? num4 : num3);
				num7 = ((!(num7 < num5)) ? num5 : num7);
				num7 = ((!(num7 < num6)) ? num6 : num7);
				if (num > num7)
				{
					num = num7;
					result = i;
				}
			}
			return result;
		}

		public static int FindNearestWord(TextMeshPro text, Vector3 position, Camera camera)
		{
			Transform transform = text.transform;
			float num = float.PositiveInfinity;
			int result = 0;
			ScreenPointToWorldPointInRectangle(transform, position, camera, out position);
			for (int i = 0; i < text.textInfo.wordCount; i++)
			{
				TMP_WordInfo tMP_WordInfo = text.textInfo.wordInfo[i];
				bool flag = false;
				Vector3 vector = Vector3.zero;
				Vector3 vector2 = Vector3.zero;
				Vector3 vector3 = Vector3.zero;
				Vector3 vector4 = Vector3.zero;
				for (int j = 0; j < tMP_WordInfo.characterCount; j++)
				{
					int num2 = tMP_WordInfo.firstCharacterIndex + j;
					TMP_CharacterInfo tMP_CharacterInfo = text.textInfo.characterInfo[num2];
					int lineNumber = tMP_CharacterInfo.lineNumber;
					bool flag2 = ((num2 <= text.maxVisibleCharacters && tMP_CharacterInfo.lineNumber <= text.maxVisibleLines && (text.OverflowMode != TextOverflowModes.Page || tMP_CharacterInfo.pageNumber + 1 == text.pageToDisplay)) ? true : false);
					if (!flag && flag2)
					{
						flag = true;
						vector = transform.TransformPoint(new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.descender, 0f));
						vector2 = transform.TransformPoint(new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.ascender, 0f));
						if (tMP_WordInfo.characterCount == 1)
						{
							flag = false;
							vector3 = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
							vector4 = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
							if (PointIntersectRectangle(position, vector, vector2, vector4, vector3))
							{
								return i;
							}
						}
					}
					if (flag && j == tMP_WordInfo.characterCount - 1)
					{
						flag = false;
						vector3 = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
						vector4 = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
						if (PointIntersectRectangle(position, vector, vector2, vector4, vector3))
						{
							return i;
						}
					}
					else if (flag && lineNumber != text.textInfo.characterInfo[num2 + 1].lineNumber)
					{
						flag = false;
						vector3 = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
						vector4 = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
						if (PointIntersectRectangle(position, vector, vector2, vector4, vector3))
						{
							return i;
						}
					}
				}
				float num3 = DistanceToLine(vector, vector2, position);
				float num4 = DistanceToLine(vector2, vector4, position);
				float num5 = DistanceToLine(vector4, vector3, position);
				float num6 = DistanceToLine(vector3, vector, position);
				float num7 = ((!(num3 < num4)) ? num4 : num3);
				num7 = ((!(num7 < num5)) ? num5 : num7);
				num7 = ((!(num7 < num6)) ? num6 : num7);
				if (num > num7)
				{
					num = num7;
					result = i;
				}
			}
			return result;
		}

		public static int FindIntersectingLink(TextMeshProUGUI text, Vector3 position, Camera camera)
		{
			Transform transform = text.transform;
			ScreenPointToWorldPointInRectangle(transform, position, camera, out position);
			for (int i = 0; i < text.textInfo.linkCount; i++)
			{
				TMP_LinkInfo tMP_LinkInfo = text.textInfo.linkInfo[i];
				bool flag = false;
				Vector3 a = Vector3.zero;
				Vector3 b = Vector3.zero;
				Vector3 zero = Vector3.zero;
				Vector3 zero2 = Vector3.zero;
				for (int j = 0; j < tMP_LinkInfo.linkTextLength; j++)
				{
					int num = tMP_LinkInfo.linkTextfirstCharacterIndex + j;
					TMP_CharacterInfo tMP_CharacterInfo = text.textInfo.characterInfo[num];
					int lineNumber = tMP_CharacterInfo.lineNumber;
					if (!flag)
					{
						flag = true;
						a = transform.TransformPoint(new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.descender, 0f));
						b = transform.TransformPoint(new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.ascender, 0f));
						if (tMP_LinkInfo.linkTextLength == 1)
						{
							flag = false;
							zero = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
							zero2 = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
							if (PointIntersectRectangle(position, a, b, zero2, zero))
							{
								return i;
							}
						}
					}
					if (flag && j == tMP_LinkInfo.linkTextLength - 1)
					{
						flag = false;
						zero = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
						zero2 = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
						if (PointIntersectRectangle(position, a, b, zero2, zero))
						{
							return i;
						}
					}
					else if (flag && lineNumber != text.textInfo.characterInfo[num + 1].lineNumber)
					{
						flag = false;
						zero = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
						zero2 = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
						if (PointIntersectRectangle(position, a, b, zero2, zero))
						{
							return i;
						}
					}
				}
			}
			return -1;
		}

		public static int FindIntersectingLink(TextMeshPro text, Vector3 position, Camera camera)
		{
			Transform transform = text.transform;
			ScreenPointToWorldPointInRectangle(transform, position, camera, out position);
			for (int i = 0; i < text.textInfo.linkCount; i++)
			{
				TMP_LinkInfo tMP_LinkInfo = text.textInfo.linkInfo[i];
				bool flag = false;
				Vector3 a = Vector3.zero;
				Vector3 b = Vector3.zero;
				Vector3 zero = Vector3.zero;
				Vector3 zero2 = Vector3.zero;
				for (int j = 0; j < tMP_LinkInfo.linkTextLength; j++)
				{
					int num = tMP_LinkInfo.linkTextfirstCharacterIndex + j;
					TMP_CharacterInfo tMP_CharacterInfo = text.textInfo.characterInfo[num];
					int lineNumber = tMP_CharacterInfo.lineNumber;
					if (!flag)
					{
						flag = true;
						a = transform.TransformPoint(new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.descender, 0f));
						b = transform.TransformPoint(new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.ascender, 0f));
						if (tMP_LinkInfo.linkTextLength == 1)
						{
							flag = false;
							zero = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
							zero2 = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
							if (PointIntersectRectangle(position, a, b, zero2, zero))
							{
								return i;
							}
						}
					}
					if (flag && j == tMP_LinkInfo.linkTextLength - 1)
					{
						flag = false;
						zero = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
						zero2 = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
						if (PointIntersectRectangle(position, a, b, zero2, zero))
						{
							return i;
						}
					}
					else if (flag && lineNumber != text.textInfo.characterInfo[num + 1].lineNumber)
					{
						flag = false;
						zero = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
						zero2 = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
						if (PointIntersectRectangle(position, a, b, zero2, zero))
						{
							return i;
						}
					}
				}
			}
			return -1;
		}

		public static int FindNearestLink(TextMeshProUGUI text, Vector3 position, Camera camera)
		{
			RectTransform rectTransform = text.rectTransform;
			ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			float num = float.PositiveInfinity;
			int result = 0;
			for (int i = 0; i < text.textInfo.linkCount; i++)
			{
				TMP_LinkInfo tMP_LinkInfo = text.textInfo.linkInfo[i];
				bool flag = false;
				Vector3 vector = Vector3.zero;
				Vector3 vector2 = Vector3.zero;
				Vector3 vector3 = Vector3.zero;
				Vector3 vector4 = Vector3.zero;
				for (int j = 0; j < tMP_LinkInfo.linkTextLength; j++)
				{
					int num2 = tMP_LinkInfo.linkTextfirstCharacterIndex + j;
					TMP_CharacterInfo tMP_CharacterInfo = text.textInfo.characterInfo[num2];
					int lineNumber = tMP_CharacterInfo.lineNumber;
					if (!flag)
					{
						flag = true;
						vector = rectTransform.TransformPoint(new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.descender, 0f));
						vector2 = rectTransform.TransformPoint(new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.ascender, 0f));
						if (tMP_LinkInfo.linkTextLength == 1)
						{
							flag = false;
							vector3 = rectTransform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
							vector4 = rectTransform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
							if (PointIntersectRectangle(position, vector, vector2, vector4, vector3))
							{
								return i;
							}
						}
					}
					if (flag && j == tMP_LinkInfo.linkTextLength - 1)
					{
						flag = false;
						vector3 = rectTransform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
						vector4 = rectTransform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
						if (PointIntersectRectangle(position, vector, vector2, vector4, vector3))
						{
							return i;
						}
					}
					else if (flag && lineNumber != text.textInfo.characterInfo[num2 + 1].lineNumber)
					{
						flag = false;
						vector3 = rectTransform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
						vector4 = rectTransform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
						if (PointIntersectRectangle(position, vector, vector2, vector4, vector3))
						{
							return i;
						}
					}
				}
				float num3 = DistanceToLine(vector, vector2, position);
				float num4 = DistanceToLine(vector2, vector4, position);
				float num5 = DistanceToLine(vector4, vector3, position);
				float num6 = DistanceToLine(vector3, vector, position);
				float num7 = ((!(num3 < num4)) ? num4 : num3);
				num7 = ((!(num7 < num5)) ? num5 : num7);
				num7 = ((!(num7 < num6)) ? num6 : num7);
				if (num > num7)
				{
					num = num7;
					result = i;
				}
			}
			return result;
		}

		public static int FindNearestLink(TextMeshPro text, Vector3 position, Camera camera)
		{
			Transform transform = text.transform;
			ScreenPointToWorldPointInRectangle(transform, position, camera, out position);
			float num = float.PositiveInfinity;
			int result = 0;
			for (int i = 0; i < text.textInfo.linkCount; i++)
			{
				TMP_LinkInfo tMP_LinkInfo = text.textInfo.linkInfo[i];
				bool flag = false;
				Vector3 vector = Vector3.zero;
				Vector3 vector2 = Vector3.zero;
				Vector3 vector3 = Vector3.zero;
				Vector3 vector4 = Vector3.zero;
				for (int j = 0; j < tMP_LinkInfo.linkTextLength; j++)
				{
					int num2 = tMP_LinkInfo.linkTextfirstCharacterIndex + j;
					TMP_CharacterInfo tMP_CharacterInfo = text.textInfo.characterInfo[num2];
					int lineNumber = tMP_CharacterInfo.lineNumber;
					if (!flag)
					{
						flag = true;
						vector = transform.TransformPoint(new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.descender, 0f));
						vector2 = transform.TransformPoint(new Vector3(tMP_CharacterInfo.bottomLeft.x, tMP_CharacterInfo.ascender, 0f));
						if (tMP_LinkInfo.linkTextLength == 1)
						{
							flag = false;
							vector3 = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
							vector4 = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
							if (PointIntersectRectangle(position, vector, vector2, vector4, vector3))
							{
								return i;
							}
						}
					}
					if (flag && j == tMP_LinkInfo.linkTextLength - 1)
					{
						flag = false;
						vector3 = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
						vector4 = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
						if (PointIntersectRectangle(position, vector, vector2, vector4, vector3))
						{
							return i;
						}
					}
					else if (flag && lineNumber != text.textInfo.characterInfo[num2 + 1].lineNumber)
					{
						flag = false;
						vector3 = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.descender, 0f));
						vector4 = transform.TransformPoint(new Vector3(tMP_CharacterInfo.topRight.x, tMP_CharacterInfo.ascender, 0f));
						if (PointIntersectRectangle(position, vector, vector2, vector4, vector3))
						{
							return i;
						}
					}
				}
				float num3 = DistanceToLine(vector, vector2, position);
				float num4 = DistanceToLine(vector2, vector4, position);
				float num5 = DistanceToLine(vector4, vector3, position);
				float num6 = DistanceToLine(vector3, vector, position);
				float num7 = ((!(num3 < num4)) ? num4 : num3);
				num7 = ((!(num7 < num5)) ? num5 : num7);
				num7 = ((!(num7 < num6)) ? num6 : num7);
				if (num > num7)
				{
					num = num7;
					result = i;
				}
			}
			return result;
		}

		private static bool PointIntersectRectangle(Vector3 m, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
		{
			Vector3 vector = b - a;
			Vector3 rhs = m - a;
			Vector3 vector2 = c - b;
			Vector3 rhs2 = m - b;
			float num = Vector3.Dot(vector, rhs);
			float num2 = Vector3.Dot(vector2, rhs2);
			return 0f <= num && num <= Vector3.Dot(vector, vector) && 0f <= num2 && num2 <= Vector3.Dot(vector2, vector2);
		}

		public static bool ScreenPointToWorldPointInRectangle(Transform transform, Vector2 screenPoint, Camera cam, out Vector3 worldPoint)
		{
			worldPoint = Vector2.zero;
			Ray ray = RectTransformUtility.ScreenPointToRay(cam, screenPoint);
			float enter;
			if (!new Plane(transform.rotation * Vector3.back, transform.position).Raycast(ray, out enter))
			{
				return false;
			}
			worldPoint = ray.GetPoint(enter);
			return true;
		}

		private static bool IntersectLinePlane(LineSegment line, Vector3 point, Vector3 normal, out Vector3 intersectingPoint)
		{
			intersectingPoint = Vector3.zero;
			Vector3 vector = line.Point2 - line.Point1;
			Vector3 rhs = line.Point1 - point;
			float num = Vector3.Dot(normal, vector);
			float num2 = 0f - Vector3.Dot(normal, rhs);
			if (Mathf.Abs(num) < Mathf.Epsilon)
			{
				if (num2 == 0f)
				{
					return true;
				}
				return false;
			}
			float num3 = num2 / num;
			if (num3 < 0f || num3 > 1f)
			{
				return false;
			}
			intersectingPoint = line.Point1 + num3 * vector;
			return true;
		}

		public static float DistanceToLine(Vector3 a, Vector3 b, Vector3 point)
		{
			Vector3 vector = b - a;
			Vector3 vector2 = a - point;
			float num = Vector3.Dot(vector, vector2);
			if (num > 0f)
			{
				return Vector3.Dot(vector2, vector2);
			}
			Vector3 vector3 = point - b;
			if (Vector3.Dot(vector, vector3) > 0f)
			{
				return Vector3.Dot(vector3, vector3);
			}
			Vector3 vector4 = vector2 - vector * (num / Vector3.Dot(vector, vector));
			return Vector3.Dot(vector4, vector4);
		}

		public static char ToLowerFast(char c)
		{
			return "-------------------------------- !-#$%&-()*+,-./0123456789:;<=>?@abcdefghijklmnopqrstuvwxyz[-]^_`abcdefghijklmnopqrstuvwxyz{|}~-"[c];
		}

		public static char ToUpperFast(char c)
		{
			return "-------------------------------- !-#$%&-()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[-]^_`ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~-"[c];
		}

		public static int GetSimpleHashCode(string s)
		{
			int num = 0;
			for (int i = 0; i < s.Length; i++)
			{
				num = ((num << 5) + num) ^ s[i];
			}
			return num;
		}

		public static uint GetSimpleHashCodeLowercase(string s)
		{
			uint num = 5381u;
			for (int i = 0; i < s.Length; i++)
			{
				num = ((num << 5) + num) ^ ToLowerFast(s[i]);
			}
			return num;
		}
	}
}
