using System;
using UnityEngine;

public class CircusPlatformingLevelMagicianBullet : BasicProjectile
{
	[SerializeField]
	private SpriteRenderer puffs;

	protected override float DestroyLifetime
	{
		get
		{
			return 0f;
		}
	}

	public event Action OnProjectileDeath;

	protected override void Start()
	{
		base.Start();
		AudioManager.PlayLoop("circus_magician_magic_loop");
		emitAudioFromObject.Add("circus_magician_magic_loop");
		puffs.flipX = Rand.Bool();
		puffs.flipY = Rand.Bool();
		DestroyDistance = 0f;
	}

	protected override void OnDestroy()
	{
		AudioManager.Stop("circus_magician_magic_loop");
		if (this.OnProjectileDeath != null)
		{
			this.OnProjectileDeath();
		}
		base.OnDestroy();
	}
}
