using System.Collections;
using UnityEngine;

public class FlyingGenieLevelSphinx : AbstractProjectile
{
	private const string SplitParameterName = "Split";

	private const string ProjectilesLayer = "Projectiles";

	[SerializeField]
	private SpriteRenderer sphinxRenderer;

	[SerializeField]
	private float outOfChestY;

	[SerializeField]
	private float outOfChestSpeed;

	public FlyingGenieLevelSphinxPiece[] sphinxPieces;

	private AbstractPlayerController player;

	private LevelProperties.FlyingGenie.Sphinx properties;

	private Vector3 targetPos;

	private bool moving;

	private string[] pinkPattern;

	private int pinkIndex;

	public void Init(Vector3 startPos, Vector3 targetPos, LevelProperties.FlyingGenie.Sphinx properties, AbstractPlayerController player, string[] pinkPattern, int pinkIndex)
	{
		base.transform.position = startPos;
		this.properties = properties;
		this.player = player;
		this.targetPos = targetPos;
		this.pinkPattern = pinkPattern;
		this.pinkIndex = pinkIndex;
		moving = true;
		StartCoroutine(move_cr());
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		if (base.damageDealer != null)
		{
			base.damageDealer.DealDamage(hit);
		}
		base.OnCollisionPlayer(hit, phase);
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	private IEnumerator move_cr()
	{
		float startPos = (base.transform.position + Vector3.up * outOfChestY).y;
		while (base.transform.position.y < startPos)
		{
			base.transform.AddPosition(0f, outOfChestY * outOfChestSpeed * (float)CupheadTime.Delta);
			yield return null;
		}
		sphinxRenderer.sortingLayerName = "Projectiles";
		sphinxRenderer.sortingOrder = 2;
		if (player == null || player.IsDead)
		{
			player = PlayerManager.GetNext();
		}
		Vector3 targetPos = player.transform.position;
		while (base.transform.position != targetPos)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, targetPos, properties.sphinxSpeed * (float)CupheadTime.Delta);
			yield return null;
		}
		moving = false;
		yield return CupheadTime.WaitForSeconds(this, properties.splitDelay);
		animator.SetTrigger("Split");
	}

	public void Split()
	{
		StartCoroutine(split_cr());
		AudioManager.Play("genie_scarab_release");
		emitAudioFromObject.Add("genie_scarab_release");
	}

	private IEnumerator split_cr()
	{
		GetComponent<Collider2D>().enabled = false;
		int counter = (int)Mathf.Round(properties.sphinxSpawnNum / 2f);
		bool moveRight = false;
		for (int i = 0; i < sphinxPieces.Length; i++)
		{
			if (player == null || player.IsDead)
			{
				player = PlayerManager.GetNext();
			}
			int pink = (pinkIndex + i * 2) % pinkPattern.Length;
			sphinxPieces[i].StartMoving(properties, player, counter, moveRight, pinkPattern, pink);
			moveRight = !moveRight;
			yield return null;
		}
		Die();
		while (base.transform.childCount > 1)
		{
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}

	protected override void Die()
	{
		GetComponent<SpriteRenderer>().enabled = false;
		base.Die();
	}
}
