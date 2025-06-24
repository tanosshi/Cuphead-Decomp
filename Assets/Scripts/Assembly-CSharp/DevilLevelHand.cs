using System.Collections;
using UnityEngine;

public class DevilLevelHand : AbstractCollidableObject
{
	public enum State
	{
		Uninitialized = 0,
		Idle = 1
	}

	public State state;

	public bool despawned;

	public bool isDead;

	[SerializeField]
	private bool onLeft;

	[SerializeField]
	private float shootAngle;

	[SerializeField]
	private Transform bulletRoot;

	[SerializeField]
	private BasicProjectile bulletPrefab;

	[SerializeField]
	private BasicProjectile bulletPinkPrefab;

	[Header("Sprites")]
	[SerializeField]
	private SpriteRenderer demonSprite;

	private Vector3 demonLocalStartPos;

	[SerializeField]
	private SpriteRenderer handSprite;

	private Vector3 handLocalStartPos;

	private LevelProperties.Devil.Hands properties;

	private DamageReceiver damageReceiver;

	private DamageDealer damageDealer;

	private Vector3 startPos;

	private float hp;

	private float maxHp;

	private bool firstTime = true;

	private bool isInvincible = true;

	private bool isSliding;

	private bool startAtTop = true;

	private int pinkStringIndex;

	protected override void Awake()
	{
		base.Awake();
		isDead = false;
		damageDealer = DamageDealer.NewEnemy();
		demonSprite.GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
		demonSprite.GetComponent<CollisionChild>().OnPlayerCollision += OnCollisionPlayer;
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
		if (!isInvincible)
		{
			hp -= info.damage;
			if (hp < 0f)
			{
				Die();
			}
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (damageDealer != null)
		{
			damageDealer.DealDamage(hit);
		}
	}

	public void StartPattern(LevelProperties.Devil.Hands properties)
	{
		this.properties = properties;
		pinkStringIndex = Random.Range(0, properties.pinkString.Length);
		maxHp = properties.HP;
		hp = maxHp;
		state = State.Idle;
		startPos = new Vector2(base.transform.position.x, properties.yRange.max);
		base.transform.position = startPos;
		handLocalStartPos = handSprite.transform.localPosition;
		demonLocalStartPos = demonSprite.transform.localPosition;
		base.gameObject.SetActive(true);
		StartCoroutine(move_cr());
	}

	public void SpawnIn()
	{
		StartCoroutine(move_in_cr());
	}

	private IEnumerator move_in_cr()
	{
		startAtTop = true;
		despawned = false;
		base.transform.position = startPos;
		handSprite.transform.localPosition = handLocalStartPos;
		demonSprite.transform.localPosition = demonLocalStartPos;
		base.animator.Play("Off", 1);
		base.animator.Play("Hand_Loop");
		firstTime = true;
		float xPos = 547f;
		Vector3 start = new Vector3(startPos.x, properties.yRange.max);
		Vector3 end = new Vector3((!onLeft) ? xPos : (0f - xPos), properties.yRange.max);
		float t = 0f;
		float time = 1f;
		YieldInstruction wait = new WaitForFixedUpdate();
		while (t < time)
		{
			t += CupheadTime.FixedDelta;
			base.transform.position = Vector3.Lerp(start, end, t / time);
			yield return wait;
		}
		isInvincible = false;
		StartCoroutine(hand_move_up_cr());
	}

	private IEnumerator move_cr()
	{
		float moveTime = (properties.yRange.max - properties.yRange.min) / properties.speed;
		float t = 0f;
		float startY = demonSprite.transform.position.y;
		float endY = properties.yRange.min;
		while (true)
		{
			if (!isSliding)
			{
				yield return null;
				continue;
			}
			startY = demonSprite.transform.position.y;
			endY = properties.yRange.min;
			startAtTop = false;
			while (isSliding)
			{
				t = 0f;
				while (t < moveTime && isSliding)
				{
					demonSprite.transform.SetPosition(null, Mathf.Lerp(startY, endY, t / moveTime));
					t += CupheadTime.FixedDelta;
					yield return new WaitForFixedUpdate();
				}
				startY = ((!startAtTop) ? properties.yRange.min : properties.yRange.max);
				endY = ((!startAtTop) ? properties.yRange.max : properties.yRange.min);
				startAtTop = !startAtTop;
				yield return new WaitForFixedUpdate();
			}
			yield return new WaitForFixedUpdate();
		}
	}

	public void Fire()
	{
		if (properties.pinkString[pinkStringIndex] == 'P')
		{
			BasicProjectile basicProjectile = bulletPinkPrefab.Create(bulletRoot.position, shootAngle, properties.bulletSpeed);
			basicProjectile.transform.SetScale((!onLeft) ? 1 : 1, onLeft ? 1 : (-1));
		}
		else
		{
			BasicProjectile basicProjectile2 = bulletPrefab.Create(bulletRoot.position, shootAngle, properties.bulletSpeed);
			basicProjectile2.transform.SetScale((!onLeft) ? 1 : 1, onLeft ? 1 : (-1));
		}
		pinkStringIndex = (pinkStringIndex + 1) % properties.pinkString.Length;
	}

	public void Die()
	{
		isSliding = false;
		isInvincible = true;
		StartCoroutine(demon_move_down_cr());
	}

	private IEnumerator hand_move_up_cr()
	{
		base.animator.SetTrigger("OnRelease");
		yield return base.animator.WaitForAnimationToEnd(this, "Hand_Release_Start");
		isSliding = true;
		float t = 0f;
		float time = 0.5f;
		float start = handSprite.transform.position.y;
		float end = 860f;
		YieldInstruction wait = new WaitForFixedUpdate();
		while (t < time)
		{
			t += CupheadTime.FixedDelta;
			handSprite.transform.SetPosition(null, Mathf.Lerp(start, end, t / time));
			yield return wait;
		}
		yield return wait;
	}

	private IEnumerator demon_move_down_cr()
	{
		base.animator.SetTrigger("OnDeath");
		float t = 0f;
		float time = 1f;
		float start = demonSprite.transform.position.y;
		float end = -860f;
		YieldInstruction wait = new WaitForFixedUpdate();
		while (t < time)
		{
			t += CupheadTime.FixedDelta;
			demonSprite.transform.SetPosition(null, Mathf.Lerp(start, end, t / time));
			yield return wait;
		}
		yield return wait;
		if (isDead)
		{
			Object.Destroy(base.gameObject);
		}
		despawned = true;
		hp = maxHp;
	}

	private void SFXAttack()
	{
		AudioManager.Play("fat_bat_attack");
		emitAudioFromObject.Add("fat_bat_attack");
	}

	private void SFXDeath()
	{
		AudioManager.Play("fat_bat_die");
		emitAudioFromObject.Add("fat_bat_die");
	}

	private void SFXHandRelease()
	{
		AudioManager.Play("p3_hand_release_start");
		emitAudioFromObject.Add("p3_hand_release_start");
	}

	private void SFXFatSpawn()
	{
		AudioManager.Play("fat_bat_spawn");
		emitAudioFromObject.Add("fat_bat_spawn");
	}
}
