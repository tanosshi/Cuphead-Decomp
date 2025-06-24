using System.Collections;
using UnityEngine;

public class BatLevelPentagram : AbstractCollidableObject
{
	private LevelProperties.Bat.Pentagrams properties;

	private AbstractPlayerController player;

	private bool onRight;

	public void Init(Vector2 pos, LevelProperties.Bat.Pentagrams properties, AbstractPlayerController player, bool onRight)
	{
		base.transform.position = pos;
		this.properties = properties;
		this.player = player;
		this.onRight = onRight;
		GetComponent<Collider2D>().enabled = false;
		StartCoroutine(move_cr());
		base.transform.SetScale(properties.pentagramSize, properties.pentagramSize, 1f);
	}

	private IEnumerator move_cr()
	{
		Vector3 pos = base.transform.position;
		float endPos = 560f;
		if (onRight)
		{
			while (base.transform.position.x > player.transform.position.x)
			{
				pos.x = Mathf.MoveTowards(base.transform.position.x, player.transform.position.x, properties.xSpeed * (float)CupheadTime.Delta);
				base.transform.position = pos;
				yield return null;
			}
		}
		else
		{
			while (base.transform.position.x < player.transform.position.x)
			{
				pos.x = Mathf.MoveTowards(base.transform.position.x, player.transform.position.x, properties.xSpeed * (float)CupheadTime.Delta);
				base.transform.position = pos;
				yield return null;
			}
		}
		GetComponent<Collider2D>().enabled = true;
		while (base.transform.position.y < endPos)
		{
			pos.y = Mathf.MoveTowards(base.transform.position.y, endPos, properties.ySpeed * (float)CupheadTime.Delta);
			base.transform.position = pos;
			yield return null;
		}
		Die();
		yield return null;
	}

	private void Die()
	{
		Object.Destroy(base.gameObject);
	}
}
