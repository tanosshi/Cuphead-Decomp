using System.Collections;
using UnityEngine;

public class TrainLevelEngineBossFireProjectile : AbstractProjectile
{
	public enum State
	{
		Init = 0,
		Moving = 1,
		Dead = 2
	}

	private const string IdleStateName = "Idle";

	private const string DirectionParameterName = "Direction";

	private static readonly Vector3 TrailOffset = new Vector3(0f, 10f, 0f);

	private static ContactFilter2D filter = default(ContactFilter2D).NoFilter();

	private static Collider2D[] buffer = new Collider2D[10];

	[SerializeField]
	private Effect trailPrefab;

	public const float GROUND_Y = -300f;

	private State state;

	private Vector2 velocity;

	private float gravity;

	private CircleCollider2D collider2d;

	private SpriteRenderer spriteRenderer;

	public void Create(Vector2 pos, Vector2 velocity, float gravity)
	{
		InstantiatePrefab<TrainLevelEngineBossFireProjectile>().Init(pos, velocity, gravity);
	}

	protected override void Start()
	{
		base.Start();
		animator.SetFloat("Direction", (!(velocity.x > 0f)) ? 1 : (-1));
		animator.Play("Idle", 0, Random.value);
		base.transform.eulerAngles = new Vector3(0f, 0f, Random.Range(0, 360));
		collider2d = GetComponent<CircleCollider2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		StartCoroutine(spawnTrail_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (state == State.Moving)
		{
			base.transform.AddPosition(velocity.x * (float)CupheadTime.Delta, velocity.y * (float)CupheadTime.Delta);
			velocity.y -= gravity * (float)CupheadTime.Delta;
			if (base.transform.position.y < -300f)
			{
				Die();
			}
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		int num = collider2d.OverlapCollider(filter, buffer);
		for (int i = 0; i < num; i++)
		{
			if ((bool)buffer[i].GetComponent<TrainLevelPlatform>())
			{
				Die();
				break;
			}
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
		Die();
	}

	protected override void Die()
	{
		base.Die();
		base.transform.rotation = Quaternion.identity;
		spriteRenderer.flipX = Rand.Bool();
		state = State.Dead;
	}

	private void Init(Vector2 pos, Vector2 velocity, float gravity)
	{
		base.transform.position = pos;
		this.velocity = velocity;
		this.gravity = gravity;
		state = State.Moving;
	}

	public IEnumerator spawnTrail_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, 0.1f);
			trailPrefab.Create(base.transform.position + TrailOffset).Play();
		}
	}
}
