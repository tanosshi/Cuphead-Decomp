using System.Collections;
using UnityEngine;

public class FlyingGenieLevelSword : AbstractProjectile
{
	private const string PinkParameterName = "Pink";

	private const string SpinParameterName = "Spin";

	private const string AttackParameterName = "Attack";

	private const string ProjectilesLayer = "Projectiles";

	private const float spinRotationOffset = 35f;

	[SerializeField]
	private float outOfChestY;

	[SerializeField]
	private float outOfChestSpeed;

	[SerializeField]
	private float swordRotationSpeed;

	[SerializeField]
	private float fastSpinTime;

	[SerializeField]
	private SpriteRenderer swordRenderer;

	private LevelProperties.FlyingGenie.Swords properties;

	private AbstractPlayerController player;

	private Vector3 endPos;

	private Vector3 startPos;

	public void Init(Vector3 startPos, Vector3 endPos, LevelProperties.FlyingGenie.Swords properties, AbstractPlayerController player)
	{
		this.startPos = startPos;
		base.transform.position = startPos;
		this.properties = properties;
		this.endPos = endPos;
		this.player = player;
		StartCoroutine(move_to_pos_cr());
		AudioManager.Play("genie_chest_sword_spawn");
		emitAudioFromObject.Add("genie_chest_sword_spawn");
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		if (base.damageDealer != null)
		{
			base.damageDealer.DealDamage(hit);
		}
		base.OnCollisionPlayer(hit, phase);
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	private IEnumerator move_to_pos_cr()
	{
		base.transform.eulerAngles = new Vector3(0f, 0f, 90f);
		while (base.transform.position.y < (startPos + Vector3.up * outOfChestY).y)
		{
			base.transform.AddPosition(0f, outOfChestY * outOfChestSpeed * (float)CupheadTime.Delta);
			yield return null;
		}
		swordRenderer.sortingLayerName = "Projectiles";
		swordRenderer.sortingOrder = 2;
		base.transform.eulerAngles = new Vector3(0f, 0f, MathUtils.DirectionToAngle(endPos - base.transform.position));
		while (base.transform.position != endPos)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, endPos, properties.swordSpeed * (float)CupheadTime.Delta);
			yield return null;
		}
		float t = 0f;
		while (t < properties.attackDelay)
		{
			base.transform.Rotate(Vector3.forward, swordRotationSpeed * (float)CupheadTime.Delta);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		StartCoroutine(move_continue_cr());
		AudioManager.Play("genie_chest_sword_attack");
		emitAudioFromObject.Add("genie_chest_sword_attack");
		yield return null;
	}

	private IEnumerator move_continue_cr()
	{
		base.transform.eulerAngles = new Vector3(0f, 0f, 35f);
		animator.SetTrigger("Spin");
		yield return CupheadTime.WaitForSeconds(this, fastSpinTime);
		animator.SetTrigger("Attack");
		if (player == null || player.IsDead)
		{
			player = PlayerManager.GetNext();
		}
		TransformExtensions.SetEulerAngles(z: MathUtils.DirectionToAngle(player.transform.position - base.transform.position), transform: base.transform);
		while (true)
		{
			base.transform.position += base.transform.right * properties.swordSpeed * CupheadTime.Delta;
			yield return null;
		}
	}

	public override void SetParryable(bool parryable)
	{
		base.SetParryable(parryable);
		animator.SetFloat("Pink", (!parryable) ? 0f : 1f);
	}
}
