using System.Collections;
using UnityEngine;

public class ClownLevelCoaster : AbstractCollidableObject
{
	[SerializeField]
	private SpriteRenderer knobSprite;

	[SerializeField]
	private Collider2D knobCollider;

	public Transform pieceRoot;

	private LevelProperties.Clown.Coaster properties;

	private ClownLevelLights warningLights;

	private SpriteRenderer sprite;

	private SpriteRenderer[] childrenSprites;

	private Vector3 frontStartPos;

	private DamageDealer damageDealer;

	private float coasterSize;

	private float coasterLength;

	private bool inView;

	public void Init(Vector3 backStartPos, Vector3 frontStartPos, LevelProperties.Clown.Coaster properties, float coasterLength, float coasterSize, ClownLevelLights warningLights)
	{
		base.transform.position = backStartPos;
		this.frontStartPos = frontStartPos;
		this.properties = properties;
		this.coasterLength = coasterLength;
		this.coasterSize = coasterSize;
		this.warningLights = warningLights;
		sprite = GetComponent<SpriteRenderer>();
	}

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
		hit.GetComponent<LevelPlayerController>().OnPitKnockUp(base.transform.position.y, 0.85f);
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	private void ChompSound()
	{
		if (inView)
		{
			AudioManager.Play("clown_coaster_main");
			emitAudioFromObject.Add("clown_coaster_main");
		}
	}

	private IEnumerator move_coaster_front_cr()
	{
		bool lightsOff = true;
		AudioManager.PlayLoop("sfx_clown_coaster_ratchet_loop");
		emitAudioFromObject.Add("sfx_clown_coaster_ratchet_loop");
		yield return CupheadTime.WaitForSeconds(this, properties.coasterBackToFrontDelay);
		while (base.transform.position.x > -640f - coasterSize * coasterLength)
		{
			base.transform.position += -base.transform.right * properties.coasterSpeed * CupheadTime.Delta;
			if (base.transform.position.x < 640f + 0.2f * coasterSize && lightsOff)
			{
				warningLights.StartWarningLights();
				lightsOff = false;
			}
			if (base.transform.position.x < -640f - coasterSize * coasterLength && !lightsOff)
			{
				warningLights.StopWarningLights();
				lightsOff = true;
			}
			yield return null;
		}
		inView = false;
		AudioManager.Stop("sfx_clown_coaster_ratchet_loop");
		Die();
		yield return null;
	}

	private IEnumerator move_coaster_back_cr()
	{
		inView = true;
		bool flashLights = false;
		AudioManager.PlayLoop("sfx_clown_coaster_distant_by");
		emitAudioFromObject.Add("sfx_clown_coaster_distant_by");
		while (base.transform.position.x < 640f + coasterSize * 0.44f * coasterLength)
		{
			base.transform.position += base.transform.right * properties.coasterSpeed * CupheadTime.Delta;
			yield return null;
		}
		AudioManager.Stop("sfx_clown_coaster_distant_by");
		FrontCoasterSetup();
		yield return null;
	}

	public void BackCoasterSetup()
	{
		int num = 97;
		knobCollider.enabled = false;
		GetComponent<Collider2D>().enabled = false;
		childrenSprites = GetComponentsInChildren<SpriteRenderer>();
		SpriteRenderer[] array = childrenSprites;
		foreach (SpriteRenderer spriteRenderer in array)
		{
			if (spriteRenderer.gameObject.GetComponent<LevelPlatform>() != null)
			{
				spriteRenderer.gameObject.GetComponent<Collider2D>().enabled = false;
			}
		}
		base.transform.SetScale(-0.44f, 0.44f);
		base.transform.SetEulerAngles(null, null, 17.57f);
		sprite.sortingLayerName = "Background";
		sprite.sortingOrder = 45;
		Color color;
		ColorUtility.TryParseHtmlString("#b6b6b6", out color);
		sprite.color = color;
		for (int j = 0; j < childrenSprites.Length; j++)
		{
			childrenSprites[j].color = color;
			childrenSprites[j].sortingLayerName = "Background";
			childrenSprites[j].sortingOrder = num - j;
			if (childrenSprites[j].GetComponent<ClownLevelRiders>() != null)
			{
				childrenSprites[j].GetComponent<Collider2D>().enabled = false;
			}
		}
		knobSprite.GetComponent<SpriteRenderer>().sortingLayerName = "Background";
		knobSprite.GetComponent<SpriteRenderer>().sortingOrder = num + 1;
		StartCoroutine(move_coaster_back_cr());
	}

	public void FrontCoasterSetup()
	{
		int num = 79;
		knobCollider.enabled = true;
		GetComponent<Collider2D>().enabled = true;
		SpriteRenderer[] array = childrenSprites;
		foreach (SpriteRenderer spriteRenderer in array)
		{
			if (spriteRenderer.gameObject.GetComponent<LevelPlatform>() != null)
			{
				spriteRenderer.gameObject.GetComponent<Collider2D>().enabled = true;
			}
		}
		base.transform.position = frontStartPos;
		base.transform.SetScale(1f, 1f);
		base.transform.SetEulerAngles(null, null, 0f);
		sprite.sortingLayerName = "Player";
		sprite.sortingOrder = 80;
		Color color;
		ColorUtility.TryParseHtmlString("#FFFFFFFF", out color);
		sprite.color = color;
		for (int j = 0; j < childrenSprites.Length; j++)
		{
			childrenSprites[j].color = color;
			if (childrenSprites[j].transform.parent == base.transform || childrenSprites[j].transform == base.transform)
			{
				childrenSprites[j].sortingLayerName = "Player";
				childrenSprites[j].sortingOrder = num - j;
			}
			else if (childrenSprites[j].GetComponent<ClownLevelRiders>() != null)
			{
				childrenSprites[j].GetComponent<Collider2D>().enabled = true;
				childrenSprites[j].sortingLayerName = "Player";
				childrenSprites[j].sortingOrder = num - j;
				childrenSprites[j].GetComponent<ClownLevelRiders>().FrontLayers(num - j);
			}
			else if (!childrenSprites[j].transform.parent.GetComponent<ClownLevelRiders>())
			{
				childrenSprites[j].sortingLayerName = "Default";
				childrenSprites[j].sortingOrder = 4;
			}
		}
		knobSprite.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
		knobSprite.GetComponent<SpriteRenderer>().sortingOrder = num + 1;
		StartCoroutine(move_coaster_front_cr());
	}

	protected void Die()
	{
		Object.Destroy(base.gameObject);
	}
}
