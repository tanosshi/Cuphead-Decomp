using System.Collections;
using UnityEngine;

public class TreePlatformingLevelButterfly : AbstractPausableComponent
{
	[SerializeField]
	private GameObject sprite1;

	[SerializeField]
	private GameObject sprite2;

	[SerializeField]
	private GameObject sprite3;

	private const float ROTATE_FRAME_TIME = 1f / 24f;

	private Vector2 direction;

	private Vector2 normalized;

	private float velocity;

	private float sinTime;

	private float angle;

	private float rotation;

	private MinMax sinMinMax = new MinMax(1f, 15f);

	private float sineSize;

	private float frameTime;

	private float t;

	private int loopCounter;

	private int maxCounter;

	public bool isActive { get; private set; }

	protected override void Start()
	{
		base.Start();
		maxCounter = Random.Range(4, 7);
		if (sprite3.GetComponent<ParrySwitch>() != null)
		{
			sprite3.GetComponent<ParrySwitch>().OnActivate += Deactivate;
		}
	}

	public void Init(float velocity, float scale, int color)
	{
		isActive = true;
		base.transform.SetScale(scale);
		base.transform.SetEulerAngles(null, null, rotation = ((!(scale < 0f)) ? base.transform.eulerAngles.z : (0f - base.transform.eulerAngles.z)));
		this.velocity = velocity;
		SelectColor(color);
		sineSize = sinMinMax.RandomFloat();
		CalculateSin();
		Setup();
	}

	public void Init(Vector2 pos, float rotation, float maxVelocity, int color, float sinSize, float flyTime)
	{
		isActive = true;
		this.rotation = rotation;
		base.transform.position = pos;
		SelectColor(color);
		sineSize = ((sineSize != 0f) ? sinSize : sinMinMax.RandomFloat());
		StartCoroutine(adjust_speed_cr(new MinMax(0f, maxVelocity), flyTime));
		CalculateSin();
		Setup();
	}

	private void Setup()
	{
		string stateName = "P" + Random.Range(1, 5);
		base.animator.Play(stateName);
		sinTime = Random.Range(1.5f, 2.8f);
		StartCoroutine(check_dist_cr());
		StartCoroutine(move_cr());
	}

	public void Deactivate()
	{
		isActive = false;
		StopAllCoroutines();
		sprite1.SetActive(false);
		sprite2.SetActive(false);
		sprite3.SetActive(false);
	}

	private void SelectColor(int color)
	{
		sprite1.SetActive(false);
		sprite2.SetActive(false);
		sprite3.SetActive(false);
		switch (color)
		{
		case 1:
			sprite1.SetActive(true);
			break;
		case 2:
			sprite2.SetActive(true);
			break;
		case 3:
			sprite3.SetActive(true);
			break;
		}
	}

	private void CalculateSin()
	{
		Vector2 zero = Vector2.zero;
		zero.x = (direction.x + base.transform.position.x) / 2f;
		zero.y = (direction.y + base.transform.position.y) / 2f;
		float num = 0f - (direction.x - base.transform.position.x) / (direction.y - base.transform.position.y);
		float num2 = zero.y - num * zero.x;
		Vector2 zero2 = Vector2.zero;
		zero2.x = zero.x + 1f;
		zero2.y = num * zero2.x + num2;
		normalized = Vector3.zero;
		normalized = zero2 - zero;
		normalized.Normalize();
	}

	private IEnumerator move_cr()
	{
		while (true)
		{
			frameTime += CupheadTime.Delta;
			if (frameTime > 1f / 24f)
			{
				frameTime -= 1f / 24f;
				direction = MathUtils.AngleToDirection(rotation);
				Vector2 vector = base.transform.position;
				angle += sinTime * (float)CupheadTime.Delta;
				vector += normalized * Mathf.Sin(angle) * sineSize;
				vector += direction * velocity * CupheadTime.Delta;
				base.transform.position = vector;
				t += CupheadTime.Delta;
				if (t > sinTime)
				{
					sineSize = sinMinMax.RandomFloat();
					t = 0f;
				}
			}
			yield return null;
		}
	}

	private IEnumerator adjust_speed_cr(MinMax adjustment, float time)
	{
		float val = 0f;
		while (t < time)
		{
			velocity = adjustment.GetFloatAt(val);
			if (val < 1f)
			{
				val = t / time;
				t += CupheadTime.Delta;
			}
			else
			{
				val = 1f;
			}
			yield return null;
		}
		yield return null;
	}

	private IEnumerator check_dist_cr()
	{
		while (true)
		{
			float dist = Vector3.Distance(CupheadLevelCamera.Current.transform.position, base.transform.position);
			if (dist > 2000f)
			{
				break;
			}
			yield return null;
		}
		Deactivate();
		yield return null;
	}

	private void Counter()
	{
		if (loopCounter < maxCounter)
		{
			loopCounter++;
			return;
		}
		string stateName = "P" + Random.Range(1, 5);
		base.animator.Play(stateName);
		maxCounter = Random.Range(4, 6);
		loopCounter = 0;
	}
}
