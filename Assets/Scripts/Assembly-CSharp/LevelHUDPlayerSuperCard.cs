using UnityEngine;

public class LevelHUDPlayerSuperCard : AbstractMonoBehaviour
{
	private const float SPEED = 10f;

	private const float Y_DIFF = -32f;

	private bool initialized;

	private float current;

	private float max;

	private Vector3 start;

	private Vector3 end;

	private Vector3 target;

	private PlayerId playerId;

	protected override void Start()
	{
		base.Start();
		end = base.transform.localPosition;
		start = end + new Vector3(0f, -32f, 0f);
	}

	protected override void Update()
	{
		base.Update();
		UpdatePosition();
	}

	private void UpdatePosition()
	{
		if (initialized)
		{
			target = Vector3.Lerp(start, end, current / max);
			base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, target, (float)CupheadTime.Delta * 10f);
		}
	}

	public void Init(PlayerId playerId, float exCost)
	{
		max = exCost;
		this.playerId = playerId;
		switch (playerId)
		{
		case PlayerId.PlayerOne:
			base.animator.SetInteger("Player", 0);
			break;
		case PlayerId.PlayerTwo:
			base.animator.SetInteger("Player", 1);
			break;
		default:
			Debug.LogWarning(string.Concat("Player '", playerId, "' not yet configured"));
			break;
		}
		initialized = true;
	}

	public void SetAmount(float amount)
	{
		current = Mathf.Clamp(amount, 0f, max);
	}

	public void SetSuper(bool super)
	{
		base.animator.SetBool("Super", super);
	}

	public void SetEx(bool ex)
	{
		base.animator.SetBool("Ex", ex);
	}
}
