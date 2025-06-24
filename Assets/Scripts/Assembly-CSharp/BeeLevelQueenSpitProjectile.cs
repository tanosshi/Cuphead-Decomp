using System.Collections;
using UnityEngine;

public class BeeLevelQueenSpitProjectile : AbstractProjectile
{
	[SerializeField]
	private Effect trailPrefab;

	private Vector2 time = new Vector2(0.43f, 0.06f);

	private float speed = 700f;

	protected override float DestroyLifetime
	{
		get
		{
			return 1000f;
		}
	}

	public BeeLevelQueenSpitProjectile Create(Vector2 pos, Vector2 scale, float speed, Vector2 time)
	{
		BeeLevelQueenSpitProjectile beeLevelQueenSpitProjectile = base.Create(pos, 0f, scale) as BeeLevelQueenSpitProjectile;
		beeLevelQueenSpitProjectile.speed = speed;
		beeLevelQueenSpitProjectile.time = time;
		return beeLevelQueenSpitProjectile;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(rotate_cr());
		StartCoroutine(move_cr());
		StartCoroutine(trail_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (base.damageDealer != null)
		{
			base.damageDealer.DealDamage(hit);
		}
	}

	private void End()
	{
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}

	private IEnumerator trail_cr()
	{
		while (true)
		{
			trailPrefab.Create(base.transform.position);
			yield return CupheadTime.WaitForSeconds(this, 0.25f);
		}
	}

	private IEnumerator move_cr()
	{
		float scale = base.transform.localScale.x;
		while (true)
		{
			Vector2 move = base.transform.right * speed * CupheadTime.Delta * scale;
			base.transform.AddPosition(move.x, move.y);
			yield return null;
			if (base.transform.position.y > 720f)
			{
				End();
			}
		}
	}

	private IEnumerator rotate_cr()
	{
		float rotTime = 0.15f;
		float scale = base.transform.localScale.x;
		yield return CupheadTime.WaitForSeconds(this, 0.05f);
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, time.x);
			yield return StartCoroutine(tweenRotation_cr(0f, 90f * scale, rotTime));
			yield return CupheadTime.WaitForSeconds(this, time.y);
			yield return StartCoroutine(tweenRotation_cr(90f * scale, 180f * scale, rotTime));
			AudioManager.Play("bee_spit_bullet_turn");
			emitAudioFromObject.Add("bee_spit_bullet_turn");
			yield return CupheadTime.WaitForSeconds(this, time.x);
			yield return StartCoroutine(tweenRotation_cr(180f * scale, 90f * scale, rotTime));
			yield return CupheadTime.WaitForSeconds(this, time.y);
			yield return StartCoroutine(tweenRotation_cr(90f * scale, 0f, rotTime));
		}
	}

	private IEnumerator tweenRotation_cr(float start, float end, float time)
	{
		base.transform.SetEulerAngles(null, null, start);
		float t = 0f;
		while (t < time)
		{
			TransformExtensions.SetEulerAngles(z: EaseUtils.Ease(EaseUtils.EaseType.linear, start, end, t / time), transform: base.transform);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.SetEulerAngles(null, null, end);
	}

	private IEnumerator tween_cr(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease)
	{
		base.transform.position = start;
		float t = 0f;
		while (t < time)
		{
			float val = t / time;
			float x = EaseUtils.Ease(ease, start.x, end.x, val);
			TransformExtensions.SetLocalPosition(y: EaseUtils.Ease(ease, start.y, end.y, val), transform: base.transform, x: x, z: 0f);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.position = end;
	}
}
