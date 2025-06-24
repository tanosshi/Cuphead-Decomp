using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIImageAnimationLoop : AbstractMonoBehaviour
{
	public enum Mode
	{
		Linear = 0,
		Shuffle = 1,
		Random = 2
	}

	[SerializeField]
	private Mode mode;

	[SerializeField]
	private float frameDelay = 0.07f;

	[SerializeField]
	private Sprite[] sprites;

	[SerializeField]
	private bool IgnoreGlobalTime;

	private Image image;

	protected override void Awake()
	{
		base.Awake();
		image = GetComponent<Image>();
		ignoreGlobalTime = IgnoreGlobalTime;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(anim_cr());
	}

	private void Shuffle()
	{
		List<Sprite> list = new List<Sprite>(sprites);
		System.Random random = new System.Random();
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int index = random.Next(num + 1);
			Sprite value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
		sprites = list.ToArray();
	}

	private IEnumerator anim_cr()
	{
		if (mode == Mode.Shuffle)
		{
			Shuffle();
		}
		int i = 0;
		while (true)
		{
			image.sprite = sprites[i];
			i++;
			if (i >= sprites.Length)
			{
				i = 0;
			}
			if (!IgnoreGlobalTime)
			{
				yield return CupheadTime.WaitForSeconds(this, frameDelay);
			}
			else
			{
				yield return new WaitForSeconds(frameDelay);
			}
			if (mode == Mode.Random)
			{
				Shuffle();
			}
		}
	}
}
