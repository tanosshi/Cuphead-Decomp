using System.Collections;
using UnityEngine;

public class FlowerLevelPollenProjectile : BasicProjectile
{
	[SerializeField]
	private FlowerLevelPollenPetal petalPink;

	[SerializeField]
	private FlowerLevelPollenPetal petal;

	private bool manual;

	private float time;

	private float speed;

	private float waveStrength;

	private float initPosY;

	private Transform target;

	private float pct;

	public void InitPollen(float speed, float strength, int type, Transform target)
	{
		pct = 0f;
		time = 0.7795515f;
		manual = true;
		this.speed = 0f - speed;
		waveStrength = strength;
		this.target = target;
		Speed = 0f;
		if (type == 1)
		{
			SetParryable(true);
			animator.Play("Pink_Idle");
		}
		StartCoroutine(move_cr());
		StartCoroutine(spawn_petals_cr(type));
	}

	public void StartMoving()
	{
		manual = false;
		Speed = speed;
		initPosY = base.transform.position.y;
	}

	private IEnumerator move_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		while (true)
		{
			if (!manual)
			{
				Vector3 position = base.transform.position;
				position.y = initPosY + Mathf.Sin(time * 6f) * (waveStrength * pct) * CupheadTime.GlobalSpeed;
				base.transform.position = position;
				if (pct < 1f)
				{
					pct += CupheadTime.FixedDelta * 2f;
				}
				else
				{
					pct = 1f;
				}
			}
			else
			{
				base.transform.position = target.position;
				Speed = 0f;
			}
			time += CupheadTime.FixedDelta;
			yield return wait;
		}
	}

	private IEnumerator spawn_petals_cr(int type)
	{
		while (true)
		{
			if (type == 1)
			{
				petalPink.Create(base.transform.position);
			}
			else
			{
				petal.Create(base.transform.position);
			}
			yield return CupheadTime.WaitForSeconds(this, Random.Range(0.2f, 1f));
		}
	}

	protected override void Die()
	{
		base.Die();
		Object.Destroy(base.gameObject);
	}
}
