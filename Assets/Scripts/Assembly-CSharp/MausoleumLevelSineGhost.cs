using System.Collections;
using UnityEngine;

public class MausoleumLevelSineGhost : MausoleumLevelGhostBase
{
	[SerializeField]
	private SpriteDeathParts hat;

	private Vector3 pointAtTarget;

	private Vector3 normalized;

	private float rotation;

	private float angle;

	private LevelProperties.Mausoleum.SineGhost properties;

	public MausoleumLevelSineGhost Create(Vector2 position, float rotation, float speed, LevelProperties.Mausoleum.SineGhost properties)
	{
		MausoleumLevelSineGhost mausoleumLevelSineGhost = base.Create(position, rotation, speed) as MausoleumLevelSineGhost;
		mausoleumLevelSineGhost.rotation = rotation;
		mausoleumLevelSineGhost.properties = properties;
		return mausoleumLevelSineGhost;
	}

	protected override void Start()
	{
		base.Start();
		CalculateDirection();
		CalculateSin();
		StartCoroutine(move_cr());
	}

	private void CalculateSin()
	{
		Vector2 zero = Vector2.zero;
		zero = MathUtils.AngleToDirection(rotation) / 2f;
		float num = 0f - (zero.x - base.transform.position.x) / (zero.y - base.transform.position.y);
		float num2 = zero.y - num * zero.x;
		Vector2 zero2 = Vector2.zero;
		zero2.x = zero.x + 1f;
		zero2.y = num * zero2.x + num2;
		normalized = Vector3.zero;
		normalized = zero2 - zero;
		normalized.Normalize();
	}

	private void CalculateDirection()
	{
		Vector2 zero = Vector2.zero;
		zero = MathUtils.AngleToDirection(rotation);
		float value = Mathf.Atan2(zero.y, zero.x) * 57.29578f;
		pointAtTarget = MathUtils.AngleToDirection(value);
		base.transform.SetEulerAngles(null, null, value);
	}

	private IEnumerator move_cr()
	{
		Vector3 pos = base.transform.position;
		while (true)
		{
			angle += properties.waveSpeed * (float)CupheadTime.Delta;
			if ((float)CupheadTime.Delta != 0f)
			{
				pos += normalized * Mathf.Sin(angle + properties.waveAmount) * (properties.waveAmount / 2f);
			}
			pos += pointAtTarget * properties.ghostSpeed * CupheadTime.Delta;
			base.transform.position = pos;
			yield return null;
		}
	}

	protected override void Die()
	{
		if (!base.isDead)
		{
			SpriteDeathParts spriteDeathParts = hat.CreatePart(base.transform.position);
			spriteDeathParts.animator.SetBool("HatA", Rand.Bool());
		}
		base.Die();
	}
}
