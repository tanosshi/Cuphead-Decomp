using System.Collections;
using UnityEngine;

public class DragonLevelMeteor : AbstractProjectile
{
	public enum State
	{
		Up = 1,
		Down = -1,
		Both = 0,
		Forward = 10
	}

	public class Properties
	{
		public float timeY;

		public float speedX;

		public State state;

		public Properties(float timeY, float speedX, State state)
		{
			this.timeY = timeY;
			this.speedX = speedX;
			this.state = state;
		}
	}

	private Properties properties;

	[SerializeField]
	private Effect smokePrefab;

	public DragonLevelMeteor Create(Vector2 pos, Properties properties)
	{
		DragonLevelMeteor dragonLevelMeteor = base.Create() as DragonLevelMeteor;
		dragonLevelMeteor.properties = properties;
		dragonLevelMeteor.transform.position = pos;
		return dragonLevelMeteor;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(smoke_cr());
		StartCoroutine(moveX_cr());
		if (properties.state != State.Forward)
		{
			StartCoroutine(moveY_cr());
		}
		StartCoroutine(rotate_cr());
		AudioManager.PlayLoop("level_dragon_left_dragon_meteor_a_loop");
		emitAudioFromObject.Add("level_dragon_left_dragon_meteor_a_loop");
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	private IEnumerator rotate_cr()
	{
		Vector2 lastPos = base.transform.position;
		while (true)
		{
			base.transform.LookAt2D(lastPos);
			lastPos = base.transform.position;
			yield return null;
		}
	}

	private IEnumerator smoke_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, 0.1f);
			smokePrefab.Create(base.transform.position).transform.SetEulerAngles(0f, 0f, base.transform.eulerAngles.z + Random.Range(-45f, 45f));
			yield return null;
		}
	}

	private IEnumerator moveX_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		while (base.transform.position.x > -840f)
		{
			base.transform.AddPosition((0f - properties.speedX) * CupheadTime.FixedDelta);
			yield return wait;
		}
		Die();
	}

	private IEnumerator moveY_cr()
	{
		int state = (int)properties.state;
		Vector2 start = base.transform.position;
		Vector2 end = new Vector2(start.x - properties.speedX / 2f, 300f * (float)state);
		yield return TweenPositionY(start.y, end.y, properties.timeY / 2f, EaseUtils.EaseType.easeOutSine);
		while (true)
		{
			state *= -1;
			start = base.transform.position;
			end = new Vector2(start.x - properties.speedX, 300f * (float)state);
			yield return TweenPositionY(start.y, end.y, properties.timeY, EaseUtils.EaseType.easeInOutSine);
		}
	}

	protected override void Die()
	{
		base.Die();
		StopAllCoroutines();
		AudioManager.Stop("level_dragon_left_dragon_meteor_a_loop");
	}
}
