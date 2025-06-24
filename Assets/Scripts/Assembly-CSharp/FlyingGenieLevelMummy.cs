using System.Collections;
using UnityEngine;

public class FlyingGenieLevelMummy : BasicProjectile
{
	public enum MummyType
	{
		Classic = 0,
		Chomper = 1,
		Grabby = 2
	}

	[SerializeField]
	private SpriteRenderer purpleSprite;

	private LevelProperties.FlyingGenie.Coffin properties;

	private Vector3 normalized;

	private DamageReceiver damageReceiver;

	private float loopSize = 5f;

	private float hp;

	private float rotation;

	public MummyType type { get; private set; }

	public FlyingGenieLevelMummy Create(Vector3 position, float speed, float rotation, LevelProperties.FlyingGenie.Coffin properties, MummyType type, float hp, int sortingOrder)
	{
		FlyingGenieLevelMummy flyingGenieLevelMummy = base.Create(position, rotation, speed) as FlyingGenieLevelMummy;
		flyingGenieLevelMummy.transform.position = position;
		flyingGenieLevelMummy.properties = properties;
		flyingGenieLevelMummy.type = type;
		flyingGenieLevelMummy.hp = hp;
		flyingGenieLevelMummy.rotation = rotation;
		flyingGenieLevelMummy.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
		flyingGenieLevelMummy.purpleSprite.sortingOrder = sortingOrder + 1;
		return flyingGenieLevelMummy;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		hp -= info.damage;
		if (hp < 0f)
		{
			Die();
		}
	}

	protected override void Start()
	{
		base.Start();
		AudioManager.Play("genie_mummy_voice_attack");
		emitAudioFromObject.Add("genie_mummy_voice_attack");
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		StartCoroutine(fade_purple_cr());
		switch (type)
		{
		case MummyType.Classic:
			CalculateSin();
			if (properties.mummyASinWave)
			{
				StartCoroutine(classic_bounce_cr());
			}
			animator.Play("Classic");
			break;
		case MummyType.Chomper:
			animator.Play("Chomper");
			break;
		case MummyType.Grabby:
			animator.Play("Grabby");
			break;
		}
	}

	private IEnumerator fade_purple_cr()
	{
		float t = 0f;
		float time = 1.5f;
		Color start = purpleSprite.color;
		Color end = new Color(1f, 1f, 1f, 0f);
		while (t < time)
		{
			purpleSprite.color = Color.Lerp(start, end, t / time);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		yield return null;
	}

	private void CalculateSin()
	{
		Vector3 vector = MathUtils.AngleToDirection(rotation);
		Vector2 zero = Vector2.zero;
		zero.x = (vector.x + base.transform.position.x) / 2f;
		zero.y = (vector.y + base.transform.position.y) / 2f;
		float num = 0f - (vector.x - base.transform.position.x) / (vector.y - base.transform.position.y);
		float num2 = zero.y - num * zero.x;
		Vector2 zero2 = Vector2.zero;
		zero2.x = zero.x + 1f;
		zero2.y = num * zero2.x + num2;
		normalized = Vector3.zero;
		normalized = zero2 - zero;
		normalized.Normalize();
	}

	private IEnumerator classic_bounce_cr()
	{
		Vector3 pos = base.transform.position;
		float angle = 0f;
		while (true)
		{
			angle += 10f * (float)CupheadTime.Delta;
			if ((float)CupheadTime.Delta != 0f)
			{
				pos = normalized * Mathf.Sin(angle) * 2f;
				base.transform.position += pos;
			}
			yield return null;
		}
	}

	private IEnumerator grabby_speed_cr()
	{
		float t = 0f;
		float time = 0.5f;
		while (t < time)
		{
			Speed = Mathf.Lerp(t: EaseUtils.Ease(EaseUtils.EaseType.linear, 0f, 1f, t / time), a: 0f - properties.mummyCSpeed, b: 0f);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		yield return null;
	}

	private void ChangeSpeed()
	{
		if (properties.mummyCSlowdown)
		{
			Speed = 0f - properties.mummyCSpeed;
			StartCoroutine(grabby_speed_cr());
		}
	}

	protected override void Die()
	{
		base.Die();
		AudioManager.Stop("genie_mummy_voice_attack");
		AudioManager.Play("genie_mummy_voice_die");
		emitAudioFromObject.Add("genie_mummy_voice_die");
		GetComponent<Collider2D>().enabled = false;
		GetComponent<SpriteRenderer>().enabled = false;
	}
}
