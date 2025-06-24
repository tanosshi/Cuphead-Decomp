using System.Collections;
using UnityEngine;

public class ClownLevelLights : AbstractPausableComponent
{
	[SerializeField]
	private SpriteRenderer redLight;

	[SerializeField]
	private SpriteRenderer greenLight;

	private const float fadeTime = 0.083f;

	private bool isOn;

	protected override void Start()
	{
		base.Start();
		redLight.enabled = false;
		greenLight.enabled = false;
		StartCoroutine(warning_lights_cr());
	}

	public void StartWarningLights()
	{
		AudioManager.PlayLoop("clown_warning_lights_loop");
		emitAudioFromObject.Add("clown_warning_lights_loop");
		redLight.enabled = true;
		greenLight.enabled = true;
		isOn = true;
	}

	public void StopWarningLights()
	{
		AudioManager.Stop("clown_warning_lights_loop");
		redLight.enabled = false;
		greenLight.enabled = false;
		isOn = false;
	}

	private IEnumerator warning_lights_cr()
	{
		float t = 0f;
		while (true)
		{
			redLight.color = new Color(1f, 1f, 1f, 1f);
			greenLight.color = new Color(1f, 1f, 1f, 0f);
			if (isOn)
			{
				t = 0f;
				while (t < 0.083f)
				{
					redLight.color = new Color(1f, 1f, 1f, 1f - t / 0.083f);
					greenLight.color = new Color(1f, 1f, 1f, t / 0.083f);
					t += (float)CupheadTime.Delta;
					yield return null;
				}
				redLight.color = new Color(1f, 1f, 1f, 0f);
				greenLight.color = new Color(1f, 1f, 1f, 1f);
				t = 0f;
				yield return CupheadTime.WaitForSeconds(this, 0.083f);
				yield return null;
				while (t < 0.083f)
				{
					redLight.color = new Color(1f, 1f, 1f, t / 0.083f);
					greenLight.color = new Color(1f, 1f, 1f, 1f - t / 0.083f);
					t += (float)CupheadTime.Delta;
					yield return null;
				}
				redLight.color = new Color(1f, 1f, 1f, 1f);
				greenLight.color = new Color(1f, 1f, 1f, 0f);
				yield return CupheadTime.WaitForSeconds(this, 0.083f);
				yield return null;
			}
			yield return null;
		}
	}
}
