using UnityEngine;

public class WinScreenCharacterAnimation : AbstractMonoBehaviour
{
	[SerializeField]
	private Animator results;

	[SerializeField]
	private SpriteRenderer blinkLayer;

	[SerializeField]
	private SpriteRenderer blinkLayer2;

	public bool is2Player;

	private int singleBlinkAmount;

	private int doubleBlinkAmount;

	private int singleCount;

	private int doubleCount;

	private bool p1Turn;

	protected override void Start()
	{
		base.Start();
		blinkLayer.enabled = false;
		if (blinkLayer2 != null)
		{
			blinkLayer2.enabled = false;
		}
		results.SetBool("pickedA", Rand.Bool());
		singleBlinkAmount = Random.Range(0, 4) + 4;
		doubleBlinkAmount = Random.Range(0, 10) + 16;
	}

	private void EndCycle()
	{
		blinkLayer.enabled = false;
		if (blinkLayer2 != null)
		{
			blinkLayer2.enabled = false;
		}
		if (singleCount < singleBlinkAmount)
		{
			singleCount++;
		}
		else
		{
			if (blinkLayer2 != null)
			{
				if (is2Player)
				{
					blinkLayer2.enabled = true;
				}
				else
				{
					blinkLayer.enabled = true;
				}
				is2Player = !is2Player;
			}
			else
			{
				blinkLayer.enabled = true;
			}
			singleCount = 0;
			singleBlinkAmount = Random.Range(0, 4) + 4;
		}
		if (blinkLayer2 != null)
		{
			if (doubleCount < doubleBlinkAmount)
			{
				doubleCount++;
				return;
			}
			blinkLayer2.enabled = true;
			blinkLayer.enabled = true;
			doubleCount = 0;
			doubleBlinkAmount = Random.Range(0, 10) + 16;
		}
	}
}
