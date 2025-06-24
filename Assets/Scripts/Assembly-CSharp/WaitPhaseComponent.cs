using DialoguerCore;
using DialoguerEditor;
using UnityEngine;

public class WaitPhaseComponent : MonoBehaviour
{
	public DialogueEditorWaitTypes type;

	public WaitPhase phase;

	public bool go;

	public float duration;

	public float elapsed;

	public void Init(WaitPhase phase, DialogueEditorWaitTypes type, float duration)
	{
		this.phase = phase;
		this.type = type;
		this.duration = duration;
		elapsed = 0f;
		go = true;
	}

	private void Update()
	{
		if (!go)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		switch (type)
		{
		case DialogueEditorWaitTypes.Seconds:
			elapsed += deltaTime;
			if (elapsed >= duration)
			{
				waitComplete();
			}
			break;
		case DialogueEditorWaitTypes.Frames:
			elapsed += 1f;
			if (elapsed >= duration)
			{
				waitComplete();
			}
			break;
		}
	}

	private void waitComplete()
	{
		go = false;
		phase.waitComplete();
		phase = null;
		Object.Destroy(base.gameObject);
	}
}
