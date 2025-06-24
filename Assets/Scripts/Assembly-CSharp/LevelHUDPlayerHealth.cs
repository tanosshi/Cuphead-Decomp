using UnityEngine;
using UnityEngine.UI;

public class LevelHUDPlayerHealth : AbstractLevelHUDComponent
{
	private const string HealthParameter = "Health";

	private Image image;

	private int lastHealth;

	protected override void Awake()
	{
		base.Awake();
		image = GetComponent<Image>();
	}

	public override void Init(LevelHUDPlayer hud)
	{
		base.Init(hud);
		lastHealth = base._player.stats.Health;
		OnHealthChanged(base._player.stats.Health);
	}

	public void OnHealthChanged(int health)
	{
		base.animator.SetInteger("Health", Mathf.Clamp(health, 0, base._player.stats.HealthMax));
		base.animator.Play("Entry");
		if (lastHealth != health)
		{
			OnChangedHealth();
		}
		lastHealth = health;
	}

	private void OnChangedHealth()
	{
		TweenValue(0f, 1f, 0.3f, EaseUtils.EaseType.easeOutSine, ChangedHealthTween);
	}

	private void ChangedHealthTween(float value)
	{
		Color white = Color.white;
		Color gray = Color.gray;
		base.transform.localScale = Vector3.one * Mathf.Lerp(2f, 1f, value);
		image.color = Color.Lerp(white, gray, value);
	}
}
