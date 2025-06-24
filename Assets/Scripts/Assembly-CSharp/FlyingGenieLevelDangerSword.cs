using UnityEngine;

public class FlyingGenieLevelDangerSword : AbstractProjectile
{
	private const string AttackState = "Attack";

	private Vector3 startPos;

	[SerializeField]
	private float speed;

	protected override float DestroyLifetime
	{
		get
		{
			return 0f;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		startPos = base.transform.position;
		base.gameObject.SetActive(false);
		DestroyDistance = 0f;
	}

	protected override void OnEnable()
	{
		base.transform.position = startPos;
		animator.Play("Attack");
		AudioManager.Play("genie_sword_attack");
		emitAudioFromObject.Add("genie_sword_attack");
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		if (base.damageDealer != null)
		{
			base.damageDealer.DealDamage(hit);
		}
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
		base.transform.AddPosition(0f, (0f - speed) * (float)CupheadTime.Delta);
		if (base.transform.position.y < -460f)
		{
			base.gameObject.SetActive(false);
		}
	}
}
