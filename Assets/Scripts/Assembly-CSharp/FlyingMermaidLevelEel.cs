using System.Collections;
using UnityEngine;

public class FlyingMermaidLevelEel : AbstractCollidableObject
{
	public enum State
	{
		Unspawned = 0,
		Spawned = 1
	}

	[SerializeField]
	private float riseTime;

	[SerializeField]
	private float riseDistance;

	[SerializeField]
	private float leaveTime;

	[SerializeField]
	private MinMax segmentY;

	[SerializeField]
	private int numSegments;

	[SerializeField]
	private BasicProjectile projectilePrefab;

	[SerializeField]
	private Transform projectileRoot;

	[SerializeField]
	private FlyingMermaidLevelEelSegment headSegmentPrefab;

	[SerializeField]
	private FlyingMermaidLevelEelSegment[] bodySegmentPrefabs;

	private LevelProperties.FlyingMermaid.Eel properties;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private string[] bulletPinkPattern;

	private int bulletPinkIndex;

	private float initialY;

	private float hp;

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
		hp -= info.damage;
		if (hp < 0f && state == State.Spawned)
		{
			Die(true, false);
		}
	}

	public void Die(bool explode, bool permanent)
	{
		StopAllCoroutines();
		Collider2D component = GetComponent<Collider2D>();
		component.enabled = false;
		base.animator.SetTrigger("Despawn");
		base.animator.ResetTrigger("Continue");
		base.animator.ResetTrigger("Idle");
		base.animator.ResetTrigger("Leave");
		float num = ((!(GetComponent<SpriteRenderer>().sortingLayerName == "Foreground")) ? (-270) : (-380));
		if (explode && state == State.Spawned)
		{
			SpriteRenderer component2 = GetComponent<SpriteRenderer>();
			for (int i = 0; i < numSegments; i++)
			{
				float floatAt = segmentY.GetFloatAt((float)i / ((float)numSegments - 1f));
				Vector2 position = base.transform.position;
				position.y += floatAt;
				if (position.y < num + 30f)
				{
					continue;
				}
				FlyingMermaidLevelEelSegment flyingMermaidLevelEelSegment = null;
				flyingMermaidLevelEelSegment = ((i != numSegments - 1) ? bodySegmentPrefabs.RandomChoice() : headSegmentPrefab);
				string text = component2.sortingLayerName;
				int sortingOrder = component2.sortingOrder;
				int num2 = Random.Range(-1, 2);
				if (text == "Foreground")
				{
					switch (num2)
					{
					case -1:
						text = "Enemies";
						sortingOrder = 1000;
						break;
					case 1:
						sortingOrder = 21;
						break;
					}
				}
				else
				{
					switch (num2)
					{
					case -1:
						text = "Background";
						sortingOrder = 75;
						break;
					case 1:
						text = "Foreground";
						sortingOrder = 1;
						break;
					}
				}
				flyingMermaidLevelEelSegment.Create(position, text, sortingOrder);
			}
		}
		if (!permanent)
		{
			StartCoroutine(main_cr());
		}
		state = State.Unspawned;
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	public void Init(LevelProperties.FlyingMermaid.Eel properties)
	{
		this.properties = properties;
		initialY = base.transform.localPosition.y;
		Collider2D component = GetComponent<Collider2D>();
		component.enabled = false;
		bulletPinkPattern = properties.bulletPinkString.Split(',');
		bulletPinkIndex = Random.Range(0, bulletPinkPattern.Length);
	}

	public void StartPattern()
	{
		StartCoroutine(main_cr());
	}

	public IEnumerator main_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, properties.appearDelay.RandomFloat());
		state = State.Spawned;
		base.animator.SetTrigger("Spawn");
		AudioManager.Play("level_mermaid_eel_intro");
		float t = 0f;
		hp = properties.hp;
		Collider2D collider = GetComponent<Collider2D>();
		collider.enabled = true;
		while (t < riseTime - 0.25f)
		{
			t += (float)CupheadTime.Delta;
			base.transform.SetLocalPosition(null, Mathf.Lerp(initialY - riseDistance, initialY, t / riseTime));
			yield return null;
		}
		base.animator.SetTrigger("Continue");
		while (t < riseTime)
		{
			t += (float)CupheadTime.Delta;
			base.transform.SetLocalPosition(null, Mathf.Lerp(initialY - riseDistance, initialY, t / riseTime));
			yield return null;
		}
		base.transform.SetLocalPosition(null, initialY);
		yield return base.animator.WaitForAnimationToStart(this, "Idle");
		for (int numAttacks = properties.attackAmount.RandomInt(); numAttacks >= 0; numAttacks--)
		{
			yield return CupheadTime.WaitForSeconds(this, properties.idleTime.RandomFloat());
			AudioManager.Play("level_mermaid_eel_attack_start");
			base.animator.SetTrigger("Attack");
			yield return base.animator.WaitForAnimationToEnd(this, "Attack_Start");
			FireProjectiles();
			yield return base.animator.WaitForAnimationToEnd(this, "Attack_End");
			AudioManager.Play("level_mermaid_eel_attack_end");
		}
		yield return CupheadTime.WaitForSeconds(this, properties.idleTime.RandomFloat());
		base.animator.SetTrigger("Leave");
		yield return base.animator.WaitForAnimationToEnd(this, "Leave_Start");
		AudioManager.Play("level_mermaid_eel_attack_leave");
		t = 0f;
		bool spawnedSplash = false;
		SpriteRenderer sprite = GetComponent<SpriteRenderer>();
		float waterY = ((!(sprite.sortingLayerName == "Foreground")) ? (-270) : (-380));
		while (t < leaveTime)
		{
			t += (float)CupheadTime.Delta;
			base.transform.SetLocalPosition(null, Mathf.Lerp(initialY, initialY - riseDistance, t / leaveTime));
			if (!spawnedSplash && base.transform.position.y < waterY - 80f)
			{
				FlyingMermaidLevelSplashManager.Instance.SpawnSplashMedium(base.gameObject, 35f, true, waterY + 80f);
				spawnedSplash = true;
			}
			yield return null;
		}
		Die(false, false);
	}

	private void FireProjectiles()
	{
		SpriteRenderer component = GetComponent<SpriteRenderer>();
		for (int i = 0; (float)i < properties.numBullets; i++)
		{
			float floatAt = properties.spreadAngle.GetFloatAt((float)i / (properties.numBullets - 1f));
			BasicProjectile basicProjectile = projectilePrefab.Create(projectileRoot.position, floatAt, properties.bulletSpeed);
			basicProjectile.SetParryable(bulletPinkPattern[bulletPinkIndex][0] == 'P');
			bulletPinkIndex = (bulletPinkIndex + 1) % bulletPinkPattern.Length;
		}
	}
}
