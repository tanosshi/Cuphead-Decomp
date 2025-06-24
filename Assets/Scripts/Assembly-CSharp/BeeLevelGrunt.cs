using UnityEngine;

public class BeeLevelGrunt : AbstractCollidableObject
{
	[SerializeField]
	private BeeLevelGruntBriefcase briefcasePrefab;

	private float health;

	private float speed;

	private DamageDealer damageDealer;

	private new Animator animator;

	private bool dead;

	public BeeLevelGrunt Create(Vector2 pos, int xScale, int health, float speed)
	{
		BeeLevelGrunt beeLevelGrunt = Object.Instantiate(this);
		beeLevelGrunt.speed = speed;
		beeLevelGrunt.health = health;
		beeLevelGrunt.transform.SetScale(xScale, 1f, 1f);
		beeLevelGrunt.transform.position = pos;
		return beeLevelGrunt;
	}

	protected override void Awake()
	{
		base.Awake();
		GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
		damageDealer = new DamageDealer(1f, 1f, true, false, false);
		animator = GetComponent<Animator>();
	}

	protected override void Update()
	{
		base.Update();
		if (!dead)
		{
			if (base.transform.position.x < -1280f || base.transform.position.x > 1280f)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				base.transform.AddPosition(speed * (float)CupheadTime.Delta * (0f - base.transform.localScale.x));
			}
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		health -= info.damage;
		if (health <= 0f)
		{
			Die();
		}
	}

	private void Die()
	{
		AudioManager.Play(Sfx.Level_Bee_Grunt_Death);
		dead = true;
		briefcasePrefab.Create((int)base.transform.localScale.x, base.transform.position);
		GetComponent<Collider2D>().enabled = false;
		animator.Play("Die");
		base.transform.SetEulerAngles(0f, 0f, Random.Range(0, 360));
		base.transform.SetScale(MathUtils.PlusOrMinus(), MathUtils.PlusOrMinus(), 1f);
	}

	private void OnDeathAnimComplete()
	{
		Object.Destroy(base.gameObject);
	}
}
