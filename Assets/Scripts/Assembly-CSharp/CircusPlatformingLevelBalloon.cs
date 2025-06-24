using System.Collections;
using UnityEngine;

public class CircusPlatformingLevelBalloon : AbstractPlatformingLevelEnemy
{
	private const string Blue = "B";

	private const string Green = "G";

	private const string Pink = "P";

	private const string DeathParameterName = "Death";

	private const string BoyIdle = "BoyIdle";

	private const string GirlIdle = "GirlIdle";

	private const string PinkIdle = "PinkIdle";

	private const string BoyIdleSound = "circus_balloon_boy_idle";

	private const string GirlIdleSound = "circus_balloon_girl_idle";

	[SerializeField]
	private float bulletSpeed;

	[SerializeField]
	private BasicProjectile projectile;

	private float rotation;

	private float velocity;

	private float sinVelocity;

	private float sinSize;

	private string spreadCount;

	private string idleSoundSelected;

	protected override void OnStart()
	{
	}

	public void Init(Vector2 pos, float rotation, string spreadCount, string c)
	{
		base.transform.position = pos;
		this.rotation = rotation;
		this.spreadCount = spreadCount;
		velocity = base.Properties.flyingFishVelocity;
		sinVelocity = base.Properties.flyingFishSinVelocity;
		sinSize = base.Properties.flyingFishSinSize;
		SetColor(c);
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(movement_cr());
		StartCoroutine(idle_audio_delayer_cr(idleSoundSelected, 2f, 4f));
		emitAudioFromObject.Add(idleSoundSelected);
	}

	private void SetColor(string c)
	{
		switch (c)
		{
		case "B":
			base.animator.Play("BoyIdle");
			idleSoundSelected = "circus_balloon_boy_idle";
			break;
		case "G":
			base.animator.Play("GirlIdle");
			idleSoundSelected = "circus_balloon_girl_idle";
			break;
		case "P":
			base.animator.Play("PinkIdle");
			idleSoundSelected = "circus_balloon_girl_idle";
			_canParry = true;
			break;
		}
	}

	private IEnumerator movement_cr()
	{
		float angle = 0f;
		Vector3 xVelocity = Vector3.zero;
		while (true)
		{
			angle += base.Properties.flyingFishSinVelocity * (float)CupheadTime.Delta;
			xVelocity = ((rotation != 180f) ? base.transform.right : (-base.transform.right));
			Vector3 moveY = new Vector3(0f, Mathf.Sin(angle) * (float)CupheadTime.Delta * 60f * base.Properties.flyingFishSinSize);
			Vector3 moveX = xVelocity * base.Properties.flyingFishVelocity * CupheadTime.Delta;
			if ((float)CupheadTime.Delta != 0f)
			{
				base.transform.position += moveX + moveY;
			}
			if (!CupheadLevelCamera.Current.ContainsPoint(base.transform.position, AbstractPlatformingLevelEnemy.CAMERA_DEATH_PADDING))
			{
				Die();
			}
			yield return null;
		}
	}

	protected override void Die()
	{
		if (base.Health <= 0f)
		{
			AudioManager.Play("circus_balloon_hit");
			emitAudioFromObject.Add("circus_balloon_hit");
			base.animator.SetTrigger("Death");
		}
	}

	public new void Explode()
	{
		string[] array = spreadCount.Split(',');
		float result = 0f;
		for (int i = 0; i < array.Length; i++)
		{
			float.TryParse(array[i], out result);
			SpawnBullet(result);
		}
	}

	public void OnEndDeathAnim()
	{
		Object.Destroy(base.gameObject);
	}

	private void SpawnBullet(float angle)
	{
		projectile.Create(base.transform.position, angle, bulletSpeed);
	}

	private void SoundBalloonDeathAnim()
	{
		AudioManager.Play("circus_balloon_death");
		emitAudioFromObject.Add("circus_balloon_death");
	}
}
