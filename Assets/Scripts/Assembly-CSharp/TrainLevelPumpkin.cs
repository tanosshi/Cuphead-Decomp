using System.Collections;
using UnityEngine;

public class TrainLevelPumpkin : AbstractCollidableObject
{
	[SerializeField]
	private TrainLevelPumpkinProjectile brickPrefab;

	private TrainLevelPumpkinProjectile brick;

	private int direction;

	private float speed;

	private float health;

	private float fallTime;

	private Transform target;

	private DamageReceiver damageReceiver;

	private new Animator animator;

	public void Create(Vector2 pos, int direction, float speed, float health, float fallTime, Transform target)
	{
		TrainLevelPumpkin trainLevelPumpkin = InstantiatePrefab<TrainLevelPumpkin>();
		trainLevelPumpkin.transform.position = pos;
		trainLevelPumpkin.transform.SetScale(-direction, 1f, 1f);
		trainLevelPumpkin.direction = direction;
		trainLevelPumpkin.health = health;
		trainLevelPumpkin.speed = speed;
		trainLevelPumpkin.target = target;
		trainLevelPumpkin.fallTime = fallTime;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(x_cr());
		StartCoroutine(drop_cr());
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		animator = GetComponent<Animator>();
		brick = brickPrefab.Create() as TrainLevelPumpkinProjectile;
		brick.fallTime = fallTime;
		brick.transform.SetParent(base.transform);
		brick.transform.ResetLocalTransforms();
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		health -= info.damage;
		if (health <= 0f)
		{
			Die();
		}
	}

	private void Drop()
	{
		if (brick != null)
		{
			brick.Drop();
			brick = null;
			StartCoroutine(y_cr());
		}
	}

	private void Die()
	{
		StopAllCoroutines();
		Drop();
		animator.Play("Die");
		AudioManager.Play("train_pumpkin_die");
		emitAudioFromObject.Add("train_pumpkin_die");
	}

	private void OnDeathAnimComplete()
	{
		End();
	}

	private void End()
	{
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}

	private IEnumerator x_cr()
	{
		while (true)
		{
			base.transform.AddPosition(speed * (float)CupheadTime.Delta * (float)direction);
			yield return null;
		}
	}

	private IEnumerator y_cr()
	{
		float ySpeed = 1f;
		float mult = 1.1f;
		while (true)
		{
			base.transform.AddPosition(0f, ySpeed * (float)CupheadTime.Delta);
			if ((float)CupheadTime.Delta != 0f)
			{
				ySpeed *= mult;
			}
			yield return null;
			if (base.transform.position.y > 720f)
			{
				End();
			}
		}
	}

	private IEnumerator drop_cr()
	{
		bool check = true;
		while (check)
		{
			if (direction > 0 && base.transform.position.x > target.position.x)
			{
				check = false;
				Drop();
			}
			if (direction < 0 && base.transform.position.x < target.position.x)
			{
				check = false;
				Drop();
			}
			yield return null;
		}
	}
}
