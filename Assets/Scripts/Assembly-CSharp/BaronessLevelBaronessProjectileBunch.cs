using System.Collections;
using UnityEngine;

public class BaronessLevelBaronessProjectileBunch : AbstractProjectile
{
	public LevelProperties.Baroness.BaronessVonBonbon properties;

	private BaronessLevelCastle parent;

	private float velocity;

	private float rotationSpeed;

	private bool isActive;

	private Vector3 pointAt;

	public void Init(Vector2 pos, float velocity, float pointAt, float rotationSpeed, LevelProperties.Baroness.BaronessVonBonbon properties, BaronessLevelCastle parent)
	{
		base.transform.position = pos;
		this.properties = properties;
		this.pointAt = MathUtils.AngleToDirection(pointAt);
		this.rotationSpeed = rotationSpeed;
		this.velocity = velocity;
		this.parent = parent;
		this.parent.OnDeathEvent += KillProjectileBunch;
	}

	private void KillProjectileBunch()
	{
		isActive = false;
	}

	protected override void Awake()
	{
		base.Awake();
		isActive = true;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(scale_up_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (!isActive)
		{
			Dying();
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		base.transform.position += pointAt * velocity * CupheadTime.FixedDelta;
	}

	private IEnumerator scale_up_cr()
	{
		float t = 0f;
		float time = 0.3f;
		base.transform.SetScale(0f, 0f, 0f);
		while (t < time)
		{
			t += (float)CupheadTime.Delta;
			base.transform.SetScale(t / time, t / time, t / time);
			yield return null;
		}
		yield return null;
	}

	private void Dying()
	{
		if (GetComponent<SpriteRenderer>() != null)
		{
			GetComponent<SpriteRenderer>().enabled = false;
		}
		base.Die();
	}
}
