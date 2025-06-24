using System;
using System.Collections;
using UnityEngine;

public class RobotLevelRobot : LevelProperties.Robot.Entity
{
	private Action attackCallback;

	[SerializeField]
	private Effect headcannonSmoke;

	[SerializeField]
	private Transform[] walkingPositions;

	[SerializeField]
	private RobotLevelRobotHead head;

	[SerializeField]
	private RobotLevelRobotBodyPart chest;

	[SerializeField]
	private RobotLevelRobotHatch hatch;

	[Space(10f)]
	[SerializeField]
	private GameObject finalForm;

	private bool introEnded;

	private float walkPCT;

	private float walkTime;

	private int remainingPrimaryAttacks = 3;

	private DamageDealer damageDealer;

	[SerializeField]
	private CollisionChild[] collisionChilds;

	public event Action OnDeathEvent;

	public event Action OnPrimaryDeathEvent;

	public event Action OnSecondaryDeathEvent;

	public event Action OnIntroEvent;

	private event Action callback;

	protected override void Awake()
	{
		CollisionChild[] array = collisionChilds;
		foreach (CollisionChild s in array)
		{
			RegisterCollisionChild(s);
		}
		base.Awake();
	}

	public override void LevelInit(LevelProperties.Robot properties)
	{
		Level.Current.OnIntroEvent += OnIntro;
		if (Level.Current.mode == Level.Mode.Easy)
		{
			Level.Current.OnWinEvent += OnDeathDance;
		}
		damageDealer = DamageDealer.NewEnemy();
		walkPCT = (walkTime = 0f);
		StartCoroutine(disableIntro_cr());
		base.LevelInit(properties);
	}

	private IEnumerator disableIntro_cr()
	{
		yield return new WaitForEndOfFrame();
		base.animator.enabled = false;
	}

	protected override void Update()
	{
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
		if (introEnded)
		{
			float num = Mathf.Max(PlayerManager.GetNext().center.x, PlayerManager.GetNext().center.x);
			if (num > base.transform.position.x)
			{
				UpdatePosition(true);
			}
			else
			{
				UpdatePosition(false);
			}
		}
		base.Update();
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}

	private void IntroEnded()
	{
		AudioManager.Play("robot_vocals_laugh");
		emitAudioFromObject.Add("robot_vocals_laugh");
		introEnded = true;
		base.animator.SetBool("MainAnimationActive", false);
		head.GetComponent<RobotLevelRobotHead>().InitBodyPart(this, base.properties, 1);
		chest.GetComponent<RobotLevelRobotChest>().InitBodyPart(this, base.properties);
		hatch.GetComponent<RobotLevelRobotHatch>().InitBodyPart(this, base.properties);
	}

	public void TriggerPhaseTwo(Action callback)
	{
		base.animator.Play("Phase2 Transition", 2);
		this.callback = callback;
	}

	private void OnDeathDance()
	{
		chest.animator.Play("Off", 1);
		base.animator.Play("Death Dance");
		StartCoroutine(death_cr());
	}

	private IEnumerator death_cr()
	{
		yield return new WaitForEndOfFrame();
		for (int i = 0; i < 3; i++)
		{
			base.transform.GetChild(i).gameObject.SetActive(false);
		}
		if (this.OnDeathEvent != null)
		{
			this.OnDeathEvent();
		}
		if (Level.Current.mode != Level.Mode.Easy && this.callback != null)
		{
			this.callback();
		}
	}

	private void OnRobotIntro()
	{
		chest.GetComponent<RobotLevelRobotChest>().InitAnims();
		hatch.GetComponent<RobotLevelRobotHatch>().InitAnims();
	}

	private void OnIntro()
	{
		SoundRobotIntro();
		base.animator.enabled = true;
	}

	public void PrimaryDied()
	{
		if (this.OnPrimaryDeathEvent != null)
		{
			this.OnPrimaryDeathEvent();
		}
		remainingPrimaryAttacks--;
		if (remainingPrimaryAttacks <= 0 && this.OnSecondaryDeathEvent != null)
		{
			this.OnSecondaryDeathEvent();
		}
	}

	private void UpdatePosition(bool closeGap)
	{
		float num = 4f;
		float levelTime = Level.Current.LevelTime;
		float num2 = Level.Current.LevelTime + num;
		if (closeGap)
		{
			Vector3 position = walkingPositions[0].position;
			Vector3 position2 = walkingPositions[1].position;
			Move(position, position2, num, 1);
		}
		else
		{
			Vector3 position = walkingPositions[1].position;
			Vector3 position2 = walkingPositions[0].position;
			Move(position, position2, num, -1);
		}
	}

	private void Move(Vector3 startPosition, Vector3 endPosition, float duration, int direction)
	{
		walkTime += (float)CupheadTime.Delta * (float)direction;
		if (direction < 0)
		{
			if (walkTime <= 0f)
			{
				walkTime = 0f;
			}
		}
		else if (walkTime >= duration)
		{
			walkTime = duration;
		}
		walkPCT = walkTime / duration;
		if (walkPCT >= 1f)
		{
			walkPCT = 1f;
		}
		if (direction < 0)
		{
			walkPCT = 1f - walkPCT;
		}
		base.transform.position = startPosition + (endPosition - startPosition) * walkPCT;
	}

	private void SpawnSmoke()
	{
		headcannonSmoke.Create(head.transform.position);
	}

	private void OnDeathSFX()
	{
		AudioManager.Play("robot_vocals_dying");
		emitAudioFromObject.Add("robot_vocals_dying");
	}

	private void SoundRobotIntro()
	{
		AudioManager.Play("robot_intro");
		emitAudioFromObject.Add("robot_intro");
	}
}
