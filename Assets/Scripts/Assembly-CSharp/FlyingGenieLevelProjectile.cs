using System.Collections;
using UnityEngine;

public class FlyingGenieLevelProjectile : AbstractProjectile
{
	private LevelProperties.FlyingGenie.Scan properties;

	private AbstractPlayerController player;

	private float speed;

	public FlyingGenieLevelProjectile Create(Vector2 pos, LevelProperties.FlyingGenie.Scan properties, AbstractPlayerController player)
	{
		FlyingGenieLevelProjectile flyingGenieLevelProjectile = base.Create() as FlyingGenieLevelProjectile;
		flyingGenieLevelProjectile.properties = properties;
		flyingGenieLevelProjectile.transform.position = pos;
		flyingGenieLevelProjectile.player = player;
		return flyingGenieLevelProjectile;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(move_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	private IEnumerator move_cr()
	{
		animator.Play("Bullet_Warning");
		yield return CupheadTime.WaitForSeconds(this, properties.bulletWarningDuration);
		animator.Play("Bullet");
		if (player == null || player.IsDead)
		{
			player = PlayerManager.GetNext();
		}
		Vector3 direction = player.transform.position - base.transform.position;
		speed = properties.bulletSpeedMin;
		base.transform.SetEulerAngles(null, null, MathUtils.DirectionToAngle(direction));
		StartCoroutine(acceleration_cr());
		while (true)
		{
			base.transform.position += base.transform.right * speed * CupheadTime.Delta;
			yield return null;
		}
	}

	private IEnumerator acceleration_cr()
	{
		while (speed < properties.bulletSpeedMax)
		{
			speed += properties.bulletAcceleration * (float)CupheadTime.Delta;
			yield return null;
		}
		yield return null;
	}
}
