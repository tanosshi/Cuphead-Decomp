using System.Collections;
using UnityEngine;

public class FlyingBlimpLevelFadeForeground : FlyingBlimpLevelScrollingSpriteSpawnerBase
{
	[SerializeField]
	private FlyingBlimpLevelMoonLady moonLady;

	private Transform[] nightSprite;

	private Transform spawnedChild;

	private float fadeTime;

	private int index;

	private bool startedChange;

	protected override void Awake()
	{
		base.Awake();
		fadeTime = 10f;
		nightSprite = new Transform[spritePrefabs.Length];
		if (spawnedChild != null)
		{
			spawnedChild.transform.gameObject.GetComponent<SpriteRenderer>().enabled = false;
		}
		for (int i = 0; i < nightSprite.Length; i++)
		{
			nightSprite[i] = spritePrefabs[i].sprite.transform.GetChild(0);
			nightSprite[i].transform.gameObject.GetComponent<SpriteRenderer>().enabled = false;
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
			if (spawnedChild != null)
			{
				spawnedChild.transform.gameObject.GetComponent<SpriteRenderer>().enabled = true;
				spawnedChild.transform.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, t / fadeTime);
			}
			for (int i = 0; i < nightSprite.Length; i++)
			{
				if (nightSprite[i].transform != null)
				{
					nightSprite[i].transform.gameObject.GetComponent<SpriteRenderer>().enabled = true;
					nightSprite[i].transform.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, t / fadeTime);
				}
			}
			speed = Mathf.Lerp(startSpeed, endSpeed, t / fadeTime);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		if (spawnedChild != null)
		{
			spawnedChild.transform.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
		}
		for (int j = 0; j < nightSprite.Length; j++)
		{
			if (nightSprite[j].transform != null)
			{
				nightSprite[j].transform.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
				yield return null;
			}
		}
		yield return null;
	}
}
