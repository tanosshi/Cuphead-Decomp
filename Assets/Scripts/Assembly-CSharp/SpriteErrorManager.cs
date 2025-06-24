using System;
using UnityEngine;

public class SpriteErrorManager : AbstractMonoBehaviour
{
	[Serializable]
	public class Pair
	{
		public Sprite sprite;

		[Range(1f, 100f)]
		public int chance = 10;

		[HideInInspector]
		public string name;

		public static void InitializePairs(Pair[] p)
		{
			for (int i = 0; i < p.Length; i++)
			{
				p[i].name = p[i].sprite.name.Replace("_error", string.Empty);
			}
		}
	}

	public const string ERROR_STRING = "_error";

	[SerializeField]
	private Pair[] errors;

	private string lastFrame;

	private SpriteRenderer spriteRenderer;

	protected override void Awake()
	{
		base.Awake();
		spriteRenderer = GetComponent<SpriteRenderer>();
		Pair.InitializePairs(errors);
	}

	protected override void OnWillRenderObject()
	{
		base.OnWillRenderObject();
		Pair[] array = errors;
		foreach (Pair pair in array)
		{
			string text = spriteRenderer.sprite.name;
			if (text == pair.name)
			{
				if (lastFrame == text || pair.chance > UnityEngine.Random.Range(1, 101))
				{
					spriteRenderer.sprite = pair.sprite;
					lastFrame = text;
					Debug.Log(pair.name);
				}
				break;
			}
			lastFrame = string.Empty;
		}
	}
}
