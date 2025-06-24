using System.Collections;
using UnityEngine;

public class MapNPCBarbershopSong : MonoBehaviour
{
	private CupheadInput.AnyPlayerInput input;

	[SerializeField]
	private Animator[] barbershopAnimators = new Animator[4];

	private Coroutine songCoroutine;

	public bool songEndedOrPlayerStop;

	private bool delay;

	private void Start()
	{
		input = new CupheadInput.AnyPlayerInput();
		AddDialoguerEvents();
	}

	private void Update()
	{
		if (songCoroutine == null || !delay || !input.GetAnyButtonDown())
		{
			return;
		}
		StopCoroutine(songCoroutine);
		songCoroutine = null;
		delay = false;
		songEndedOrPlayerStop = true;
		AudioManager.Stop("mus_barbershop");
		AudioManager.FadeBGMVolume(1f, 0.5f, false);
		AudioManager.FadeSFXVolume("hint_genie", 1f, 0.5f);
		for (int i = 0; i < barbershopAnimators.Length; i++)
		{
			barbershopAnimators[i].SetTrigger("endsong");
		}
		songEndedOrPlayerStop = false;
		if (Map.Current != null)
		{
			Map.Current.CurrentState = Map.State.Ready;
		}
		for (int j = 0; j < Map.Current.players.Length; j++)
		{
			if (!(Map.Current.players[j] == null))
			{
				Map.Current.players[j].Enable();
			}
		}
	}

	private void OnDestroy()
	{
		RemoveDialoguerEvents();
	}

	private IEnumerator sing_cr()
	{
		AudioManager.FadeBGMVolume(0f, 0.5f, true);
		AudioManager.Play("mus_barbershop");
		yield return null;
		for (int i = 0; i < Map.Current.players.Length; i++)
		{
			if (!(Map.Current.players[i] == null))
			{
				Map.Current.players[i].Disable();
			}
		}
		if (Map.Current != null)
		{
			Map.Current.CurrentState = Map.State.Event;
		}
		for (int j = 0; j < barbershopAnimators.Length; j++)
		{
			barbershopAnimators[j].SetTrigger("sing");
		}
		yield return CupheadTime.WaitForSeconds(this, 0.5f);
		delay = true;
		yield return barbershopAnimators[3].WaitForAnimationToStart(this, "anim_map_barbershop_sing_hold");
		barbershopAnimators[0].SetTrigger("trans");
		yield return new WaitForSeconds(1f / 12f);
		barbershopAnimators[1].SetTrigger("trans");
		yield return new WaitForSeconds(1f / 12f);
		barbershopAnimators[2].SetTrigger("trans");
		yield return new WaitForSeconds(1f / 12f);
		barbershopAnimators[3].SetTrigger("trans");
		yield return barbershopAnimators[3].WaitForAnimationToStart(this, "anim_map_barbershop_sing_idle_boil", true);
		barbershopAnimators[0].SetTrigger("blink");
		yield return new WaitForSeconds(1f / 12f);
		barbershopAnimators[1].SetTrigger("blink");
		yield return new WaitForSeconds(1f / 12f);
		barbershopAnimators[2].SetTrigger("blink");
		yield return new WaitForSeconds(1f / 12f);
		barbershopAnimators[3].SetTrigger("blink");
		yield return barbershopAnimators[3].WaitForAnimationToStart(this, "anim_map_barbershop_sing_main_loop", true);
		while (!songEndedOrPlayerStop)
		{
			yield return null;
			if (!AudioManager.CheckIfPlaying("mus_barbershop"))
			{
				songEndedOrPlayerStop = true;
			}
		}
		for (int k = 0; k < barbershopAnimators.Length; k++)
		{
			barbershopAnimators[k].SetTrigger("endsong");
		}
		yield return null;
		songCoroutine = null;
		songEndedOrPlayerStop = false;
		if (Map.Current != null)
		{
			Map.Current.CurrentState = Map.State.Ready;
		}
		for (int l = 0; l < Map.Current.players.Length; l++)
		{
			if (!(Map.Current.players[l] == null))
			{
				Map.Current.players[l].Enable();
			}
		}
	}

	public void AddDialoguerEvents()
	{
		Dialoguer.events.onMessageEvent += OnDialoguerMessageEvent;
	}

	public void RemoveDialoguerEvents()
	{
		Dialoguer.events.onMessageEvent -= OnDialoguerMessageEvent;
	}

	private void OnDialoguerMessageEvent(string message, string metadata)
	{
		if (message == "QuartetSing")
		{
			if (songCoroutine != null)
			{
				StopCoroutine(songCoroutine);
			}
			songCoroutine = StartCoroutine(sing_cr());
		}
	}
}
