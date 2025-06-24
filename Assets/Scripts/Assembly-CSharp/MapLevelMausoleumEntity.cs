using System.Collections;
using UnityEngine;

public class MapLevelMausoleumEntity : AbstractMapLevelDependentEntity
{
	[SerializeField]
	private GameObject ToEnable;

	[SerializeField]
	private GameObject ToDisable;

	[SerializeField]
	private Effect poofPrefab;

	[SerializeField]
	private Transform poofRoot;

	[SerializeField]
	private Super superUnlock;

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

	protected override bool ValidateSucess()
	{
		return PlayerData.Data.IsUnlocked(PlayerId.PlayerOne, superUnlock) && PlayerData.Data.IsUnlocked(PlayerId.PlayerTwo, superUnlock);
	}

	protected override bool ValidateCondition(Levels level)
	{
		if (!Level.Won)
		{
			return false;
		}
		if (Level.PreviousLevel != level)
		{
			return false;
		}
		return true;
	}

	public override void DoTransition()
	{
		StartCoroutine(transition_cr());
	}

	private IEnumerator transition_cr()
	{
		poofPrefab.Create(poofRoot.position, new Vector3(0.01f, 0.01f, 1f));
		yield return CupheadTime.WaitForSeconds(this, 0.25f);
		ToEnable.SetActive(true);
		ToDisable.SetActive(false);
		yield return CupheadTime.WaitForSeconds(this, 0.36f);
		base.CurrentState = State.Complete;
	}
}
