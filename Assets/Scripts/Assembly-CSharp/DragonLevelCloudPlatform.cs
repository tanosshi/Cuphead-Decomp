using System.Collections;
using UnityEngine;

public class DragonLevelCloudPlatform : LevelPlatform
{
	[SerializeField]
	public SpriteRenderer top;

	private float minX;

	private float maxX;

	private new Animator animator;

	private LevelProperties.Dragon.Clouds properties;

	private DragonLevelPlatformManager manager;

	private float speed;

	protected override void Awake()
	{
		base.Awake();
		animator = GetComponent<Animator>();
		animator.SetInteger("Cloud", Random.Range(0, 3));
		minX = -640f - GetComponent<SpriteRenderer>().bounds.size.x / 2f;
		maxX = 640f + GetComponent<SpriteRenderer>().bounds.size.x / 2f;
	}

	public override void AddChild(Transform player)
	{
		base.AddChild(player);
		animator.SetBool("HasPlayer", true);
	}

	public override void OnPlayerExit(Transform player)
	{
		base.OnPlayerExit(player);
		if (base.players.Count <= 0)
		{
			animator.SetBool("HasPlayer", false);
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		top.sprite = null;
	}

	public void GetProperties(DragonLevelPlatformManager manager, LevelProperties.Dragon.Clouds properties)
	{
		this.properties = properties;
		this.manager = manager;
		speed = ((!properties.movingRight) ? properties.cloudSpeed : (0f - properties.cloudSpeed));
		StartCoroutine(move_cr());
	}

	public void GetProperties(LevelProperties.Dragon.Clouds properties, bool firstTime)
	{
		this.properties = properties;
		speed = ((!properties.movingRight) ? properties.cloudSpeed : (0f - properties.cloudSpeed));
		if (firstTime)
		{
			StartCoroutine(move_cr());
		}
	}

	private IEnumerator move_cr()
	{
		while (true)
		{
			base.transform.AddPosition((0f - DragonLevel.SPEED) * speed * (float)CupheadTime.Delta);
			yield return null;
			if (properties.movingRight)
			{
				if (base.transform.position.x >= maxX)
				{
					if (manager != null)
					{
						manager.DestroyObjectPool(this);
					}
					else
					{
						Object.Destroy(base.gameObject);
					}
				}
			}
			else if (base.transform.position.x <= minX)
			{
				if (manager != null)
				{
					manager.DestroyObjectPool(this);
				}
				else
				{
					Object.Destroy(base.gameObject);
				}
			}
		}
	}
}
