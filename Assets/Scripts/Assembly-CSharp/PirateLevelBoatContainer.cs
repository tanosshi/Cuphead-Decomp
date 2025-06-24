using UnityEngine;

public class PirateLevelBoatContainer : AbstractPausableComponent
{
	public enum State
	{
		Bobbing = 0,
		ToStatic = 1,
		Static = 2
	}

	[SerializeField]
	private float targetY;

	private State state;

	private Vector3 startPos;

	private Vector3 endPos;

	private float time;

	protected override void Awake()
	{
		base.Awake();
		startPos = base.transform.position;
		endPos = startPos + new Vector3(0f, targetY, 0f);
	}

	protected override void Update()
	{
		base.Update();
		if (PauseManager.state != PauseManager.State.Paused && state != State.Static)
		{
			float t = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, Mathf.PingPong(time, 1f));
			base.transform.position = Vector3.Lerp(startPos, endPos, t);
			time += Time.deltaTime / 2f;
		}
	}

	public void EndBobbing()
	{
		base.transform.position = startPos;
		state = State.Static;
	}
}
