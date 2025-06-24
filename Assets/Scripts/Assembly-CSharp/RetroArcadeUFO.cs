using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetroArcadeUFO : RetroArcadeEnemy
{
	private const float OFFSCREEN_Y = 500f;

	private const float ONSCREEN_Y = 300f;

	private const float MOVE_Y_SPEED = 500f;

	public const float WIDTH = 600f;

	public const float HEIGHT = 300f;

	public const float INNER_WIDTH = 500f;

	public const float INNER_HEIGHT = 150f;

	public const float INNER_TURNAROUND_X = 220f;

	private LevelProperties.RetroArcade properties;

	private LevelProperties.RetroArcade.UFO p;

	[SerializeField]
	private RetroArcadeUFOTurret turretPrefab;

	[SerializeField]
	private RetroArcadeUFOAlien alienPrefab;

	[SerializeField]
	private RetroArcadeUFOMole molePrefab;

	private RetroArcadeUFOAlien alien;

	private List<RetroArcadeUFOTurret> turrets;

	private RetroArcadeUFOMole mole;

	public void LevelInit(LevelProperties.RetroArcade properties)
	{
		this.properties = properties;
	}

	public void StartUFO()
	{
		base.gameObject.SetActive(true);
		p = properties.CurrentState.uFO;
		base.transform.SetPosition(0f, 500f);
		MoveY(-200f, 500f);
		alien = alienPrefab.Create(this, p);
		mole = molePrefab.Create(p);
		turrets = new List<RetroArcadeUFOTurret>();
		for (int i = 0; i < p.turretCount; i++)
		{
			RetroArcadeUFOTurret item = turretPrefab.Create(this, p, (float)i / (float)p.turretCount);
			turrets.Add(item);
		}
		StartCoroutine(shoot_cr());
	}

	private IEnumerator shoot_cr()
	{
		while (true)
		{
			float waitTime = p.shotRate.min * Mathf.Pow(p.shotRate.max / p.shotRate.min, 1f - alien.NormalizedHpRemaining);
			yield return CupheadTime.WaitForSeconds(this, waitTime);
			foreach (RetroArcadeUFOTurret turret in turrets)
			{
				turret.Shoot();
			}
		}
	}

	private IEnumerator moveOffscreen_cr()
	{
		MoveY(200f, 500f);
		while (movingY)
		{
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}

	public void OnAlienDie()
	{
		StopAllCoroutines();
		StartCoroutine(moveOffscreen_cr());
		properties.DealDamageToNextNamedState();
		mole.OnWaveEnd();
	}
}
