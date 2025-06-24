using System.Collections;
using UnityEngine;

public class FlyingGenieLevelSphinxPiece : AbstractProjectile
{
	[SerializeField]
	private FlyingGenieLevelMiniCat miniCat;

	private LevelProperties.FlyingGenie.Sphinx properties;

	private AbstractPlayerController player;

	private bool moveRight;

	private int maxCounter;

	private string[] pinkPattern;

	private int pinkIndex;

	protected override bool DestroyedAfterLeavingScreen
	{
		get
		{
			return true;
		}
	}

	public void StartMoving(LevelProperties.FlyingGenie.Sphinx properties, AbstractPlayerController player, int maxCounter, bool moveRight, string[] pinkPattern, int pinkIndex)
	{
		this.properties = properties;
		this.player = player;
		GetComponent<Collider2D>().enabled = true;
		this.maxCounter = maxCounter;
		this.moveRight = moveRight;
		this.pinkPattern = pinkPattern;
		this.pinkIndex = pinkIndex;
		StartCoroutine(move_cr());
	}

	private IEnumerator move_cr()
	{
		StartCoroutine(spawn_minis_cr());
		while (true)
		{
			base.transform.position += base.transform.right * properties.sphinxSplitSpeed * CupheadTime.Delta * (moveRight ? 1 : (-1));
			yield return null;
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	private IEnumerator spawn_minis_cr()
	{
		int counter = 0;
		while (base.transform.position.y < 720f && base.transform.position.y > -360f && counter < maxCounter)
		{
			FlyingGenieLevelMiniCat p = miniCat.Create(base.transform.position, 0f, player, properties);
			p.SetParryable(pinkPattern[pinkIndex][0] == 'P');
			pinkIndex = (pinkIndex + 1) % pinkPattern.Length;
			counter++;
			yield return CupheadTime.WaitForSeconds(this, properties.miniSpawnDelay);
		}
		yield return null;
	}

	protected override void RandomizeVariant()
	{
	}

	protected override void SetTrigger(Triggers trigger)
	{
	}
}
