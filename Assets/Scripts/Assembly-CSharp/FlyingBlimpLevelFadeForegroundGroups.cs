using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingBlimpLevelFadeForegroundGroups : FlyingBlimpLevelScrollingSpriteSpawnerBase
{
	[SerializeField]
	private FlyingBlimpLevelMoonLady moonLady;

	private List<Transform> daySprites;

	private List<Transform> nightSprites;

	private Transform spawnedChild;

	private float fadeTime;

	private int index;

	private int allDayChildren;

	private bool startedChange;

	protected override void Awake()
	{
		base.Awake();
		fadeTime = 10f;
		daySprites = new List<Transform>();
		nightSprites = new List<Transform>();
		if (spawnedChild != null)
		{
			spawnedChild.transform.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
		}
		for (int i = 0; i < spritePrefabs.Length; i++)
		{
			Transform[] childTransforms = spritePrefabs[i].sprite.transform.GetChildTransforms();
			foreach (Transform transform in childTransforms)
			{
				daySprites.Add(transform.transform);
				nightSprites.Add(transform.transform.GetChild(0));
			}
		}
		for (int k = 0; k < nightSprites.Count; k++)
		{
			if (nightSprites[k].transform != null)
			{
				nightSprites[k].transform.gameObject.GetComponent<SpriteRenderer>().enabled = false;
			}
		}
	}

	protected override void OnSpawn(GameObject obj)
	{
		base.OnSpawn(obj);
		spawnedChild = obj.transform.GetChild(0);
	}

	protected override void Update()
	{
		base.Update();
		if (moonLady.state == FlyingBlimpLevelMoonLady.State.Morph && !startedChange)
		{
			startedChange = true;
			StartChange();
		}
	}

	private void StartChange()
	{
		StartCoroutine(change_cr());
	}

	private IEnumerator change_cr()
	{
		float t = 0f;
		float startSpeed = speed;
		float endSpeed = speed + speed * 0.3f;
		while (t < fadeTime)
		{
			for (int i = 0; i < nightSprites.Count; i++)
			{
				if (nightSprites[i].transform != null)
				{
					nightSprites[i].transform.gameObject.GetComponent<SpriteRenderer>().enabled = true;
					nightSprites[i].transform.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, t / fadeTime);
				}
			}
			speed = Mathf.Lerp(startSpeed, endSpeed, t / fadeTime);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		for (int j = 0; j < nightSprites.Count; j++)
		{
			if (nightSprites[j].transform != null)
			{
				nightSprites[j].transform.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
			}
		}
	}
}
