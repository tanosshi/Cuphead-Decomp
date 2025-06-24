using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformingLevelShootingEnemy : AbstractPlatformingLevelEnemy
{
	public enum TriggerType
	{
		Range = 0,
		TriggerVolumes = 1,
		OnScreen = 2,
		Indefinite = 3
	}

	public enum Direction
	{
		Left = 0,
		Right = 1
	}

	[Serializable]
	public class TriggerVolumeProperties
	{
		public enum Shape
		{
			BoxCollider = 0,
			CircleCollider = 1
		}

		public enum Space
		{
			RelativeSpace = 0,
			WorldSpace = 1
		}

		public Shape shape;

		public Space space;

		public Vector2 position = Vector2.zero;

		public Vector2 boxSize = new Vector2(100f, 100f);

		public float circleRadius = 100f;

		public Rect ToRect()
		{
			Rect result = new Rect(position, boxSize);
			result.x -= result.width / 2f;
			result.y -= result.height / 2f;
			return result;
		}
	}

	[Header("Trigger Properties")]
	[SerializeField]
	private TriggerType _triggerType;

	[SerializeField]
	private List<TriggerVolumeProperties> _triggerVolumes;

	[SerializeField]
	protected Effect _shootEffect;

	[SerializeField]
	protected Transform _effectRoot;

	[SerializeField]
	private Transform _projectileRoot;

	[SerializeField]
	private bool _hasShootingAnimation;

	[SerializeField]
	private MinMax _initialShotDelay;

	[SerializeField]
	private bool _hasFacingDirection;

	[SerializeField]
	private float _ArcExtraSpeedUnderPlayerMultiplier;

	public float triggerRange = 1000f;

	public float onScreenTriggerPadding;

	protected AbstractPlayerController _target;

	private Transform _aim;

	private bool _hasFired;

	private float _projectileDelay;

	private Direction _direction;

	private const float GIZMO_LETTER_LENGTH = 40f;

	protected override void Start()
	{
		base.Start();
		_aim = new GameObject("Aim").transform;
		_aim.SetParent(_projectileRoot);
		_aim.ResetLocalTransforms();
	}

	protected override void Update()
	{
		base.Update();
	}

	protected override void OnStart()
	{
		_projectileDelay = base.Properties.ProjectileDelay.RandomFloat();
		switch (_triggerType)
		{
		case TriggerType.Range:
			StartCoroutine(ranged_cr());
			break;
		case TriggerType.TriggerVolumes:
			StartCoroutine(triggerVolumes_cr());
			break;
		case TriggerType.Indefinite:
			StartCoroutine(indefinite_cr());
			break;
		case TriggerType.OnScreen:
			StartCoroutine(onscreen_cr());
			break;
		default:
			Debug.LogWarning("TriggerType '" + _triggerType.ToString() + "' not configured");
			break;
		}
	}

	protected virtual void StartShoot()
	{
		base.animator.SetTrigger("Shoot");
	}

	protected virtual void Shoot()
	{
		float num = base.Properties.ProjectileAngle;
		float speed = base.Properties.ProjectileSpeed;
		if (_target == null || _target.IsDead)
		{
			_target = PlayerManager.GetNext();
		}
		switch (base.Properties.ProjectileAimMode)
		{
		case EnemyProperties.AimMode.AimedAtPlayer:
			_aim.LookAt2D(_target.center);
			num = _aim.transform.eulerAngles.z;
			break;
		case EnemyProperties.AimMode.ArcAimedAtPlayer:
		{
			float num2 = float.MaxValue;
			Vector2 vector2 = (Vector2)_target.center - (Vector2)_projectileRoot.position;
			vector2.x = Mathf.Abs(vector2.x);
			MinMax minMax2 = new MinMax(base.Properties.ArcProjectileMinAngle, base.Properties.ProjectileAngle);
			MinMax minMax = new MinMax(base.Properties.ArcProjectileMinSpeed, base.Properties.ProjectileSpeed);
			if (vector2.y > 0f && _ArcExtraSpeedUnderPlayerMultiplier > 0f)
			{
				float num19 = minMax.max / base.Properties.ProjectileGravity;
				float num20 = minMax.max * num19 - 0.5f * base.Properties.ProjectileGravity * num19 * num19;
				float num21 = num20 + vector2.y * _ArcExtraSpeedUnderPlayerMultiplier;
				float num22 = Mathf.Sqrt(2f * num21 / base.Properties.ProjectileGravity);
				minMax.max = num22 * base.Properties.ProjectileGravity;
				minMax.min *= minMax.max / base.Properties.ProjectileSpeed;
			}
			for (float num23 = 0f; num23 < 1f; num23 += 0.01f)
			{
				float floatAt = minMax2.GetFloatAt(num23);
				float floatAt2 = minMax.GetFloatAt(num23);
				Vector2 vector5 = MathUtils.AngleToDirection(floatAt) * floatAt2;
				float num24 = vector2.x / vector5.x;
				float num25 = vector5.y * num24 - 0.5f * base.Properties.ProjectileGravity * num24 * num24;
				float num26 = Mathf.Abs(vector2.y - num25);
				if (base.Properties.ProjectileGravity > 0.01f)
				{
					float num27 = vector5.y - base.Properties.ProjectileGravity * num24;
					if (num27 > 0f)
					{
						continue;
					}
				}
				if (num26 < num2)
				{
					num2 = num26;
					num = floatAt;
					speed = floatAt2;
				}
			}
			if ((!_hasFacingDirection && _target.center.x < base.transform.position.x) || (_hasFacingDirection && _direction == Direction.Left))
			{
				num = 180f - num;
			}
			break;
		}
		case EnemyProperties.AimMode.Spread:
		{
			Vector3 vector = MathUtils.AngleToDirection(base.Properties.ProjectileAngle);
			float num2 = float.MaxValue;
			Vector2 vector2 = (Vector2)vector - (Vector2)_projectileRoot.position;
			vector2.x = Mathf.Abs(vector2.x);
			MinMax minMax = new MinMax(base.Properties.ArcProjectileMinSpeed, base.Properties.ProjectileSpeed);
			if (vector2.y > 0f)
			{
				float num11 = minMax.max / base.Properties.ProjectileGravity;
				float num12 = minMax.max * num11 - 0.5f * base.Properties.ProjectileGravity * num11 * num11;
				float num13 = num12 + vector2.y * _ArcExtraSpeedUnderPlayerMultiplier;
				float num14 = Mathf.Sqrt(2f * num13 / base.Properties.ProjectileGravity);
				minMax.max = num14 * base.Properties.ProjectileGravity;
				minMax.min *= minMax.max / base.Properties.ProjectileSpeed;
			}
			float num15 = minMax.RandomFloat();
			Vector2 vector4 = MathUtils.AngleToDirection(base.Properties.ProjectileAngle) * num15;
			float num16 = vector2.x / vector4.x;
			float num17 = vector4.y * num16 - 0.5f * base.Properties.ProjectileGravity * num16 * num16;
			float num18 = Mathf.Abs(vector2.y - num17);
			if (num18 < num2)
			{
				num2 = num18;
				num = base.Properties.ProjectileAngle;
				speed = num15;
			}
			for (int i = 0; i < 2; i++)
			{
				float rotation = ((i != 1) ? 90f : (180f - num));
				BasicProjectile basicProjectile = base.Properties.ProjectilePrefab.Create(_projectileRoot.position, rotation, speed);
				basicProjectile.SetParryable(base.Properties.ProjectileParryable);
				basicProjectile.Gravity = base.Properties.ProjectileGravity;
			}
			break;
		}
		case EnemyProperties.AimMode.Arc:
		{
			Vector3 vector = MathUtils.AngleToDirection(base.Properties.ProjectileAngle);
			float num2 = float.MaxValue;
			Vector2 vector2 = (Vector2)vector - (Vector2)_projectileRoot.position;
			vector2.x = Mathf.Abs(vector2.x);
			MinMax minMax = new MinMax(base.Properties.ArcProjectileMinSpeed, base.Properties.ProjectileSpeed);
			if (vector2.y > 0f)
			{
				float num3 = minMax.max / base.Properties.ProjectileGravity;
				float num4 = minMax.max * num3 - 0.5f * base.Properties.ProjectileGravity * num3 * num3;
				float num5 = num4 + vector2.y * _ArcExtraSpeedUnderPlayerMultiplier;
				float num6 = Mathf.Sqrt(2f * num5 / base.Properties.ProjectileGravity);
				minMax.max = num6 * base.Properties.ProjectileGravity;
				minMax.min *= minMax.max / base.Properties.ProjectileSpeed;
			}
			float num7 = minMax.RandomFloat();
			Vector2 vector3 = MathUtils.AngleToDirection(base.Properties.ProjectileAngle) * num7;
			float num8 = vector2.x / vector3.x;
			float num9 = vector3.y * num8 - 0.5f * base.Properties.ProjectileGravity * num8 * num8;
			float num10 = Mathf.Abs(vector2.y - num9);
			if (num10 < num2)
			{
				num2 = num10;
				num = base.Properties.ProjectileAngle;
				speed = num7;
			}
			break;
		}
		default:
			Debug.LogWarning("AimMode '" + base.Properties.ProjectileAimMode.ToString() + "' is not configured!");
			break;
		case EnemyProperties.AimMode.Straight:
			break;
		}
		BasicProjectile basicProjectile2 = base.Properties.ProjectilePrefab.Create(_projectileRoot.position, num, speed);
		basicProjectile2.SetParryable(base.Properties.ProjectileParryable);
		basicProjectile2.SetStoneTime(base.Properties.ProjectileStoneTime);
		basicProjectile2.Gravity = base.Properties.ProjectileGravity;
		SpawnShootEffect();
	}

	protected virtual void SpawnShootEffect()
	{
		if (_shootEffect != null)
		{
			_shootEffect.Create(_effectRoot.position);
		}
	}

	protected void setDirection(Direction direction)
	{
		_direction = direction;
		base.transform.SetScale((_direction != Direction.Right) ? 1 : (-1));
	}

	private IEnumerator indefinite_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, _initialShotDelay.RandomFloat());
		while (true)
		{
			if (_hasShootingAnimation)
			{
				StartShoot();
				yield return base.animator.WaitForAnimationToStart(this, "Shoot");
				_target = PlayerManager.GetNext();
			}
			else
			{
				_target = PlayerManager.GetNext();
				Shoot();
			}
			yield return CupheadTime.WaitForSeconds(this, _projectileDelay);
		}
	}

	private IEnumerator onscreen_cr()
	{
		while (true)
		{
			if (!CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(0f - onScreenTriggerPadding, 0f)))
			{
				yield return null;
				continue;
			}
			if (!_hasFired)
			{
				yield return CupheadTime.WaitForSeconds(this, _initialShotDelay.RandomFloat());
				_hasFired = true;
				continue;
			}
			if (_hasShootingAnimation)
			{
				StartShoot();
				yield return base.animator.WaitForAnimationToStart(this, "Shoot");
				_target = PlayerManager.GetNext();
			}
			else
			{
				_target = PlayerManager.GetNext();
				Shoot();
			}
			yield return CupheadTime.WaitForSeconds(this, _projectileDelay);
		}
	}

	private IEnumerator ranged_cr()
	{
		PlayerId lastPlayer = PlayerId.None;
		while (true)
		{
			PlayerId currentPlayer = PlayerId.PlayerOne;
			bool inRange = false;
			while (!inRange)
			{
				bool cuphead = IsPlayerInRange(PlayerId.PlayerOne);
				bool mugman = PlayerManager.Multiplayer && IsPlayerInRange(PlayerId.PlayerTwo);
				if (cuphead && mugman)
				{
					currentPlayer = ((lastPlayer == PlayerId.PlayerOne) ? PlayerId.PlayerTwo : PlayerId.PlayerOne);
					inRange = true;
				}
				else if (cuphead && !mugman)
				{
					currentPlayer = PlayerId.PlayerOne;
					inRange = true;
				}
				else if (!cuphead && mugman)
				{
					currentPlayer = PlayerId.PlayerTwo;
					inRange = true;
				}
				lastPlayer = currentPlayer;
				_target = PlayerManager.GetPlayer(currentPlayer);
				yield return null;
			}
			if (!_hasFired)
			{
				yield return CupheadTime.WaitForSeconds(this, _initialShotDelay.RandomFloat());
				_hasFired = true;
				continue;
			}
			if (_hasShootingAnimation)
			{
				StartShoot();
				yield return base.animator.WaitForAnimationToStart(this, "Shoot");
				_target = PlayerManager.GetPlayer(currentPlayer);
			}
			else
			{
				_target = PlayerManager.GetPlayer(currentPlayer);
				Shoot();
			}
			yield return CupheadTime.WaitForSeconds(this, _projectileDelay);
		}
	}

	private bool IsPlayerInRange(PlayerId player)
	{
		return Vector2.Distance(base.transform.position, PlayerManager.GetPlayer(player).center) <= triggerRange;
	}

	private IEnumerator triggerVolumes_cr()
	{
		PlayerId lastPlayer = PlayerId.None;
		while (true)
		{
			PlayerId currentPlayer = PlayerId.PlayerOne;
			bool within = false;
			while (!within)
			{
				bool cuphead = IsPlayerInVolumes(PlayerId.PlayerOne);
				bool mugman = PlayerManager.Multiplayer && IsPlayerInVolumes(PlayerId.PlayerTwo);
				if (cuphead && mugman)
				{
					currentPlayer = ((lastPlayer == PlayerId.PlayerOne) ? PlayerId.PlayerTwo : PlayerId.PlayerOne);
					within = true;
				}
				else if (cuphead && !mugman)
				{
					currentPlayer = PlayerId.PlayerOne;
					within = true;
				}
				else if (!cuphead && mugman)
				{
					currentPlayer = PlayerId.PlayerTwo;
					within = true;
				}
				lastPlayer = currentPlayer;
				yield return null;
			}
			if (!_hasFired)
			{
				yield return CupheadTime.WaitForSeconds(this, _initialShotDelay.RandomFloat());
				_hasFired = true;
				continue;
			}
			if (_hasShootingAnimation)
			{
				StartShoot();
				yield return base.animator.WaitForAnimationToStart(this, "Shoot");
				_target = PlayerManager.GetPlayer(currentPlayer);
			}
			else
			{
				_target = PlayerManager.GetPlayer(currentPlayer);
				Shoot();
			}
			yield return CupheadTime.WaitForSeconds(this, _projectileDelay);
		}
	}

	protected virtual bool IsPlayerInVolumes(PlayerId player)
	{
		Vector2 point = PlayerManager.GetPlayer(player).center;
		foreach (TriggerVolumeProperties triggerVolume in _triggerVolumes)
		{
			switch (triggerVolume.shape)
			{
			case TriggerVolumeProperties.Shape.BoxCollider:
			{
				Rect rect = RectUtils.NewFromCenter(triggerVolume.position.x, triggerVolume.position.y, triggerVolume.boxSize.x, triggerVolume.boxSize.y);
				if (triggerVolume.space == TriggerVolumeProperties.Space.RelativeSpace)
				{
					rect.x += base.transform.position.x;
					rect.y += base.transform.position.y;
				}
				if (rect.Contains(point))
				{
					return true;
				}
				break;
			}
			case TriggerVolumeProperties.Shape.CircleCollider:
			{
				Vector2 position = triggerVolume.position;
				if (triggerVolume.space == TriggerVolumeProperties.Space.RelativeSpace)
				{
					position.x += base.transform.position.x;
					position.y += base.transform.position.y;
				}
				if (MathUtils.CircleContains(position, triggerVolume.circleRadius, point))
				{
					return true;
				}
				break;
			}
			default:
				Debug.LogWarning("TriggerVolumeProperties.Shape '" + triggerVolume.shape.ToString() + "' not configured!");
				break;
			}
		}
		return false;
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		DrawGizmos(0.2f);
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		DrawGizmos(1f);
	}

	private void DrawGizmos(float alpha)
	{
		if (base.Properties != null)
		{
			switch (_triggerType)
			{
			case TriggerType.Range:
				DrawRangeTriggerGizmos(alpha);
				break;
			case TriggerType.TriggerVolumes:
				DrawTriggerVolumesTriggerGizmos(alpha);
				break;
			case TriggerType.Indefinite:
				DrawIndefiniteTriggerGizmos(alpha);
				break;
			}
			switch (base.Properties.ProjectileAimMode)
			{
			case EnemyProperties.AimMode.AimedAtPlayer:
				DrawAimedAtPlayerAimGizmos(alpha);
				break;
			case EnemyProperties.AimMode.Straight:
				DrawStraightAimGizmos(alpha);
				break;
			}
		}
	}

	private void DrawStraightAimGizmos(float alpha)
	{
		Color red = Color.red;
		red.a = alpha;
		Gizmos.color = red;
		Vector3 position = base.transform.position;
		Vector3 to = position + Quaternion.Euler(0f, 0f, base.Properties.ProjectileAngle) * Vector3.right * triggerRange;
		Vector3 to2 = position + Quaternion.Euler(0f, 0f, base.Properties.ProjectileAngle) * Vector3.right * 10000f;
		Gizmos.DrawLine(position, to);
		red.a *= 0.25f;
		Gizmos.color = red;
		Gizmos.DrawLine(position, to2);
	}

	private void DrawAimedAtPlayerAimGizmos(float alpha)
	{
		Color red = Color.red;
		red.a = alpha;
		Gizmos.color = red;
		Vector3 vector = base.transform.position + new Vector3(-100f, 100f, 0f);
		Vector3 size = Vector3.one * 40f / 2f;
		size.z = 0.001f;
		Vector3 vector2 = vector + new Vector3((0f - size.x) / 2f, size.y / 2f, 0f);
		Vector3 to = vector2;
		to.y -= size.y * 2f;
		Gizmos.DrawWireCube(vector, size);
		Gizmos.DrawLine(vector2, to);
	}

	private void DrawRangeTriggerGizmos(float alpha)
	{
		Color yellow = Color.yellow;
		yellow.a = alpha;
		Gizmos.color = yellow;
		Gizmos.DrawWireSphere(base.transform.position, triggerRange);
	}

	private void DrawTriggerVolumesTriggerGizmos(float alpha)
	{
		Color yellow = Color.yellow;
		yellow.a = alpha;
		Gizmos.color = yellow;
		foreach (TriggerVolumeProperties triggerVolume in _triggerVolumes)
		{
			Vector2 position = triggerVolume.position;
			if (triggerVolume.space == TriggerVolumeProperties.Space.RelativeSpace)
			{
				position += (Vector2)base.transform.position;
			}
			switch (triggerVolume.shape)
			{
			case TriggerVolumeProperties.Shape.CircleCollider:
				Gizmos.DrawWireSphere(position, triggerVolume.circleRadius);
				break;
			case TriggerVolumeProperties.Shape.BoxCollider:
				Gizmos.DrawWireCube(position, triggerVolume.boxSize);
				break;
			}
		}
	}

	private void DrawIndefiniteTriggerGizmos(float alpha)
	{
		Color yellow = Color.yellow;
		yellow.a = alpha;
		Gizmos.color = yellow;
		Vector3 vector = base.transform.position + new Vector3(100f, 100f, 0f);
		Vector3 vector2 = new Vector3(vector.x, vector.y + 10f, 0f);
		Vector3 vector3 = vector2;
		vector3.y -= 40f;
		Vector3 vector4 = vector2;
		vector4.x -= 10f;
		Vector3 to = vector2;
		to.x += 10f;
		Vector3 vector5 = vector3;
		vector5.x -= 10f;
		Vector3 to2 = vector3;
		to2.x += 10f;
		Gizmos.DrawLine(vector2, vector3);
		Gizmos.DrawLine(vector4, to);
		Gizmos.DrawLine(vector5, to2);
	}
}
