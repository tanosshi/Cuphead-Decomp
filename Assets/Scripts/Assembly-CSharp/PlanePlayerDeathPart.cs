using System.Collections;
using UnityEngine;

public class PlanePlayerDeathPart : AbstractMonoBehaviour
{
	private const float VELOCITY_X_MIN = -500f;

	private const float VELOCITY_X_MAX = 500f;

	private const float VELOCITY_Y_MIN = 500f;

	private const float VELOCITY_Y_MAX = 1000f;

	private const float GRAVITY = -6000f;

	private Vector2 velocity;

	private float accumulatedGravity;

	public PlanePlayerDeathPart CreatePart(PlayerId player, Vector3 position)
	{
		PlanePlayerDeathPart planePlayerDeathPart = InstantiatePrefab<PlanePlayerDeathPart>();
		planePlayerDeathPart.transform.position = position;
		planePlayerDeathPart.animator.SetInteger("Player", (int)player);
		return planePlayerDeathPart;
	}

	protected override void Awake()
	{
		base.Awake();
		velocity = new Vector2(Random.Range(-500f, 500f), Random.Range(500f, 1000f));
		StartCoroutine(move_cr());
	}

	private IEnumerator move_cr()
	{
		while (true)
		{
			base.transform.position += (Vector3)(velocity + new Vector2(-300f, accumulatedGravity)) * base.LocalDeltaTime;
			accumulatedGravity += -6000f * base.LocalDeltaTime;
			yield return null;
		}
	}

	public void GameOverUnpause()
	{
		base.animator.enabled = true;
		AnimationHelper component = GetComponent<AnimationHelper>();
		component.IgnoreGlobal = true;
		ignoreGlobalTime = true;
		base.enabled = true;
	}
}
