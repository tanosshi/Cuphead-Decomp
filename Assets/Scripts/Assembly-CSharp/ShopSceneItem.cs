using System.Collections;
using UnityEngine;

public class ShopSceneItem : AbstractMonoBehaviour
{
	public enum State
	{
		Ready = 0,
		Busy = 1
	}

	public enum SpriteState
	{
		Inactive = 0,
		Selected = 1,
		Purchased = 2
	}

	private const float FLOAT_TIME = 0.3f;

	public ItemType itemType;

	[Space(5f)]
	public Weapon weapon = Weapon.None;

	public Super super = Super.None;

	public Charm charm = Charm.None;

	[Header("Sprites")]
	public SpriteRenderer spriteInactive;

	public SpriteRenderer spriteSelected;

	public SpriteRenderer spritePurchased;

	public SpriteRenderer spriteShadowObject;

	public Sprite spriteShadow;

	public float cantPurchaseYMovementPosition;

	public float cantPurchaseYMovementValue;

	public Vector3 poofOffset;

	[HideInInspector]
	public Vector3 endPosition;

	public PlayerId player;

	[HideInInspector]
	public Vector3 startPosition;

	public Vector3 originalShadowScale;

	public SpriteRenderer buyAnimation;

	private Coroutine selectionCoroutine;

	public State state { get; private set; }

	public bool Purchased
	{
		get
		{
			switch (itemType)
			{
			case ItemType.Weapon:
				return PlayerData.Data.IsUnlocked(player, weapon);
			case ItemType.Super:
				return PlayerData.Data.IsUnlocked(player, super);
			case ItemType.Charm:
				return PlayerData.Data.IsUnlocked(player, charm);
			default:
				Debug.LogWarning(string.Concat("ItemType '", itemType, "' not yet configured"));
				return false;
			}
		}
	}

	public string DisplayName
	{
		get
		{
			switch (itemType)
			{
			case ItemType.Weapon:
				return WeaponProperties.GetDisplayName(weapon);
			case ItemType.Super:
				return WeaponProperties.GetDisplayName(super);
			case ItemType.Charm:
				return WeaponProperties.GetDisplayName(charm);
			default:
				Debug.LogWarning(string.Concat("ItemType '", itemType, "' not yet configured"));
				return string.Empty;
			}
		}
	}

	public string Subtext
	{
		get
		{
			switch (itemType)
			{
			case ItemType.Weapon:
				return WeaponProperties.GetSubtext(weapon);
			case ItemType.Super:
				return WeaponProperties.GetSubtext(super);
			case ItemType.Charm:
				return WeaponProperties.GetSubtext(charm);
			default:
				Debug.LogWarning(string.Concat("ItemType '", itemType, "' not yet configured"));
				return string.Empty;
			}
		}
	}

	public string Description
	{
		get
		{
			switch (itemType)
			{
			case ItemType.Weapon:
				return WeaponProperties.GetDescription(weapon);
			case ItemType.Super:
				return WeaponProperties.GetDescription(super);
			case ItemType.Charm:
				return WeaponProperties.GetDescription(charm);
			default:
				Debug.LogWarning(string.Concat("ItemType '", itemType, "' not yet configured"));
				return string.Empty;
			}
		}
	}

	public int Value
	{
		get
		{
			switch (itemType)
			{
			case ItemType.Weapon:
				return WeaponProperties.GetValue(weapon);
			case ItemType.Super:
				return WeaponProperties.GetValue(super);
			case ItemType.Charm:
				return WeaponProperties.GetValue(charm);
			default:
				Debug.LogWarning(string.Concat("ItemType '", itemType, "' not yet configured"));
				return 0;
			}
		}
	}

	public void Init(PlayerId player)
	{
		startPosition = base.transform.localPosition;
		endPosition = startPosition;
		endPosition.y += 40f;
		this.player = player;
		if (Purchased)
		{
			SetSprite(SpriteState.Purchased);
		}
		else
		{
			SetSprite(SpriteState.Inactive);
		}
	}

	private void SetSprite(SpriteState spriteState)
	{
		spriteInactive.enabled = false;
		spriteSelected.enabled = false;
		spritePurchased.enabled = false;
		switch (spriteState)
		{
		case SpriteState.Inactive:
			spriteInactive.enabled = true;
			break;
		case SpriteState.Selected:
			spriteSelected.enabled = true;
			break;
		case SpriteState.Purchased:
			spritePurchased.enabled = true;
			break;
		default:
			Debug.LogWarning(string.Concat("SpriteState '", spriteState, "' not yet configured"));
			break;
		}
	}

	public void Select()
	{
		if (state == State.Ready)
		{
			if (!Purchased)
			{
				SetSprite(SpriteState.Selected);
			}
			StopAllCoroutines();
			StartCoroutine(float_cr(base.transform.localPosition, endPosition, spriteShadowObject.transform.localScale, originalShadowScale * 0.8f));
		}
	}

	public void Deselect()
	{
		if (state == State.Ready)
		{
			if (!Purchased)
			{
				SetSprite(SpriteState.Inactive);
			}
			StopAllCoroutines();
			StartCoroutine(float_cr(base.transform.localPosition, startPosition, spriteShadowObject.transform.localScale, originalShadowScale));
		}
	}

	private void UpdateFloat(float value)
	{
		base.transform.localPosition = Vector3.Lerp(startPosition, endPosition, value);
	}

	private void UpdatePurchasedColor(float value)
	{
		Color white = Color.white;
		Color black = Color.black;
		spritePurchased.color = Color.Lerp(white, black, value);
	}

	public bool Purchase()
	{
		if (state != State.Ready)
		{
			return false;
		}
		if (Purchased)
		{
			return false;
		}
		bool flag = false;
		switch (itemType)
		{
		case ItemType.Weapon:
			flag = PlayerData.Data.Buy(player, weapon);
			break;
		case ItemType.Super:
			flag = PlayerData.Data.Buy(player, super);
			break;
		case ItemType.Charm:
			flag = PlayerData.Data.Buy(player, charm);
			break;
		default:
			Debug.LogWarning(string.Concat("ItemType '", itemType, "' not yet configured"));
			break;
		}
		if (flag)
		{
			Debug.Log("SUCCESS PURCHASE");
			StartCoroutine(purchase_cr());
			if (HasBoughtEverything())
			{
				Debug.Log("BoughtAllItems");
				OnlineManager.Instance.Interface.UnlockAchievement(player, "BoughtAllItems");
			}
			if (!PlayerData.Data.hasMadeFirstPurchase)
			{
				PlayerData.Data.shouldShowShopkeepTooltip = true;
				PlayerData.Data.hasMadeFirstPurchase = true;
				PlayerData.SaveCurrentFile();
			}
		}
		return flag;
	}

	private bool HasBoughtEverything()
	{
		ShopSceneItem[] charmItems = ShopScene.Current.GetCharmItems(player);
		for (int i = 0; i < charmItems.Length; i++)
		{
			if (!charmItems[i].Purchased)
			{
				return false;
			}
		}
		charmItems = ShopScene.Current.GetWeaponItems(player);
		for (int j = 0; j < charmItems.Length; j++)
		{
			if (!charmItems[j].Purchased)
			{
				return false;
			}
		}
		return true;
	}

	private IEnumerator float_cr(Vector3 start, Vector3 end, Vector3 startShadowScale, Vector3 endShadowScale)
	{
		float t = 0f;
		float time = 0.3f * (Vector3.Distance(start, end) / Vector3.Distance(startPosition, endPosition));
		while (t < time)
		{
			float val = t / time;
			base.transform.localPosition = Vector3.Lerp(start, end, EaseUtils.Ease(EaseUtils.EaseType.easeOutSine, 0f, 1f, val));
			spriteShadowObject.transform.localScale = Vector3.Lerp(startShadowScale, endShadowScale, EaseUtils.Ease(EaseUtils.EaseType.easeOutSine, 0f, 1f, val));
			t += base.LocalDeltaTime;
			yield return null;
		}
		base.transform.localPosition = end;
		yield return null;
	}

	private IEnumerator purchase_cr()
	{
		state = State.Busy;
		SetSprite(SpriteState.Purchased);
		SpriteRenderer buyAnim = Object.Instantiate(buyAnimation, GetComponentInChildren<SpriteRenderer>().bounds.center, Quaternion.identity);
		buyAnim.sortingOrder = GetComponentInChildren<SpriteRenderer>().sortingOrder;
		spriteShadowObject.gameObject.SetActive(false);
		yield return TweenValue(0f, 1f, 0.0001f, EaseUtils.EaseType.linear, UpdatePurchasedColor);
		state = State.Ready;
		base.gameObject.SetActive(false);
	}
}
