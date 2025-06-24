using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DicePalaceFlyingMemoryLevelGameManager : LevelProperties.DicePalaceFlyingMemory.Entity
{
	private static DicePalaceFlyingMemoryLevelGameManager singletonGameManager;

	public DicePalaceFlyingMemoryLevelContactPoint[,] contactPoints;

	public int contactDimX;

	public int contactDimY;

	[SerializeField]
	private Transform cardStopRoot;

	[SerializeField]
	private List<Sprite> chosenFlippedDownCards;

	[SerializeField]
	private DicePalaceFlyingMemoryLevelContactPoint contactPointPrefab;

	[SerializeField]
	private DicePalaceFlyingMemoryLevelStuffedToy stuffedToy;

	[SerializeField]
	private DicePalaceFlyingMemoryLevelCard cardPrefab;

	[SerializeField]
	private DicePalaceFlyingMemoryLevelBot botPrefab;

	private DicePalaceFlyingMemoryLevelCard[,] cards;

	private Vector3 hiddenPosition;

	private int GridDimX = 4;

	private int GridDimY = 3;

	private int patternOrderIndex;

	private int shapeNum;

	private int matchCounter;

	private float maxHP;

	private float space;

	private bool checkForFlipped;

	private bool matchMade;

	private string[] patternOrder;

	public static DicePalaceFlyingMemoryLevelGameManager Instance
	{
		get
		{
			if (singletonGameManager == null)
			{
				GameObject gameObject = new GameObject();
				gameObject.name = "GameManager";
				singletonGameManager = gameObject.AddComponent<DicePalaceFlyingMemoryLevelGameManager>();
			}
			return singletonGameManager;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		hiddenPosition = base.transform.position;
		singletonGameManager = this;
	}

	public override void LevelInit(LevelProperties.DicePalaceFlyingMemory properties)
	{
		base.LevelInit(properties);
		patternOrder = properties.CurrentState.flippyCard.patternOrder.GetRandom().Split(',');
		maxHP = properties.CurrentHealth;
		contactDimX = GridDimX + 1;
		contactDimY = GridDimY + 1;
		GenerateGrid();
	}

	protected override void Update()
	{
		base.Update();
		if (checkForFlipped)
		{
			CheckIfFlipped();
		}
	}

	private void GenerateGrid()
	{
		float x = cardPrefab.GetComponent<Renderer>().bounds.size.x;
		float y = cardPrefab.GetComponent<Renderer>().bounds.size.y;
		float num = x + 10f;
		float num2 = y + 10f;
		cards = new DicePalaceFlyingMemoryLevelCard[GridDimX, GridDimY];
		contactPoints = new DicePalaceFlyingMemoryLevelContactPoint[contactDimX, contactDimY];
		for (int i = 0; i < GridDimY; i++)
		{
			for (int j = 0; j < GridDimX; j++)
			{
				Vector3 vector = new Vector3((float)j * num, (float)(-i) * num2);
				cards[j, i] = Object.Instantiate(cardPrefab);
				cards[j, i].transform.position = vector + base.transform.position;
				cards[j, i].transform.parent = base.transform;
				AssignCards(j, i);
			}
		}
		for (int k = 0; k < contactDimY; k++)
		{
			for (int l = 0; l < contactDimX; l++)
			{
				Vector3 vector2 = new Vector3((float)l * num - num / 2f, (float)(-k) * num2 + num2 / 2f);
				contactPoints[l, k] = Object.Instantiate(contactPointPrefab);
				contactPoints[l, k].transform.position = vector2 + base.transform.position;
				contactPoints[l, k].transform.parent = base.transform;
				contactPoints[l, k].Xcoord = l;
				contactPoints[l, k].Ycoord = k;
			}
		}
		StartCoroutine(start_game_cr());
	}

	private void AssignCards(int x, int y)
	{
		DicePalaceFlyingMemoryLevelCard.Card card = DicePalaceFlyingMemoryLevelCard.Card.Flowers;
		if (patternOrder[patternOrderIndex] == "1A")
		{
			card = DicePalaceFlyingMemoryLevelCard.Card.Cuphead;
		}
		else if (patternOrder[patternOrderIndex] == "1B")
		{
			card = DicePalaceFlyingMemoryLevelCard.Card.Chips;
		}
		else if (patternOrder[patternOrderIndex] == "2A")
		{
			card = DicePalaceFlyingMemoryLevelCard.Card.Flowers;
		}
		else if (patternOrder[patternOrderIndex] == "2B")
		{
			card = DicePalaceFlyingMemoryLevelCard.Card.Shield;
		}
		else if (patternOrder[patternOrderIndex] == "3A")
		{
			card = DicePalaceFlyingMemoryLevelCard.Card.Spindle;
		}
		else if (patternOrder[patternOrderIndex] == "3B")
		{
			card = DicePalaceFlyingMemoryLevelCard.Card.Mugman;
		}
		else
		{
			Debug.Log("Invalid pattern!");
		}
		int index = Random.Range(0, chosenFlippedDownCards.Count);
		cards[x, y].card = card;
		cards[x, y].GetComponent<SpriteRenderer>().sprite = chosenFlippedDownCards[index];
		chosenFlippedDownCards.Remove(chosenFlippedDownCards[index]);
		patternOrderIndex = (patternOrderIndex + 1) % patternOrder.Length;
	}

	private IEnumerator start_game_cr()
	{
		for (int i = 0; i < GridDimY; i++)
		{
			for (int j = 0; j < GridDimX; j++)
			{
				cards[j, i].FlipUp();
			}
		}
		float t = 0f;
		float time = 1.3f;
		Vector2 start = base.transform.position;
		while (t < time)
		{
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, t / time);
			base.transform.position = Vector2.Lerp(start, cardStopRoot.position, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.position = cardStopRoot.position;
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.flippyCard.initialRevealTime);
		for (int k = 0; k < GridDimY; k++)
		{
			for (int l = 0; l < GridDimX; l++)
			{
				cards[l, k].EnableCards();
			}
		}
		checkForFlipped = true;
		if (base.properties.CurrentState.bots.botsOn)
		{
			StartCoroutine(spawning_bots_cr());
		}
		yield return null;
	}

	private IEnumerator slide_cr(Vector3 endPosition)
	{
		float t = 0f;
		float time = 1.3f;
		Vector2 start = base.transform.position;
		while (t < time)
		{
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, t / time);
			base.transform.position = Vector2.Lerp(start, endPosition, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.position = endPosition;
	}

	private void CheckIfFlipped()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < GridDimY; i++)
		{
			for (int j = 0; j < GridDimX; j++)
			{
				if (!cards[j, i].flippedUp || cards[j, i].permanentlyFlipped)
				{
					continue;
				}
				num++;
				cards[j, i].GetComponent<Collider2D>().enabled = false;
				if (num >= 2)
				{
					checkForFlipped = false;
					if (cards[num2, num3].card == cards[j, i].card)
					{
						matchMade = true;
						cards[j, i].permanentlyFlipped = true;
						cards[num2, num3].permanentlyFlipped = true;
					}
					else
					{
						matchMade = false;
					}
					StartCoroutine(disable_all_cards_cr());
					num = 0;
				}
				else
				{
					num2 = j;
					num3 = i;
				}
			}
		}
	}

	private IEnumerator disable_all_cards_cr()
	{
		for (int i = 0; i < GridDimY; i++)
		{
			for (int j = 0; j < GridDimX; j++)
			{
				if (!cards[j, i].permanentlyFlipped)
				{
					cards[j, i].DisableCard();
				}
				else
				{
					cards[j, i].GetComponent<Collider2D>().enabled = false;
				}
			}
		}
		yield return CupheadTime.WaitForSeconds(this, 0.5f);
		StartCoroutine(open_timer_cr());
		yield return null;
	}

	private IEnumerator open_timer_cr()
	{
		int HPToLower = (int)maxHP / (GridDimX * GridDimY / 2);
		if (matchMade)
		{
			StartCoroutine(slide_cr(hiddenPosition));
			matchCounter++;
			stuffedToy.Open();
			while (base.properties.CurrentHealth >= maxHP - (float)(HPToLower * matchCounter))
			{
				yield return null;
			}
		}
		else
		{
			stuffedToy.guessedWrong = true;
			yield return CupheadTime.WaitForSeconds(this, 1f);
		}
		for (int i = 0; i < GridDimY; i++)
		{
			for (int j = 0; j < GridDimX; j++)
			{
				if (!cards[j, i].permanentlyFlipped)
				{
					cards[j, i].EnableCards();
				}
			}
		}
		StartCoroutine(slide_cr(cardStopRoot.position));
		stuffedToy.Closed();
		checkForFlipped = true;
		yield return null;
	}

	private void SpawnBot(int xCoord, int yCoord, bool moveOnY)
	{
		AbstractPlayerController next = PlayerManager.GetNext();
		DicePalaceFlyingMemoryLevelBot dicePalaceFlyingMemoryLevelBot = Object.Instantiate(botPrefab);
		dicePalaceFlyingMemoryLevelBot.Init(base.properties.CurrentState.bots, contactPoints[xCoord, yCoord], moveOnY, base.properties.CurrentState.bots.botsHP, next);
	}

	private IEnumerator spawning_bots_cr()
	{
		LevelProperties.DicePalaceFlyingMemory.Bots p = base.properties.CurrentState.bots;
		string[] spawnPattern = p.spawnOrder.GetRandom().Split(',');
		int number = 0;
		int Xcoord = 0;
		int Ycoord = 0;
		bool Yset = false;
		while (true)
		{
			for (int i = 0; i < spawnPattern.Length; i++)
			{
				string[] spawnLocation = spawnPattern[i].Split(':');
				string[] array = spawnLocation;
				foreach (string text in array)
				{
					if (text[0] == 'U')
					{
						Ycoord = 0;
						Yset = true;
					}
					else if (text[0] == 'D')
					{
						Ycoord = contactDimY - 1;
						Yset = true;
					}
					else if (text[0] == 'L')
					{
						Xcoord = 0;
						Yset = false;
					}
					else if (text[0] == 'R')
					{
						Xcoord = contactDimX - 1;
						Yset = false;
					}
					else
					{
						int.TryParse(text, out number);
					}
				}
				if (Yset)
				{
					Xcoord = number;
				}
				else
				{
					Ycoord = number;
				}
				SpawnBot(Xcoord, Ycoord, Yset);
				yield return CupheadTime.WaitForSeconds(this, p.spawnDelay);
			}
			yield return null;
		}
	}
}
