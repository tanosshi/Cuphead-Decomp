using System;
using System.Collections;
using UnityEngine;

public class RobotLevelGem : AbstractCollidableObject
{
	[SerializeField]
	private RobotLevelGemProjectile bulletPrefab;

	private RobotLevelHelihead parent;

	private LevelProperties.Robot properties;

	private Transform[] spawnPoints;

	private bool isFirstWave = true;

	private bool waveRotation;

	private int nextBulletIndex;

	private float rotation;

	private bool isBlueGem;

	public void InitFinalStage(RobotLevelHelihead parent, LevelProperties.Robot properties, bool isBlueGem)
	{
		this.parent = parent;
		this.properties = properties;
		RobotLevelHelihead robotLevelHelihead = this.parent;
		robotLevelHelihead.OnDeath = (Action)Delegate.Combine(robotLevelHelihead.OnDeath, new Action(OnDeath));
		spawnPoints = base.transform.GetChildTransforms();
		rotation = 0f;
		if (isFirstWave)
		{
			bulletPrefab.CreatePool(200);
		}
		this.isBlueGem = isBlueGem;
		if (isBlueGem)
		{
			waveRotation = properties.CurrentState.blueGem.gemWaveRotation;
		}
		else
		{
			waveRotation = properties.CurrentState.redGem.gemWaveRotation;
		}
		StartCoroutine(rotate_cr());
		StartCoroutine(attack_cr());
	}

	private IEnumerator attack_cr()
	{
		while (true)
		{
			if (isBlueGem)
			{
				FireBullets(properties.CurrentState.blueGem.numberOfSpawnPoints);
				yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.blueGem.bulletSpawnDelay);
			}
			else
			{
				FireBullets(properties.CurrentState.redGem.numberOfSpawnPoints);
				yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.redGem.bulletSpawnDelay);
			}
		}
	}

	private IEnumerator rotate_cr()
	{
		float rotationSpeed;
		MinMax rotationRange;
		if (isBlueGem)
		{
			rotationSpeed = properties.CurrentState.blueGem.robotRotationSpeed;
			rotationRange = properties.CurrentState.blueGem.gemRotationRange;
		}
		else
		{
			rotationSpeed = properties.CurrentState.redGem.robotRotationSpeed;
			rotationRange = properties.CurrentState.redGem.gemRotationRange;
		}
		while (true)
		{
			if (waveRotation && (Vector3.Angle(Vector3.up, base.transform.right) > rotationRange.max || Vector3.Angle(Vector3.up, base.transform.right) < rotationRange.min))
			{
				rotationSpeed *= -1f;
			}
			rotation += rotationSpeed;
			base.transform.eulerAngles = Vector3.forward * rotation;
			yield return null;
		}
	}

	public void OnAttackEnd()
	{
		StopAllCoroutines();
	}

	protected override void OnDestroy()
	{
		StopAllCoroutines();
		base.OnDestroy();
	}

	private void OnDeath()
	{
		RobotLevelHelihead robotLevelHelihead = parent;
		robotLevelHelihead.OnDeath = (Action)Delegate.Remove(robotLevelHelihead.OnDeath, new Action(OnDeath));
		OnDestroy();
	}

	private void FireBullets(int count, float offset = 0f, bool parryable = false)
	{
		offset = rotation;
		for (int i = 0; i < count; i++)
		{
			float num = 360f * ((float)i / (float)count);
			if (isBlueGem)
			{
				bulletPrefab.Spawn(base.transform.position, Quaternion.Euler(new Vector3(0f, 0f, offset + num - 180f))).Init(properties.CurrentState.blueGem.bulletSpeed, properties.CurrentState.blueGem.bulletSpeedAcceleration, properties.CurrentState.blueGem.bulletSineWaveStrength, properties.CurrentState.blueGem.bulletWaveSpeedMultiplier, properties.CurrentState.blueGem.bulletLifeTime, isBlueGem).SetParryable(parryable);
			}
			else
			{
				bulletPrefab.Spawn(base.transform.position, Quaternion.Euler(new Vector3(0f, 0f, offset + num - 180f))).Init(properties.CurrentState.redGem.bulletSpeed, properties.CurrentState.redGem.bulletSpeedAcceleration, properties.CurrentState.redGem.bulletSineWaveStrength, properties.CurrentState.redGem.bulletWaveSpeedMultiplier, properties.CurrentState.redGem.bulletLifeTime, isBlueGem).SetParryable(parryable);
			}
		}
	}
}
