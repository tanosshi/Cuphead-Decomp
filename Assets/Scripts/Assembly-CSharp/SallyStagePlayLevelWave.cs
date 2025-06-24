using System.Collections;
using UnityEngine;

public class SallyStagePlayLevelWave : AbstractCollidableObject
{
	private DamageDealer damageDealer;

	private LevelProperties.SallyStagePlay.Tidal properties;

	private Vector3 startPos;

	public bool isMoving { get; private set; }

	protected override void Start()
	{
		base.Start();
		damageDealer = DamageDealer.NewEnemy();
		startPos = base.transform.position;
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

	public void StartWave(LevelProperties.SallyStagePlay.Tidal properties)
	{
		this.properties = properties;
		base.transform.position = startPos;
		StartCoroutine(move_cr());
	}

	private IEnumerator move_cr()
	{
		float sizeX = GetComponent<Renderer>().bounds.size.x;
		isMoving = true;
		while (base.transform.position.x < 640f + sizeX)
		{
			base.transform.position += base.transform.right * properties.tidalSpeed * CupheadTime.Delta;
			yield return null;
		}
		isMoving = false;
		yield return null;
	}

	private void SoundBigWaveFeet()
	{
		if (isMoving)
		{
			AudioManager.Play("sally_wave");
			emitAudioFromObject.Add("sally_wave");
		}
	}

	private void SoundBigWaveVoice()
	{
		if (isMoving)
		{
			AudioManager.Play("sally_wave_sweet");
			emitAudioFromObject.Add("sally_wave_sweet");
		}
	}
}
