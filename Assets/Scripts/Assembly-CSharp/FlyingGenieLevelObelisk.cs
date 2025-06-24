using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingGenieLevelObelisk : AbstractCollidableObject
{
	[SerializeField]
	private GameObject baseA;

	[SerializeField]
	private GameObject baseB;

	[SerializeField]
	private BoxCollider2D bouncerWall;

	[SerializeField]
	private BoxCollider2D ceiling;

	[SerializeField]
	private BoxCollider2D ground;

	[SerializeField]
	private FlyingGenieLevelGenieHead genieHead;

	[SerializeField]
	private FlyingGenieLevelObeliskBlock obeliskBlock;

	private List<FlyingGenieLevelObeliskBlock> obeliskBlocks;

	private List<FlyingGenieLevelGenieHead> genieHeads;

	private LevelProperties.FlyingGenie.Obelisk properties;

	private FlyingGenieLevelGenie parent;

	private DamageDealer damageDealer;

	private Vector3 startPosition;

	private Vector3 newEmitPosition;

	private float health;

	private float shootAngle;

	private string[] genieHeadNums;

	private string[] loops = new string[2] { "genie_pillar_main_loop", "genie_pillar_destructable_loop" };

	public bool isOn { get; private set; }

	public bool isFirst { get; private set; }

	public void Init(Vector2 pos, LevelProperties.FlyingGenie.Obelisk properties, FlyingGenieLevelGenie parent, bool isFirst)
	{
		this.isFirst = isFirst;
		base.transform.position = pos;
		startPosition = pos;
		this.properties = properties;
		this.parent = parent;
		if (isFirst)
		{
			AudioManager.PlayLoop("genie_pillar_main_loop");
			AudioManager.PlayLoop("genie_pillar_destructable_loop");
			emitAudioFromObject.Add("genie_pillar_main_loop");
			emitAudioFromObject.Add("genie_pillar_destructable_loop");
		}
	}

	protected override void Start()
	{
		base.Start();
		damageDealer = DamageDealer.NewEnemy();
		if (Rand.Bool())
		{
			baseA.SetActive(true);
		}
		else
		{
			baseB.SetActive(true);
		}
	}

	public void ActivateObelisk(string[] genieHeadNums)
	{
		isOn = true;
		bouncerWall.enabled = true;
		base.transform.position = startPosition;
		this.genieHeadNums = genieHeadNums;
		float num = obeliskBlock.GetComponent<Renderer>().bounds.size.y / 1.95f;
		int num2 = 100;
		int result = 0;
		int num3 = 5;
		bool flag = false;
		bool flag2 = false;
		obeliskBlocks = new List<FlyingGenieLevelObeliskBlock>();
		genieHeads = new List<FlyingGenieLevelGenieHead>();
		bool[] array = new bool[num3];
		int[] array2 = new int[num3];
		for (int i = 0; i < num3; i++)
		{
			flag = false;
			foreach (string s in genieHeadNums)
			{
				int.TryParse(s, out result);
				if (result == i + 1)
				{
					flag = true;
				}
				if (genieHeadNums.Length < 2 && result == 2)
				{
					flag2 = true;
				}
			}
			array[i] = flag;
		}
		int num4 = ((result <= 1) ? (num3 - 1) : (result - 1 - 1));
		if (genieHeadNums.Length > 1)
		{
			if (genieHeadNums[0][0] == '2' && genieHeadNums[1][0] == '5')
			{
				array2[0] = 1;
				array2[2] = 5;
				array2[3] = 1;
			}
			else if (genieHeadNums[0][0] == '1' && genieHeadNums[1][0] == '4')
			{
				array2[1] = 5;
				array2[2] = 1;
				array2[4] = 5;
			}
			else if (genieHeadNums[0][0] == '1' && genieHeadNums[1][0] == '5')
			{
				array2[1] = 4;
				array2[2] = 5;
				array2[3] = 1;
			}
		}
		else
		{
			for (int k = 0; k < num3; k++)
			{
				array2[num4] = k + 1;
				num4 = (num4 + 1) % num3;
			}
		}
		bool flag3 = Rand.Bool();
		for (int l = 0; l < num3; l++)
		{
			Vector3 vector = new Vector3(0f, (float)(-l) * num - num / 1.5f, 0f);
			if (array[l])
			{
				FlyingGenieLevelGenieHead flyingGenieLevelGenieHead = Object.Instantiate(genieHead);
				flyingGenieLevelGenieHead.Init(base.transform.position + vector, properties, properties.obeliskGenieHP, parent);
				flyingGenieLevelGenieHead.transform.parent = base.transform;
				flyingGenieLevelGenieHead.GetComponent<SpriteRenderer>().sortingOrder = num2;
				flyingGenieLevelGenieHead.animator.SetBool("GoClockwise", flag3);
				genieHeads.Add(flyingGenieLevelGenieHead);
			}
			else
			{
				FlyingGenieLevelObeliskBlock flyingGenieLevelObeliskBlock = Object.Instantiate(obeliskBlock);
				flyingGenieLevelObeliskBlock.Init(base.transform.position + vector, properties);
				flyingGenieLevelObeliskBlock.transform.parent = base.transform;
				flyingGenieLevelObeliskBlock.GetComponent<SpriteRenderer>().sortingOrder = num2;
				flyingGenieLevelObeliskBlock.animator.SetBool("GoClockwise", flag3);
				flyingGenieLevelObeliskBlock.animator.SetInteger("PickBlock", array2[l]);
				obeliskBlocks.Add(flyingGenieLevelObeliskBlock);
			}
			if (!flag2)
			{
				flag3 = !flag3;
			}
			num2 -= 2;
		}
		StartCoroutine(move_cr());
		if (properties.normalShotOn)
		{
			StartCoroutine(shoot_cr());
		}
	}

	public void SetColliders(float width, float offset)
	{
		ceiling.transform.position = new Vector3(offset, ceiling.transform.position.y);
		ceiling.size = new Vector3(width, ceiling.size.y);
		ground.transform.position = new Vector3(offset, -360f);
		ground.size = new Vector3(width, ground.size.y);
	}

	private IEnumerator shoot_cr()
	{
		string[] anglePattern = properties.obeliskShotDirection.GetRandom().Split(',');
		string[] pinkPattern = properties.obeliskPinkString.GetRandom().Split(',');
		int angleIndex = Random.Range(0, anglePattern.Length);
		int pinkIndex = Random.Range(0, pinkPattern.Length);
		float angle = 0f;
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, properties.obeliskShootDelay);
			yield return null;
			foreach (FlyingGenieLevelObeliskBlock block in obeliskBlocks)
			{
				float.TryParse(anglePattern[angleIndex], out angle);
				if (pinkPattern[pinkIndex][0] == 'P')
				{
					block.ShootPink(angle);
				}
				else
				{
					block.ShootRegular(angle);
				}
				yield return null;
			}
			angleIndex %= anglePattern.Length;
			pinkIndex %= pinkPattern.Length;
			yield return null;
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	private IEnumerator move_cr()
	{
		isFirst = true;
		YieldInstruction wait = new WaitForFixedUpdate();
		while (base.transform.position.x > -840f)
		{
			base.transform.AddPosition((0f - properties.obeliskMovementSpeed) * (float)CupheadTime.Delta);
			yield return wait;
		}
		isFirst = false;
		End();
		yield return null;
	}

	private void End()
	{
		isOn = false;
		foreach (FlyingGenieLevelObeliskBlock obeliskBlock in obeliskBlocks)
		{
			Object.Destroy(obeliskBlock.gameObject);
		}
		foreach (FlyingGenieLevelGenieHead genieHead in genieHeads)
		{
			if (genieHead != null)
			{
				Object.Destroy(genieHead.gameObject);
			}
		}
		obeliskBlocks.Clear();
		genieHeads.Clear();
		StopAllCoroutines();
		bouncerWall.enabled = false;
	}
}
