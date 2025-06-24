using UnityEngine;

public class ProjectileSpawner : AbstractPausableComponent
{
	public enum Type
	{
		Straight = 0,
		Aimed = 1
	}

	public Type type;

	public float delay = 1f;

	public float speed = 500f;

	public bool parryable;

	[Space(10f)]
	public float stoneTime;

	[Space(10f)]
	[SerializeField]
	private BasicProjectile projectilePrefab;

	public float angle;

	private float timer;

	private bool started;

	private Transform aim;

	protected override void Start()
	{
		base.Start();
		Level.Current.OnLevelStartEvent += OnStart;
		aim = new GameObject("Aim").transform;
		aim.SetParent(base.transform);
		aim.ResetLocalTransforms();
	}

	public void OnStart()
	{
		started = true;
	}

	public void OnStop()
	{
		started = false;
	}

	protected override void Update()
	{
		base.Update();
		if (!started || Level.Current == null || PlayerManager.Count < 1 || projectilePrefab == null)
		{
			return;
		}
		if (timer >= delay)
		{
			if (type == Type.Aimed)
			{
				aim.LookAt2D(PlayerManager.GetNext().transform);
				angle = aim.transform.eulerAngles.z;
			}
			BasicProjectile basicProjectile = projectilePrefab.Create(base.transform.position, angle, speed);
			if (parryable)
			{
				basicProjectile.SetParryable(parryable);
			}
			if (stoneTime > 0f)
			{
				basicProjectile.SetStoneTime(stoneTime);
			}
			timer = 0f;
		}
		else
		{
			timer += CupheadTime.Delta;
		}
	}
}
