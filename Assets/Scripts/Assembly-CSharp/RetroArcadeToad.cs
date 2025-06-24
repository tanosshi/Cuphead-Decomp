using System.Collections;
using UnityEngine;

public class RetroArcadeToad : RetroArcadeEnemy
{
	private const float OFFSCREEN_Y = 300f;

	private const float BASE_Y = 250f;

	private const float MOVE_Y_SPEED = 500f;

	[SerializeField]
	private BasicProjectile projectilePrefab;

	[SerializeField]
	private Transform projectileRoot;

	private LevelProperties.RetroArcade.Toad properties;

	private RetroArcadeToadManager parent;

	public RetroArcadeToad Create(RetroArcadeToadManager parent, LevelProperties.RetroArcade.Toad properties, float xPos)
	{
		RetroArcadeToad retroArcadeToad = InstantiatePrefab<RetroArcadeToad>();
		retroArcadeToad.transform.SetPosition(xPos, 300f);
		retroArcadeToad.properties = properties;
		retroArcadeToad.parent = parent;
		retroArcadeToad.hp = properties.hp;
		retroArcadeToad.MoveY(-50f, 500f);
		retroArcadeToad.StartCoroutine(retroArcadeToad.shoot_cr());
		return retroArcadeToad;
	}

	private IEnumerator shoot_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, properties.initialAttackDelay.RandomFloat());
		while (true)
		{
			Shoot();
			yield return CupheadTime.WaitForSeconds(this, parent.attackDelay);
		}
	}

	public void Shoot()
	{
		projectilePrefab.Create(projectileRoot.position, -90f, properties.shotSpeed);
	}

	public override void Die()
	{
		base.Die();
		StartCoroutine(moveOffscreen_cr());
		parent.OnToadDie();
	}

	private IEnumerator moveOffscreen_cr()
	{
		MoveY(300f - base.transform.position.y, 500f);
		while (movingY)
		{
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}
}
