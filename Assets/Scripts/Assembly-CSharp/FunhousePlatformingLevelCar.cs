using System.Collections;
using UnityEngine;

public class FunhousePlatformingLevelCar : AbstractCollidableObject
{
	private static bool CARS_ALIVE;

	[SerializeField]
	private GameObject[] carSprites;

	private bool leader;

	private bool last;

	private float speed;

	private DamageDealer damageDealer;

	protected override void Awake()
	{
		base.Awake();
		CARS_ALIVE = true;
		damageDealer = DamageDealer.NewEnemy();
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	public void Init(Vector2 pos, float rotation, float carSpeed, int index, bool leader, bool last)
	{
		base.transform.position = pos;
		base.transform.SetEulerAngles(null, null, rotation);
		speed = carSpeed;
		this.leader = leader;
		this.last = last;
		GameObject[] array = carSprites;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
		carSprites[index].SetActive(true);
		StartCoroutine(move_cr());
	}

	private IEnumerator move_cr()
	{
		if (leader)
		{
			AudioManager.PlayLoop("funhouse_car_idle");
			emitAudioFromObject.Add("funhouse_car_idle");
		}
		YieldInstruction wait = new WaitForFixedUpdate();
		float size = GetComponent<Collider2D>().bounds.size.x;
		while (base.transform.position.x > CupheadLevelCamera.Current.Bounds.xMin - (size + 50f))
		{
			base.transform.position += Vector3.left * speed * CupheadTime.FixedDelta;
			yield return wait;
		}
		if (last && !CARS_ALIVE)
		{
			AudioManager.Stop("funhouse_car_idle");
		}
		CARS_ALIVE = false;
		Object.Destroy(base.gameObject);
	}
}
