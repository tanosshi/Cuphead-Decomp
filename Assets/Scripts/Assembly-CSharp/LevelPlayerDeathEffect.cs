using UnityEngine;

public class LevelPlayerDeathEffect : Effect
{
	[SerializeField]
	private SpriteRenderer cuphead;

	[SerializeField]
	private SpriteRenderer mugman;

	[SerializeField]
	private SpriteRenderer shadow;

	private PlayerId id;

	private bool playerGrounded;

	public void Init(Vector2 pos, PlayerId id, bool playerGrounded)
	{
		base.transform.position = pos;
		this.id = id;
		this.playerGrounded = playerGrounded;
		if (id == PlayerId.PlayerOne || id != PlayerId.PlayerTwo)
		{
			cuphead.enabled = true;
		}
		else
		{
			mugman.enabled = true;
		}
		if (playerGrounded)
		{
			shadow.enabled = true;
		}
	}

	public void Init(Vector2 pos)
	{
		base.transform.position = pos;
		shadow.enabled = false;
	}
}
