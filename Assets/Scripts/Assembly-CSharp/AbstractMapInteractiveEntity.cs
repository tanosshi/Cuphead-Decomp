using System;
using System.Collections;
using UnityEngine;

public abstract class AbstractMapInteractiveEntity : MapSprite
{
	public enum Interactor
	{
		Cuphead = 0,
		Mugman = 1,
		Either = 2,
		Both = 3
	}

	protected enum State
	{
		Inactive = 0,
		Ready = 1,
		Activated = 2
	}

	[Serializable]
	public class PositionProperties
	{
		[Header("One Player")]
		public Vector2 singlePlayer = new Vector2(0f, -1f);

		[Header("Two Players")]
		public Vector2 playerOne = new Vector2(-1f, -1f);

		public Vector2 playerTwo = new Vector2(1f, -1f);
	}

	protected const string MapWorld1 = "MapWorld_1";

	protected const string MapWorld2 = "MapWorld_2";

	protected const string MapWorld3 = "MapWorld_3";

	protected const string MapWorld4Exit = "KingDiceToWorld3WorldMap";

	protected const string Inkwell = "Inkwell";

	protected const string Mausoleum = "Mausoleum";

	protected const string Mausoleum1 = "Mausoleum_1";

	protected const string Mausoleum2 = "Mausoleum_2";

	protected const string Mausoleum3 = "Mausoleum_3";

	protected const string Devil = "Devil";

	protected const string DicePalaceMain = "DicePalaceMain";

	protected const string KingDice = "KingDice";

	protected const string Shop = "Shop";

	protected const string ElderKettleLevel = "ElderKettleLevel";

	public Interactor interactor = Interactor.Either;

	public Vector2 interactionPoint;

	public float interactionDistance = 1f;

	public AbstractUIInteractionDialogue.Properties dialogueProperties;

	public Vector2 dialogueOffset;

	public PositionProperties returnPositions;

	public bool playerCanWalkBehind = true;

	[HideInInspector]
	public MapUIInteractionDialogue[] dialogues = new MapUIInteractionDialogue[2];

	private bool lastInteractable;

	private bool lockInput;

	private bool[] showed = new bool[2];

	protected State state { get; private set; }

	protected MapPlayerController playerActivating { get; private set; }

	protected MapPlayerController playerChecking { get; private set; }

	protected override bool ChangesDepth
	{
		get
		{
			return playerCanWalkBehind;
		}
	}

	public event Action OnActivateEvent;

	protected override void Awake()
	{
		base.Awake();
		lockInput = true;
		StartCoroutine(lock_input_cr());
	}

	private IEnumerator lock_input_cr()
	{
		yield return new WaitForSeconds(1f);
		lockInput = false;
		yield return null;
	}

	protected override void Update()
	{
		base.Update();
		if (lockInput || InterruptingPrompt.IsInterrupting())
		{
			return;
		}
		Check();
		if (state == State.Activated || MapConfirmStartUI.Current.CurrentState != AbstractMapSceneStartUI.State.Inactive || MapDifficultySelectStartUI.Current.CurrentState != AbstractMapSceneStartUI.State.Inactive || MapEventNotification.Current.showing)
		{
			return;
		}
		switch (interactor)
		{
		default:
			if (PlayerWithinDistance(0) && Map.Current.players[0].input.actions.GetButtonDown(13))
			{
				Activate(Map.Current.players[0]);
			}
			break;
		case Interactor.Mugman:
			if (PlayerWithinDistance(1) && Map.Current.players[1].input.actions.GetButtonDown(13))
			{
				Activate(Map.Current.players[1]);
			}
			break;
		case Interactor.Either:
			if (PlayerWithinDistance(0) && Map.Current.players[0].input.actions.GetButtonDown(13))
			{
				Activate(Map.Current.players[0]);
			}
			else if (PlayerWithinDistance(1) && Map.Current.players[1].input.actions.GetButtonDown(13))
			{
				Activate(Map.Current.players[1]);
			}
			break;
		case Interactor.Both:
			if (!(Map.Current.players[0] == null) && !(Map.Current.players[1] == null) && PlayerWithinDistance(0) && PlayerWithinDistance(1))
			{
				if (Map.Current.players[0].input.actions.GetButtonDown(13) && Map.Current.players[1].input.actions.GetButton(13))
				{
					Activate(Map.Current.players[0]);
				}
				else if (Map.Current.players[1].input.actions.GetButtonDown(13) && Map.Current.players[0].input.actions.GetButton(13))
				{
					Activate(Map.Current.players[1]);
				}
			}
			break;
		}
	}

	protected MapPlayerController[] PlayersAbleToActivate()
	{
		if (Map.Current.CurrentState != Map.State.Ready)
		{
			return new MapPlayerController[0];
		}
		switch (interactor)
		{
		default:
			playerChecking = Map.Current.players[0];
			if (PlayerWithinDistance(0))
			{
				return new MapPlayerController[1] { playerChecking };
			}
			break;
		case Interactor.Mugman:
			playerChecking = Map.Current.players[1];
			if (PlayerWithinDistance(1))
			{
				return new MapPlayerController[1] { playerChecking };
			}
			break;
		case Interactor.Either:
			playerChecking = Map.Current.players[0];
			if (PlayerWithinDistance(0) && PlayerWithinDistance(1))
			{
				return new MapPlayerController[2]
				{
					Map.Current.players[0],
					Map.Current.players[1]
				};
			}
			if (PlayerWithinDistance(0))
			{
				return new MapPlayerController[1] { Map.Current.players[0] };
			}
			playerChecking = Map.Current.players[1];
			if (PlayerWithinDistance(1))
			{
				return new MapPlayerController[1] { Map.Current.players[1] };
			}
			break;
		case Interactor.Both:
			playerChecking = Map.Current.players[0];
			if (PlayerWithinDistance(0) && PlayerWithinDistance(1))
			{
				return new MapPlayerController[2]
				{
					Map.Current.players[0],
					Map.Current.players[1]
				};
			}
			break;
		}
		return new MapPlayerController[0];
	}

	protected bool AbleToActivate()
	{
		return PlayersAbleToActivate().Length > 0;
	}

	private bool PlayerWithinDistance(int i)
	{
		if (Map.Current.players[i] == null || Map.Current.players[i].state != MapPlayerController.State.Walking)
		{
			return false;
		}
		Vector2 a = (Vector2)base.transform.position + interactionPoint;
		Vector2 b = Map.Current.players[i].transform.position;
		return Vector2.Distance(a, b) <= interactionDistance;
	}

	protected virtual void Check()
	{
		MapPlayerController[] array = PlayersAbleToActivate();
		bool[] array2 = new bool[2];
		showed.CopyTo(array2, 0);
		for (int i = 0; i < showed.Length; i++)
		{
			showed[i] = false;
		}
		for (int j = 0; j < array.Length; j++)
		{
			if ((int)array[j].id < showed.Length)
			{
				showed[(int)array[j].id] = true;
			}
		}
		for (int k = 0; k < Map.Current.players.Length; k++)
		{
			if (Map.Current.players[k] == null)
			{
				continue;
			}
			int id = (int)Map.Current.players[k].id;
			if (array2[id] != showed[id])
			{
				if (showed[id])
				{
					dialogues[id] = Show(Map.Current.players[k].input);
					continue;
				}
				Hide(dialogues[id]);
				dialogues[id] = null;
			}
		}
	}

	protected virtual void ReCheck()
	{
		MapPlayerController[] array = PlayersAbleToActivate();
		bool[] array2 = new bool[2];
		showed.CopyTo(array2, 0);
		for (int i = 0; i < showed.Length; i++)
		{
			showed[i] = false;
		}
		for (int j = 0; j < array.Length; j++)
		{
			if ((int)array[j].id < showed.Length)
			{
				showed[(int)array[j].id] = true;
			}
		}
		for (int k = 0; k < Map.Current.players.Length; k++)
		{
			if (Map.Current.players[k] == null)
			{
				continue;
			}
			int id = (int)Map.Current.players[k].id;
			if (array2[id] != showed[id])
			{
				if (showed[id])
				{
					dialogues[id] = Show(Map.Current.players[k].input);
					continue;
				}
				Hide(dialogues[id]);
				dialogues[id] = null;
			}
		}
	}

	protected virtual void Activate(MapPlayerController player)
	{
		MapUIInteractionDialogue mapUIInteractionDialogue = dialogues[(int)player.id];
		if (!(mapUIInteractionDialogue == null))
		{
			playerActivating = player;
			mapUIInteractionDialogue.Close();
			mapUIInteractionDialogue = null;
			state = State.Activated;
			if (this.OnActivateEvent != null)
			{
				this.OnActivateEvent();
			}
			Activate();
		}
	}

	protected virtual void Activate()
	{
	}

	protected virtual MapUIInteractionDialogue Show(PlayerInput player)
	{
		AudioManager.Play("world_map_level_bubble_appear");
		state = State.Ready;
		return MapUIInteractionDialogue.Create(dialogueProperties, player, dialogueOffset);
	}

	public virtual void Hide(MapUIInteractionDialogue dialogue)
	{
		AudioManager.Play("world_map_level_bubble_disappear");
		if (!(dialogue == null))
		{
			dialogue.Close();
			dialogue = null;
			state = State.Inactive;
		}
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere((Vector2)base.baseTransform.position + dialogueOffset, 0.05f);
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere((Vector2)base.baseTransform.position + dialogueOffset, 0.06f);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere((Vector2)base.baseTransform.position + interactionPoint, 0.05f);
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere((Vector2)base.baseTransform.position + interactionPoint, 0.06f);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere((Vector2)base.baseTransform.position + interactionPoint, interactionDistance);
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere((Vector2)base.baseTransform.position + interactionPoint, interactionDistance + 0.01f);
		Vector3 vector = new Vector3(0.3f, 0.3f, 0.3f);
		Vector3 size = vector * 0.9f;
		Vector3 vector2 = new Vector3(0.25f, 0.25f, 0.25f);
		Vector3 size2 = vector2 * 0.9f;
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube((Vector3)returnPositions.singlePlayer + base.transform.position, vector);
		Gizmos.color = Color.black;
		Gizmos.DrawWireCube((Vector3)returnPositions.playerOne + base.transform.position, vector2);
		Gizmos.DrawWireCube((Vector3)returnPositions.playerTwo + base.transform.position, vector2);
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube((Vector3)returnPositions.singlePlayer + base.transform.position, size);
		Gizmos.DrawWireCube((Vector3)returnPositions.playerOne + base.transform.position, size2);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube((Vector3)returnPositions.playerTwo + base.transform.position, size2);
		Gizmos.color = Color.white;
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		if (Application.isPlaying)
		{
			switch (interactor)
			{
			case Interactor.Cuphead:
				DrawGizmoLineToPlayer(0, PlayerWithinDistance(0));
				break;
			case Interactor.Mugman:
				DrawGizmoLineToPlayer(1, PlayerWithinDistance(1));
				break;
			case Interactor.Either:
				DrawGizmoLineToPlayer(0, PlayerWithinDistance(0));
				DrawGizmoLineToPlayer(1, PlayerWithinDistance(1));
				break;
			case Interactor.Both:
				DrawGizmoLineToPlayer(0, PlayerWithinDistance(0) && PlayerWithinDistance(1));
				DrawGizmoLineToPlayer(1, PlayerWithinDistance(0) && PlayerWithinDistance(1));
				break;
			}
		}
	}

	private void DrawGizmoLineToPlayer(int i, bool valid)
	{
		if (!(Map.Current.players[i] == null))
		{
			Gizmos.color = ((!valid) ? Color.red : Color.green);
			Gizmos.DrawLine((Vector2)base.transform.position + interactionPoint, Map.Current.players[i].transform.position);
		}
	}
}
