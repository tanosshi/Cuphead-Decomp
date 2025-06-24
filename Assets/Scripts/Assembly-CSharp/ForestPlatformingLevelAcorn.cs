using System;
using System.Collections;
using UnityEngine;

public class ForestPlatformingLevelAcorn : AbstractPlatformingLevelEnemy
{
	public enum Direction
	{
		Left = 0,
		Right = 1
	}

	[SerializeField]
	private ForestPlatformingLevelAcornPropeller propellerPrefab;

	private const float SCREEN_PADDING = 100f;

	private const float DROP_EASE_TIME = 0.5f;

	private Direction _direction;

	private AbstractPlayerController _player;

	private bool _hasDropped;

	private bool _enteredScreen;

	private ForestPlatformingLevelAcornMaker parent;

	public ForestPlatformingLevelAcorn Spawn(ForestPlatformingLevelAcornMaker parent, Vector2 position, Direction direction, bool moveUpFirst)
	{
		ForestPlatformingLevelAcorn forestPlatformingLevelAcorn = InstantiatePrefab<ForestPlatformingLevelAcorn>();
		forestPlatformingLevelAcorn.transform.position = position;
		forestPlatformingLevelAcorn._startCondition = StartCondition.Instant;
		forestPlatformingLevelAcorn._direction = direction;
		forestPlatformingLevelAcorn._player = PlayerManager.GetNext();
		forestPlatformingLevelAcorn.parent = parent;
		if (moveUpFirst)
		{
			forestPlatformingLevelAcorn.StartCoroutine(forestPlatformingLevelAcorn.move_up_cr());
		}
		else
		{
			forestPlatformingLevelAcorn.StartCoroutine(forestPlatformingLevelAcorn.main_cr());
		}
		return forestPlatformingLevelAcorn;
	}

	public ForestPlatformingLevelAcorn Spawn(Vector2 position, Direction direction, bool moveUpFirst)
	{
		ForestPlatformingLevelAcorn forestPlatformingLevelAcorn = InstantiatePrefab<ForestPlatformingLevelAcorn>();
		forestPlatformingLevelAcorn.transform.position = position;
		forestPlatformingLevelAcorn._startCondition = StartCondition.Instant;
		forestPlatformingLevelAcorn._direction = direction;
		forestPlatformingLevelAcorn._player = PlayerManager.GetNext();
		if (moveUpFirst)
		{
			forestPlatformingLevelAcorn.StartCoroutine(forestPlatformingLevelAcorn.move_up_cr());
		}
		else
		{
			forestPlatformingLevelAcorn.StartCoroutine(forestPlatformingLevelAcorn.main_cr());
		}
		return forestPlatformingLevelAcorn;
	}

	protected override void Awake()
	{
		base.Awake();
		AudioManager.PlayLoop("level_acorn_fly");
		emitAudioFromObject.Add("level_acorn_fly");
		base.DeathSound = "level_frogs_tall_spit_shoot";
	}

	protected override void Start()
	{
		base.Start();
		if (parent != null)
		{
			ForestPlatformingLevelAcornMaker forestPlatformingLevelAcornMaker = parent;
			forestPlatformingLevelAcornMaker.killAcorns = (Action)Delegate.Combine(forestPlatformingLevelAcornMaker.killAcorns, new Action(Kill));
			StartCoroutine(acorn_death_timer_cr());
		}
	}

	protected override void OnStart()
	{
	}

	protected override void Update()
	{
		base.Update();
		if (CupheadLevelCamera.Current.ContainsPoint(base.transform.position) && !_enteredScreen)
		{
			_enteredScreen = true;
		}
		if (_enteredScreen && !CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(100f, 100f)))
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		if (base.transform.position.x < (float)PlatformingLevel.Current.Left - 100f || base.transform.position.x > (float)PlatformingLevel.Current.Right + 100f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		base.transform.SetScale((_direction == Direction.Left) ? 1 : (-1));
	}

	private IEnumerator move_up_cr()
	{
		float yOffset = 100f;
		while (base.transform.position.y < CupheadLevelCamera.Current.Bounds.yMax - yOffset)
		{
			base.transform.AddPosition(0f, base.Properties.AcornFlySpeed * (float)CupheadTime.Delta);
			yield return null;
		}
		StartCoroutine(main_cr());
		yield return null;
	}

	private IEnumerator main_cr()
	{
		while ((_direction == Direction.Left && base.transform.position.x > _player.center.x) || (_direction == Direction.Right && base.transform.position.x < _player.center.x))
		{
			base.transform.AddPosition((_direction != Direction.Right) ? ((0f - base.Properties.AcornFlySpeed) * CupheadTime.FixedDelta) : (base.Properties.AcornFlySpeed * CupheadTime.FixedDelta));
			yield return new WaitForFixedUpdate();
			if (_player == null || _player.IsDead)
			{
				_player = PlayerManager.GetNext();
			}
		}
		base.animator.SetTrigger("Drop");
		AudioManager.Play("level_acorn_drop");
		emitAudioFromObject.Add("level_acorn_drop");
		float t = 0f;
		_hasDropped = true;
		LaunchPropeller();
		while (t < 0.5f)
		{
			base.transform.AddPosition(0f, (0f - base.Properties.AcornDropSpeed) * CupheadTime.FixedDelta * t / 0.5f);
			t += CupheadTime.FixedDelta;
			yield return new WaitForFixedUpdate();
		}
		while (true)
		{
			base.transform.AddPosition(0f, (0f - base.Properties.AcornDropSpeed) * CupheadTime.FixedDelta);
			yield return new WaitForFixedUpdate();
		}
	}

	private IEnumerator acorn_death_timer_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 0.2f);
		ForestPlatformingLevelAcornMaker forestPlatformingLevelAcornMaker = parent;
		forestPlatformingLevelAcornMaker.killAcorns = (Action)Delegate.Remove(forestPlatformingLevelAcornMaker.killAcorns, new Action(Kill));
		yield return null;
	}

	private void LaunchPropeller()
	{
		propellerPrefab.Create(base.transform.position, base.Properties.AcornPropellerSpeed);
	}

	protected override void Die()
	{
		if (!_hasDropped)
		{
			LaunchPropeller();
			AudioManager.Stop("level_acorn_fly");
		}
		base.Die();
	}

	private void Kill()
	{
		base.Die();
	}
}
