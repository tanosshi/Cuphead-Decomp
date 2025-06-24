using System.Collections.Generic;
using UnityEngine;

public class EnemyDatabase : ScriptableObject
{
	public const string PATH = "EnemyDatabase/data_enemies";

	private static EnemyDatabase _instance;

	public List<EnemyProperties> enemyProperties;

	public int index;

	private static EnemyProperties defaultProperties = new EnemyProperties();

	public static EnemyDatabase Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Resources.Load<EnemyDatabase>("EnemyDatabase/data_enemies");
			}
			return _instance;
		}
	}

	public static EnemyProperties GetProperties(EnemyID id)
	{
		switch (id)
		{
		default:
			Debug.LogWarning(string.Concat("Enemy ID '", id, "' not setup!"));
			return defaultProperties;
		case EnemyID.Undefined:
			Debug.LogWarning("Enemy ID is Undefined. Set enemy type before continuing!");
			return null;
		case EnemyID.blue_goblin:
			return Instance.enemyProperties[0];
		case EnemyID.pink_goblin:
			return Instance.enemyProperties[1];
		case EnemyID.wind:
			return Instance.enemyProperties[2];
		case EnemyID.blob_runner:
			return Instance.enemyProperties[3];
		case EnemyID.lobber:
			return Instance.enemyProperties[4];
		case EnemyID.flower_grunt:
			return Instance.enemyProperties[5];
		case EnemyID.mushroom:
			return Instance.enemyProperties[6];
		case EnemyID.chomper:
			return Instance.enemyProperties[7];
		case EnemyID.acorn:
			return Instance.enemyProperties[8];
		case EnemyID.acornmaker:
			return Instance.enemyProperties[9];
		case EnemyID.spiker:
			return Instance.enemyProperties[10];
		case EnemyID.ladybug:
			return Instance.enemyProperties[11];
		case EnemyID.dragonfly:
			return Instance.enemyProperties[12];
		case EnemyID.dragonflyshot:
			return Instance.enemyProperties[13];
		case EnemyID.woodpecker:
			return Instance.enemyProperties[14];
		case EnemyID.beetle:
			return Instance.enemyProperties[15];
		case EnemyID.lobster:
			return Instance.enemyProperties[16];
		case EnemyID.barnacle:
			return Instance.enemyProperties[17];
		case EnemyID.urchin:
			return Instance.enemyProperties[18];
		case EnemyID.crab:
			return Instance.enemyProperties[19];
		case EnemyID.krill:
			return Instance.enemyProperties[20];
		case EnemyID.clam:
			return Instance.enemyProperties[21];
		case EnemyID.starfish:
			return Instance.enemyProperties[22];
		case EnemyID.flyingfish:
			return Instance.enemyProperties[23];
		case EnemyID.satyr:
			return Instance.enemyProperties[24];
		case EnemyID.mudman:
			return Instance.enemyProperties[25];
		case EnemyID.smallmudman:
			return Instance.enemyProperties[26];
		case EnemyID.dragon:
			return Instance.enemyProperties[27];
		case EnemyID.miner:
			return Instance.enemyProperties[28];
		case EnemyID.fan:
			return Instance.enemyProperties[29];
		case EnemyID.flamer:
			return Instance.enemyProperties[30];
		case EnemyID.wall:
			return Instance.enemyProperties[31];
		case EnemyID.funhousewall:
			return Instance.enemyProperties[32];
		case EnemyID.funwall2:
			return Instance.enemyProperties[33];
		case EnemyID.rocket:
			return Instance.enemyProperties[34];
		case EnemyID.jack:
			return Instance.enemyProperties[35];
		case EnemyID.duck:
			return Instance.enemyProperties[36];
		case EnemyID.miniduck:
			return Instance.enemyProperties[37];
		case EnemyID.jackinbox:
			return Instance.enemyProperties[38];
		case EnemyID.tuba:
			return Instance.enemyProperties[39];
		case EnemyID.starcannon:
			return Instance.enemyProperties[40];
		case EnemyID.balloon:
			return Instance.enemyProperties[41];
		case EnemyID.pretzel:
			return Instance.enemyProperties[42];
		case EnemyID.arcade:
			return Instance.enemyProperties[43];
		case EnemyID.ballrunner:
			return Instance.enemyProperties[44];
		case EnemyID.magician:
			return Instance.enemyProperties[45];
		case EnemyID.polebot:
			return Instance.enemyProperties[46];
		case EnemyID.log:
			return Instance.enemyProperties[47];
		case EnemyID.hotdog:
			return Instance.enemyProperties[48];
		}
	}
}
