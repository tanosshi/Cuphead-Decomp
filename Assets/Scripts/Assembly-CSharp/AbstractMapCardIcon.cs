using System.Collections;
using System.Collections.Generic;
using RektTransform;
using UnityEngine;
using UnityEngine.UI;

public class AbstractMapCardIcon : AbstractMonoBehaviour
{
	private const float FRAME_DELAY = 0.07f;

	[SerializeField]
	private Image iconImage;

	private Sprite[] icons;

	public void SetIcons(string iconPath, bool isGrey)
	{
		List<Sprite> list = new List<Sprite>();
		Sprite sprite = Resources.Load<Sprite>(iconPath);
		if (sprite != null)
		{
			list.Add(sprite);
		}
		if (iconPath != WeaponProperties.GetIconPath(Weapon.None))
		{
			for (int i = 0; i < 4; i++)
			{
				string text = ((!isGrey) ? "_000" : "_grey_000");
				Sprite sprite2 = Resources.Load<Sprite>(iconPath + text + i);
				if (!(sprite2 == null))
				{
					list.Add(sprite2);
				}
			}
		}
		icons = list.ToArray();
		StopAllCoroutines();
		StartCoroutine(animate_cr());
	}

	public void SetIcons(string iconPath)
	{
		List<Sprite> list = new List<Sprite>();
		Sprite sprite = Resources.Load<Sprite>(iconPath);
		if (sprite != null)
		{
			list.Add(sprite);
		}
		Sprite sprite2 = Resources.Load<Sprite>(iconPath);
		list.Add(sprite2);
		icons = list.ToArray();
		SetIcon(sprite2);
	}

	public virtual void SelectIcon()
	{
		if (base.animator != null)
		{
			base.animator.Play("Select");
		}
	}

	private void SetIcon(Sprite sprite)
	{
		if (!(sprite == null))
		{
			iconImage.sprite = sprite;
			iconImage.rectTransform.SetSize(sprite.texture.width, sprite.texture.height);
		}
	}

	private IEnumerator animate_cr()
	{
		int i = 0;
		while (true)
		{
			yield return new WaitForSeconds(0.07f);
			if (icons == null || icons.Length < 1)
			{
				SetIcon(null);
				continue;
			}
			i++;
			if (i > icons.Length - 1)
			{
				i = 0;
			}
			SetIcon(icons[i]);
		}
	}
}
