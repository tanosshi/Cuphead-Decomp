using System;
using System.Collections;
using UnityEngine;

public class BeeLevelTurbineBullet : AbstractProjectile
{
	[SerializeField]
	private Effect trailPrefab;

	[SerializeField]
	private SpriteRenderer sprite;

	private LevelProperties.Bee.TurbineBlasters properties;

	private float velocity;

	private float rotation;

	private float circleAngle;

	private float loopSizeY = 200f;

	private float loopSizeX = 500f;

	private bool onRight;

	private Vector3 direction;

	private Vector3 pivotPoint;

	public BeeLevelTurbineBullet Create(Vector2 pos, float rotation, bool onRight, LevelProperties.Bee.TurbineBlasters properties)
	{
		BeeLevelTurbineBullet beeLevelTurbineBullet = base.Create() as BeeLevelTurbineBullet;
		beeLevelTurbineBullet.properties = properties;
		beeLevelTurbineBullet.transform.position = pos;
		beeLevelTurbineBullet.rotation = rotation;
		beeLevelTurbineBullet.onRight = onRight;
		beeLevelTurbineBullet.direction = MathUtils.AngleToDirection(rotation);
		beeLevelTurbineBullet.velocity = properties.bulletSpeed;
		beeLevelTurbineBullet.transform.SetEulerAngles(0f, 0f, rotation);
		beeLevelTurbineBullet.sprite.flipX = onRight;
		return beeLevelTurbineBullet;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(trail_cr());
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
		while (base.transform.position.y < 360f - loopSizeY)
		{
			base.transform.position += direction * velocity * CupheadTime.Delta;
			yield return null;
		}
		StartCoroutine(move_in_circle_cr());
		yield return null;
	}

	private IEnumerator move_in_circle_cr()
	{
		pivotPoint = base.transform.position + Vector3.right * ((!onRight) ? loopSizeX : (0f - loopSizeX));
		Vector3 handleRotationX = Vector3.zero;
		float offset = 100f;
		circleAngle -= (float)Math.PI / 2f;
		float endPos;
		float endVelocity;
		float rotateInCir;
		if (onRight)
		{
			endPos = -640f - offset;
			endVelocity = 0f - velocity;
			rotateInCir = -90f;
		}
		else
		{
			endPos = 640f + offset;
			endVelocity = velocity;
			rotateInCir = 90f;
		}
		while (circleAngle < 6.108652f)
		{
			circleAngle += properties.bulletCircleTime * (float)CupheadTime.Delta;
			handleRotationX = ((!onRight) ? new Vector3(Mathf.Sin(circleAngle) * loopSizeX, 0f, 0f) : new Vector3((0f - Mathf.Sin(circleAngle)) * loopSizeX, 0f, 0f));
			Vector3 handleRotationY = new Vector3(0f, Mathf.Cos(circleAngle) * loopSizeY, 0f);
			base.transform.position = pivotPoint;
			base.transform.position += handleRotationX + handleRotationY;
			TransformExtensions.SetEulerAngles(z: MathUtils.DirectionToAngle(pivotPoint - base.transform.position) + rotateInCir, transform: base.transform, x: 0f, y: 0f);
			yield return null;
		}
		while (base.transform.position.x != endPos)
		{
			base.transform.AddPosition(endVelocity * (float)CupheadTime.Delta);
			yield return null;
		}
		Die();
		yield return null;
	}

	private IEnumerator trail_cr()
	{
		while (true)
		{
			trailPrefab.Create(base.transform.position);
			yield return CupheadTime.WaitForSeconds(this, 0.25f);
		}
	}

	protected override void Die()
	{
		base.Die();
		base.transform.SetEulerAngles(0f, 0f, 0f);
	}
}
