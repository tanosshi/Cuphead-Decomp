using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonLevelPlatformManager : AbstractPausableComponent
{
	private const float LEFT_X = -1280f;

	private const float RIGHT_X = 1280f;

	[SerializeField]
	public List<DragonLevelCloudPlatform> platforms;

	[SerializeField]
	private DragonLevelCloudPlatform platformPrefab;

	private LevelProperties.Dragon.Clouds properties;

	private int maxPlatforms = 20;

	private float startPosition;

	private bool toggleDelay;

	public void Init(LevelProperties.Dragon.Clouds properties)
	{
		this.properties = properties;
		toggleDelay = true;
		platforms = new List<DragonLevelCloudPlatform>();
		for (int i = 0; i < maxPlatforms; i++)
		{
			DragonLevelCloudPlatform dragonLevelCloudPlatform = Object.Instantiate(platformPrefab);
			dragonLevelCloudPlatform.gameObject.SetActive(false);
			dragonLevelCloudPlatform.transform.parent = base.transform;
			platforms.Add(dragonLevelCloudPlatform);
		}
		StartCoroutine(spawn_platforms());
		StartCoroutine(run_delay_cr());
	}

	public void UpdateProperties(LevelProperties.Dragon.Clouds properties)
	{
		toggleDelay = true;
		this.properties = properties;
		foreach (DragonLevelCloudPlatform platform in platforms)
		{
			platform.GetProperties(properties, false);
		}
		StartCoroutine(run_delay_cr());
	}

	public void DestroyObjectPool(DragonLevelCloudPlatform obj)
	{
		platforms.Add(obj);
		obj.gameObject.SetActive(false);
	}

	private IEnumerator run_delay_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, properties.cloudSpeed / 200f);
		toggleDelay = false;
	}

	private IEnumerator spawn_platforms()
	{
		List<string> positions = new List<string>(properties.cloudPositions);
		int mainIndex = Random.Range(0, positions.Count);
		string[] positionString = positions[mainIndex].Split(',');
		int positionIndex = Random.Range(0, positionString.Length);
		int platformIndex = 0;
		float platformWidth = platformPrefab.GetComponent<Renderer>().bounds.size.x / 2f;
		float waitTime = 0f;
		float position = 0f;
		while (true)
		{
			if (toggleDelay)
			{
				yield return null;
				continue;
			}
			positionString = positions[mainIndex].Split(',');
			startPosition = ((!properties.movingRight) ? (640f + platformWidth) : (-640f - platformWidth));
			if (positionString[positionIndex][0] == 'D')
			{
				float.TryParse(positionString[positionIndex].Substring(1), out waitTime);
			}
			else
			{
				string[] array = positionString[positionIndex].Split('-');
				string[] array2 = array;
				foreach (string s in array2)
				{
					float.TryParse(s, out position);
					platforms[platformIndex].transform.position = new Vector3(startPosition, 360f - position, 0f);
					platforms[platformIndex].gameObject.SetActive(true);
					platforms[platformIndex].GetProperties(this, properties);
					platformIndex = (platformIndex + 1) % platforms.Count;
				}
				waitTime = properties.cloudDelay;
			}
			yield return CupheadTime.WaitForSeconds(this, waitTime);
			if (positionIndex < positionString.Length - 1)
			{
				positionIndex++;
			}
			else if (positions.Count <= 1)
			{
				positionIndex = 0;
				mainIndex = 0;
				positions = new List<string>(properties.cloudPositions);
			}
			else
			{
				positions.Remove(positions[mainIndex]);
				positionIndex = 0;
				mainIndex = Random.Range(0, positions.Count);
			}
		}
	}
}
