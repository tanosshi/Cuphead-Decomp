using UnityEngine;

public class FlyingMermaidLevelSplashManager : AbstractPausableComponent
{
	public static FlyingMermaidLevelSplashManager splashManager;

	public Transform spawnRootFront;

	public Transform spawnRootBack;

	[SerializeField]
	private Effect MegasplashLarge;

	[SerializeField]
	private Effect MegasplashMedium;

	[SerializeField]
	private Effect SplashMedium;

	[SerializeField]
	private Effect SplashSmall;

	public static FlyingMermaidLevelSplashManager Instance
	{
		get
		{
			if (splashManager == null)
			{
				GameObject gameObject = new GameObject();
				gameObject.name = "SplashManager";
				splashManager = gameObject.AddComponent<FlyingMermaidLevelSplashManager>();
			}
			return splashManager;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		splashManager = this;
	}

	protected override void OnTriggerEnter2D(Collider2D collider)
	{
		base.OnTriggerEnter2D(collider);
		if (collider.gameObject.tag == Tags.EnemyProjectile.ToString() && collider.gameObject.GetComponent<FlyingMermaidLevelNoSplashMarker>() == null)
		{
			if (collider.GetComponent<Collider2D>().bounds.size.x > 200f)
			{
				SpawnMegaSplashMedium(collider.gameObject);
			}
			else if (collider.GetComponent<Collider2D>().bounds.size.x > 50f)
			{
				SpawnSplashMedium(collider.gameObject);
			}
			else
			{
				SpawnSplashSmall(collider.gameObject);
			}
		}
	}

	public void SpawnMegaSplashLarge(GameObject gameObject, float extraX = 0f, bool overrideY = false, float y = 0f)
	{
		CreateSplash(MegasplashLarge, gameObject, extraX, overrideY, y);
	}

	public void SpawnMegaSplashMedium(GameObject gameObject, float extraX = 0f, bool overrideY = false, float y = 0f)
	{
		CreateSplash(MegasplashMedium, gameObject, extraX, overrideY, y);
	}

	public void SpawnSplashMedium(GameObject gameObject, float extraX = 0f, bool overrideY = false, float y = 0f)
	{
		CreateSplash(SplashMedium, gameObject, extraX, overrideY, y);
	}

	public void SpawnSplashSmall(GameObject gameObject)
	{
		CreateSplash(SplashSmall, gameObject);
	}

	private void CreateSplash(Effect effect, GameObject gameObject, float extraX = 0f, bool overrideY = false, float y = 0f)
	{
		float num = 0f;
		if (gameObject.GetComponent<Renderer>() != null)
		{
			num = gameObject.GetComponent<Renderer>().bounds.size.y / 4f;
		}
		Vector3 position = new Vector3(gameObject.transform.position.x + extraX, gameObject.transform.position.y - num);
		if (overrideY)
		{
			position.y = y;
		}
		Effect effect2 = Object.Instantiate(effect);
		effect2.transform.position = position;
		if (gameObject.GetComponent<SpriteRenderer>() != null)
		{
			effect2.GetComponent<SpriteRenderer>().sortingLayerName = gameObject.GetComponent<SpriteRenderer>().sortingLayerName;
			effect2.GetComponent<SpriteRenderer>().sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
		}
	}
}
