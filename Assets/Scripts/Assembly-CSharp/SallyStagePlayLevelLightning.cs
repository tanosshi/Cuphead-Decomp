using System.Collections;
using UnityEngine;

public class SallyStagePlayLevelLightning : AbstractProjectile
{
	[SerializeField]
	private Transform sprite;

	private LevelProperties.SallyStagePlay.Lightning properties;

	private Vector3 velocity;

	private Vector3 collisionPoint;

	private float speed;

	private float rotation;

	private bool lightningLast;

	public SallyStagePlayLevelLightning Create(Vector2 pos, float rotation, float speed, LevelProperties.SallyStagePlay.Lightning properties, bool lightningLast)
	{
		SallyStagePlayLevelLightning sallyStagePlayLevelLightning = base.Create(pos, rotation) as SallyStagePlayLevelLightning;
		sallyStagePlayLevelLightning.speed = speed;
		sallyStagePlayLevelLightning.rotation = rotation;
		sallyStagePlayLevelLightning.properties = properties;
		sallyStagePlayLevelLightning.lightningLast = lightningLast;
		return sallyStagePlayLevelLightning;
	}

	protected override void Start()
	{
		base.Start();
		sprite.SetEulerAngles(null, null, 0f);
		animator.Play(Random.Range(0, 3).ToString());
		StartCoroutine(move_cr());
		AudioManager.PlayLoop("sally_sally_lightning_move_loop");
		emitAudioFromObject.Add("sally_sally_lightning_move_loop");
		AudioManager.Play("sally_thunder");
		sprite.GetComponent<CollisionChild>().OnPlayerCollision += OnCollisionPlayer;
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}

	protected IEnumerator move_cr()
	{
		velocity = base.transform.right;
		while (true)
		{
			base.transform.position += velocity * speed * CupheadTime.FixedDelta;
			yield return new WaitForFixedUpdate();
		}
	}

	protected override void OnCollisionGround(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionGround(hit, phase);
		if (phase == CollisionPhase.Enter)
		{
			Vector3 position = base.transform.position;
			Vector3 vector = new Vector3(base.transform.position.x, Level.Current.Ceiling, 0f);
			collisionPoint = vector - position;
			StartCoroutine(change_direction_cr(collisionPoint));
		}
	}

	protected IEnumerator change_direction_cr(Vector3 collisionPoint)
	{
		base.transform.SetEulerAngles(null, null, 0f - rotation);
		sprite.SetEulerAngles(null, null, 0f);
		velocity = 1f * (-2f * Vector3.Dot(velocity, Vector3.Normalize(collisionPoint.normalized)) * Vector3.Normalize(collisionPoint.normalized) + velocity);
		yield return new WaitForEndOfFrame();
		AudioManager.Play("sally_thunder_impact");
		while (base.transform.position.y < (float)(Level.Current.Ceiling + 100))
		{
			yield return null;
		}
		if (lightningLast)
		{
			AudioManager.Stop("sally_sally_lightning_move_loop");
			AudioManager.Play("sally_thunder_end");
		}
		Die();
		yield return null;
	}

	protected override void Die()
	{
		StopAllCoroutines();
		base.Die();
	}
}
