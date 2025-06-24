using System.Collections;
using UnityEngine;

public class FlyingGenieLevelBomb : AbstractProjectile
{
	public enum BombType
	{
		Regular = 0,
		Diagonal = 1,
		PlusSized = 2
	}

	public BombType bombType;

	public bool readyToDetonate;

	[SerializeField]
	private GameObject[] explosionBeams;

	private LevelProperties.FlyingGenie.Bomb properties;

	private new DamageDealer damageDealer;

	private Vector3 targetPos;

	public FlyingGenieLevelBomb Create(Vector2 pos, Vector3 targetPos, LevelProperties.FlyingGenie.Bomb properties)
	{
		FlyingGenieLevelBomb flyingGenieLevelBomb = base.Create() as FlyingGenieLevelBomb;
		flyingGenieLevelBomb.transform.position = pos;
		flyingGenieLevelBomb.properties = properties;
		flyingGenieLevelBomb.targetPos = targetPos;
		return flyingGenieLevelBomb;
	}

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		GameObject[] array = explosionBeams;
		foreach (GameObject gameObject in array)
		{
			gameObject.GetComponent<SpriteRenderer>().enabled = false;
			gameObject.GetComponent<Collider2D>().enabled = false;
			gameObject.GetComponent<CollisionChild>().OnPlayerCollision += OnCollisionPlayer;
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void Start()
	{
		base.Start();
		readyToDetonate = false;
		GameObject[] array = explosionBeams;
		foreach (GameObject gameObject in array)
		{
			if (bombType == BombType.Regular)
			{
				gameObject.transform.SetScale(properties.bombRegularSize, properties.bombRegularSize);
			}
			else if (bombType == BombType.Diagonal)
			{
				gameObject.transform.SetScale(properties.bombDiagonalSize, properties.bombDiagonalSize);
			}
			else if (bombType == BombType.PlusSized)
			{
				gameObject.transform.SetScale(properties.bombPlusSize, properties.bombPlusSize);
			}
		}
		StartCoroutine(start_cr());
	}

	private IEnumerator start_cr()
	{
		while (base.transform.position != targetPos)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, targetPos, properties.bombSpeed * (float)CupheadTime.Delta);
			yield return null;
		}
		readyToDetonate = true;
		yield return null;
	}

	public void Explode()
	{
		StartCoroutine(explode_cr());
	}

	private IEnumerator explode_cr()
	{
		GameObject[] array = explosionBeams;
		foreach (GameObject gameObject in array)
		{
			gameObject.GetComponent<SpriteRenderer>().enabled = true;
			gameObject.GetComponent<Collider2D>().enabled = true;
		}
		yield return CupheadTime.WaitForSeconds(this, 1f);
		GetComponent<SpriteRenderer>().enabled = false;
		GameObject[] array2 = explosionBeams;
		foreach (GameObject gameObject2 in array2)
		{
			gameObject2.gameObject.SetActive(false);
		}
		readyToDetonate = false;
		Die();
		yield return null;
	}

	protected override void Die()
	{
		StopAllCoroutines();
		base.Die();
	}
}
