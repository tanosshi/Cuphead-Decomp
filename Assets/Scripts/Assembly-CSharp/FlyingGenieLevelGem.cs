using System.Collections;
using UnityEngine;

public class FlyingGenieLevelGem : AbstractProjectile
{
	private const int BigVariations = 3;

	private const int VariationsTotal = 10;

	private const string VariationParameterName = "Variation";

	private const string ProjectilesLayer = "Projectiles";

	[SerializeField]
	private SpriteRenderer gemRenderer;

	[SerializeField]
	private float outOfChestY;

	[SerializeField]
	private float outOfChestSpeed;

	private AbstractPlayerController player;

	private float offsetY;

	private bool isBig;

	private float angle;

	private float firingAngle = 50f;

	private float velocityX;

	private float speed;

	protected override bool DestroyedAfterLeavingScreen
	{
		get
		{
			return true;
		}
	}

	public FlyingGenieLevelGem Create(Vector2 pos, AbstractPlayerController player, float offsetY, float speed, bool parryable, bool isBig)
	{
		FlyingGenieLevelGem flyingGenieLevelGem = base.Create() as FlyingGenieLevelGem;
		flyingGenieLevelGem.transform.position = pos;
		flyingGenieLevelGem.player = player;
		flyingGenieLevelGem.offsetY = offsetY;
		flyingGenieLevelGem.speed = speed;
		flyingGenieLevelGem.SetParryable(parryable);
		flyingGenieLevelGem.isBig = isBig;
		return flyingGenieLevelGem;
	}

	protected override void Start()
	{
		base.Start();
		int num = ((!isBig) ? Random.Range(2, 9) : Random.Range(0, 3));
		if (base.CanParry)
		{
			num = 9;
		}
		animator.SetFloat("Variation", (float)num / 9f);
		angle = MathUtils.DirectionToAngle(player.transform.position - base.transform.position);
		Vector3 vector = new Vector3(0f, offsetY, 0f);
		float value = MathUtils.DirectionToAngle(player.transform.position - (base.transform.position + vector));
		base.transform.SetEulerAngles(null, null, value);
		StartCoroutine(check_gem_cr());
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	private IEnumerator check_gem_cr()
	{
		float startPos = (base.transform.position + Vector3.up * outOfChestY).y;
		while (base.transform.position.y < startPos)
		{
			base.transform.AddPosition(0f, outOfChestY * outOfChestSpeed * (float)CupheadTime.Delta);
			yield return null;
		}
		gemRenderer.sortingLayerName = "Projectiles";
		gemRenderer.sortingOrder = 2;
		while (base.transform.position.x > -640f && base.transform.position.y < 360f && base.transform.position.y > -360f)
		{
			base.transform.position += base.transform.right * speed * CupheadTime.Delta;
			yield return null;
		}
		Die();
		yield return null;
	}

	protected override void Die()
	{
		GetComponent<SpriteRenderer>().enabled = false;
		base.Die();
	}
}
