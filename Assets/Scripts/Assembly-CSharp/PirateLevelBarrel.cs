using System.Collections;
using UnityEngine;

public class PirateLevelBarrel : LevelProperties.Pirate.Entity
{
	public enum State
	{
		Init = 0,
		Move = 1,
		Fall = 2,
		Hold = 3,
		Up = 4,
		Safe = 5
	}

	public enum Direction
	{
		Right = 0,
		Left = 1
	}

	public const float MIN_X = -570f;

	public const float MAX_X = 235f;

	public const float UP_Y = 250f;

	public const float DOWN_Y = -225f;

	public const float RANGE = 120f;

	[SerializeField]
	private Effect particlesEffect;

	[SerializeField]
	private Effect dustEffect;

	private State state;

	private Direction direction = Direction.Left;

	private new Animator animator;

	private IEnumerator moveCoroutine;

	private DamageDealer damageDealer;

	public override void LevelInit(LevelProperties.Pirate properties)
	{
		base.LevelInit(properties);
		Level.Current.OnStateChangedEvent += OnStateChanged;
		Level.Current.OnLevelStartEvent += OnLevelStart;
		animator = GetComponent<Animator>();
		damageDealer = new DamageDealer(base.properties.CurrentState.barrel.damage, 1f);
		damageDealer.SetDirection(DamageDealer.Direction.Neutral, base.transform);
		state = State.Move;
	}

	private void OnLevelStart()
	{
		moveCoroutine = move_cr();
		StartCoroutine(moveCoroutine);
	}

	private void OnStateChanged()
	{
		damageDealer.SetDamage(base.properties.CurrentState.barrel.damage);
		StopCoroutine(moveCoroutine);
		moveCoroutine = move_cr();
		StartCoroutine(moveCoroutine);
	}

	protected override void Update()
	{
		base.Update();
		AbstractPlayerController[] array = new AbstractPlayerController[2]
		{
			PlayerManager.GetPlayer(PlayerId.PlayerOne),
			PlayerManager.GetPlayer(PlayerId.PlayerTwo)
		};
		if (state == State.Move)
		{
			float num = base.transform.position.x - 60f;
			float num2 = base.transform.position.x + 60f;
			AbstractPlayerController[] array2 = array;
			foreach (AbstractPlayerController abstractPlayerController in array2)
			{
				if (!(abstractPlayerController == null) && !(abstractPlayerController.transform == null) && !abstractPlayerController.IsDead && abstractPlayerController.center.x > num && abstractPlayerController.center.x < num2)
				{
					PlayerFound();
					break;
				}
			}
		}
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	private void PlayerFound()
	{
		state = State.Fall;
		StartCoroutine(fall_cr());
	}

	private IEnumerator move_cr()
	{
		float time = base.properties.CurrentState.barrel.moveTime;
		float p = (base.transform.position.x - -570f) / 805f;
		if (direction == Direction.Left)
		{
			p = 1f - p;
		}
		float t = time * p;
		while (true)
		{
			if (direction == Direction.Right)
			{
				while (t < time)
				{
					yield return StartCoroutine(waitForMove_cr());
					float val = t / time;
					TransformExtensions.SetPosition(x: EaseUtils.Ease(EaseUtils.EaseType.linear, -570f, 235f, val), transform: base.transform);
					t += (float)CupheadTime.Delta;
					yield return null;
				}
				t = 0f;
				direction = Direction.Left;
			}
			if (direction == Direction.Left)
			{
				while (t < time)
				{
					yield return StartCoroutine(waitForMove_cr());
					float val2 = t / time;
					TransformExtensions.SetPosition(x: EaseUtils.Ease(EaseUtils.EaseType.linear, 235f, -570f, val2), transform: base.transform);
					t += (float)CupheadTime.Delta;
					yield return null;
				}
				t = 0f;
				direction = Direction.Right;
			}
		}
	}

	private IEnumerator waitForMove_cr()
	{
		while (state != State.Move && state != State.Safe)
		{
			yield return null;
		}
	}

	private IEnumerator fall_cr()
	{
		AudioManager.Play("level_pirate_barrel_drop_attack");
		emitAudioFromObject.Add("level_pirate_barrel_drop_attack");
		animator.SetTrigger("OnFall");
		state = State.Fall;
		GetComponent<Collider2D>().enabled = true;
		LevelProperties.Pirate.State properties = base.properties.CurrentState;
		float t = 0f;
		float time = properties.barrel.fallTime;
		while (t < time)
		{
			float val = t / time;
			TransformExtensions.SetPosition(y: EaseUtils.Ease(EaseUtils.EaseType.easeInQuart, 250f, -225f, val), transform: base.transform);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.SetPosition(null, -225f);
		dustEffect.Create(base.transform.position);
		particlesEffect.Create(base.transform.position);
		animator.SetTrigger("OnSmash");
		state = State.Hold;
		CupheadLevelCamera.Current.Shake(8f, 0.6f);
		yield return CupheadTime.WaitForSeconds(this, properties.barrel.groundHold);
		t = 0f;
		time = properties.barrel.riseTime;
		animator.SetTrigger("OnUp");
		state = State.Up;
		while (t < time)
		{
			float val2 = t / time;
			TransformExtensions.SetPosition(y: EaseUtils.Ease(EaseUtils.EaseType.linear, -225f, 250f, val2), transform: base.transform);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.SetPosition(null, 250f);
		animator.SetTrigger("OnSafe");
		state = State.Safe;
		yield return CupheadTime.WaitForSeconds(this, properties.barrel.safeTime);
		animator.SetTrigger("OnReady");
		state = State.Move;
	}
}
