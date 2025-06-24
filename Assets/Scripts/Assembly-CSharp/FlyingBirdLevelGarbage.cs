using System.Collections;
using UnityEngine;

public class FlyingBirdLevelGarbage : BasicProjectile
{
	private const float ROTATE_FRAME_TIME = 1f / 24f;

	[SerializeField]
	private bool isBoot;

	private float bootSpeed;

	protected override void Start()
	{
		base.Start();
		if (!isBoot)
		{
			StartCoroutine(not_boot_cr());
		}
		else
		{
			animator.SetBool("OnClockwise", Rand.Bool());
		}
		StartCoroutine(change_layer_cr());
	}

	private IEnumerator not_boot_cr()
	{
		float frameTime = 0f;
		bootSpeed = ((!Rand.Bool()) ? 600f : 300f);
		bootSpeed = ((!Rand.Bool()) ? bootSpeed : (0f - bootSpeed));
		while (true)
		{
			frameTime += (float)CupheadTime.Delta;
			if (frameTime > 1f / 24f)
			{
				frameTime -= 1f / 24f;
				base.transform.Rotate(0f, 0f, bootSpeed * (float)CupheadTime.Delta);
			}
			yield return null;
		}
	}

	private IEnumerator change_layer_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		GetComponent<SpriteRenderer>().sortingLayerName = SpriteLayer.Projectiles.ToString();
	}

	protected override void Die()
	{
		base.Die();
		GetComponent<SpriteRenderer>().enabled = false;
	}
}
