using System.Collections;
using UnityEngine;

public class BaronessLevelPlatform : PlatformingLevelPlatformSag
{
	[SerializeField]
	private BaronessLevelCastle castle;

	private float speed = 200f;

	private int counter;

	private int maxCounter;

	private LevelProperties.Baroness.Platform properties;

	public void getProperties(LevelProperties.Baroness.Platform properties)
	{
		this.properties = properties;
		Vector3 position = base.transform.position;
		position.y = properties.YPosition;
		base.transform.position = position;
		maxCounter = Random.Range(4, 9);
		StartCoroutine(move_cr());
	}

	private IEnumerator move_cr()
	{
		bool movingLeft = true;
		while (true)
		{
			Vector3 pos = base.transform.position;
			if (movingLeft)
			{
				if (castle.state == BaronessLevelCastle.State.Chase)
				{
					base.animator.Play("Fast");
				}
				pos.x = Mathf.MoveTowards(base.transform.position.x, -640f + properties.LeftBoundaryOffset, speed * (float)CupheadTime.Delta);
				movingLeft = ((base.transform.position.x != -640f + properties.LeftBoundaryOffset) ? true : false);
			}
			else
			{
				if (castle.state == BaronessLevelCastle.State.Chase)
				{
					base.animator.Play("Slow");
				}
				pos.x = Mathf.MoveTowards(base.transform.position.x, (float)Level.Current.Right - properties.RightBoundaryOffset, speed * (float)CupheadTime.Delta);
				movingLeft = ((base.transform.position.x == (float)Level.Current.Right - properties.RightBoundaryOffset) ? true : false);
			}
			base.transform.position = pos;
			yield return null;
		}
	}

	private void SweatCounter()
	{
		if (counter < maxCounter)
		{
			counter++;
			return;
		}
		base.animator.Play("Sweat");
		counter = 0;
		maxCounter = Random.Range(4, 9);
	}
}
