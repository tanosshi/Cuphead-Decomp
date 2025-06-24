using System.Collections;
using UnityEngine;

public class TrainLevelEngineBossDropperProjectile : AbstractProjectile
{
	private enum State
	{
		Y = 0,
		X = 1
	}

	private const string HorizontalParameterName = "Horizontal";

	private const float ScaleTime = 0.4f;

	private const float StartScale = 0.5f;

	[SerializeField]
	private Effect dustFX;

	[SerializeField]
	private CircleCollider2D verticalCollider;

	[SerializeField]
	private BoxCollider2D horizontalCollider;

	private State state;

	private Vector2 velocity;

	private float gravity;

	public TrainLevelEngineBossDropperProjectile Create(Vector2 pos, float upSpeed, float xSpeed, float gravity)
	{
		TrainLevelEngineBossDropperProjectile trainLevelEngineBossDropperProjectile = InstantiatePrefab<TrainLevelEngineBossDropperProjectile>();
		trainLevelEngineBossDropperProjectile.Init(pos, upSpeed, xSpeed, gravity);
		return trainLevelEngineBossDropperProjectile;
	}

	private void Init(Vector2 pos, float upSpeed, float xSpeed, float gravity)
	{
		base.transform.position = pos;
		velocity.y = upSpeed;
		velocity.x = xSpeed;
		this.gravity = gravity;
		base.transform.localScale = Vector3.one * 0.5f;
		StartCoroutine(go_cr());
		StartCoroutine(scale_cr());
	}

	private IEnumerator scale_cr()
	{
		float t = 0f;
		while (t < 0.4f)
		{
			base.transform.localScale = Vector3.Lerp(Vector3.one * 0.5f, Vector3.one, t / 0.4f);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
	}

	private IEnumerator go_cr()
	{
		AbstractPlayerController target = PlayerManager.GetNext();
		while (base.transform.position.y > target.center.y)
		{
			Vector3 vel = Vector3.zero;
			base.transform.AddPosition(0f, velocity.y * (float)CupheadTime.Delta);
			velocity.y -= gravity * (float)CupheadTime.Delta;
			yield return null;
			if (target == null || target.IsDead)
			{
				target = PlayerManager.GetNext();
			}
		}
		int direction = ((target.center.x > base.transform.position.x) ? 1 : (-1));
		base.transform.localScale = new Vector3(-direction, 1f, 1f);
		animator.SetTrigger("Horizontal");
		dustFX.Create(base.transform.position, new Vector3(direction, 1f, 1f)).Play();
		verticalCollider.enabled = false;
		horizontalCollider.enabled = true;
		while (true)
		{
			base.transform.AddPosition((float)direction * velocity.x * (float)CupheadTime.Delta);
			yield return null;
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
		Die();
	}
}
