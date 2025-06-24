using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetroArcadeBigRobot : RetroArcadeEnemy
{
	[SerializeField]
	private RetroArcadeOrbiterRobot[] orbiterPrefabs;

	[SerializeField]
	private RetroArcadeRobotBouncingProjectile projectilePrefab;

	[SerializeField]
	private Transform projectileRoot;

	private float OFFSCREEN_Y = 300f;

	private const float MOVE_OFFSCREEN_SPEED = 500f;

	private LevelProperties.RetroArcade.Robots properties;

	private float t;

	private RetroArcadeOrbiterRobot[] orbiters;

	private RetroArcadeRobotManager manager;

	private bool groupDead;

	public RetroArcadeBigRobot Create(float xPos, LevelProperties.RetroArcade.Robots properties, float sinOffset, RetroArcadeRobotManager manager, string[] orbiterPattern)
	{
		RetroArcadeBigRobot retroArcadeBigRobot = InstantiatePrefab<RetroArcadeBigRobot>();
		retroArcadeBigRobot.t = sinOffset * (float)Math.PI * 2f;
		float num = OFFSCREEN_Y + properties.smallRobotRotationDistance - properties.mainRobotY.min;
		retroArcadeBigRobot.properties = properties;
		retroArcadeBigRobot.transform.position = new Vector2(xPos, retroArcadeBigRobot.getYPos(retroArcadeBigRobot.t) + num);
		retroArcadeBigRobot.hp = properties.mainRobotHp;
		retroArcadeBigRobot.manager = manager;
		float num2 = sinOffset * 360f;
		retroArcadeBigRobot.orbiters = new RetroArcadeOrbiterRobot[3];
		for (int i = 0; i < 3; i++)
		{
			int result;
			if (int.TryParse(orbiterPattern[i], out result) && result > 0 && result <= orbiterPrefabs.Length)
			{
				retroArcadeBigRobot.orbiters[i] = retroArcadeBigRobot.orbiterPrefabs[result - 1].Create(retroArcadeBigRobot, properties, num2);
				num2 += 120f;
			}
		}
		retroArcadeBigRobot.MoveY(0f - num, properties.mainRobotMoveSpeed);
		retroArcadeBigRobot.StartCoroutine(retroArcadeBigRobot.shoot_cr());
		retroArcadeBigRobot.StartCoroutine(retroArcadeBigRobot.orbiterShoot_cr());
		return retroArcadeBigRobot;
	}

	protected override void Start()
	{
		base.Start();
		base.PointsWorth = properties.pointsGained;
		base.PointsBonus = properties.pointsBonus;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (movingY || groupDead)
		{
			return;
		}
		t += CupheadTime.FixedDelta * (properties.mainRobotMoveSpeed / (properties.mainRobotY.max - properties.mainRobotY.min)) * (float)Math.PI;
		base.transform.SetPosition(null, getYPos(t));
		bool flag = true;
		RetroArcadeOrbiterRobot[] array = orbiters;
		foreach (RetroArcadeOrbiterRobot retroArcadeOrbiterRobot in array)
		{
			if (!retroArcadeOrbiterRobot.IsDead)
			{
				flag = false;
			}
		}
		if (flag)
		{
			StartCoroutine(moveOffscreen_cr());
			groupDead = true;
			manager.OnRobotGroupDie();
		}
	}

	private float getYPos(float t)
	{
		return properties.mainRobotY.GetFloatAt(Mathf.Sin(t) * 0.5f + 0.5f);
	}

	private IEnumerator moveOffscreen_cr()
	{
		MoveY(OFFSCREEN_Y + properties.smallRobotRotationDistance - base.transform.position.y, 500f);
		while (movingY)
		{
			yield return null;
		}
		RetroArcadeOrbiterRobot[] array = orbiters;
		foreach (RetroArcadeOrbiterRobot retroArcadeOrbiterRobot in array)
		{
			UnityEngine.Object.Destroy(retroArcadeOrbiterRobot.gameObject);
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private IEnumerator shoot_cr()
	{
		while (movingY)
		{
			yield return null;
		}
		string[] pattern = properties.mainRobotShootString.Split(',');
		int currentIndex = UnityEngine.Random.Range(0, pattern.Length);
		while (!base.IsDead)
		{
			float waitTime = 0f;
			float.TryParse(pattern[currentIndex], out waitTime);
			yield return CupheadTime.WaitForSeconds(this, waitTime);
			if (base.IsDead)
			{
				break;
			}
			float shootAngle = MathUtils.DirectionToAngle(PlayerManager.GetNext().center - projectileRoot.position);
			projectilePrefab.Create(projectileRoot.position, properties.mainRobotShootSpeed, shootAngle, properties.mainRobotShotBounce);
		}
	}

	private IEnumerator orbiterShoot_cr()
	{
		while (movingY)
		{
			yield return null;
		}
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, properties.smallRobotAttackDelay.RandomFloat());
			List<RetroArcadeOrbiterRobot> aliveOrbiters = new List<RetroArcadeOrbiterRobot>();
			RetroArcadeOrbiterRobot[] array = orbiters;
			foreach (RetroArcadeOrbiterRobot retroArcadeOrbiterRobot in array)
			{
				if (!retroArcadeOrbiterRobot.IsDead)
				{
					aliveOrbiters.Add(retroArcadeOrbiterRobot);
				}
			}
			if (aliveOrbiters.Count == 0)
			{
				break;
			}
			aliveOrbiters.RandomChoice().Shoot();
		}
	}
}
