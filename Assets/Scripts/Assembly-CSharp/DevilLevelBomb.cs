using System.Collections;
using UnityEngine;

public class DevilLevelBomb : AbstractProjectile
{
	[SerializeField]
	private DevilLevelBombExplosion explosionPrefab;

	[SerializeField]
	private SpriteRenderer shadowSprite;

	private LevelProperties.Devil.BombEye properties;

	private Vector2 startPos;

	private Vector2 endPos;

	private float t;

	private bool flipX;

	private bool flipY;

	private bool onLeft;

	private bool comingOut = true;

	public DevilLevelBomb Create(Vector2 pos, LevelProperties.Devil.BombEye properties, bool onLeft)
	{
		DevilLevelBomb devilLevelBomb = InstantiatePrefab<DevilLevelBomb>();
		devilLevelBomb.properties = properties;
		devilLevelBomb.transform.position = pos;
		devilLevelBomb.startPos = pos;
		devilLevelBomb.flipX = Rand.Bool();
		devilLevelBomb.flipY = Rand.Bool();
		devilLevelBomb.onLeft = onLeft;
		return devilLevelBomb;
	}

	protected override void Start()
	{
		base.Start();
		AudioManager.Play("p3_bomb_appear");
		emitAudioFromObject.Add("p3_bomb_appear");
		StartCoroutine(move_cr());
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
	}

	private IEnumerator move_cr()
	{
		float t = 0f;
		float time = 0.5f;
		float end = ((!onLeft) ? (startPos.x - 300f) : (startPos.x + 300f));
		while (t < time)
		{
			t += CupheadTime.FixedDelta;
			base.transform.SetPosition(Mathf.Lerp(startPos.x, end, t / time));
			yield return new WaitForFixedUpdate();
		}
		StartCoroutine(fade_shadow_cr());
		endPos = base.transform.position;
		comingOut = false;
		yield return null;
	}

	private IEnumerator fade_shadow_cr()
	{
		float t = 0f;
		float time = 1f;
		while (t < time)
		{
			t += (float)CupheadTime.Delta;
			shadowSprite.color = new Color(1f, 1f, 1f, 1f - t / time);
			yield return null;
		}
		yield return null;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (!base.dead && !comingOut)
		{
			t += CupheadTime.FixedDelta;
			if (t > properties.explodeDelay)
			{
				Explode();
				Die();
				return;
			}
			Vector2 vector = endPos;
			vector.x += Mathf.Sin(t * properties.xSinSpeed * (float)((!flipX) ? 1 : (-1))) * properties.xSinHeight;
			vector.y += Mathf.Sin(t * properties.ySinSpeed * (float)((!flipY) ? 1 : (-1))) * properties.ySinHeight;
			base.transform.SetPosition(vector.x, vector.y);
		}
	}

	private void Explode()
	{
		explosionPrefab.Create(base.transform.position);
	}

	protected override void Die()
	{
		base.Die();
		Object.Destroy(base.gameObject);
	}
}
