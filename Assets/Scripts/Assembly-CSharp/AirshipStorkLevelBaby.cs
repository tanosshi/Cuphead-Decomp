using System.Collections;
using UnityEngine;

public class AirshipStorkLevelBaby : AbstractCollidableObject
{
	public enum State
	{
		Move = 0,
		Dying = 1
	}

	private LevelProperties.AirshipStork.Babies properties;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private float onGroundY;

	private float gravity;

	private float health;

	private bool inAir;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		health -= info.damage;
		if (health <= 0f && state != State.Dying)
		{
			state = State.Dying;
			Die();
		}
	}

	public void Init(LevelProperties.AirshipStork.Babies properties, Vector2 pos, float health)
	{
		this.properties = properties;
		this.health = health;
		base.transform.position = pos;
		StartCoroutine(jump_cr());
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	private IEnumerator jump_cr()
	{
		state = State.Move;
		string[] pattern = properties.babyDelayString.GetRandom().Split(',');
		int i = Random.Range(0, pattern.Length);
		float waitTime = 0f;
		onGroundY = Level.Current.Ground;
		while (base.transform.position.x > -740f)
		{
			if (pattern[i][0] == 'D')
			{
				float.TryParse(pattern[i].Substring(1), out waitTime);
			}
			else
			{
				yield return CupheadTime.WaitForSeconds(this, waitTime);
				bool goingUp = true;
				bool highJump = pattern[i][0] == 'H';
				float velocityY = ((!highJump) ? properties.lowVerticalSpeed : properties.highVerticalSpeed);
				float speedX = ((!highJump) ? properties.lowHorizontalSpeed : properties.highHorizontalSpeed);
				gravity = ((!highJump) ? properties.lowGravity : properties.highGravity);
				inAir = true;
				while (goingUp || base.transform.position.y > onGroundY)
				{
					velocityY -= gravity * CupheadTime.FixedDelta;
					base.transform.AddPosition((0f - speedX) * CupheadTime.FixedDelta, velocityY * CupheadTime.FixedDelta);
					if (velocityY < 0f && goingUp)
					{
						goingUp = false;
					}
					yield return null;
				}
				base.transform.SetPosition(null, onGroundY);
				inAir = false;
			}
			i = (i + 1) % pattern.Length;
		}
		Die();
	}

	private void Die()
	{
		Object.Destroy(base.gameObject);
	}
}
