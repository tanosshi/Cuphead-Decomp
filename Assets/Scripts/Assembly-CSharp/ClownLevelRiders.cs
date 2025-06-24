using UnityEngine;

public class ClownLevelRiders : AbstractCollidableObject
{
	[SerializeField]
	private SpriteRenderer backSeat;

	[SerializeField]
	private SpriteRenderer backRider;

	public bool inFront;

	private DamageDealer damageDealer;

	private SpriteRenderer sprite;

	private SpriteRenderer[] sprites;

	protected override void Start()
	{
		base.Start();
		damageDealer = DamageDealer.NewEnemy();
		sprite = GetComponent<SpriteRenderer>();
		if (inFront)
		{
			base.animator.SetBool("InFront", true);
		}
		else
		{
			base.animator.SetBool("InFront", false);
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

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	public void BackLayers(int cartLayer)
	{
		sprites = GetComponentsInChildren<SpriteRenderer>();
		for (int i = 0; i < sprites.Length; i++)
		{
			sprites[i].sortingLayerName = "Background";
			sprites[i].sortingOrder = cartLayer;
		}
	}

	public void FrontLayers(int cartLayer)
	{
		sprites = GetComponentsInChildren<SpriteRenderer>();
		for (int i = 0; i < sprites.Length; i++)
		{
			if (sprites[i] == backRider || sprites[i] == backSeat)
			{
				sprites[i].sortingLayerName = "Default";
				sprites[i].sortingOrder = 10 - i;
			}
			else
			{
				sprites[i].sortingLayerName = "Player";
				sprites[i].sortingOrder = cartLayer;
			}
		}
	}
}
