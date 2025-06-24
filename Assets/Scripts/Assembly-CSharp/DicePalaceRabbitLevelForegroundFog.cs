using UnityEngine;

public class DicePalaceRabbitLevelForegroundFog : AbstractPausableComponent
{
	[SerializeField]
	private Transform pivotPoint;

	private float loopSize = 5f;

	private float speed = 2f;

	private float angle;

	private float time;

	private float fadeTime = 5f;

	private bool fadingOut = true;

	protected override void Update()
	{
		base.Update();
		angle += speed * (float)CupheadTime.Delta;
		Vector3 vector = new Vector3((0f - Mathf.Sin(angle)) * loopSize, 0f, 0f);
		Vector3 vector2 = new Vector3(0f, Mathf.Cos(angle) * loopSize, 0f);
		base.transform.position = pivotPoint.position;
		base.transform.position += vector + vector2;
		if (fadingOut)
		{
			if (time < fadeTime)
			{
				if (GetComponent<SpriteRenderer>().color.a > 0.5f)
				{
					GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f - time / fadeTime);
				}
				time += CupheadTime.Delta;
			}
			else
			{
				fadingOut = !fadingOut;
				time = 0f;
			}
		}
		else if (time < fadeTime)
		{
			GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f + time / fadeTime);
			time += CupheadTime.Delta;
		}
		else
		{
			fadingOut = !fadingOut;
			time = 0f;
		}
	}
}
