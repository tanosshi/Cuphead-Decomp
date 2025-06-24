using System.Collections;
using UnityEngine;

public class RobotLevelHatchShotbot : AbstractCollidableObject
{
	[SerializeField]
	private Effect smokeEffect;

	[SerializeField]
	private SpriteDeathParts[] deathParts;

	[SerializeField]
	private GameObject projectile;

	[SerializeField]
	private Sprite spriteSpecial;

	[SerializeField]
	private Vector2 time;

	private float speedPCT;

	private int health;

	private int flightSpeed;

	private int bulletSpeed;

	private int pinkBulletCount;

	private int shotsFired;

	private float shootDelay;

	private DamageDealer damageDealer;

	private const int MAX_HEIGHT = 460;

	public void InitShotbot(int hp, int bulletSpeed, int pinkBulletCount, float shootDelay, int flightSpeed)
	{
		speedPCT = 200f / (float)flightSpeed;
		health = hp;
		this.flightSpeed = flightSpeed;
		this.bulletSpeed = bulletSpeed;
		this.pinkBulletCount = pinkBulletCount;
		shotsFired = 0;
		this.shootDelay = shootDelay;
		damageDealer = DamageDealer.NewEnemy();
		StartCoroutine(rotate_cr());
		StartCoroutine(move_cr());
		StartCoroutine(intro_cr());
		StartCoroutine(fire_cr());
	}

	public RobotLevelHatchShotbot Create()
	{
		GameObject gameObject = Object.Instantiate(base.gameObject);
		gameObject.transform.SetEulerAngles(0f, 0f, 180f);
		return gameObject.GetComponent<RobotLevelHatchShotbot>();
	}

	private IEnumerator intro_cr()
	{
		float rotTime = 0.15f;
		float scale = base.transform.localScale.x;
		base.transform.SetEulerAngles(null, null, 180f);
		yield return CupheadTime.WaitForSeconds(this, 0.5f * speedPCT);
		yield return StartCoroutine(tweenRotation_cr(180f * scale, 270f * scale, rotTime / 3f * speedPCT));
		yield return StartCoroutine(tweenRotation_cr(270f * scale, 180f * scale, rotTime / 3f * speedPCT));
		yield return null;
	}

	private IEnumerator move_cr()
	{
		float scale = base.transform.localScale.x;
		while (true)
		{
			Vector2 move = base.transform.right * flightSpeed * CupheadTime.Delta * scale;
			base.transform.AddPosition(move.x, move.y);
			yield return null;
			if (base.transform.position.y > 460f)
			{
				End();
			}
		}
	}

	private IEnumerator rotate_cr()
	{
		float rotTime = 0.15f * speedPCT;
		float scale = base.transform.localScale.x;
		yield return CupheadTime.WaitForSeconds(this, 1.8f * speedPCT);
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, time.x * speedPCT);
			yield return StartCoroutine(tweenRotation_cr(180f * scale, 90f * scale, rotTime));
			yield return CupheadTime.WaitForSeconds(this, time.y * speedPCT);
			yield return StartCoroutine(tweenRotation_cr(90f * scale, 0f, rotTime));
			yield return CupheadTime.WaitForSeconds(this, time.x * speedPCT);
			yield return StartCoroutine(tweenRotation_cr(0f, 90f * scale, rotTime));
			yield return CupheadTime.WaitForSeconds(this, time.y * speedPCT);
			yield return StartCoroutine(tweenRotation_cr(90f * scale, 180f * scale, rotTime));
		}
	}

	private IEnumerator tweenRotation_cr(float start, float end, float time)
	{
		base.transform.SetEulerAngles(null, null, start);
		float t = 0f;
		while (t < time)
		{
			TransformExtensions.SetEulerAngles(z: EaseUtils.Ease(EaseUtils.EaseType.linear, start, end, t / time), transform: base.transform);
			t += (float)CupheadTime.Delta / 3f;
			yield return null;
		}
		base.transform.SetEulerAngles(null, null, end);
	}

	private IEnumerator fire_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, shootDelay);
			AudioManager.Play("robot_shotbot_shoot");
			emitAudioFromObject.Add("robot_shotbot_shoot");
			GameObject proj = Object.Instantiate(projectile);
			proj.transform.position = base.transform.position;
			proj.transform.right = (PlayerManager.GetNext().center - base.transform.position).normalized;
			proj.GetComponent<BasicProjectile>().Speed = bulletSpeed;
			if (shotsFired >= pinkBulletCount)
			{
				shotsFired = 0;
				proj.GetComponent<SpriteRenderer>().sprite = spriteSpecial;
				proj.GetComponent<BasicProjectile>().SetParryable(true);
			}
			else
			{
				shotsFired++;
			}
			yield return null;
		}
	}

	protected override void OnCollisionPlayerProjectile(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		health--;
		if (health <= 0)
		{
			Dead();
		}
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

	private void Dead()
	{
		AudioManager.Play("robot_shotbot_death");
		emitAudioFromObject.Add("robot_shotbot_death");
		StopAllCoroutines();
		CreateSmoke();
		Object.Destroy(base.gameObject);
	}

	private void End()
	{
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}

	private void CreateSmoke()
	{
		smokeEffect.Create(base.transform.position);
		SpriteDeathParts[] array = deathParts;
		foreach (SpriteDeathParts spriteDeathParts in array)
		{
			spriteDeathParts.CreatePart(base.transform.position);
		}
	}
}
