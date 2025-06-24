using System.Collections;
using UnityEngine;

public class FlyingBlimpLevelUFO : AbstractCollidableObject
{
	public bool typeB;

	[SerializeField]
	private Transform beamPrefab;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private CollisionChild collisionChild;

	private Vector3 ufoMidPoint;

	private Vector3 ufoStopPoint;

	private AbstractPlayerController player;

	private LevelProperties.FlyingBlimp.UFO properties;

	private float speed;

	private float health;

	private float proximity;

	private bool beamTriggered;

	protected override void Awake()
	{
		base.Awake();
		beamTriggered = false;
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		damageDealer = DamageDealer.NewEnemy();
		collisionChild = beamPrefab.GetComponent<CollisionChild>();
		collisionChild.OnPlayerCollision += OnCollisionPlayer;
		float num = Random.Range(0, 2);
		if (num == 0f)
		{
			beamPrefab.SetScale(0f - base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
		}
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		health -= info.damage;
		if (health < 0f)
		{
			Die();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	public void Init(Vector2 startPos, Vector2 midPos, Vector2 endPos, float speed, float health, LevelProperties.FlyingBlimp.UFO properties)
	{
		base.transform.position = startPos;
		ufoMidPoint = midPos;
		ufoStopPoint = endPos;
		this.speed = speed;
		this.properties = properties;
		this.health = health;
		StartCoroutine(to_position_cr());
	}

	private IEnumerator to_position_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		while (base.transform.position != ufoMidPoint)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, ufoMidPoint, speed * CupheadTime.FixedDelta);
			yield return wait;
		}
		base.transform.GetComponent<SpriteRenderer>().sortingOrder = 3;
		while (base.transform.position != ufoStopPoint)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, ufoStopPoint, speed * CupheadTime.FixedDelta);
			yield return wait;
		}
		StartCoroutine(move_cr());
	}

	private IEnumerator move_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		float offset = 50f;
		while (base.transform.position.x > -640f - offset)
		{
			player = PlayerManager.GetNext();
			float dist = player.transform.position.x - base.transform.position.x;
			Vector3 pos = base.transform.position;
			pos.x += (0f - speed) * CupheadTime.FixedDelta;
			base.transform.position = pos;
			proximity = ((!typeB) ? properties.UFOProximityA : properties.UFOProximityB);
			if (dist > 0f - proximity && dist < proximity && !beamTriggered)
			{
				beamTriggered = true;
				StartCoroutine(ActivateBeam());
			}
			yield return wait;
		}
		Die();
	}

	private IEnumerator ActivateBeam()
	{
		base.animator.SetTrigger("StartBeam");
		yield return CupheadTime.WaitForSeconds(this, properties.UFOWarningBeamDuration);
		AudioManager.Play("level_flying_blimp_moon_UFO_fire_laser");
		base.animator.SetTrigger("Continue");
		yield return CupheadTime.WaitForSeconds(this, properties.beamDuration);
		base.animator.SetTrigger("End");
	}

	private void Die()
	{
		Object.Destroy(base.gameObject);
	}
}
