using System.Collections;
using UnityEngine;

public class DragonLevelPotion : AbstractProjectile
{
	public enum PotionType
	{
		Horizontal = 0,
		Vertical = 1,
		Both = 2
	}

	public enum State
	{
		Alive = 0,
		Dead = 1
	}

	private const string ExplodeTrigger = "Explode";

	[SerializeField]
	private BasicProjectile bulletPrefab;

	public PotionType type;

	private LevelProperties.Dragon.Potions properties;

	private DamageReceiver damageReceiver;

	private float hp;

	private float rotation;

	public State state { get; private set; }

	public void Init(Vector2 pos, float hp, float rotation, LevelProperties.Dragon.Potions properties)
	{
		base.transform.position = pos;
		this.hp = hp;
		this.properties = properties;
		this.rotation = rotation;
		base.transform.SetScale(properties.potionScale, properties.potionScale, properties.potionScale);
		base.transform.SetEulerAngles(0f, 0f, rotation);
		state = State.Alive;
		StartCoroutine(move_cr());
	}

	protected override void Awake()
	{
		base.Awake();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		hp -= info.damage;
		if (hp < 0f && state != State.Dead)
		{
			state = State.Dead;
			StartCoroutine(handle_die_cr());
		}
	}

	private IEnumerator move_cr()
	{
		while (true)
		{
			base.transform.position += base.transform.right * properties.potionSpeed * CupheadTime.Delta;
			yield return null;
		}
	}

	private IEnumerator handle_die_cr()
	{
		switch (type)
		{
		case PotionType.Horizontal:
			SpawnProjectile(Vector3.right);
			SpawnProjectile(-Vector3.right);
			break;
		case PotionType.Vertical:
			SpawnProjectile(Vector3.up);
			SpawnProjectile(-Vector3.up);
			break;
		case PotionType.Both:
			SpawnProjectile(Vector3.right);
			SpawnProjectile(-Vector3.right);
			SpawnProjectile(Vector3.up);
			SpawnProjectile(-Vector3.up);
			break;
		}
		Animator anim = GetComponent<Animator>();
		anim.SetTrigger("Explode");
		GetComponent<Collider2D>().enabled = false;
		yield return CupheadTime.WaitForSeconds(this, 0.5f);
		Object.Destroy(base.gameObject);
		yield return null;
	}

	private void SpawnProjectile(Vector3 direction)
	{
		float num = MathUtils.DirectionToAngle(direction);
		bulletPrefab.Create(base.transform.position, num, properties.spitBulletSpeed).transform.SetScale(properties.explosionBulletScale, properties.explosionBulletScale, properties.explosionBulletScale);
	}
}
