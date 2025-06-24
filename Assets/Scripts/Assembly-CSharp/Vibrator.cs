using System.Collections;
using Rewired;
using UnityEngine;

public class Vibrator : AbstractMonoBehaviour
{
	private Coroutine vibrateCoroutine;

	public static Vibrator Current { get; private set; }

	public static void Vibrate(float amount, float time, PlayerId player)
	{
		Current._Vibrate(amount, time, player);
	}

	public static void StopVibrating(PlayerId player)
	{
		Current._StopVibrating(player);
	}

	protected override void Awake()
	{
		base.Awake();
		Current = this;
	}

	private void _Vibrate(float amount, float time, PlayerId playerId)
	{
		if (vibrateCoroutine != null)
		{
			StopCoroutine(vibrateCoroutine);
		}
		if (amount <= 0f)
		{
			_StopVibrating(playerId);
		}
		else
		{
			vibrateCoroutine = StartCoroutine(vibrate_cr(amount, time, playerId));
		}
	}

	private void _StopVibrating(PlayerId playerId)
	{
		Player player = ReInput.players.GetPlayer((int)playerId);
		foreach (Joystick joystick in player.controllers.Joysticks)
		{
			joystick.StopVibration();
		}
	}

	private IEnumerator vibrate_cr(float amount, float time, PlayerId playerId)
	{
		Player player = ReInput.players.GetPlayer((int)playerId);
		foreach (Joystick joystick in player.controllers.Joysticks)
		{
			if (joystick.supportsVibration)
			{
				joystick.SetVibration(amount, amount);
			}
		}
		if (!(time > 0f))
		{
			yield break;
		}
		yield return new WaitForSeconds(time);
		foreach (Joystick joystick2 in player.controllers.Joysticks)
		{
			if (joystick2.supportsVibration)
			{
				joystick2.StopVibration();
			}
		}
	}
}
