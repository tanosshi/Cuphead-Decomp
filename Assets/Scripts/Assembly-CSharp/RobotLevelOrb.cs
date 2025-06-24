using System.Collections;
using UnityEngine;

public class RobotLevelOrb : AbstractProjectile
{
	[SerializeField]
	private GameObject lasers;

	[SerializeField]
	private Transform top;

	[SerializeField]
	private Transform bottom;

	[SerializeField]
	private SpriteRenderer pinkTop;

	[SerializeField]
	private SpriteRenderer pinkBottom;

	private RobotLevelRobotChest parent;

	private LevelProperties.Robot properties;

	private DamageReceiver damageReceiver;

	private bool activeShields;

	private bool wasActive;

	private float health;

	private int speed;

	private Vector3 offsetAfterSpawn;

	private Vector3 bottomStartPos;

	private Vector3 topStartPos;

	protected override void Awake()
	{
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(fade_in_cr());
	}

	protected override void Update()
	{
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
		base.Update();
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}

	protected virtual void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (!activeShields)
		{
			health -= info.damage;
			if (health <= 0f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	public override void OnParry(AbstractPlayerController player)
	{
		if (activeShields)
		{
			lasers.SetActive(false);
			activeShields = false;
			pinkTop.enabled = false;
			pinkBottom.enabled = false;
			SetParryable(false);
			AudioManager.Play("robot_orb_death");
			emitAudioFromObject.Add("robot_orb_death");
			animator.SetTrigger("Continue");
			StartCoroutine(slide_in_cr());
		}
	}

	public RobotLevelOrb Create(Vector3 position, Vector3 offsetAfterSpawn)
	{
		GameObject gameObject = Object.Instantiate(base.gameObject, position, Quaternion.identity);
		RobotLevelOrb component = gameObject.GetComponent<RobotLevelOrb>();
		component.offsetAfterSpawn = offsetAfterSpawn;
		return component;
	}

	private IEnumerator fade_in_cr()
	{
		base.transform.SetScale(0.5f, 0.5f);
		pinkTop.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
		pinkBottom.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
		float t = 0f;
		float time = 0.9f;
		while (t < time)
		{
			t += (float)CupheadTime.Delta;
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeInSine, 0.5f, 1f, t / time);
			base.transform.SetScale(val, val);
			pinkTop.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, t / time);
			pinkBottom.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, t / time);
			yield return null;
		}
		yield return null;
	}

	public void InitOrb(RobotLevelRobotChest parent, LevelProperties.Robot properties)
	{
		base.transform.position += Vector3.left * properties.CurrentState.orb.orbMovementSpeed * CupheadTime.Delta;
		this.parent = parent;
		this.properties = properties;
		if (properties.CurrentState.orb.orbShieldIsActive)
		{
			SetParryable(true);
			activeShields = true;
			pinkTop.enabled = true;
			pinkBottom.enabled = true;
		}
		else
		{
			activeShields = false;
		}
		health = properties.CurrentState.orb.orbHP;
		speed = properties.CurrentState.orb.orbMovementSpeed;
		base.transform.right = Vector3.left;
		StartCoroutine(fade_color_cr());
		StartCoroutine(lasers_cr());
		StartCoroutine(move_cr());
	}

	public void InitChildOrb(int speed, float health, bool activeShields)
	{
		this.speed = speed;
		this.health = health;
		this.activeShields = activeShields;
		StartCoroutine(move_cr());
		lasers.SetActive(lasers.activeSelf);
	}

	private IEnumerator lasers_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.orb.orbInitalOpenDelay);
		if (activeShields)
		{
			StartCoroutine(slide_out_cr());
			yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.orb.orbInitialLaserDelay);
			if (activeShields)
			{
				animator.Play("Laser_Start");
				lasers.SetActive(true);
				AudioManager.PlayLoop("robot_orb_spark_loop");
				yield return null;
			}
		}
	}

	private IEnumerator shields_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.orb.orbSpawnDelay);
		activeShields = true;
	}

	private IEnumerator move_cr()
	{
		while (true)
		{
			if (base.transform.position.x < offsetAfterSpawn.x && base.transform.position.y < offsetAfterSpawn.y)
			{
				base.transform.position += Vector3.up * speed * CupheadTime.Delta * 0.5f;
			}
			base.transform.position += Vector3.left * speed * CupheadTime.Delta;
			if (base.transform.position.x < (float)Level.Current.Left - GetComponents<BoxCollider2D>()[0].size.x / 2f)
			{
				AudioManager.Stop("robot_orb_spark_loop");
				Object.Destroy(base.gameObject);
			}
			yield return null;
		}
	}

	private IEnumerator slide_out_cr()
	{
		Debug.Log("slide_out_cr");
		float sizeY = GetComponent<Collider2D>().bounds.size.y;
		float localPosTop = top.transform.localPosition.y + sizeY / 4f;
		float localPosBottom = bottom.transform.localPosition.y - sizeY / 4f;
		Vector3 topPos = top.transform.localPosition;
		Vector3 bottomPos = bottom.transform.localPosition;
		float time = 0.5f;
		float t = 0f;
		if (activeShields)
		{
			wasActive = true;
			AudioManager.Play("robot_orb_spark_start");
			animator.Play("Sparks_Start");
		}
		while (t < time)
		{
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, t / time);
			topPos.y = Mathf.Lerp(topStartPos.y, localPosTop, val);
			bottomPos.y = Mathf.Lerp(bottomStartPos.y, localPosBottom, val);
			top.transform.localPosition = topPos;
			bottom.transform.localPosition = bottomPos;
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		yield return null;
	}

	private IEnumerator slide_in_cr()
	{
		Vector3 topPos = top.transform.localPosition;
		Vector3 bottomPos = bottom.transform.localPosition;
		float time = 0.5f;
		float t = 0f;
		while (t < time)
		{
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, t / time);
			topPos.y = Mathf.Lerp(topPos.y, topStartPos.y, val);
			bottomPos.y = Mathf.Lerp(bottomPos.y, bottomStartPos.y, val);
			top.transform.localPosition = topPos;
			bottom.transform.localPosition = bottomPos;
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		if (wasActive)
		{
			Debug.Log("Sparks_End");
			AudioManager.Stop("robot_orb_spark_loop");
			animator.Play("Sparks_End");
		}
		yield return null;
	}

	protected virtual IEnumerator fade_color_cr()
	{
		float t = 0f;
		float fadeTime = 0.5f;
		while (t < fadeTime)
		{
			if (!activeShields)
			{
				top.GetComponent<SpriteRenderer>().color = new Color(t / fadeTime, t / fadeTime, t / fadeTime, 1f);
				bottom.GetComponent<SpriteRenderer>().color = new Color(t / fadeTime, t / fadeTime, t / fadeTime, 1f);
			}
			else
			{
				pinkTop.color = new Color(t / fadeTime, t / fadeTime, t / fadeTime, 1f);
				pinkBottom.color = new Color(t / fadeTime, t / fadeTime, t / fadeTime, 1f);
			}
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		yield return null;
	}
}
