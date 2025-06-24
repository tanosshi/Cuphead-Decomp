using System;
using System.Collections;
using Rewired;
using UnityEngine;

public class AbstractMapSceneStartUI : AbstractMonoBehaviour
{
	public enum State
	{
		Inactive = 0,
		Animating = 1,
		Active = 2,
		Loading = 3
	}

	[HideInInspector]
	public string level;

	private CanvasGroup canvasGroup;

	private Player player;

	public State CurrentState { get; protected set; }

	protected bool Able
	{
		get
		{
			if (CurrentState != State.Active)
			{
				return false;
			}
			if (MapEquipUI.Current.CurrentState != MapEquipUI.State.Inactive)
			{
				return false;
			}
			if (Map.Current.CurrentState != Map.State.Ready)
			{
				return false;
			}
			if (InterruptingPrompt.IsInterrupting())
			{
				return false;
			}
			return true;
		}
	}

	public event Action OnLoadLevelEvent;

	public event Action OnBackEvent;

	protected override void Awake()
	{
		base.Awake();
		timeLayer = CupheadTime.Layer.UI;
		ignoreGlobalTime = true;
		canvasGroup = GetComponent<CanvasGroup>();
		SetAlpha(0f);
	}

	protected override void Start()
	{
		base.Start();
		PlayerManager.OnPlayerJoinedEvent += OnPlayerJoined;
		PlayerManager.OnPlayerJoinedEvent += OnPlayerLeft;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		PlayerManager.OnPlayerJoinedEvent -= OnPlayerJoined;
		PlayerManager.OnPlayerJoinedEvent -= OnPlayerLeft;
	}

	protected bool GetButtonDown(CupheadButton button)
	{
		if (!Able || InterruptingPrompt.IsInterrupting())
		{
			return false;
		}
		if (player != null && player.GetButtonDown((int)button))
		{
			return true;
		}
		return false;
	}

	private void OnPlayerJoined(PlayerId playerId)
	{
	}

	private void OnPlayerLeft(PlayerId playerId)
	{
	}

	protected void LoadLevel()
	{
		CurrentState = State.Loading;
		if (this.OnLoadLevelEvent != null)
		{
			this.OnLoadLevelEvent();
		}
		this.OnLoadLevelEvent = null;
	}

	public void In(MapPlayerController playerController)
	{
		player = playerController.input.actions;
		Debug.LogWarning("START IN TRUE");
		StartCoroutine(fade_cr(1f, State.Active));
	}

	protected void Out()
	{
		StartCoroutine(fade_cr(0f, State.Inactive));
		if (this.OnBackEvent != null)
		{
			this.OnBackEvent();
		}
		this.OnBackEvent = null;
	}

	protected void SetAlpha(float alpha)
	{
		canvasGroup.alpha = alpha;
	}

	private IEnumerator fade_cr(float end, State endState)
	{
		float t = 0f;
		float start = canvasGroup.alpha;
		CurrentState = State.Animating;
		while (t < 0.2f)
		{
			float val = t / 0.2f;
			SetAlpha(Mathf.Lerp(start, end, val));
			t += Time.deltaTime;
			yield return null;
		}
		SetAlpha(end);
		CurrentState = endState;
	}
}
