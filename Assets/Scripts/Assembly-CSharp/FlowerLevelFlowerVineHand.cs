using System.Collections;
using UnityEngine;

public class FlowerLevelFlowerVineHand : AbstractCollidableObject
{
	[SerializeField]
	private int platformOneXPosition;

	[SerializeField]
	private int platformTwoXPosition;

	[SerializeField]
	private int platformThreeXPosition;

	[Space(10f)]
	[SerializeField]
	private int vineHandSpawnYPosition;

	private int holdCount;

	private float firstHoldDelay;

	private float secondHoldDelay;

	private Vector3 spawnPosition;

	private DamageDealer damageDealer;

	public void OnVineHandSpawn(float firstHold, float secondHold, int attackPosOne, int attackPosTwo = 0)
	{
		holdCount = 0;
		int num = attackPosOne;
		for (int i = 0; i < 2; i++)
		{
			if (i == 1)
			{
				if (attackPosTwo == 0)
				{
					break;
				}
				num = attackPosTwo;
			}
			switch (num)
			{
			case 1:
				spawnPosition = new Vector3(platformOneXPosition, vineHandSpawnYPosition, 0f);
				break;
			case 2:
				spawnPosition = new Vector3(platformTwoXPosition, vineHandSpawnYPosition, 0f);
				break;
			case 3:
				spawnPosition = new Vector3(platformThreeXPosition, vineHandSpawnYPosition, 0f);
				break;
			default:
				Debug.Log("Spawn position: " + num + " does not exist!");
				break;
			}
			Create(firstHold, secondHold);
		}
	}

	public void InitVineHand(float first, float second)
	{
		firstHoldDelay = first;
		secondHoldDelay = second;
		damageDealer = DamageDealer.NewEnemy();
	}

	private void Create(float first, float second)
	{
		GameObject gameObject = Object.Instantiate(base.gameObject, spawnPosition, Quaternion.identity);
		gameObject.GetComponent<FlowerLevelFlowerVineHand>().InitVineHand(first, second);
	}

	protected override void Update()
	{
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
		base.Update();
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		if (damageDealer != null)
		{
			damageDealer.DealDamage(hit);
		}
		base.OnCollisionPlayer(hit, phase);
	}

	private IEnumerator holdDelay(float delay)
	{
		base.animator.SetBool("OnHold", true);
		if (delay != 0f)
		{
			yield return CupheadTime.WaitForSeconds(this, delay);
		}
		base.animator.SetBool("OnHold", false);
		yield return null;
	}

	private void OnHold()
	{
		if (holdCount == 0)
		{
			StartCoroutine(holdDelay(firstHoldDelay));
			holdCount++;
		}
		else
		{
			StartCoroutine(holdDelay(secondHoldDelay));
		}
	}

	private void OnRetracted()
	{
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}

	private void ContinueBackAnimation()
	{
		base.animator.SetTrigger("ContinueBackAnimation");
	}

	private void SoundVineHandGrow()
	{
		AudioManager.Play("flower_vinehand_grow");
		emitAudioFromObject.Add("flower_vinehand_grow");
	}

	private void SoundVineHandGrowContinue()
	{
		AudioManager.Play("flower_vinehand_grow_continue");
		emitAudioFromObject.Add("flower_vinehand_grow_continue");
	}

	private void SoundVineHandGrowRetract()
	{
		AudioManager.Play("flower_vinehand_grow_retract");
		emitAudioFromObject.Add("flower_vinehand_grow_retract");
	}
}
