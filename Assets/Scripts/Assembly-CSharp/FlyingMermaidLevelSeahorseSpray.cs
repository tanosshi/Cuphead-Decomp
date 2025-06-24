using System.Collections.Generic;
using UnityEngine;

public class FlyingMermaidLevelSeahorseSpray : AbstractPausableComponent
{
	private class PlayerInfo
	{
		public PlanePlayerMotor.Force force;

		public float timeSinceFx;

		public float fxWaitTime;

		public int lastFxVariant = -1;
	}

	public float width = 20f;

	private Dictionary<PlanePlayerMotor, PlayerInfo> playerInfos = new Dictionary<PlanePlayerMotor, PlayerInfo>();

	private LevelProperties.FlyingMermaid.Seahorse properties;

	[SerializeField]
	private Effect effectPrefab;

	[SerializeField]
	private Transform topRoot;

	private bool ended;

	protected override void Update()
	{
		base.Update();
		if (ended)
		{
			return;
		}
		foreach (PlanePlayerMotor key in playerInfos.Keys)
		{
			if (key == null)
			{
				continue;
			}
			if (Mathf.Abs(key.transform.position.x - base.transform.position.x) < width / 2f && key.player.center.y < topRoot.position.y)
			{
				playerInfos[key].force.enabled = true;
				playerInfos[key].timeSinceFx += CupheadTime.Delta;
				if (playerInfos[key].timeSinceFx >= playerInfos[key].fxWaitTime)
				{
					Effect effect = effectPrefab.Create(key.player.center + new Vector3(0f, -40f));
					int num = (playerInfos[key].lastFxVariant + Random.Range(0, 3)) % 3;
					effect.animator.SetInteger("Effect", num);
					playerInfos[key].lastFxVariant = num;
					playerInfos[key].fxWaitTime = Random.Range(0.125f, 0.17f);
					playerInfos[key].timeSinceFx = 0f;
				}
			}
			else
			{
				playerInfos[key].force.enabled = false;
				playerInfos[key].fxWaitTime = 0f;
			}
		}
	}

	public void Init(LevelProperties.FlyingMermaid.Seahorse properties)
	{
		this.properties = properties;
		PlanePlayerMotor[] array = Object.FindObjectsOfType<PlanePlayerMotor>();
		foreach (PlanePlayerMotor planePlayerMotor in array)
		{
			PlanePlayerMotor.Force force = new PlanePlayerMotor.Force(new Vector2(0f, properties.waterForce), false);
			planePlayerMotor.AddForce(force);
			PlayerInfo playerInfo = new PlayerInfo();
			playerInfo.force = force;
			playerInfos[planePlayerMotor] = playerInfo;
		}
	}

	public void End()
	{
		ended = true;
		PlanePlayerMotor[] array = Object.FindObjectsOfType<PlanePlayerMotor>();
		foreach (PlanePlayerMotor planePlayerMotor in array)
		{
			planePlayerMotor.RemoveForce(playerInfos[planePlayerMotor].force);
		}
	}
}
