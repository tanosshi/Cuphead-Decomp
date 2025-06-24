using System;
using System.Collections;
using UnityEngine;

public class ClownLevelClownHelium : LevelProperties.Clown.Entity
{
	public enum State
	{
		BumperCar = 0,
		Helium = 1,
		Death = 2
	}

	[Serializable]
	public class PipePositions
	{
		public Transform pipeEntrance;

		public int orderNum;
	}

	[SerializeField]
	private Animator tankEffects;

	[SerializeField]
	private ClownLevelClownHorse clownHorse;

	[SerializeField]
	private GameObject head;

	[SerializeField]
	private Transform pivotPoint;

	[SerializeField]
	private Transform heliumStopPos;

	[SerializeField]
	private PipePositions[] pipePositions;

	[SerializeField]
	private ClownLevelDogBalloon regularDog;

	[SerializeField]
	private ClownLevelDogBalloon pinkDog;

	private DamageReceiver damageReceiver;

	private bool headMoving;

	private float angle;

	private float loopSize = 10f;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		damageReceiver = head.GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		head.GetComponent<Collider2D>().enabled = false;
	}

	public override void LevelInit(LevelProperties.Clown properties)
	{
		base.LevelInit(properties);
		pivotPoint.transform.position = heliumStopPos.transform.position;
		headMoving = true;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
	}

	protected virtual float hitPauseCoefficient()
	{
		return (!GetComponent<DamageReceiver>().IsHitPaused) ? 1f : 0f;
	}

	public void StartHeliumTank()
	{
		StopAllCoroutines();
		state = State.Helium;
		StartCoroutine(helium_tank_intro_cr());
	}

	private IEnumerator helium_tank_intro_cr()
	{
		base.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
		float t = 0f;
		float time = 5f;
		Vector2 start = base.transform.position;
		while (t < time)
		{
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, t / time);
			base.transform.position = Vector2.Lerp(start, heliumStopPos.position, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.position = heliumStopPos.position;
		StartCoroutine(helium_tank_cr());
		StartCoroutine(tank_effects_cr());
		StartCoroutine(pipe_puffs_cr());
		yield return null;
	}

	private void SpawnBalloonDogs(ClownLevelDogBalloon dogPrefab, Vector3 startPos, bool isFlipped)
	{
		LevelProperties.Clown.HeliumClown heliumClown = base.properties.CurrentState.heliumClown;
		AbstractPlayerController next = PlayerManager.GetNext();
		if (dogPrefab != null)
		{
			ClownLevelDogBalloon clownLevelDogBalloon = UnityEngine.Object.Instantiate(dogPrefab);
			clownLevelDogBalloon.Init(heliumClown.dogHP, startPos, heliumClown.dogSpeed, next, heliumClown, isFlipped);
		}
	}

	private void HeliumTankSFX()
	{
		AudioManager.Play("clown_helium_tanks");
		emitAudioFromObject.Add("clown_helium_tanks");
	}

	private IEnumerator helium_tank_cr()
	{
		emitAudioFromObject.Add("clown_helium_tanks");
		AudioManager.Play("clown_helium_intro_continue");
		emitAudioFromObject.Add("clown_helium_intro_continue");
		AudioManager.Play("clown_helium_extend_pipes");
		emitAudioFromObject.Add("clown_helium_extend_pipes");
		base.animator.SetTrigger("Continue");
		yield return base.animator.WaitForAnimationToEnd(this, "Helium_Intro_End", 3);
		LevelProperties.Clown.HeliumClown p = base.properties.CurrentState.heliumClown;
		string[] spawnPattern = p.dogSpawnOrder.GetRandom().Split(',');
		string[] delayPattern = p.dogDelayString.GetRandom().Split(',');
		string[] typePattern = p.dogTypeString.GetRandom().Split(',');
		Vector3 pickedPipePos = Vector3.zero;
		float waitTime = 0f;
		bool isFlipped = false;
		int spawnIndex = UnityEngine.Random.Range(0, spawnPattern.Length);
		int delayIndex = UnityEngine.Random.Range(0, delayPattern.Length);
		int typeIndex = UnityEngine.Random.Range(0, typePattern.Length);
		while (true)
		{
			ClownLevelDogBalloon toSpawn = null;
			string[] nextPos = spawnPattern[spawnIndex].Split('-');
			string[] array = nextPos;
			foreach (string s in array)
			{
				int pipeSelection;
				int.TryParse(s, out pipeSelection);
				PipePositions[] array2 = this.pipePositions;
				foreach (PipePositions pipePositions in array2)
				{
					if (pipePositions.orderNum == pipeSelection)
					{
						pickedPipePos = pipePositions.pipeEntrance.position;
						isFlipped = ((pipeSelection > 3) ? true : false);
					}
				}
				if (typePattern[typeIndex][0] == 'R')
				{
					toSpawn = regularDog;
				}
				else if (typePattern[typeIndex][0] == 'P')
				{
					toSpawn = pinkDog;
				}
				SpawnBalloonDogs(toSpawn, pickedPipePos, isFlipped);
				typeIndex = (typeIndex + 1) % typePattern.Length;
			}
			float.TryParse(delayPattern[delayIndex], out waitTime);
			yield return CupheadTime.WaitForSeconds(this, waitTime);
			spawnIndex = (spawnIndex + 1) % spawnPattern.Length;
			delayIndex = (delayIndex + 1) % delayPattern.Length;
			yield return null;
		}
	}

	private IEnumerator pipe_puffs_cr()
	{
		string order = "0,5,1,4,2,3,5,1,2,3";
		int orderIndex = UnityEngine.Random.Range(0, order.Split(',').Length);
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, UnityEngine.Random.Range(0.16f, 0.65f));
			pipePositions[int.Parse(order.Split(',')[orderIndex])].pipeEntrance.GetComponent<Animator>().SetInteger("Type", UnityEngine.Random.Range(0, 3));
			pipePositions[int.Parse(order.Split(',')[orderIndex])].pipeEntrance.GetComponent<Animator>().SetTrigger("OnPuff");
			orderIndex = (orderIndex + 1) % order.Split(',').Length;
			yield return null;
		}
	}

	private IEnumerator tank_effects_cr()
	{
		bool isRight = Rand.Bool();
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, UnityEngine.Random.Range(0.16f, 0.85f));
			tankEffects.SetBool("isLeft", isRight);
			tankEffects.SetTrigger("OnPuff");
			isRight = !isRight;
			yield return null;
		}
	}

	private void SetHead()
	{
		pivotPoint.transform.position = head.transform.position;
		head.GetComponent<Collider2D>().enabled = true;
		base.animator.SetTrigger("Head");
		StartCoroutine(head_moving_cr());
	}

	private void SetBody()
	{
		base.animator.Play("Helium_Idle");
	}

	private IEnumerator head_moving_cr()
	{
		while (true)
		{
			if (headMoving)
			{
				PathMovement();
			}
			yield return null;
		}
	}

	private void PathMovement()
	{
		angle += 1.8f * (float)CupheadTime.Delta * hitPauseCoefficient();
		Vector3 vector = new Vector3((0f - Mathf.Sin(angle)) * loopSize, 0f, 0f);
		Vector3 vector2 = new Vector3(0f, Mathf.Cos(angle) * loopSize, 0f);
		head.transform.position = pivotPoint.position;
		head.transform.position += vector + vector2;
	}

	public void StartDeath()
	{
		StopAllCoroutines();
		StartCoroutine(death_cr());
	}

	private IEnumerator death_cr()
	{
		head.GetComponent<Collider2D>().enabled = false;
		StartCoroutine(head_moving_cr());
		base.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
		head.transform.parent = null;
		StartCoroutine(head_death_cr());
		float moveSpeed = base.properties.CurrentState.heliumClown.heliumMoveSpeed;
		float acceleration = base.properties.CurrentState.heliumClown.heliumAcceleration;
		float endPos = -860f;
		while (base.transform.position.y > endPos)
		{
			moveSpeed += acceleration;
			base.transform.AddPosition(0f, (0f - moveSpeed) * (float)CupheadTime.Delta);
			yield return null;
		}
		yield return null;
	}

	private IEnumerator head_death_cr()
	{
		StartExplosions();
		float moveSpeed = base.properties.CurrentState.heliumClown.heliumMoveSpeed;
		float acceleration = base.properties.CurrentState.heliumClown.heliumAcceleration;
		float endPos = 1060f;
		float t = 0f;
		float time = 1f;
		Vector2 start = head.transform.position;
		Vector2 end = new Vector3(head.transform.position.x, heliumStopPos.transform.position.y - 50f, 0f);
		headMoving = false;
		base.animator.SetTrigger("Dead");
		yield return CupheadTime.WaitForSeconds(this, 1f);
		while (t < time)
		{
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, t / time);
			head.transform.position = Vector2.Lerp(start, end, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		yield return CupheadTime.WaitForSeconds(this, 0.3f);
		while (head.transform.position.y < endPos)
		{
			if ((float)CupheadTime.Delta != 0f)
			{
				moveSpeed += acceleration;
				head.transform.AddPosition(0f, moveSpeed * (float)CupheadTime.Delta);
			}
			yield return null;
		}
		EndExplosions();
		clownHorse.StartCarouselHorse();
		UnityEngine.Object.Destroy(head.gameObject);
		UnityEngine.Object.Destroy(base.gameObject);
		yield return null;
	}

	private void StartExplosions()
	{
		head.GetComponent<LevelBossDeathExploder>().StartExplosion();
	}

	private void EndExplosions()
	{
		head.GetComponent<LevelBossDeathExploder>().StopExplosions();
	}
}
