using UnityEngine;

public class LevelPlayerShadow : AbstractLevelPlayerComponent
{
	[Range(1f, 1000f)]
	[SerializeField]
	private int maxDistance = 250;

	[SerializeField]
	private Sprite[] shadowSprites;

	private Transform shadow;

	private SpriteRenderer renderer;

	private readonly int groundMask = 1048576;

	private readonly int ceilingMask = 524288;

	protected override void Start()
	{
		base.Start();
		shadow = new GameObject(base.gameObject.name + "_Shadow").transform;
		renderer = shadow.gameObject.AddComponent<SpriteRenderer>();
		shadow.position = new Vector3(base.transform.position.x, Level.Current.Ground, 0f);
		renderer.sprite = shadowSprites[0];
	}

	protected override void Update()
	{
		base.Update();
		if ((base.player.motor.Grounded && !base.player.motor.Dashing) || base.player.IsDead || (base.player.stats.Loadout.charm == Charm.charm_smoke_dash && base.player.motor.Dashing))
		{
			renderer.enabled = false;
			return;
		}
		renderer.enabled = true;
		Vector3 position = shadow.position;
		position.x = base.transform.position.x;
		BoxCollider2D collider = base.player.collider;
		RaycastHit2D raycastHit2D = Physics2D.BoxCast(base.player.transform.position, new Vector2(base.player.collider.size.x, 1f), 0f, (!base.player.motor.GravityReversed) ? Vector2.down : Vector2.up, maxDistance, (!base.player.motor.GravityReversed) ? groundMask : ceilingMask);
		if (raycastHit2D.collider == null)
		{
			renderer.enabled = false;
			return;
		}
		LevelPlatform component = raycastHit2D.collider.gameObject.GetComponent<LevelPlatform>();
		if (component != null && !component.AllowShadows)
		{
			renderer.enabled = false;
			return;
		}
		position.y = raycastHit2D.point.y;
		shadow.position = position;
		SetSprite();
	}

	private void SetSprite()
	{
		int num = (int)(Mathf.Abs(base.transform.position.y - shadow.position.y) / (float)maxDistance * (float)shadowSprites.Length);
		if (num < 0 || num >= shadowSprites.Length)
		{
			renderer.enabled = false;
			return;
		}
		renderer.enabled = true;
		renderer.sprite = shadowSprites[num];
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (shadow != null)
		{
			Object.Destroy(shadow.gameObject);
		}
	}

	public Vector3 ShadowPosition()
	{
		return shadow.position;
	}
}
