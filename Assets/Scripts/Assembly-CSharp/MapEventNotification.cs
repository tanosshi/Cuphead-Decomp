using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapEventNotification : AbstractMonoBehaviour
{
	public enum Type
	{
		SoulContract = 0,
		Super = 1,
		Coin = 2,
		ThreeCoins = 3,
		Blueprint = 4,
		Tooltip = 5,
		TooltipEquip = 6
	}

	[SerializeField]
	private Image background;

	[SerializeField]
	private TextMeshProUGUI text;

	[SerializeField]
	private TMP_FontAsset BossTitleFont;

	[SerializeField]
	private LocalizationHelper localizationHelper;

	[SerializeField]
	private LocalizationHelper notificationLocalizationHelper;

	[SerializeField]
	private RectTransform sparkleTransformContract;

	[SerializeField]
	private RectTransform sparkleTransformCoin1;

	[SerializeField]
	private RectTransform sparkleTransformCoin2;

	[SerializeField]
	private RectTransform sparkleTransformCoin3;

	[SerializeField]
	private GameObject sparklePrefab;

	[SerializeField]
	private CanvasGroup glyphCanvasGroup;

	[SerializeField]
	private GameObject coin2;

	[SerializeField]
	private GameObject coin3;

	[SerializeField]
	private GameObject super1;

	[SerializeField]
	private GameObject super2;

	[SerializeField]
	private GameObject super3;

	[SerializeField]
	private GameObject confirmGlyph;

	[Header("Tooltips")]
	[SerializeField]
	private CanvasGroup tooltipCanvasGroup;

	[SerializeField]
	private Image tooltipPortrait;

	[SerializeField]
	private LocalizationHelper tooltipLocalizationHelper;

	[SerializeField]
	private GameObject tooltipEquipGlyph;

	[SerializeField]
	private Sprite TurtleSprite;

	[SerializeField]
	private Sprite CanteenSprite;

	[SerializeField]
	private Sprite ShopkeepSprite;

	[SerializeField]
	private Sprite ForkSprite;

	[SerializeField]
	private Sprite KingDiceSprite;

	[SerializeField]
	private Sprite MausoleumSprite;

	private CanvasGroup canvasGroup;

	public bool showing;

	private bool sparkling;

	private bool coinShowing;

	private bool tooltipShowing;

	private bool tooltipEquipShowing;

	private bool superShowing;

	private Animator[] sparkleAnimatorsContract = new Animator[3];

	private Animator[] sparkleAnimatorsCoin1 = new Animator[3];

	private Animator[] sparkleAnimatorsCoin2 = new Animator[3];

	private Animator[] sparkleAnimatorsCoin3 = new Animator[3];

	private float timeBeforeNextSparkleContract = 0.2f;

	private float timeBeforeNextSparkleCoin1 = 0.2f;

	private float timeBeforeNextSparkleCoin2 = 0.2f;

	private float timeBeforeNextSparkleCoin3 = 0.2f;

	[SerializeField]
	private float timeBetweenSparkle = 0.3f;

	private RectTransform[] activeSparkleZones = new RectTransform[0];

	private CupheadInput.AnyPlayerInput input;

	public Queue<Action> EventQueue = new Queue<Action>();

	public static MapEventNotification Current { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		Current = this;
		input = new CupheadInput.AnyPlayerInput();
		canvasGroup = GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0f;
		for (int i = 0; i < sparkleAnimatorsContract.Length; i++)
		{
			sparkleAnimatorsContract[i] = UnityEngine.Object.Instantiate(sparklePrefab, sparkleTransformContract).GetComponent<Animator>();
		}
		for (int j = 0; j < sparkleAnimatorsCoin1.Length; j++)
		{
			sparkleAnimatorsCoin1[j] = UnityEngine.Object.Instantiate(sparklePrefab, sparkleTransformCoin1).GetComponent<Animator>();
		}
		for (int k = 0; k < sparkleAnimatorsCoin2.Length; k++)
		{
			sparkleAnimatorsCoin2[k] = UnityEngine.Object.Instantiate(sparklePrefab, sparkleTransformCoin2).GetComponent<Animator>();
		}
		for (int l = 0; l < sparkleAnimatorsCoin3.Length; l++)
		{
			sparkleAnimatorsCoin3[l] = UnityEngine.Object.Instantiate(sparklePrefab, sparkleTransformCoin3).GetComponent<Animator>();
		}
		base.gameObject.SetActive(false);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (Current == this)
		{
			Current = null;
		}
	}

	protected override void Update()
	{
		base.Update();
		if (superShowing)
		{
			if (input.GetAnyButtonDown())
			{
				StartCoroutine(tweenOut_cr());
				base.animator.SetTrigger("hide_super");
				superShowing = false;
			}
			timeBeforeNextSparkleCoin1 -= CupheadTime.Delta;
			for (int i = 0; i < sparkleAnimatorsCoin1.Length; i++)
			{
				if (!(timeBeforeNextSparkleCoin1 > 0f) && sparkleAnimatorsCoin1[i].GetCurrentAnimatorStateInfo(0).IsName("Empty"))
				{
					timeBeforeNextSparkleCoin1 = timeBetweenSparkle;
					sparkleAnimatorsCoin1[i].transform.position = new Vector3(sparkleTransformCoin1.position.x + UnityEngine.Random.Range(sparkleTransformCoin1.sizeDelta.x * -0.5f, sparkleTransformCoin1.sizeDelta.x * 0.5f), sparkleTransformCoin1.position.y + UnityEngine.Random.Range(sparkleTransformCoin1.sizeDelta.y * -0.5f, sparkleTransformCoin1.sizeDelta.y * 0.5f), 101f);
					sparkleAnimatorsCoin1[i].SetTrigger(UnityEngine.Random.Range(0, 5).ToString());
				}
			}
		}
		if (tooltipShowing && input.GetAnyButtonDown())
		{
			StartCoroutine(tweenOut_cr());
			base.animator.SetTrigger("hide_tooltip");
			tooltipShowing = false;
		}
		if (tooltipEquipShowing && input.GetButtonDown(CupheadButton.EquipMenu))
		{
			StartCoroutine(tweenOut_cr(0.5f));
			base.animator.SetTrigger("hide_tooltip");
			tooltipShowing = false;
		}
		if (coinShowing)
		{
			if (input.GetAnyButtonDown())
			{
				StartCoroutine(tweenOut_cr());
				base.animator.SetTrigger("hide_coin");
				coinShowing = false;
			}
			timeBeforeNextSparkleCoin1 -= CupheadTime.Delta;
			timeBeforeNextSparkleCoin2 -= CupheadTime.Delta;
			timeBeforeNextSparkleCoin3 -= CupheadTime.Delta;
			for (int j = 0; j < sparkleAnimatorsCoin1.Length; j++)
			{
				if (!(timeBeforeNextSparkleCoin1 > 0f) && sparkleAnimatorsCoin1[j].GetCurrentAnimatorStateInfo(0).IsName("Empty"))
				{
					timeBeforeNextSparkleCoin1 = timeBetweenSparkle;
					sparkleAnimatorsCoin1[j].transform.position = new Vector3(sparkleTransformCoin1.position.x + UnityEngine.Random.Range(sparkleTransformCoin1.sizeDelta.x * -0.5f, sparkleTransformCoin1.sizeDelta.x * 0.5f), sparkleTransformCoin1.position.y + UnityEngine.Random.Range(sparkleTransformCoin1.sizeDelta.y * -0.5f, sparkleTransformCoin1.sizeDelta.y * 0.5f), 101f);
					sparkleAnimatorsCoin1[j].SetTrigger(UnityEngine.Random.Range(0, 5).ToString());
				}
			}
			for (int k = 0; k < sparkleAnimatorsCoin2.Length; k++)
			{
				if (!(timeBeforeNextSparkleCoin2 > 0f) && sparkleAnimatorsCoin2[k].GetCurrentAnimatorStateInfo(0).IsName("Empty"))
				{
					timeBeforeNextSparkleCoin2 = timeBetweenSparkle;
					sparkleAnimatorsCoin2[k].transform.position = new Vector3(sparkleTransformCoin2.position.x + UnityEngine.Random.Range(sparkleTransformCoin2.sizeDelta.x * -0.5f, sparkleTransformCoin2.sizeDelta.x * 0.5f), sparkleTransformCoin2.position.y + UnityEngine.Random.Range(sparkleTransformCoin2.sizeDelta.y * -0.5f, sparkleTransformCoin2.sizeDelta.y * 0.5f), 101f);
					sparkleAnimatorsCoin2[k].SetTrigger(UnityEngine.Random.Range(0, 5).ToString());
				}
			}
			for (int l = 0; l < sparkleAnimatorsCoin3.Length; l++)
			{
				if (!(timeBeforeNextSparkleCoin3 > 0f) && sparkleAnimatorsCoin3[l].GetCurrentAnimatorStateInfo(0).IsName("Empty"))
				{
					timeBeforeNextSparkleCoin3 = timeBetweenSparkle;
					sparkleAnimatorsCoin3[l].transform.position = new Vector3(sparkleTransformCoin3.position.x + UnityEngine.Random.Range(sparkleTransformCoin3.sizeDelta.x * -0.5f, sparkleTransformCoin3.sizeDelta.x * 0.5f), sparkleTransformCoin3.position.y + UnityEngine.Random.Range(sparkleTransformCoin3.sizeDelta.y * -0.5f, sparkleTransformCoin3.sizeDelta.y * 0.5f), 101f);
					sparkleAnimatorsCoin3[l].SetTrigger(UnityEngine.Random.Range(0, 5).ToString());
				}
			}
		}
		if (!sparkling)
		{
			return;
		}
		if (input.GetAnyButtonDown())
		{
			StartCoroutine(tweenOut_cr());
			base.animator.SetTrigger("hide");
			sparkling = false;
		}
		timeBeforeNextSparkleContract -= CupheadTime.Delta;
		for (int m = 0; m < sparkleAnimatorsContract.Length; m++)
		{
			if (!(timeBeforeNextSparkleContract > 0f) && sparkleAnimatorsContract[m].GetCurrentAnimatorStateInfo(0).IsName("Empty"))
			{
				timeBeforeNextSparkleContract = timeBetweenSparkle;
				sparkleAnimatorsContract[m].transform.position = new Vector3(sparkleTransformContract.position.x + UnityEngine.Random.Range(sparkleTransformContract.sizeDelta.x * -0.5f, sparkleTransformContract.sizeDelta.x * 0.5f), sparkleTransformContract.position.y + UnityEngine.Random.Range(sparkleTransformContract.sizeDelta.y * -0.5f, sparkleTransformContract.sizeDelta.y * 0.5f), 101f);
				sparkleAnimatorsContract[m].SetTrigger(UnityEngine.Random.Range(0, 5).ToString());
			}
		}
	}

	public void SparkleStart()
	{
		sparkling = true;
		StartCoroutine(showGlyphs_cr());
	}

	protected IEnumerator showGlyphs_cr()
	{
		yield return new WaitForSeconds(0.5f);
		float t = 0f;
		while (t < 0.2f)
		{
			float val = t / 0.2f;
			glyphCanvasGroup.alpha = Mathf.Lerp(0f, 1f, val);
			t += Time.deltaTime;
			yield return null;
		}
		glyphCanvasGroup.alpha = 1f;
		while (!input.GetButtonDown(CupheadButton.Accept))
		{
			yield return null;
		}
		base.animator.SetTrigger("hide");
		yield return null;
		yield return base.animator.WaitForAnimationToEnd(this, "anim_map_ui_contract_end", 0);
		base.gameObject.SetActive(false);
	}

	public void ShowEvent(Type eventType)
	{
		EventQueue.Enqueue(delegate
		{
			InternalShowEvent(eventType);
		});
	}

	private void InternalShowEvent(Type eventType)
	{
		base.gameObject.SetActive(true);
		super1.SetActive(false);
		super2.SetActive(false);
		super3.SetActive(false);
		coin2.SetActive(false);
		coin3.SetActive(false);
		InterruptingPrompt.SetCanInterrupt(true);
		switch (eventType)
		{
		case Type.SoulContract:
			confirmGlyph.SetActive(true);
			activeSparkleZones = new RectTransform[1] { sparkleTransformContract };
			AudioManager.Play("world_map_soul_contract_open");
			AudioManager.PlayLoop("world_map_soul_contract_stamp_shimmer_loop");
			base.animator.SetTrigger("show");
			localizationHelper.ApplyTranslation(Localization.Find(Level.PreviousLevel.ToString()));
			localizationHelper.textMeshProComponent.text = localizationHelper.textMeshProComponent.text.ToUpper();
			notificationLocalizationHelper.ApplyTranslation(Localization.Find("UnlockContract"), new LocalizationHelper.LocalizationSubtext[1]
			{
				new LocalizationHelper.LocalizationSubtext("CONTRACT", Localization.Find(Level.PreviousLevel.ToString()).translation.text)
			});
			text.font = BossTitleFont;
			break;
		case Type.Super:
			confirmGlyph.SetActive(true);
			base.animator.SetTrigger("show_super");
			activeSparkleZones = new RectTransform[1] { sparkleTransformCoin1 };
			AudioManager.Stop("world_level_bridge_building_poof");
			AudioManager.Play("world_map_super_open");
			AudioManager.PlayLoop("world_map_super_loop");
			StartCoroutine(SuperInRoutine());
			notificationLocalizationHelper.ApplyTranslation(Localization.Find("UnlockSuper"));
			switch (PlayerData.Data.CurrentMap)
			{
			case Scenes.scene_map_world_1:
				super1.SetActive(true);
				break;
			case Scenes.scene_map_world_2:
				super2.SetActive(true);
				break;
			case Scenes.scene_map_world_3:
				super3.SetActive(true);
				break;
			}
			break;
		case Type.Coin:
			confirmGlyph.SetActive(true);
			activeSparkleZones = new RectTransform[1] { sparkleTransformCoin1 };
			AudioManager.Play("world_map_coin_open");
			StartCoroutine(CoinInRoutine());
			notificationLocalizationHelper.ApplyTranslation(Localization.Find("GotACoin"));
			base.animator.SetTrigger("show_coin");
			break;
		case Type.ThreeCoins:
			confirmGlyph.SetActive(true);
			activeSparkleZones = new RectTransform[3] { sparkleTransformCoin1, sparkleTransformCoin2, sparkleTransformCoin3 };
			coin2.SetActive(true);
			coin3.SetActive(true);
			AudioManager.Play("world_map_coin_open");
			StartCoroutine(CoinInRoutine());
			notificationLocalizationHelper.ApplyTranslation(Localization.Find("GotThreeCoins"));
			base.animator.SetTrigger("show_coin");
			break;
		case Type.Tooltip:
			confirmGlyph.SetActive(true);
			tooltipEquipGlyph.SetActive(false);
			StartCoroutine(TooltipInRoutine());
			base.animator.SetTrigger("show_tooltip");
			AudioManager.Play("menu_cardup");
			break;
		case Type.TooltipEquip:
			confirmGlyph.SetActive(false);
			tooltipEquipGlyph.SetActive(true);
			StartCoroutine(TooltipEquipInRoutine());
			base.animator.SetTrigger("show_tooltip");
			AudioManager.Play("menu_cardup");
			break;
		}
		showing = true;
		StartCoroutine(tweenIn_cr());
	}

	public void ShowTooltipEvent(TooltipEvent tooltipEvent)
	{
		InterruptingPrompt.SetCanInterrupt(true);
		switch (tooltipEvent)
		{
		case TooltipEvent.Turtle:
			tooltipPortrait.sprite = TurtleSprite;
			tooltipLocalizationHelper.ApplyTranslation(Localization.Find("Pacifist_Tooltip_NewAudioVisMode"));
			ShowEvent(Type.Tooltip);
			break;
		case TooltipEvent.Canteen:
			tooltipPortrait.sprite = CanteenSprite;
			tooltipLocalizationHelper.ApplyTranslation(Localization.Find("Canteen_Tooltip_ShmupWeapons"));
			ShowEvent(Type.Tooltip);
			break;
		case TooltipEvent.ShopKeep:
			tooltipPortrait.sprite = ShopkeepSprite;
			tooltipLocalizationHelper.ApplyTranslation(Localization.Find("Shopkeeper_Tooltip_NewPurchase"));
			ShowEvent(Type.TooltipEquip);
			break;
		case TooltipEvent.Professional:
			tooltipPortrait.sprite = ForkSprite;
			tooltipLocalizationHelper.ApplyTranslation(Localization.Find("Professional_Tooltip_SuperEquip"));
			ShowEvent(Type.Tooltip);
			break;
		case TooltipEvent.KingDice:
			tooltipPortrait.sprite = KingDiceSprite;
			tooltipLocalizationHelper.ApplyTranslation(Localization.Find("KingDice_Tooltip_RegularSoulContracts"));
			ShowEvent(Type.Tooltip);
			break;
		case TooltipEvent.Mausoleum:
			tooltipPortrait.sprite = MausoleumSprite;
			tooltipLocalizationHelper.ApplyTranslation(Localization.Find("Chalice_Tooltip_NewSuperEquip"));
			ShowEvent(Type.TooltipEquip);
			break;
		default:
			tooltipPortrait.sprite = null;
			tooltipLocalizationHelper.ApplyTranslation(Localization.Find("Shopkeeper_Tooltip_NewPurchase"));
			ShowEvent(Type.Tooltip);
			break;
		}
	}

	protected IEnumerator CoinInRoutine()
	{
		yield return new WaitForSeconds(1f);
		coinShowing = true;
	}

	protected IEnumerator TooltipInRoutine()
	{
		yield return new WaitForSeconds(1f);
		tooltipShowing = true;
	}

	protected IEnumerator TooltipEquipInRoutine()
	{
		yield return new WaitForSeconds(1f);
		tooltipEquipShowing = true;
	}

	protected IEnumerator SuperInRoutine()
	{
		yield return new WaitForSeconds(1f);
		superShowing = true;
	}

	protected IEnumerator tweenIn_cr()
	{
		float t = 0f;
		while (t < 0.2f)
		{
			float val = t / 0.2f;
			canvasGroup.alpha = Mathf.Lerp(0f, 1f, val);
			t += Time.deltaTime;
			yield return null;
		}
		canvasGroup.alpha = 1f;
	}

	protected IEnumerator tweenOut_cr(float time = 1.5f)
	{
		AudioManager.FadeSFXVolume("world_map_soul_contract_stamp_shimmer_loop", 0f, 5f);
		AudioManager.FadeSFXVolume("world_map_super_loop", 0f, 5f);
		yield return new WaitForSeconds(time);
		float t = 0f;
		while (t < 0.2f)
		{
			float val = t / 0.2f;
			canvasGroup.alpha = Mathf.Lerp(1f, 0f, val);
			t += Time.deltaTime;
			yield return null;
		}
		canvasGroup.alpha = 0f;
		while (InterruptingPrompt.IsInterrupting())
		{
			yield return null;
		}
		showing = false;
		base.gameObject.SetActive(false);
	}

	private IEnumerator text_cr()
	{
		yield return StartCoroutine(textScale_cr(0.9f, 1.1f, 0.5f));
		yield return StartCoroutine(textScale_cr(1.1f, 0.9f, 0.5f));
		while (!input.GetButtonDown(CupheadButton.Accept))
		{
			yield return null;
		}
		showing = false;
		base.gameObject.SetActive(false);
	}

	protected IEnumerator textScale_cr(float start, float end, float time)
	{
		float t = 0f;
		while (t < time)
		{
			float val = t / time;
			text.transform.localScale = Vector3.one * EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, start, end, val);
			t += Time.deltaTime;
			yield return null;
		}
		text.transform.localScale = Vector3.one * end;
		yield return null;
	}
}
