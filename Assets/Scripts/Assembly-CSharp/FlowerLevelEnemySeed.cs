using System.Collections;
using UnityEngine;

public class FlowerLevelEnemySeed : AbstractProjectile
{
	private int fallingSpeed;

	private char type;

	private bool isActive;

	private bool plantSpawned;

	private LevelProperties.Flower properties;

	private FlowerLevelFlower parent;

	[SerializeField]
	private GameObject spawnPoint;

	[Space(10f)]
	[Header("Venus Fly Trap")]
	[SerializeField]
	private Sprite venusSeedTex;

	[SerializeField]
	private GameObject homingVenusFlyTrapSpawn;

	[Space(10f)]
	[Header("Chomper")]
	[SerializeField]
	private Sprite chomperSeedTex;

	[SerializeField]
	private GameObject chomperSpawn;

	[Space(10f)]
	[Header("Mini Flower")]
	[SerializeField]
	private Sprite miniFlowerSeedTex;

	[SerializeField]
	private GameObject miniFlowerSpawn;

	public void OnSeedSpawn(LevelProperties.Flower properties, FlowerLevelFlower parent, char type, bool isActive)
	{
		this.isActive = isActive;
		this.properties = properties;
		switch (type)
		{
		case 'A':
			animator.SetInteger("Type", 1);
			break;
		case 'B':
			animator.SetInteger("Type", 0);
			break;
		case 'C':
			animator.SetInteger("Type", 2);
			SetParryable(true);
			break;
		default:
			Debug.Log(type);
			break;
		}
		fallingSpeed = properties.CurrentState.enemyPlants.fallingSeedSpeed;
		this.type = type;
		this.parent = parent;
		this.parent.OnDeathEvent += KillSeed;
	}

	private void OnSeedLand()
	{
		StartCoroutine(onSeedLand_cr());
	}

	private IEnumerator onSeedLand_cr()
	{
		if (type == 'B')
		{
			if (!plantSpawned)
			{
				animator.Play("Chomper_Landing");
				yield return animator.WaitForAnimationToEnd(this, "Chomper_Landing");
				if (isActive)
				{
					GameObject gameObject = Object.Instantiate(chomperSpawn);
					gameObject.transform.position = base.transform.position;
					gameObject.GetComponent<FlowerLevelChomperSeed>().OnChomperStart(parent, properties.CurrentState.enemyPlants);
					plantSpawned = true;
					base.gameObject.SetActive(false);
				}
			}
		}
		else
		{
			animator.SetTrigger("Landed");
			yield return new WaitForEndOfFrame();
			yield return animator.WaitForAnimationToEnd(this, true);
			animator.Play("Ground_Burst_Start");
		}
	}

	private void KillSeed()
	{
		isActive = false;
	}

	protected override void Awake()
	{
		isActive = true;
		base.transform.localScale = new Vector3(base.transform.localScale.x * (float)MathUtils.PlusOrMinus(), base.transform.localScale.y, base.transform.localScale.z);
		base.Awake();
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	protected override void FixedUpdate()
	{
		base.transform.position += -Vector3.up * ((float)fallingSpeed * CupheadTime.FixedDelta);
		base.FixedUpdate();
	}

	protected override void OnCollisionGround(GameObject hit, CollisionPhase phase)
	{
		if (!plantSpawned)
		{
			if (isActive)
			{
				OnSeedLand();
			}
			else if (type == 'C')
			{
				type = 'A';
				OnSeedLand();
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
			fallingSpeed = 0;
			if (base.CanParry)
			{
				SetParryable(false);
			}
			plantSpawned = true;
		}
		base.OnCollisionGround(hit, phase);
	}

	protected override void OnCollisionEnemy(GameObject hit, CollisionPhase phase)
	{
		if (hit.GetComponent<FlowerLevelFlowerDamageRegion>() != null)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			switch (type)
			{
			case 'A':
				if (hit.GetComponent<FlowerLevelVenusSpawn>() != null)
				{
					isActive = false;
				}
				break;
			case 'B':
				if (hit.GetComponent<FlowerLevelChomperSeed>() != null)
				{
					isActive = false;
				}
				break;
			case 'C':
				if (hit.GetComponent<FlowerLevelMiniFlowerSpawn>() != null)
				{
					isActive = false;
				}
				break;
			}
		}
		OnCollisionEnemyProjectile(hit, phase);
	}

	protected override void Die()
	{
		base.Die();
		GetComponent<Collider2D>().enabled = false;
		StopAllCoroutines();
		parent.OnMiniFlowerDeath();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		parent.OnDeathEvent -= KillSeed;
	}

	private void OnSpawnPlant()
	{
		switch (type)
		{
		case 'A':
		{
			GameObject gameObject = Object.Instantiate(homingVenusFlyTrapSpawn);
			gameObject.transform.position = spawnPoint.transform.position;
			gameObject.GetComponent<FlowerLevelVenusSpawn>().OnVenusSpawn(parent, properties.CurrentState.enemyPlants.venusPlantHP, properties.CurrentState.enemyPlants.venusTurningSpeed, properties.CurrentState.enemyPlants.venusMovmentSpeed, properties.CurrentState.enemyPlants.venusTurningDelay);
			gameObject.transform.localScale = base.transform.localScale;
			break;
		}
		case 'C':
		{
			GameObject gameObject = Object.Instantiate(miniFlowerSpawn);
			gameObject.transform.position = spawnPoint.transform.position;
			gameObject.GetComponent<FlowerLevelMiniFlowerSpawn>().OnMiniFlowerSpawn(parent, properties.CurrentState.enemyPlants);
			gameObject.transform.localScale = base.transform.localScale;
			break;
		}
		}
		plantSpawned = true;
	}

	private void TriggerVine()
	{
		StartCoroutine(triggerVine_cr());
	}

	private IEnumerator triggerVine_cr()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		animator.Play("Trigger_Vine", 1);
	}

	private void OnDeath()
	{
		if (plantSpawned)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void GroundPopAudio()
	{
		AudioManager.Play("flower_vine_groundburst_start");
		emitAudioFromObject.Add("flower_vine_groundburst_start");
	}

	private void VineGrowLargeAudio()
	{
		AudioManager.Play("flower_venus_vine_grow_large");
		emitAudioFromObject.Add("flower_venus_vine_grow_large");
	}

	private void VineGrowMediumAudio()
	{
		AudioManager.Play("flower_venus_vine_grow_medium");
		emitAudioFromObject.Add("flower_venus_vine_grow_medium");
	}

	private void VineGrowSmallAudio()
	{
		AudioManager.Play("flower_venus_vine_grow_small");
		emitAudioFromObject.Add("flower_venus_vine_grow_small");
	}
}
