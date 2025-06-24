using System.Collections;
using UnityEngine;

public class FlyingMermaidLevelMerdusaBodyPart : LevelProperties.FlyingMermaid.Entity
{
	[SerializeField]
	private float waitTime;

	[SerializeField]
	private Vector2 velocity;

	[SerializeField]
	private float moveTime;

	[SerializeField]
	private bool stopBobbingAfterWait;

	[SerializeField]
	private float rotationSpeed;

	[SerializeField]
	private bool damagePlayer;

	private DamageDealer damageDealer;

	public bool IsSinking { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		if (damagePlayer)
		{
			damageDealer = DamageDealer.NewEnemy();
		}
		StartCoroutine(main_cr());
		StartCoroutine(check_to_delete_cr());
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (damageDealer != null)
		{
			damageDealer.DealDamage(hit);
		}
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	private IEnumerator main_cr()
	{
		AudioManager.Play("level_mermaid_merdusa_fallapart_break");
		yield return CupheadTime.WaitForSeconds(this, waitTime);
		if (stopBobbingAfterWait)
		{
			GetComponent<FlyingMermaidLevelFloater>().enabled = false;
		}
		IsSinking = true;
		float t = 0f;
		while (t < moveTime)
		{
			base.transform.AddPosition(velocity.x * (float)CupheadTime.Delta, velocity.y * (float)CupheadTime.Delta);
			base.transform.Rotate(0f, 0f, rotationSpeed * (float)CupheadTime.Delta);
			yield return null;
		}
	}

	public FlyingMermaidLevelMerdusaBodyPart Create(Vector2 pos)
	{
		FlyingMermaidLevelMerdusaBodyPart flyingMermaidLevelMerdusaBodyPart = Object.Instantiate(this);
		flyingMermaidLevelMerdusaBodyPart.transform.SetPosition(pos.x, pos.y);
		return flyingMermaidLevelMerdusaBodyPart;
	}

	private IEnumerator check_to_delete_cr()
	{
		while (!(base.transform.position.x < -1140f) && !(base.transform.position.x > 1140f) && !(base.transform.position.y < -860f) && !(base.transform.position.y > 1220f))
		{
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}
}
