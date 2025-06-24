using System.Collections;
using UnityEngine;

public class DicePalaceCigarLevelCigarSpit : AbstractProjectile
{
	[SerializeField]
	private Effect bulletFX;

	private bool onRight;

	private float time;

	private float circleSpeed;

	private Vector3 centerPoint;

	private LevelProperties.DicePalaceCigar properties;

	public void InitProjectile(LevelProperties.DicePalaceCigar properties, bool clockwise, bool onRight)
	{
		time = 0f;
		centerPoint = base.transform.position;
		this.onRight = onRight;
		if (!clockwise)
		{
			circleSpeed = 0f - properties.CurrentState.spiralSmoke.circleSpeed;
		}
		else
		{
			circleSpeed = properties.CurrentState.spiralSmoke.circleSpeed;
		}
		this.properties = properties;
		StartCoroutine(move_cr());
		StartCoroutine(bullet_trail_cr());
	}

	private IEnumerator move_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		while (true)
		{
			centerPoint += -base.transform.right * properties.CurrentState.spiralSmoke.horizontalSpeed * CupheadTime.FixedDelta;
			Vector3 newPos = centerPoint;
			newPos.y = centerPoint.y + Mathf.Sin(time * circleSpeed) * properties.CurrentState.spiralSmoke.spiralSmokeCircleSize;
			if (onRight)
			{
				newPos.x = centerPoint.x + Mathf.Cos(time * circleSpeed) * properties.CurrentState.spiralSmoke.spiralSmokeCircleSize;
			}
			else
			{
				newPos.x = centerPoint.x + (0f - Mathf.Cos(time * circleSpeed)) * properties.CurrentState.spiralSmoke.spiralSmokeCircleSize;
			}
			base.transform.position = newPos;
			time += CupheadTime.FixedDelta;
			yield return wait;
		}
	}

	private IEnumerator bullet_trail_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, Random.Range(0.16f, 0.2f));
			bulletFX.Create(base.transform.position);
			yield return null;
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}
}
