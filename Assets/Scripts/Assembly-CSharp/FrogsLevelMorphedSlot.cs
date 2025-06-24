using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogsLevelMorphedSlot : AbstractPausableComponent
{
	public enum State
	{
		Normal = 0,
		Flashing = 1
	}

	public enum Action
	{
		Static = 0,
		Spinning = 1,
		Ending = 2
	}

	[Serializable]
	public class Textures
	{
		public Texture2D[] normal;

		public Texture2D[] flashing;

		public Texture2D Get(State state, int frame)
		{
			if (state == State.Normal || state != State.Flashing)
			{
				return normal[frame];
			}
			return flashing[frame];
		}
	}

	private const float STOP_OFFSET = 3f;

	private const float STOP_TIME = 1f;

	private const float OFFSET_SPEED = 5f;

	private const float FLASH_TIME = 0.2f;

	[SerializeField]
	private Textures textures;

	private Material mat;

	private Dictionary<Slots.Mode, float> offsets;

	public Slots.Mode mode { get; private set; }

	public State state { get; private set; }

	public Action action { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		offsets = new Dictionary<Slots.Mode, float>();
		offsets[Slots.Mode.Snake] = 0.4f;
		offsets[Slots.Mode.Tiger] = -0.095f;
		offsets[Slots.Mode.Bison] = 0.095f;
		offsets[Slots.Mode.Oni] = -0.4f;
		mat = GetComponent<Renderer>().material;
		SetTexture(textures.Get(State.Normal, 0));
		SetOffset(offsets[Slots.Mode.Snake]);
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(animate_cr());
	}

	private void SetTexture(Texture2D texture)
	{
		mat.mainTexture = texture;
	}

	private void SetOffset(float y)
	{
		Vector2 mainTextureOffset = mat.mainTextureOffset;
		mainTextureOffset.y = y;
		mat.mainTextureOffset = mainTextureOffset;
	}

	public void StartSpin()
	{
		AudioManager.PlayLoop("level_frogs_morphed_spin_loop");
		emitAudioFromObject.Add("level_frogs_morphed_spin_loop");
		StopAllCoroutines();
		StartCoroutine(animate_cr());
		StartCoroutine(spin_cr());
	}

	public void StopSpin(Slots.Mode mode)
	{
		AudioManager.Stop("level_frogs_morphed_spin_loop");
		emitAudioFromObject.Add("level_frogs_morphed_spin_loop");
		AudioManager.Play("level_frogs_morphed_spin");
		emitAudioFromObject.Add("level_frogs_morphed_spin");
		this.mode = mode;
		action = Action.Ending;
	}

	public void Flash()
	{
		StartCoroutine(flash_cr());
	}

	private IEnumerator animate_cr()
	{
		int frame = 0;
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, 0.06f);
			SetTexture(textures.Get(state, frame));
			frame = (int)Mathf.Repeat(frame + 1, 3f);
		}
	}

	private IEnumerator spin_cr()
	{
		float offset = mat.mainTextureOffset.y;
		action = Action.Spinning;
		while (action == Action.Spinning)
		{
			offset = Mathf.Repeat(offset + 5f * (float)CupheadTime.Delta, 1f);
			SetOffset(offset);
			yield return null;
		}
		float t = 0f;
		SetOffset(-3f);
		float startOffset = mat.mainTextureOffset.y;
		while (t < 1f)
		{
			float o = EaseUtils.Ease(value: t / 1f, ease: EaseUtils.EaseType.easeOutElastic, start: startOffset, end: offsets[mode]);
			SetOffset(o);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
	}

	private IEnumerator flash_cr()
	{
		state = State.Flashing;
		yield return CupheadTime.WaitForSeconds(this, 0.2f);
		state = State.Normal;
	}
}
