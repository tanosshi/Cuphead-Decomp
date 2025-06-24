using UnityEngine;

public class MouseLevelCherryBombProjectile : AbstractProjectile
{
	public enum State
	{
		Init = 0,
		Moving = 1,
		Dead = 2
	}

	private const float ChildProjectileScale = 0.6f;

	[SerializeField]
	private Effect cloud;

	[SerializeField]
	private BasicProjectile childProjectile;

	private State state;

	private Vector2 velocity;

	private float gravity;

	private float childSpeed;

	public MouseLevelCherryBombProjectile Create(Vector2 pos, Vector2 velocity, float gravity, float childSpeed)
	{
		MouseLevelCherryBombProjectile mouseLevelCherryBombProjectile = InstantiatePrefab<MouseLevelCherryBombProjectile>();
		mouseLevelCherryBombProjectile.transform.position = pos;
		mouseLevelCherryBombProjectile.velocity = velocity;
		mouseLevelCherryBombProjectile.gravity = gravity;
		mouseLevelCherryBombProjectile.childSpeed = childSpeed;
		mouseLevelCherryBombProjectile.state = State.Moving;
		return mouseLevelCherryBombProjectile;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (state == State.Moving)
		{
			if (base.transform.position.y < (float)Level.Current.Ground + 60f)
			{
				state = State.Dead;
				animator.SetTrigger("OnExplode");
			}
			else
			{
				base.transform.AddPosition(velocity.x * CupheadTime.FixedDelta, velocity.y * CupheadTime.FixedDelta);
				velocity.y -= gravity * CupheadTime.FixedDelta;
			}
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (base.damageDealer != null)
		{
			base.damageDealer.DealDamage(hit);
		}
	}

	private void Explode()
	{
		Object.Destroy(base.gameObject);
	}

	private void SpawnChildren()
	{
		cloud.Create(new Vector3(base.transform.position.x + 20f, base.transform.position.y + 200f), new Vector3(0.52f, 0.52f, 0.52f));
		BasicProjectile basicProjectile = childProjectile.Create(base.transform.position - new Vector3(0f, 40f, 0f), 0f, new Vector2(0.6f, 0.6f), 0f - childSpeed);
		basicProjectile.GetComponent<Animator>().SetBool("isRight", false);
		BasicProjectile basicProjectile2 = childProjectile.Create(base.transform.position - new Vector3(0f, 40f, 0f), 0f, new Vector2(-0.6f, -0.6f), childSpeed);
		basicProjectile2.GetComponent<Animator>().SetBool("isRight", true);
	}

	protected override void Die()
	{
		Explode();
		base.Die();
	}

	private void SoundAnimCherryBomExp()
	{
		AudioManager.Play("level_mouse_cannon_bomb_explode");
		emitAudioFromObject.Add("level_mouse_cannon_bomb_explode");
	}
}
