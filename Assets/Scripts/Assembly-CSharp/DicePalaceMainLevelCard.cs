using UnityEngine;

public class DicePalaceMainLevelCard : AbstractProjectile
{
	private LevelProperties.DicePalaceMain.Cards properties;

	private bool onLeft;

	private Vector3 direction;

	private float scale;

	private int suitIndex;

	public override float ParryMeterMultiplier
	{
		get
		{
			return 0.25f;
		}
	}

	public DicePalaceMainLevelCard Create(Vector3 pos, LevelProperties.DicePalaceMain.Cards properties, bool onLeft, float scale, int suit)
	{
		DicePalaceMainLevelCard dicePalaceMainLevelCard = base.Create() as DicePalaceMainLevelCard;
		dicePalaceMainLevelCard.properties = properties;
		dicePalaceMainLevelCard.transform.position = pos;
		dicePalaceMainLevelCard.onLeft = onLeft;
		dicePalaceMainLevelCard.scale = scale;
		dicePalaceMainLevelCard.suitIndex = suit;
		return dicePalaceMainLevelCard;
	}

	protected override void Start()
	{
		base.Start();
		direction = ((!onLeft) ? (-base.transform.right) : base.transform.right);
		animator.SetInteger("PickSuit", suitIndex);
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		base.transform.position += direction * properties.cardSpeed * CupheadTime.FixedDelta;
	}

	public override void OnParry(AbstractPlayerController player)
	{
	}
}
