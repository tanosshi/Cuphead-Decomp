using System.Collections;
using UnityEngine;

public class MapLevelDependentObstacle : AbstractMapLevelDependentEntity
{
	[SerializeField]
	private GameObject ToEnable;

	[SerializeField]
	private bool seeEnableOnlyDuringTransition;

	[SerializeField]
	private GameObject ToDisable;

	[SerializeField]
	private bool seeDisabledOnlyDuringTransition;

	[SerializeField]
	private Effect poofPrefab;

	[SerializeField]
	private Material flashMaterial;

	[SerializeField]
	private Transform poofRoot;

	[SerializeField]
	private bool DontPlayPoofSFX;

	protected override void Start()
	{
		base.Start();
	}

	public override void OnConditionNotMet()
	{
		if (ToEnable != null)
		{
			ToEnable.SetActive(false);
		}
		if (ToDisable != null)
		{
			ToDisable.SetActive(true);
		}
	}

	public override void OnConditionMet()
	{
		if (ToEnable != null)
		{
			ToEnable.SetActive(false);
		}
		if (ToDisable != null)
		{
			ToDisable.SetActive(true);
		}
	}

	public override void OnConditionAlreadyMet()
	{
		if (ToEnable != null)
		{
			ToEnable.SetActive(true);
		}
		if (ToDisable != null)
		{
			ToDisable.SetActive(false);
		}
	}

	public override void DoTransition()
	{
		StartCoroutine(transition_cr());
	}

	public void OnChange()
	{
		if (ToEnable != null)
		{
			ToEnable.SetActive(false);
		}
		if (ToDisable != null)
		{
			ToDisable.SetActive(true);
		}
	}

	private IEnumerator transition_cr()
	{
		AudioManager.Play("world_level_bridge_building_poof");
		poofPrefab.Create(poofRoot.position, new Vector3(0.01f, 0.01f, 1f));
		yield return CupheadTime.WaitForSeconds(this, 0.25f);
		SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>(true);
		SpriteRenderer[] array = sprites;
		foreach (SpriteRenderer spriteRenderer in array)
		{
			spriteRenderer.material = flashMaterial;
		}
		if (!seeDisabledOnlyDuringTransition)
		{
			ToEnable.SetActive(true);
		}
		if (seeEnableOnlyDuringTransition)
		{
			ToDisable.SetActive(false);
		}
		yield return CupheadTime.WaitForSeconds(this, 0.04f);
		for (int j = 0; j < 4; j++)
		{
			SpriteRenderer[] array2 = sprites;
			foreach (SpriteRenderer spriteRenderer2 in array2)
			{
				spriteRenderer2.color = Color.white;
			}
			yield return CupheadTime.WaitForSeconds(this, 0.04f);
			SpriteRenderer[] array3 = sprites;
			foreach (SpriteRenderer spriteRenderer3 in array3)
			{
				spriteRenderer3.color = Color.black;
			}
			yield return CupheadTime.WaitForSeconds(this, 0.04f);
		}
		if (seeDisabledOnlyDuringTransition)
		{
			ToEnable.SetActive(true);
		}
		if (!seeEnableOnlyDuringTransition)
		{
			ToDisable.SetActive(false);
		}
		base.CurrentState = State.Complete;
	}
}
