using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DicePalaceCardGameManager : AbstractPausableComponent
{
	[SerializeField]
	private DicePalaceCardLevelColumn columnObject;

	[SerializeField]
	private DicePalaceCardLevelBlock hearts;

	[SerializeField]
	private DicePalaceCardLevelBlock spades;

	[SerializeField]
	private DicePalaceCardLevelBlock clubs;

	[SerializeField]
	private DicePalaceCardLevelBlock diamonds;

	[SerializeField]
	private DicePalaceCardLevelGridBlock gridBlockPrefab;

	private List<DicePalaceCardLevelColumn> totalColumns;

	public DicePalaceCardLevelGridBlock[,] gridBlocks;

	private LevelProperties.DicePalaceCard.Blocks properties;

	private float distanceToPlayerY;

	private float amountToDropBy;

	private float startingPos;

	private float targetPos;

	private float currentHeight = -1f;

	public int GridDimX;

	public int GridDimY;

	private float GridSpacing;

	private bool doneDropping;

	private bool checkAgain = true;

	private string[] typePattern;

	private string[] amountPattern;

	private List<int> currentStopYPos;

	private int amountIndex;

	private int typeIndex;

	private DicePalaceCardLevelBlock selectedPrefab;

	private DicePalaceCardLevelBlock.Suit[] suits = new DicePalaceCardLevelBlock.Suit[4]
	{
		DicePalaceCardLevelBlock.Suit.Hearts,
		DicePalaceCardLevelBlock.Suit.Spades,
		DicePalaceCardLevelBlock.Suit.Diamonds,
		DicePalaceCardLevelBlock.Suit.Clubs
	};

	public void GameSetup(LevelProperties.DicePalaceCard cardProperties)
	{
		properties = cardProperties.CurrentState.blocks;
		GridDimX = properties.gridWidth;
		GridDimY = properties.gridHeight;
		SetSize();
		typePattern = properties.cardTypeString.GetRandom().Split(',');
		amountPattern = properties.cardAmountString.GetRandom().Split(',');
		Vector3 position = base.transform.position;
		position.y = 360f - gridBlockPrefab.GetComponent<Renderer>().bounds.size.y;
		position.x = -640f + gridBlockPrefab.GetComponent<Renderer>().bounds.size.x;
		base.transform.position = position;
		selectedPrefab = new DicePalaceCardLevelBlock();
		totalColumns = new List<DicePalaceCardLevelColumn>();
		typeIndex = Random.Range(0, typePattern.Length);
		amountIndex = Random.Range(0, amountPattern.Length);
		GenerateGrid();
		startingPos = base.transform.position.y;
	}

	public IEnumerator start_game_cr()
	{
		while (true)
		{
			SpawnColumn();
			yield return CupheadTime.WaitForSeconds(this, properties.attackDelayRange);
		}
	}

	private void SetSize()
	{
		hearts.transform.SetScale(properties.blockSize, properties.blockSize, properties.blockSize);
		spades.transform.SetScale(properties.blockSize, properties.blockSize, properties.blockSize);
		clubs.transform.SetScale(properties.blockSize, properties.blockSize, properties.blockSize);
		diamonds.transform.SetScale(properties.blockSize, properties.blockSize, properties.blockSize);
		gridBlockPrefab.transform.SetScale(properties.blockSize, properties.blockSize, properties.blockSize);
		GridSpacing = properties.blockSize;
	}

	private void SpawnColumn()
	{
		int num = 1;
		int num2 = -1;
		float num3 = gridBlockPrefab.GetComponent<Renderer>().bounds.size.y / 2f;
		float result = 0f;
		float.TryParse(amountPattern[amountIndex], out result);
		DicePalaceCardLevelColumn item = Object.Instantiate(columnObject);
		totalColumns.Add(item);
		int index = totalColumns.Count - 1;
		Vector3 position = totalColumns[index].transform.position;
		position.x = 640f;
		position.y = 360f - num3;
		totalColumns[index].transform.position = position;
		for (int i = 0; (float)i < result; i++)
		{
			if (typePattern[typeIndex][0] == 'H')
			{
				selectedPrefab = hearts;
			}
			else if (typePattern[typeIndex][0] == 'S')
			{
				selectedPrefab = spades;
			}
			else if (typePattern[typeIndex][0] == 'D')
			{
				selectedPrefab = diamonds;
			}
			else if (typePattern[typeIndex][0] == 'C')
			{
				selectedPrefab = clubs;
			}
			typeIndex = (typeIndex + 1) % typePattern.Length;
			DicePalaceCardLevelBlock dicePalaceCardLevelBlock = Object.Instantiate(selectedPrefab);
			totalColumns[index].blockPieces.Add(dicePalaceCardLevelBlock);
			dicePalaceCardLevelBlock.transform.parent = totalColumns[index].transform;
			Vector3 position2 = totalColumns[index].blockPieces[i].transform.position;
			if (i % 2 == 0 && i != 0)
			{
				totalColumns[index].blockPieces[i].stopOffsetX = num2;
				position2.x = 640f - totalColumns[index].blockPieces[i].GetComponent<Renderer>().bounds.size.x * (float)Mathf.Abs(totalColumns[index].blockPieces[i].stopOffsetX);
				num2--;
			}
			else if (i % 2 == 1 && i != 0)
			{
				totalColumns[index].blockPieces[i].stopOffsetX = num;
				position2.x = 640f + totalColumns[index].blockPieces[i].GetComponent<Renderer>().bounds.size.x * (float)Mathf.Abs(totalColumns[index].blockPieces[i].stopOffsetX);
				num++;
			}
			else
			{
				position2.x = 640f;
			}
			position2.y = 360f - num3;
			totalColumns[index].blockPieces[i].transform.position = position2;
		}
		amountIndex = (amountIndex + 1) % amountPattern.Length;
		StartCoroutine(horizontal_moving_column(totalColumns[index]));
	}

	private IEnumerator horizontal_moving_column(DicePalaceCardLevelColumn currentColumn)
	{
		AbstractPlayerController player = PlayerManager.GetNext();
		float offset = gridBlocks[1, 0].transform.position.x - gridBlocks[0, 0].transform.position.x;
		int playerXPos = 0;
		int stopXPos = 0;
		bool selectStop = false;
		float dist = 0f;
		float distOffset = 20f;
		while (currentColumn.transform.position.x != gridBlocks[stopXPos, 0].transform.position.x)
		{
			if (player == null || player.IsDead)
			{
				player = PlayerManager.GetNext();
			}
			for (int i = 0; i < GridDimX - 1; i++)
			{
				if (player.transform.position.x > gridBlocks[i, 0].transform.position.x - offset / 2f)
				{
					if (player.transform.position.x < gridBlocks[i + 1, 0].transform.position.x - offset / 2f)
					{
						playerXPos = i;
					}
					else if (i + 1 == GridDimX - 1)
					{
						playerXPos = i + 1;
					}
				}
			}
			dist = gridBlocks[playerXPos, 0].transform.position.x - currentColumn.transform.position.x;
			Vector3 pos = currentColumn.transform.position;
			if (dist < distOffset)
			{
				selectStop = true;
			}
			int overFlow = GridDimX - playerXPos - currentColumn.blockPieces.Count;
			if (selectStop)
			{
				if (gridBlocks[1, 0].transform.position.x > player.transform.position.x && currentColumn.blockPieces.Count >= 1)
				{
					int value = ((currentColumn.blockPieces.Count % 2 != 1) ? currentColumn.blockPieces[currentColumn.blockPieces.Count - 1].stopOffsetX : (currentColumn.blockPieces[currentColumn.blockPieces.Count - 1].stopOffsetX - 1));
					stopXPos = Mathf.Abs(value) - 1;
					selectStop = false;
				}
				else if (gridBlocks[GridDimX - 1, 0].transform.position.x < player.transform.position.x || Mathf.Sign(overFlow) == -1f)
				{
					stopXPos = GridDimX - 1 - Mathf.Abs(currentColumn.blockPieces[currentColumn.blockPieces.Count - 1].stopOffsetX);
					selectStop = false;
				}
				else
				{
					stopXPos = playerXPos;
					selectStop = false;
				}
			}
			pos.x = Mathf.MoveTowards(currentColumn.transform.position.x, gridBlocks[stopXPos, GridDimY - 1].transform.position.x, properties.blockSpeed * (float)CupheadTime.Delta);
			currentColumn.transform.position = pos;
			yield return null;
		}
		StartCoroutine(vertical_moving_column(currentColumn, stopXPos));
		yield return null;
	}

	private IEnumerator vertical_moving_column(DicePalaceCardLevelColumn currentColumn, int stopXPos)
	{
		currentColumn.blockCounter = 0;
		currentColumn.blockXPos = new int[currentColumn.blockPieces.Count];
		currentColumn.columnStopYPos = new int[currentColumn.blockPieces.Count];
		for (int i = 0; i < currentColumn.blockPieces.Count; i++)
		{
			currentColumn.blockXPos[i] = stopXPos + currentColumn.blockPieces[i].stopOffsetX;
			for (int num = GridDimY - 1; num >= 0; num--)
			{
				if (!gridBlocks[currentColumn.blockXPos[i], num].hasBlock)
				{
					if (num > 0)
					{
						if (gridBlocks[currentColumn.blockXPos[i], num - 1].hasBlock)
						{
							currentColumn.columnStopYPos[i] = num;
							gridBlocks[currentColumn.blockXPos[i], num].hasBlock = true;
						}
					}
					else
					{
						currentColumn.columnStopYPos[i] = 0;
						gridBlocks[currentColumn.blockXPos[i], 0].hasBlock = true;
					}
				}
			}
			StartCoroutine(drop_block_cr(currentColumn, currentColumn.columnStopYPos[i], i));
		}
		while (currentColumn.blockCounter < currentColumn.blockPieces.Count)
		{
			yield return null;
		}
		currentColumn.blockPieces.Clear();
		currentColumn.transform.DetachChildren();
		Object.Destroy(currentColumn.gameObject);
		doneDropping = false;
		checkAgain = false;
		StartCoroutine(check_to_drop_blocks());
		while (!doneDropping)
		{
			yield return null;
		}
		CheckFullGrid();
		ScaleCheck();
		CheckForTop();
		StartCoroutine(check_to_drop_blocks());
		CheckFullGrid();
		ScaleCheck();
	}

	private IEnumerator drop_block_cr(DicePalaceCardLevelColumn currentColumn, int indexToDropTo, int blockToDrop)
	{
		while (currentColumn.blockPieces[blockToDrop].transform.position.y > gridBlocks[currentColumn.blockXPos[blockToDrop], indexToDropTo].transform.position.y)
		{
			Vector3 pos = currentColumn.blockPieces[blockToDrop].transform.position;
			pos.y = Mathf.MoveTowards(currentColumn.blockPieces[blockToDrop].transform.position.y, gridBlocks[currentColumn.blockXPos[blockToDrop], indexToDropTo].transform.position.y, properties.blockDropSpeed * (float)CupheadTime.Delta);
			currentColumn.blockPieces[blockToDrop].transform.position = pos;
			yield return null;
		}
		currentColumn.blockPieces[blockToDrop].transform.parent = base.transform;
		gridBlocks[currentColumn.blockXPos[blockToDrop], indexToDropTo].blockHeld = currentColumn.blockPieces[blockToDrop];
		currentColumn.blockCounter++;
	}

	private IEnumerator check_to_drop_blocks()
	{
		for (int i = 0; i < GridDimX; i++)
		{
			for (int num = GridDimY - 1; num >= 0; num--)
			{
				if (gridBlocks[i, num].hasBlock && gridBlocks[i, num].Ycoordinate > 0f)
				{
					int num2 = num - 1;
					int num3 = num + 1;
					DicePalaceCardLevelBlock blockHeld = gridBlocks[i, num].blockHeld;
					if (!gridBlocks[i, num2].hasBlock)
					{
						checkAgain = true;
						CheckFullGrid();
						StartCoroutine(drop_current_cr(i, num, num2, blockHeld));
						if (gridBlocks[i, num3].hasBlock && gridBlocks[i, num3].Ycoordinate < (float)GridDimY)
						{
							StartCoroutine(drop_current_cr(i, num3, num, gridBlocks[i, num3].blockHeld));
						}
					}
					else
					{
						checkAgain = false;
					}
				}
			}
		}
		if (!checkAgain)
		{
			doneDropping = true;
		}
		yield return null;
	}

	private IEnumerator drop_current_cr(int x, int y, int spaceBelow, DicePalaceCardLevelBlock block)
	{
		if (gridBlocks[x, y].blockHeld != null && !gridBlocks[x, spaceBelow].hasBlock)
		{
			if (y >= 0 && gridBlocks[x, y].hasBlock)
			{
				gridBlocks[x, y].hasBlock = false;
				gridBlocks[x, spaceBelow].hasBlock = true;
			}
			while (gridBlocks[x, y].blockHeld.transform.position.y != gridBlocks[x, spaceBelow].transform.position.y)
			{
				Vector3 pos = gridBlocks[x, y].blockHeld.transform.position;
				pos.y = Mathf.MoveTowards(gridBlocks[x, y].blockHeld.transform.position.y, gridBlocks[x, spaceBelow].transform.position.y, properties.blockDropSpeed * (float)CupheadTime.Delta);
				gridBlocks[x, y].blockHeld.transform.position = pos;
				yield return null;
			}
			gridBlocks[x, y].blockHeld = null;
			gridBlocks[x, spaceBelow].blockHeld = block;
			CheckFullGrid();
			StartCoroutine(check_to_drop_blocks());
			ScaleCheck();
		}
	}

	public void GenerateGrid()
	{
		gridBlocks = new DicePalaceCardLevelGridBlock[GridDimX, GridDimY];
		for (int i = 0; i < GridDimX; i++)
		{
			for (int j = 0; j < GridDimY; j++)
			{
				Vector3 vector = new Vector3((float)i * GridSpacing, (float)j * GridSpacing);
				gridBlocks[i, j] = Object.Instantiate(gridBlockPrefab);
				gridBlocks[i, j].transform.position = vector + base.transform.position;
				gridBlocks[i, j].transform.parent = base.transform;
				gridBlocks[i, j].Xcoordinate = i;
				gridBlocks[i, j].Ycoordinate = j;
			}
		}
	}

	private void CheckFullGrid()
	{
		for (int i = 0; i < GridDimX; i++)
		{
			for (int j = 0; j < GridDimY; j++)
			{
				if (gridBlocks[i, j].blockHeld != null)
				{
					if (gridBlocks[i, j].Xcoordinate < (float)(GridDimX - 2) && gridBlocks[i, j].Ycoordinate < (float)(GridDimY - 2) && gridBlocks[i, j].hasBlock)
					{
						DiagonalsUpCheck(i, j, gridBlocks[i, j].blockHeld.suit);
					}
					if (gridBlocks[i, j].Xcoordinate < (float)(GridDimX - 2) && gridBlocks[i, j].Ycoordinate >= 2f && gridBlocks[i, j].hasBlock)
					{
						DiagonalsDownCheck(i, j, gridBlocks[i, j].blockHeld.suit);
					}
					if (gridBlocks[i, j].Xcoordinate < (float)(GridDimX - 2) && gridBlocks[i, j].hasBlock)
					{
						RowsCheck(i, j, gridBlocks[i, j].blockHeld.suit);
					}
					if (gridBlocks[i, j].Ycoordinate < (float)(GridDimY - 2) && gridBlocks[i, j].hasBlock)
					{
						ColumnsCheck(i, j, gridBlocks[i, j].blockHeld.suit, true);
					}
				}
			}
		}
	}

	private void RowsCheck(int x, int y, DicePalaceCardLevelBlock.Suit suit)
	{
		int num = x + 1;
		int num2 = num + 1;
		int i = num2 + 1;
		if (!(gridBlocks[num, y].blockHeld != null) || !(gridBlocks[num2, y].blockHeld != null) || gridBlocks[num, y].blockHeld.suit != suit || gridBlocks[num2, y].blockHeld.suit != suit)
		{
			return;
		}
		DeleteBlock(gridBlocks[x, y]);
		DeleteBlock(gridBlocks[num, y]);
		DeleteBlock(gridBlocks[num2, y]);
		if (y < GridDimY - 2)
		{
			ColumnsCheck(x, y, suit, true);
			ColumnsCheck(num, y, suit, true);
			ColumnsCheck(num2, y, suit, true);
		}
		if (y >= 2)
		{
			ColumnsCheck(x, y, suit, false);
			ColumnsCheck(num, y, suit, false);
			ColumnsCheck(num2, y, suit, false);
		}
		if (num2 < GridDimX - 2)
		{
			DiagonalsUpCheck(x, y, suit);
			DiagonalsUpCheck(num, y, suit);
			DiagonalsUpCheck(num2, y, suit);
		}
		if (y >= 2 && x < GridDimX - 2)
		{
			DiagonalsDownCheck(x, y, suit);
			DiagonalsDownCheck(num, y, suit);
			DiagonalsDownCheck(num2, y, suit);
		}
		for (; i <= GridDimX - 1 && gridBlocks[i, y].hasBlock && gridBlocks[i, y].blockHeld.suit == suit; i++)
		{
			DeleteBlock(gridBlocks[i, y]);
			if (i >= GridDimX)
			{
				break;
			}
		}
	}

	private void ColumnsCheck(int x, int y, DicePalaceCardLevelBlock.Suit suit, bool checkingUp)
	{
		int num = 0;
		int num2 = 0;
		int num3 = y + 1;
		int num4 = num3 + 1;
		int num5 = y - 1;
		int num6 = y - 2;
		if (checkingUp)
		{
			num = num3;
			num2 = num4;
		}
		else
		{
			num = num5;
			num2 = num6;
		}
		if (!(gridBlocks[x, num].blockHeld != null) || !(gridBlocks[x, num2].blockHeld != null) || gridBlocks[x, num].blockHeld.suit != suit || gridBlocks[x, num2].blockHeld.suit != suit)
		{
			return;
		}
		DeleteBlock(gridBlocks[x, y]);
		DeleteBlock(gridBlocks[x, num]);
		DeleteBlock(gridBlocks[x, num2]);
		if (x < GridDimX - 2)
		{
			if (y >= 2)
			{
				DiagonalsDownCheck(x, y, suit);
			}
			if (num3 >= 2)
			{
				DiagonalsDownCheck(x, num3, suit);
			}
			if (num4 >= 2)
			{
				DiagonalsDownCheck(x, num4, suit);
			}
			RowsCheck(x, y, suit);
			RowsCheck(x, num3, suit);
			RowsCheck(x, num4, suit);
			if (num4 < GridDimY - 2)
			{
				DiagonalsUpCheck(x, y, suit);
				DiagonalsUpCheck(x, num3, suit);
				DiagonalsUpCheck(x, num4, suit);
			}
		}
		ExtraCheck(x, y, checkingUp, suit);
	}

	private void DiagonalsUpCheck(int x, int y, DicePalaceCardLevelBlock.Suit suit)
	{
		int num = x + 1;
		int num2 = num + 1;
		int num3 = num2 + 1;
		int num4 = y + 1;
		int num5 = num4 + 1;
		int num6 = num5 + 1;
		if (!(gridBlocks[num, num4].blockHeld != null) || !(gridBlocks[num2, num5].blockHeld != null) || gridBlocks[num, num4].blockHeld.suit != suit || gridBlocks[num2, num5].blockHeld.suit != suit)
		{
			return;
		}
		DeleteBlock(gridBlocks[x, y]);
		DeleteBlock(gridBlocks[num, num4]);
		DeleteBlock(gridBlocks[num2, num5]);
		if (num2 < GridDimX - 2)
		{
			RowsCheck(x, y, suit);
			RowsCheck(num, num4, suit);
			RowsCheck(num2, num5, suit);
		}
		if (y >= 2)
		{
			ColumnsCheck(x, y, suit, false);
		}
		if (num4 >= 2)
		{
			ColumnsCheck(num, num4, suit, false);
		}
		ColumnsCheck(num2, num5, suit, false);
		if (num5 >= 2)
		{
		}
		while (num3 <= GridDimX - 1 && num6 <= GridDimY - 1 && gridBlocks[num3, num6].hasBlock && gridBlocks[num3, num6].blockHeld.suit == suit)
		{
			DeleteBlock(gridBlocks[num3, num6]);
			if (num3 < GridDimX && num6 < GridDimY)
			{
				num3++;
				num6++;
				continue;
			}
			break;
		}
	}

	private void DiagonalsDownCheck(int x, int y, DicePalaceCardLevelBlock.Suit suit)
	{
		int num = x + 1;
		int num2 = num + 1;
		int num3 = num2 + 1;
		int num4 = y - 1;
		int num5 = num4 - 1;
		int num6 = num5 - 1;
		if (!(gridBlocks[num, num4].blockHeld != null) || !(gridBlocks[num2, num5].blockHeld != null) || gridBlocks[num, num4].blockHeld.suit != suit || gridBlocks[num2, num5].blockHeld.suit != suit)
		{
			return;
		}
		DeleteBlock(gridBlocks[x, y]);
		DeleteBlock(gridBlocks[num, num4]);
		DeleteBlock(gridBlocks[num2, num5]);
		if (num2 < GridDimX - 2)
		{
			RowsCheck(x, y, suit);
			RowsCheck(num, num4, suit);
			RowsCheck(num2, num5, suit);
		}
		if (num4 >= 2)
		{
			ColumnsCheck(num, num4, suit, false);
		}
		if (num5 >= 2)
		{
			ColumnsCheck(num2, num5, suit, false);
		}
		ColumnsCheck(x, y, suit, false);
		while (num3 <= GridDimX - 1 && num6 >= 1 && gridBlocks[num3, num6].hasBlock && gridBlocks[num3, num6].blockHeld.suit == suit)
		{
			DeleteBlock(gridBlocks[num3, num6]);
			if (num3 < GridDimX && num6 > 1)
			{
				num3++;
				num6--;
				continue;
			}
			break;
		}
	}

	private void ExtraCheck(int x, int y, bool checkingUp, DicePalaceCardLevelBlock.Suit suit)
	{
		int num = 0;
		int num2 = 0;
		if (checkingUp)
		{
			num = y + 1;
			num2 = 1;
		}
		else
		{
			num = y - 1;
			num2 = -1;
		}
		for (; num <= GridDimY - 1 && gridBlocks[x, num].hasBlock && gridBlocks[x, num].blockHeld.suit == suit; num += num2)
		{
			DeleteBlock(gridBlocks[x, num]);
			if (num >= GridDimY)
			{
				break;
			}
		}
	}

	private void ScaleCheck()
	{
		Vector3 position = base.transform.position;
		int num = 0;
		float y = 0f;
		bool flag = false;
		for (int i = 0; i < GridDimY; i++)
		{
			for (int j = 0; j < GridDimX; j++)
			{
				if (gridBlocks[j, i].blockHeld != null && (float)i != currentHeight)
				{
					y = i;
					flag = true;
					currentHeight = i;
				}
			}
			while (num < GridDimX - 1 && gridBlocks[num, i].blockHeld != null)
			{
				num++;
				if (num == GridDimX - 1)
				{
					y = i;
					flag = true;
				}
			}
		}
		if (flag)
		{
			StartCoroutine(move_scale_cr(y));
			flag = false;
		}
		base.transform.position = position;
	}

	private IEnumerator move_scale_cr(float y)
	{
		Vector3 pos = base.transform.position;
		float speed = 200f;
		targetPos = startingPos - gridBlockPrefab.GetComponent<Renderer>().bounds.size.y * (y + 1f);
		while (base.transform.position.y != targetPos)
		{
			pos.y = Mathf.MoveTowards(base.transform.position.y, targetPos, speed * (float)CupheadTime.Delta);
			base.transform.position = pos;
			yield return null;
		}
		yield return null;
	}

	private void CheckForTop()
	{
		for (int i = 0; i < GridDimX; i++)
		{
			if (gridBlocks[i, GridDimY - 1].hasBlock)
			{
				KillAllBlocks();
			}
		}
		checkAgain = false;
	}

	private void KillAllBlocks()
	{
		for (int i = 0; i < GridDimX; i++)
		{
			for (int j = 0; j < GridDimY; j++)
			{
				if (gridBlocks[i, j].hasBlock)
				{
					DeleteBlock(gridBlocks[i, j]);
				}
			}
		}
		targetPos = startingPos;
		Vector3 position = base.transform.position;
		position.y = startingPos;
		base.transform.position = position;
	}

	private void DeleteBlock(DicePalaceCardLevelGridBlock gridBlock)
	{
		if (gridBlock.blockHeld != null)
		{
			gridBlock.blockHeld.DestroyBlock();
			gridBlock.blockHeld = null;
			gridBlock.hasBlock = false;
		}
	}
}
