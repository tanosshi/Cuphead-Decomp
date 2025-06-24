using System.Collections;
using UnityEngine;

public class VeggiesLevelBeet : LevelProperties.Veggies.Entity
{
	public enum State
	{
		Start = 0,
		Go = 1,
		Complete = 2
	}

	public delegate void OnDamageTakenHandler(float damage);

	public const float MAX_Y = 360f;

	private const float POINTS_X_MIN = -150f;

	private const float POINTS_X_MAX = 640f;

	private const int POINTS_COUNT = 8;

	[SerializeField]
	private Transform babyRoot;

	[SerializeField]
	private VeggiesLevelBeetBaby babyPrefab;

	private new Animator animator;

	private new LevelProperties.Veggies.Beet properties;

	private BoxCollider2D collider;

	private float hp;

	private Transform[] points;

	private DamageDealer damageDealer;

	public State state { get; private set; }

	public event OnDamageTakenHandler OnDamageTakenEvent;

	protected override void Awake()
	{
		base.Awake();
		CreatePoints();
	}

	protected override void Start()
	{
		base.Start();
		animator = GetComponent<Animator>();
		collider = GetComponent<BoxCollider2D>();
		collider.enabled = false;
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Vector2[] array = GetPoints();
		foreach (Vector2 vector in array)
		{
			Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
			Gizmos.DrawLine(babyRoot.position, vector);
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(vector, 10f);
		}
		Gizmos.color = Color.red;
		Gizmos.DrawLine(babyRoot.position, new Vector3(-150f, 360f, 0f));
		Gizmos.DrawLine(babyRoot.position, new Vector3(640f, 360f, 0f));
	}

	public override void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
	{
		base.LevelInitWithGroup(propertyGroup);
		properties = propertyGroup as LevelProperties.Veggies.Beet;
		hp = properties.hp;
		GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
		damageDealer = new DamageDealer(1f, 0.2f, true, false, false);
		damageDealer.SetDirection(DamageDealer.Direction.Neutral, base.transform);
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (state != State.Start)
		{
			if (this.OnDamageTakenEvent != null)
			{
				this.OnDamageTakenEvent(info.damage);
			}
			hp -= info.damage;
			if (hp <= 0f)
			{
				Die();
			}
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	private void OnInAnimComplete()
	{
		collider.enabled = true;
		state = State.Go;
		StartCoroutine(beet_cr());
	}

	private void OnDeathAnimComplete()
	{
		state = State.Complete;
		Object.Destroy(base.gameObject);
	}

	private void Die()
	{
		StopAllCoroutines();
		StartCoroutine(die_cr());
	}

	private Vector2[] GetPoints()
	{
		Vector2[] array = new Vector2[8];
		for (int i = 0; i < 8; i++)
		{
			float t = (float)i / 7f;
			array[i] = Vector2.Lerp(new Vector2(-150f, 360f), new Vector2(640f, 360f), t);
		}
		return array;
	}

	private void CreatePoints()
	{
		Vector2[] array = GetPoints();
		points = new Transform[array.Length];
		for (int i = 0; i < points.Length; i++)
		{
			points[i] = new GameObject("Point " + i).transform;
			points[i].position = array[i];
			points[i].SetParent(base.transform);
		}
	}

	private float GetPointAngle(int i)
	{
		babyRoot.LookAt2D(points[i]);
		return babyRoot.eulerAngles.z;
	}

	private IEnumerator beet_cr()
	{
		string[] array = properties.babyPatterns[Random.Range(0, properties.babyPatterns.Length)].Split(',');
		int[] numbers = new int[array.Length];
		for (int i = 0; i < numbers.Length; i++)
		{
			if (!int.TryParse(array[i], out numbers[i]))
			{
				Debug.LogError("Veggies.Beet.babyPatterns is not formatted correctly!\nExpecting 4,5,6,5,4");
				StopAllCoroutines();
			}
		}
		int typeIndex = 0;
		int specialIndex = properties.alternateRate.RandomInt();
		VeggiesLevelBeetBaby.Type type = VeggiesLevelBeetBaby.Type.Regular;
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, properties.idleTime);
			animator.SetTrigger("Shoot_Start");
			yield return CupheadTime.WaitForSeconds(this, 1f);
			int loop = 0;
			int point = 0;
			while (loop < numbers.Length)
			{
				for (int j = 0; j < numbers[loop]; j++)
				{
					int newPoint;
					for (newPoint = point; newPoint == point; newPoint = Random.Range(0, 8))
					{
					}
					point = newPoint;
					if (typeIndex >= specialIndex)
					{
						type = ((!Rand.Bool()) ? VeggiesLevelBeetBaby.Type.Fat : VeggiesLevelBeetBaby.Type.Pink);
						typeIndex = 0;
						specialIndex = properties.alternateRate.RandomInt();
					}
					else
					{
						type = VeggiesLevelBeetBaby.Type.Regular;
					}
					typeIndex++;
					animator.SetTrigger("Shoot_" + type);
					babyPrefab.Create(type, properties.babySpeedUp, properties.babySpeedSpread, properties.babySpreadAngle, babyRoot.position, GetPointAngle(point));
					yield return CupheadTime.WaitForSeconds(this, properties.babyDelay);
				}
				loop++;
				if (loop < numbers.Length)
				{
					yield return CupheadTime.WaitForSeconds(this, properties.babyGroupDelay);
				}
			}
			animator.SetTrigger("Shoot_End");
			yield return CupheadTime.WaitForSeconds(this, properties.babyGroupDelay);
		}
	}

	private IEnumerator die_cr()
	{
		collider.enabled = false;
		animator.SetTrigger("Idle");
		yield return StartCoroutine(dieFlash_cr());
		animator.SetTrigger("Dead");
	}
}
