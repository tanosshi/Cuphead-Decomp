using System.Collections;
using UnityEngine;

public class TrainLevelBlindSpecterEyeProjectile : AbstractProjectile
{
	private const float FRAME_TIME = 1f / 24f;

	[SerializeField]
	private Effect effectPrefab;

	[SerializeField]
	private Transform sprite;

	private DamageReceiver damageReceiver;

	private float health;

	private float startPos;

	private float t;

	private float start;

	private float end;

	private Vector2 time;

	public TrainLevelBlindSpecterEyeProjectile Create(Vector2 pos, Vector2 time, float y, bool flipped, float health)
	{
		TrainLevelBlindSpecterEyeProjectile trainLevelBlindSpecterEyeProjectile = base.Create() as TrainLevelBlindSpecterEyeProjectile;
		trainLevelBlindSpecterEyeProjectile.transform.position = pos;
		trainLevelBlindSpecterEyeProjectile.time = time;
		trainLevelBlindSpecterEyeProjectile.end = y;
		trainLevelBlindSpecterEyeProjectile.health = health;
		if (flipped)
		{
			trainLevelBlindSpecterEyeProjectile.sprite.transform.SetScale(-1f);
		}
		return trainLevelBlindSpecterEyeProjectile;
	}

	protected override void Start()
	{
		base.Start();
		startPos = base.transform.position.y;
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		StartCoroutine(x_cr());
		StartCoroutine(y_cr());
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (base.damageDealer != null)
		{
			base.damageDealer.DealDamage(hit);
		}
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		health -= info.damage;
		if (health <= 0f)
		{
			Die();
		}
	}

	private void End()
	{
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}

	protected override void Die()
	{
		base.Die();
		StopAllCoroutines();
	}

	protected override void OnCollisionGround(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionGround(hit, phase);
		if (phase == CollisionPhase.Enter && hit.GetComponent<TrainLevelPlatform>() != null)
		{
			start = hit.transform.position.y + 20f;
			t = 1000f;
		}
	}

	private IEnumerator x_cr()
	{
		float start = base.transform.position.x;
		float t = 0f;
		while (t < time.x)
		{
			float val = t / time.x;
			TransformExtensions.SetPosition(x: Mathf.Lerp(start, -740f, val), transform: base.transform);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		End();
	}

	private IEnumerator y_cr()
	{
		int counter = 0;
		int maxCounter = 2;
		float frameTime = 0f;
		YieldInstruction wait = new WaitForFixedUpdate();
		start = base.transform.position.y;
		while (true)
		{
			AudioManager.Play("train_blindspector_eye_bounce");
			emitAudioFromObject.Add("train_blindspector_eye_bounce");
			t = 0f;
			float newY = start;
			while (t < time.y)
			{
				TransformExtensions.SetPosition(y: EaseUtils.Ease(value: t / time.y, ease: EaseUtils.EaseType.easeOutSine, start: start, end: end), transform: base.transform);
				t += CupheadTime.FixedDelta;
				yield return wait;
			}
			base.transform.SetPosition(null, end);
			start = startPos;
			yield return null;
			t = 0f;
			while (t < time.y)
			{
				TransformExtensions.SetPosition(y: EaseUtils.Ease(value: t / time.y, ease: EaseUtils.EaseType.easeInSine, start: end, end: start), transform: base.transform);
				t += CupheadTime.FixedDelta;
				yield return wait;
			}
			base.transform.SetPosition(null, start);
			effectPrefab.Create(base.transform.position);
			while (counter < maxCounter)
			{
				frameTime += CupheadTime.FixedDelta;
				if (frameTime > 1f / 24f)
				{
					counter++;
					frameTime -= 1f / 24f;
					if (counter >= 2)
					{
						base.transform.SetScale(null, 0.3f);
						break;
					}
					base.transform.SetScale(null, 0.5f);
				}
				yield return wait;
			}
			counter = 0;
			base.transform.SetScale(null, 1f);
			yield return wait;
		}
	}
}
