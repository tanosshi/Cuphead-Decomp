using System.Collections;
using UnityEngine;

public class MountainPlatformingLevelRock : AbstractProjectile
{
	[SerializeField]
	private Effect explosion;

	[SerializeField]
	private SpriteDeathParts[] deathParts;

	private Vector2 fallPos;

	private float velocity;

	private const float LAUNCH_VELOCITY_Y = 1000f;

	private const float LAUNCH_VELOCITY_X = 500f;

	private const float GRAVITY = 1000f;

	private float delay;

	public MountainPlatformingLevelRock Create(Vector2 startPos, Vector2 fallPos, float velocity, float delay)
	{
		MountainPlatformingLevelRock mountainPlatformingLevelRock = base.Create() as MountainPlatformingLevelRock;
		mountainPlatformingLevelRock.transform.position = startPos;
		mountainPlatformingLevelRock.fallPos = fallPos;
		mountainPlatformingLevelRock.velocity = velocity;
		mountainPlatformingLevelRock.delay = delay;
		return mountainPlatformingLevelRock;
	}

	protected override void Awake()
	{
		base.Awake();
		GetComponent<Collider2D>().enabled = false;
		animator.SetBool("PickedA", Rand.Bool());
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(launch_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	private IEnumerator launch_cr()
	{
		float x = Random.Range(-500f, 500f);
		while (base.transform.position.y < CupheadLevelCamera.Current.Bounds.yMax + 100f)
		{
			base.transform.AddPosition(x * (float)CupheadTime.Delta, 1000f * (float)CupheadTime.Delta);
			yield return null;
		}
		animator.SetTrigger("getBig");
		base.transform.position = fallPos;
		GetComponent<Collider2D>().enabled = true;
		GetComponent<SpriteRenderer>().sortingLayerName = SpriteLayer.Projectiles.ToString();
		yield return CupheadTime.WaitForSeconds(this, delay);
		while (true)
		{
			base.transform.AddPosition(0f, (0f - velocity) * (float)CupheadTime.Delta);
			velocity += 1000f * (float)CupheadTime.Delta;
			yield return null;
		}
	}

	protected override void Die()
	{
		base.Die();
		AudioManager.Play("castle_giant_rock_smash");
		emitAudioFromObject.Add("castle_giant_rock_smash");
		StopAllCoroutines();
		GetComponent<SpriteRenderer>().enabled = false;
		DeathParts();
		CupheadLevelCamera.Current.Shake(10f, 0.4f);
	}

	public void DeathParts()
	{
		explosion.Create(base.transform.position);
		SpriteDeathParts[] array = deathParts;
		foreach (SpriteDeathParts spriteDeathParts in array)
		{
			spriteDeathParts.CreatePart(base.transform.position);
		}
	}
}
