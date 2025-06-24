using System.Collections;
using UnityEngine;

public class FlyingBirdLevelTurret : AbstractCollidableObject
{
	public enum State
	{
		Alive = 0,
		Dying = 1,
		Dead = 2,
		Respawn = 3
	}

	public class Properties
	{
		public readonly float health;

		public readonly float inTime;

		public readonly float x;

		public readonly float bulletSpeed;

		public readonly float bulletDelay;

		public readonly float floatRange;

		public readonly float floatTime;

		public Properties(float health, float inTime, float x, float bulletSpeed, float bulletDelay, float floatRange, float floatTime)
		{
			this.health = health;
			this.inTime = inTime;
			this.x = x;
			this.bulletSpeed = bulletSpeed;
			this.bulletDelay = bulletDelay;
			this.floatRange = floatRange;
			this.floatTime = floatTime;
		}
	}

	[SerializeField]
	private BasicProjectile childPrefab;

	private Vector2 startPos;

	private Properties properties;

	private Transform aim;

	public State state { get; private set; }

	public FlyingBirdLevelTurret Create(Vector2 pos, Properties properties)
	{
		FlyingBirdLevelTurret flyingBirdLevelTurret = InstantiatePrefab<FlyingBirdLevelTurret>();
		flyingBirdLevelTurret.transform.position = pos;
		flyingBirdLevelTurret.properties = properties;
		flyingBirdLevelTurret.Init();
		return flyingBirdLevelTurret;
	}

	protected override void Awake()
	{
		base.Awake();
		aim = new GameObject("Aim").transform;
		aim.SetParent(base.transform);
		aim.ResetLocalTransforms();
	}

	private void Init()
	{
		startPos = base.transform.position;
		StartCoroutine(go_cr());
		StartCoroutine(y_cr());
	}

	private void Shoot()
	{
		AbstractPlayerController next = PlayerManager.GetNext();
		if (!(next == null) && !(next.transform == null))
		{
			aim.LookAt2D(next.transform);
			BasicProjectile basicProjectile = childPrefab.Create(base.transform.position, aim.transform.eulerAngles.z, properties.bulletSpeed);
			basicProjectile.CollisionDeath.OnlyPlayer();
			basicProjectile.DamagesType.OnlyPlayer();
		}
	}

	private IEnumerator y_cr()
	{
		float start = startPos.y + properties.floatRange / 2f;
		float end = startPos.y - properties.floatRange / 2f;
		base.transform.SetPosition(null, start);
		float t = 0f;
		while (true)
		{
			t = 0f;
			while (t < properties.floatTime)
			{
				TransformExtensions.SetPosition(y: EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, start, end, t / properties.floatTime), transform: base.transform);
				t += (float)CupheadTime.Delta;
				yield return null;
			}
			t = 0f;
			while (t < properties.floatTime)
			{
				TransformExtensions.SetPosition(y: EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, end, start, t / properties.floatTime), transform: base.transform);
				t += (float)CupheadTime.Delta;
				yield return null;
			}
			yield return null;
		}
	}

	private IEnumerator go_cr()
	{
		float t = 0f;
		while (t < properties.inTime)
		{
			float val = t / properties.inTime;
			base.transform.SetPosition(EaseUtils.Ease(EaseUtils.EaseType.easeOutSine, startPos.x, properties.x, val));
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.SetPosition(properties.x);
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, properties.bulletDelay);
			Shoot();
		}
	}
}
