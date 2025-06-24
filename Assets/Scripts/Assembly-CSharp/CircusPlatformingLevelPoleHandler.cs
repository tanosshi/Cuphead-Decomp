using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircusPlatformingLevelPoleHandler : AbstractPausableComponent
{
	[SerializeField]
	private int poleBotCount;

	[SerializeField]
	private Transform poleRoot;

	[SerializeField]
	private CircusPlatformingLevelPoleBot poleBot;

	private List<CircusPlatformingLevelPoleBot> poleBots;

	protected override void Start()
	{
		base.Start();
		SetupBots();
	}

	protected override void Update()
	{
		base.Update();
	}

	private void SetupBots()
	{
		poleBots = new List<CircusPlatformingLevelPoleBot>();
		float y = poleBot.GetComponent<BoxCollider2D>().size.y;
		for (int i = 0; i < poleBotCount; i++)
		{
			Vector2 vector = new Vector2(poleRoot.transform.position.x, poleRoot.transform.position.y + y * 1.38f * (float)i);
			poleBots.Add(poleBot.Spawn(vector));
		}
		StartCoroutine(check_to_slide_cr());
	}

	private IEnumerator check_to_slide_cr()
	{
		int indexToSlide = 1000;
		while (true)
		{
			for (int i = 0; i < poleBots.Count; i++)
			{
				if (poleBots[i].isDying)
				{
					poleBots.RemoveAt(i);
					indexToSlide = i;
					break;
				}
				if (i >= indexToSlide)
				{
					while (poleBots[i].isSliding)
					{
						yield return null;
					}
					poleBots[i].SlideDown();
				}
				if (i == poleBots.Count - 1)
				{
					indexToSlide = 1000;
				}
			}
			yield return null;
		}
	}
}
