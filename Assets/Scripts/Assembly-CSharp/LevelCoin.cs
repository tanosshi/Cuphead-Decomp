using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(100)]
[RequireComponent(typeof(Animator))]
public class LevelCoin : AbstractCollidableObject
{
	private const float COLLECT_RANGE = 100f;

	public const int NUM_COINS = 40;

	private SpriteRenderer _spriteRenderer;

	private bool _collected;

	public string GlobalID
	{
		get
		{
			return SceneManager.GetActiveScene().name + "::" + base.gameObject.name;
		}
	}

	public static void OnLevelStart()
	{
		PlayerData.Data.ResetLevelCoinManager();
	}

	public static void OnLevelComplete()
	{
		PlayerData.Data.ApplyLevelCoins();
	}

	protected override void Awake()
	{
		PlatformingLevel platformingLevel = Level.Current as PlatformingLevel;
		if ((bool)platformingLevel)
		{
			platformingLevel.LevelCoinsIDs.Add(new CoinPositionAndID(GlobalID, base.transform.position.x));
		}
		base.Awake();
		if (PlayerData.Data.GetCoinCollected(this))
		{
			_collected = true;
			Debug.LogWarning("[LevelCoin] Coin has already been collected. Destroying!");
			Object.Destroy(base.gameObject);
		}
		else
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
		}
	}

	protected override void Update()
	{
		base.Update();
		if (!_collected)
		{
			AbstractPlayerController player = PlayerManager.GetPlayer(PlayerId.PlayerOne);
			AbstractPlayerController player2 = PlayerManager.GetPlayer(PlayerId.PlayerTwo);
			if (player != null && Vector2.Distance(base.transform.position, player.center) < 100f)
			{
				Collect(player.id);
			}
			else if (player2 != null && Vector2.Distance(base.transform.position, player2.center) < 100f)
			{
				Collect(player2.id);
			}
		}
	}

	private void Collect(PlayerId player)
	{
		if (!_collected)
		{
			PlayerData.Data.SetLevelCoinCollected(this, true, player);
			_collected = true;
			AudioManager.Play("level_coin_pickup");
			base.animator.SetTrigger("OnDeath");
			base.transform.localScale *= 1.2f;
			_spriteRenderer.flipX = MathUtils.RandomBool();
			_spriteRenderer.flipY = MathUtils.RandomBool();
		}
	}

	private void OnDeathAnimComplete()
	{
		Object.Destroy(base.gameObject);
	}
}
