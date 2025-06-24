using System.Collections;
using UnityEngine;

public class DicePalaceLightLevelObject : AbstractCollidableObject
{
	[SerializeField]
	private bool onLeft;

	private bool toScreenEdge = true;

	private bool slowDown;

	private float speed;

	private float originalSpeed;

	private LevelProperties.DicePalaceLight.Objects properties;

	private DamageDealer damageDealer;

	private Vector3 startPos;

	private Vector3 endPos;

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		if (onLeft)
		{
			endPos.x = -640f;
		}
		else
		{
			endPos.x = 640f;
		}
	}

	public void Properties(LevelProperties.DicePalaceLight.Objects properties)
	{
		this.properties = properties;
		speed = properties.objectSpeed;
		originalSpeed = speed;
		startPos = base.transform.position;
		startPos.y = -360f + properties.objectStartHeight;
		base.transform.position = startPos;
		StartCoroutine(move_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
		VaryingSpeed();
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	private IEnumerator move_cr()
	{
		float offset = 200f;
		float stopDist = 100f;
		Vector3 pos = base.transform.position;
		while (true)
		{
			if (toScreenEdge)
			{
				while (base.transform.position.x != endPos.x)
				{
					float dist = endPos.x - base.transform.position.x;
					dist = Mathf.Abs(dist);
					pos.x = Mathf.MoveTowards(base.transform.position.x, endPos.x, speed * (float)CupheadTime.Delta);
					if (dist < stopDist)
					{
						slowDown = true;
					}
					base.transform.position = pos;
					yield return null;
				}
				toScreenEdge = false;
				yield return null;
			}
			else if (!toScreenEdge)
			{
				while (base.transform.position.x != startPos.x)
				{
					float dist2 = startPos.x - base.transform.position.x;
					dist2 = Mathf.Abs(dist2);
					pos.x = Mathf.MoveTowards(base.transform.position.x, startPos.x, speed * (float)CupheadTime.Delta);
					if (dist2 < stopDist)
					{
						slowDown = true;
					}
					base.transform.position = pos;
					yield return null;
				}
				toScreenEdge = true;
				yield return null;
			}
			yield return null;
		}
	}

	private void VaryingSpeed()
	{
		float num = 20f;
		if (slowDown)
		{
			if (speed <= 50f)
			{
				slowDown = false;
			}
			else
			{
				speed -= num;
			}
		}
		else if (speed < originalSpeed)
		{
			speed += num;
		}
	}
}
