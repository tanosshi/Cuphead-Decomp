using System.Collections;
using UnityEngine;

public class RetroArcadePaddleShip : RetroArcadeEnemy
{
	private const float OFFSCREEN_Y = 350f;

	private const float TURNAROUND_X = 300f;

	private const float PADDLE_PADDING = 20f;

	private const float SHIP_PADDING = 80f;

	private const float MOVE_OFFSCREEN_SPEED = 500f;

	[SerializeField]
	private Transform paddle;

	private LevelProperties.RetroArcade properties;

	private LevelProperties.RetroArcade.PaddleShip p;

	private float ySpeed;

	private Trilean2 moveDir;

	public void LevelInit(LevelProperties.RetroArcade properties)
	{
		this.properties = properties;
	}

	public void StartPaddleShip()
	{
		base.gameObject.SetActive(true);
		p = properties.CurrentState.paddleShip;
		ySpeed = p.ySpeed.RandomFloat();
		base.PointsBonus = p.pointsBonus;
		base.PointsWorth = p.pointsGained;
		base.transform.SetPosition(Random.Range(-300f, 300f), 350f);
		moveDir = new Trilean2((!Rand.Bool()) ? 1 : (-1), -1);
		hp = p.hp;
		StartCoroutine(moveY_cr());
		StartCoroutine(moveX_cr());
	}

	private IEnumerator moveY_cr()
	{
		while (base.transform.position.y > (float)Level.Current.Ceiling - 80f)
		{
			yield return new WaitForFixedUpdate();
			base.transform.AddPosition(0f, (0f - ySpeed) * CupheadTime.FixedDelta);
		}
		base.transform.SetPosition(null, (float)Level.Current.Ceiling - 80f);
		while (paddle.position.y > (float)Level.Current.Ground + 20f)
		{
			yield return new WaitForFixedUpdate();
			paddle.AddPosition(0f, (0f - ySpeed) * CupheadTime.FixedDelta);
		}
		paddle.SetPosition(null, (float)Level.Current.Ground + 20f);
		while (true)
		{
			yield return new WaitForFixedUpdate();
			if (((int)moveDir.y > 0 && base.transform.position.y > (float)Level.Current.Ceiling - 80f) || ((int)moveDir.y < 0 && base.transform.position.y < (float)Level.Current.Ground + 80f))
			{
				ref Trilean2 reference = ref moveDir;
				reference.y = (int)reference.y * -1;
			}
			base.transform.AddPosition(0f, (float)moveDir.y * ySpeed * CupheadTime.FixedDelta);
			paddle.SetPosition(null, (float)Level.Current.Ground + 20f);
		}
	}

	private IEnumerator moveX_cr()
	{
		while (true)
		{
			yield return new WaitForFixedUpdate();
			if (((int)moveDir.x > 0 && base.transform.position.x > 300f) || ((int)moveDir.x < 0 && base.transform.position.x < -300f))
			{
				ref Trilean2 reference = ref moveDir;
				reference.x = (int)reference.x * -1;
			}
			base.transform.AddPosition((float)moveDir.x * p.xSpeed * CupheadTime.FixedDelta);
		}
	}

	public override void Die()
	{
		StopAllCoroutines();
		Collider2D[] componentsInChildren = GetComponentsInChildren<Collider2D>();
		foreach (Collider2D collider2D in componentsInChildren)
		{
			collider2D.enabled = false;
		}
		base.IsDead = true;
		SpriteRenderer[] componentsInChildren2 = GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer spriteRenderer in componentsInChildren2)
		{
			spriteRenderer.color = new Color(0f, 0f, 0f, 0.25f);
		}
		properties.DealDamageToNextNamedState();
		StartCoroutine(moveOffscreen_cr());
	}

	private IEnumerator moveOffscreen_cr()
	{
		MoveY(350f - ((float)Level.Current.Ground + 20f), 500f);
		while (movingY)
		{
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}
}
