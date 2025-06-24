using System.Collections;
using UnityEngine;

public class SallyStagePlayLevelMeteor : AbstractProjectile
{
	public enum State
	{
		Meteor = 0,
		Hook = 1,
		Leaving = 2
	}

	[SerializeField]
	private GameObject meteor;

	[SerializeField]
	private ParrySwitch star;

	private DamageReceiver damageReceiver;

	private LevelProperties.SallyStagePlay.Meteor properties;

	private float hp;

	public float spawnPosition { get; private set; }

	public State state { get; private set; }

	public override float ParryMeterMultiplier
	{
		get
		{
			return 0.25f;
		}
	}

	protected override float DestroyLifetime
	{
		get
		{
			return 0f;
		}
	}

	public SallyStagePlayLevelMeteor Create(float pos, float hp, LevelProperties.SallyStagePlay.Meteor properties)
	{
		SallyStagePlayLevelMeteor sallyStagePlayLevelMeteor = base.Create() as SallyStagePlayLevelMeteor;
		sallyStagePlayLevelMeteor.properties = properties;
		sallyStagePlayLevelMeteor.spawnPosition = pos;
		sallyStagePlayLevelMeteor.hp = hp;
		return sallyStagePlayLevelMeteor;
	}

	protected override void Awake()
	{
		base.Awake();
		star.OnActivate += ParryStar;
		star.GetComponent<Collider2D>().enabled = false;
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	protected override void Start()
	{
		base.Start();
		base.transform.position = new Vector2(-640f + spawnPosition, 360f);
		StartCoroutine(move_down_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		hp -= info.damage;
		if (hp <= 0f && state == State.Meteor)
		{
			state = State.Hook;
			OnMeteorDie();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (state == State.Meteor)
		{
			base.damageDealer.DealDamage(hit);
		}
	}

	private IEnumerator move_down_cr()
	{
		AudioManager.Play("sally_meteor_ascend_decend");
		emitAudioFromObject.Add("sally_meteor_ascend_decend");
		state = State.Meteor;
		while (base.transform.position.y > (float)Level.Current.Ground + 100f)
		{
			base.transform.position -= base.transform.up * properties.meteorSpeed * CupheadTime.Delta;
			yield return null;
		}
		yield return null;
	}

	private IEnumerator move_up_cr()
	{
		AudioManager.Play("sally_meteor_ascend_decend");
		emitAudioFromObject.Add("sally_meteor_ascend_decend");
		while (star.transform.position.y < 360f - properties.hookMaxHeight)
		{
			star.transform.position += star.transform.up * properties.meteorSpeed * CupheadTime.Delta;
			yield return null;
		}
		yield return null;
	}

	private IEnumerator leave_cr()
	{
		while (star.transform.position.y < 460f)
		{
			star.transform.position += star.transform.up * properties.meteorSpeed * CupheadTime.Delta;
			yield return null;
		}
		Die();
		yield return null;
	}

	private IEnumerator leave_all_cr()
	{
		while (base.transform.position.y < 460f)
		{
			base.transform.position += base.transform.up * properties.meteorSpeed * CupheadTime.Delta;
			yield return null;
		}
		Die();
		yield return null;
	}

	private IEnumerator timer_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, properties.hookParryExitDelay);
		StartCoroutine(leave_cr());
		yield return null;
	}

	private void OnMeteorDie()
	{
		star.GetComponent<Collider2D>().enabled = true;
		GetComponent<Collider2D>().enabled = false;
		GetComponent<Animator>().SetTrigger("OpenMeteor");
		AudioManager.Play("sally_meteor_open");
		emitAudioFromObject.Add("sally_meteor_open");
		damageReceiver.enabled = false;
		StartCoroutine(move_up_cr());
		StartCoroutine(slide_meteor_cr());
	}

	public void ParryStar()
	{
		GetComponent<Animator>().SetTrigger("SpinStar");
		state = State.Leaving;
		StartCoroutine(leave_cr());
	}

	protected override void Die()
	{
		base.Die();
		state = State.Leaving;
		spawnPosition = 0f;
		GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<Collider2D>().enabled = false;
		SpriteRenderer[] componentsInChildren = GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer spriteRenderer in componentsInChildren)
		{
			spriteRenderer.enabled = false;
		}
	}

	protected override void OnCollisionOther(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionOther(hit, phase);
		if ((bool)hit.GetComponent<SallyStagePlayLevelWave>())
		{
			StartCoroutine(leave_all_cr());
		}
	}

	private IEnumerator slide_meteor_cr()
	{
		float t = 0f;
		float time = 1f;
		Vector3 start = meteor.transform.position;
		Vector3 end = new Vector3(meteor.transform.position.x, meteor.transform.position.y + 700f);
		while (t < time)
		{
			t += (float)CupheadTime.Delta;
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeInSine, 0f, 1f, t / time);
			meteor.transform.position = Vector3.Lerp(start, end, val);
			yield return null;
		}
	}
}
