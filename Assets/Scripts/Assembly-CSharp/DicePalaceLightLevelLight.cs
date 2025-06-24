using System.Collections;
using UnityEngine;

public class DicePalaceLightLevelLight : LevelProperties.DicePalaceLight.Entity
{
	[SerializeField]
	private GameObject platform1;

	[SerializeField]
	private GameObject platform2;

	[SerializeField]
	private DicePalaceLightLevelLaser laserGroup1;

	[SerializeField]
	private DicePalaceLightLevelLaser laserGroup2;

	[SerializeField]
	private DicePalaceLightLevelObject object1;

	[SerializeField]
	private DicePalaceLightLevelObject object2;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private float speed;

	private float groupSpeed1;

	private float groupSpeed2;

	private float bossMaxHealth;

	private int direction;

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	public override void LevelInit(LevelProperties.DicePalaceLight properties)
	{
		base.LevelInit(properties);
		bossMaxHealth = properties.CurrentHealth;
		object1.Properties(properties.CurrentState.objects);
		object2.Properties(properties.CurrentState.objects);
		Vector2 vector = base.transform.position;
		vector.y = properties.CurrentState.general.bossPosition;
		base.transform.position = vector;
		Vector2 vector2 = platform1.transform.position;
		Vector2 vector3 = platform2.transform.position;
		vector2.y = -360f + properties.CurrentState.general.platformOnePosition;
		vector3.y = -360f + properties.CurrentState.general.platformTwoPosition;
		platform1.transform.position = vector2;
		platform2.transform.position = vector3;
		speed = properties.CurrentState.sixWayLaser.rotationSpeedRange.min;
		StartCoroutine(intro_cr());
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
		GetNewSpeed();
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	private IEnumerator intro_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 3f);
		StartCoroutine(lasers_change_dir_cr());
		StartCoroutine(lasers_turn_on_cr());
		yield return null;
	}

	private IEnumerator lasers_change_dir_cr()
	{
		LevelProperties.DicePalaceLight.SixWayLaser p = base.properties.CurrentState.sixWayLaser;
		string[] directionPattern = p.directionAttackString.GetRandom().Split(',');
		int directionIndex = Random.Range(0, directionPattern.Length);
		laserGroup1.Properties(base.properties.CurrentState.sixWayLaser);
		laserGroup2.Properties(base.properties.CurrentState.sixWayLaser);
		groupSpeed1 = speed;
		groupSpeed2 = 0f - speed;
		StartCoroutine(rotate_cr());
		while (true)
		{
			int.TryParse(directionPattern[directionIndex], out direction);
			if (direction == 1)
			{
				groupSpeed1 = speed;
				groupSpeed2 = 0f - speed;
			}
			else
			{
				groupSpeed1 = 0f - speed;
				groupSpeed2 = speed;
			}
			yield return CupheadTime.WaitForSeconds(this, p.directionTime);
			directionIndex = (directionIndex + 1) % directionPattern.Length;
		}
	}

	private IEnumerator lasers_turn_on_cr()
	{
		LevelProperties.DicePalaceLight.SixWayLaser p = base.properties.CurrentState.sixWayLaser;
		while (true)
		{
			DicePalaceLightLevelLaser currentLaser = ((!Rand.Bool()) ? laserGroup2 : laserGroup1);
			yield return CupheadTime.WaitForSeconds(this, p.attackOffDurationRange.RandomFloat());
			currentLaser.animator.SetTrigger("Warning");
			yield return CupheadTime.WaitForSeconds(this, p.warningDuration);
			currentLaser.animator.SetBool("On", true);
			yield return CupheadTime.WaitForSeconds(this, p.attackOnDurationRange.RandomFloat());
			currentLaser.animator.SetBool("On", false);
			yield return null;
		}
	}

	public void GetNewSpeed()
	{
		MinMax rotationSpeedRange = base.properties.CurrentState.sixWayLaser.rotationSpeedRange;
		float num = base.properties.CurrentHealth / bossMaxHealth;
		float num2 = 1f - num;
		speed = rotationSpeedRange.min + rotationSpeedRange.max * num2;
		if (direction == 1)
		{
			groupSpeed1 = speed;
			groupSpeed2 = 0f - speed;
		}
		else
		{
			groupSpeed1 = 0f - speed;
			groupSpeed2 = speed;
		}
	}

	private IEnumerator rotate_cr()
	{
		while (true)
		{
			laserGroup1.transform.Rotate(Vector3.forward * groupSpeed1 * CupheadTime.Delta);
			laserGroup2.transform.Rotate(Vector3.forward * groupSpeed2 * CupheadTime.Delta);
			yield return null;
		}
	}
}
