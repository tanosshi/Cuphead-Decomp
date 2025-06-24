using System.Collections;
using UnityEngine;

public class ClownLevelSwings : AbstractCollidableObject
{
	public static float moveSpeed;

	private const float FALL_GRAVITY = -100f;

	public bool isBackSeat;

	[SerializeField]
	private Transform rod;

	private LevelProperties.Clown.Swing properties;

	private SpriteRenderer sprite;

	private Vector3 startPosition;

	private Color defaultColor;

	private bool resetWarning;

	private float spacing;

	private float lowestPoint;

	private float highPoint = 100f;

	private float distAmount = 450f;

	private float startAngleFront = 320f;

	private float startAngleBack = 40f;

	private float endAngleFront = 350f;

	private float endAngleBack = 10f;

	private float enterAngle;

	public void Init(Vector3 pos, LevelProperties.Clown.Swing properties, float spacing, float enterAngle)
	{
		this.properties = properties;
		base.transform.position = pos;
		this.spacing = spacing;
		this.enterAngle = enterAngle;
	}

	protected override void Start()
	{
		base.Start();
		sprite = GetComponent<SpriteRenderer>();
		startPosition = base.transform.position;
		defaultColor = GetComponent<SpriteRenderer>().color;
		if (isBackSeat)
		{
			rod.transform.SetEulerAngles(null, null, startAngleBack - enterAngle);
		}
		else
		{
			rod.transform.SetEulerAngles(null, null, startAngleFront + enterAngle);
		}
		StartCoroutine(rotation_cr());
		StartCoroutine(move_swing_y_cr());
		StartCoroutine(move_swing_x_cr());
		if (properties.swingDropOn)
		{
			StartCoroutine(player_check_cr());
		}
	}

	private IEnumerator move_swing_y_cr()
	{
		bool starting = true;
		float distanceLeft = 0f;
		float distanceRight = 0f;
		while (true)
		{
			Vector3 pos = base.transform.position;
			float speed = ((!starting) ? 50f : 150f);
			if (isBackSeat)
			{
				distanceLeft = 0f - distAmount;
				distanceRight = distAmount + distAmount / 2f;
			}
			else
			{
				distanceLeft = 0f - distAmount - distAmount / 2f;
				distanceRight = distAmount;
			}
			if (base.transform.position.x > distanceRight || base.transform.position.x < distanceLeft)
			{
				if (base.transform.position.y != highPoint)
				{
					pos.y = Mathf.MoveTowards(base.transform.position.y, highPoint, speed * (float)CupheadTime.Delta);
					base.transform.position = pos;
				}
				else
				{
					starting = false;
				}
			}
			else if (base.transform.position.y != lowestPoint)
			{
				pos.y = Mathf.MoveTowards(base.transform.position.y, lowestPoint, speed * (float)CupheadTime.Delta);
				base.transform.position = pos;
			}
			else
			{
				starting = false;
			}
			yield return null;
		}
	}

	private IEnumerator move_swing_x_cr()
	{
		moveSpeed = properties.swingSpeed;
		float size = base.transform.GetComponent<Renderer>().bounds.size.x;
		float sizeDivided = size / 4f;
		while (true)
		{
			if (isBackSeat)
			{
				float end = 640f - spacing * 5f;
				while (base.transform.position.x > end)
				{
					base.transform.position -= base.transform.right * moveSpeed * CupheadTime.Delta;
					yield return null;
				}
				base.transform.position = new Vector3(640f + spacing, highPoint, 0f);
			}
			else
			{
				float end2 = -640f + (spacing * 5f + size);
				base.transform.GetComponent<Collider2D>().enabled = true;
				while (base.transform.position.x < end2)
				{
					if (base.transform.position.x > 640f + sizeDivided)
					{
						base.transform.GetComponent<Collider2D>().enabled = false;
					}
					base.transform.position += base.transform.right * moveSpeed * CupheadTime.Delta;
					yield return null;
				}
				resetWarning = true;
				SwingReappear();
				base.transform.position = new Vector3(-640f - (spacing - size), highPoint, 0f);
			}
			StopCoroutine(rotation_cr());
			enterAngle = 0f;
			if (isBackSeat)
			{
				rod.transform.SetEulerAngles(null, null, startAngleBack);
			}
			else
			{
				rod.transform.SetEulerAngles(null, null, startAngleFront);
			}
			StartCoroutine(rotation_cr());
			yield return null;
		}
	}

	private IEnumerator rotation_cr()
	{
		float t = 0f;
		if (isBackSeat)
		{
			while (rod.transform.eulerAngles.z > endAngleBack)
			{
				if ((float)CupheadTime.Delta != 0f)
				{
					rod.transform.SetEulerAngles(null, null, rod.transform.eulerAngles.z - t);
					t += 0.001f;
				}
				yield return null;
			}
		}
		else
		{
			while (rod.transform.eulerAngles.z < endAngleFront)
			{
				if ((float)CupheadTime.Delta != 0f)
				{
					rod.transform.SetEulerAngles(null, null, rod.transform.eulerAngles.z + t);
					t += 0.001f;
				}
				yield return null;
			}
		}
		yield return null;
	}

	private IEnumerator player_check_cr()
	{
		AbstractPlayerController player = PlayerManager.GetNext();
		while (true)
		{
			if (player == null || player.transform.parent != base.transform)
			{
				player = PlayerManager.GetNext();
				yield return null;
				continue;
			}
			if (!resetWarning)
			{
				sprite.color = Color.red;
				yield return CupheadTime.WaitForSeconds(this, properties.swingDropWarningDuration);
				yield return null;
				sprite.color = Color.black;
				base.transform.GetComponent<Collider2D>().enabled = false;
				yield return CupheadTime.WaitForSeconds(this, properties.swingfullDropDuration);
			}
			SwingReappear();
			resetWarning = false;
			yield return null;
		}
	}

	private void SwingReappear()
	{
		base.transform.GetComponent<Collider2D>().enabled = true;
		sprite.color = defaultColor;
	}
}
