using System;
using System.Collections;
using UnityEngine;

public class TrainLevelSkeleton : LevelProperties.Train.Entity
{
	public enum Position
	{
		Right = 0,
		Center = 1,
		Left = 2
	}

	public delegate void OnDamageTakenHandler(float damage);

	[SerializeField]
	private TrainLevelSkeletonHead head;

	[SerializeField]
	private TrainLevelSkeletonHand leftHand;

	[SerializeField]
	private TrainLevelSkeletonHand rightHand;

	private DamageReceiver damageReceiver;

	private float health;

	private bool dead;

	private Position currentPosition;

	public event OnDamageTakenHandler OnDamageTakenEvent;

	public event Action OnDeathEvent;

	protected override void Awake()
	{
		base.Awake();
		damageReceiver = head.GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (!dead)
		{
			if (this.OnDamageTakenEvent != null)
			{
				this.OnDamageTakenEvent(info.damage);
			}
			health -= info.damage;
			if (health <= 0f)
			{
				Die();
			}
		}
	}

	private void Die()
	{
		if (!dead)
		{
			dead = true;
			StopAllCoroutines();
			StartCoroutine(die_cr());
		}
	}

	public override void LevelInit(LevelProperties.Train properties)
	{
		base.LevelInit(properties);
		health = properties.CurrentState.skeleton.health;
	}

	public void StartSkeleton()
	{
		AudioManager.Play("train_passenger_car_explode");
		emitAudioFromObject.Add("train_passenger_car_explode");
		StartCoroutine(loop_cr());
	}

	private void In()
	{
		AudioManager.Play(Sfx.Level_Train_Skeleton_Up);
		head.In();
		leftHand.In();
		rightHand.In();
	}

	private void Out()
	{
		AudioManager.Play("train_skeleton_hand_out");
		emitAudioFromObject.Add("train_skeleton_hand_out");
		head.Out();
		leftHand.Out();
		rightHand.Out();
	}

	private void RandomizeLocations()
	{
		Position position;
		for (position = currentPosition; position == currentPosition; position = (Position)UnityEngine.Random.Range(0, 3))
		{
		}
		currentPosition = position;
		head.SetPosition(position);
		switch (position)
		{
		default:
			leftHand.SetPosition(Position.Center);
			rightHand.SetPosition(Position.Right);
			break;
		case Position.Center:
			leftHand.SetPosition(Position.Left);
			rightHand.SetPosition(Position.Right);
			break;
		case Position.Right:
			leftHand.SetPosition(Position.Left);
			rightHand.SetPosition(Position.Center);
			break;
		}
	}

	private IEnumerator loop_cr()
	{
		currentPosition = Position.Center;
		float attackDelay = 0f;
		Animator handAnimator = rightHand.GetComponent<Animator>();
		while (true)
		{
			attackDelay = Mathf.Lerp(base.properties.CurrentState.skeleton.attackDelay.max, base.properties.CurrentState.skeleton.attackDelay.min, health / base.properties.CurrentState.skeleton.health);
			In();
			yield return handAnimator.WaitForAnimationToEnd(this, "In");
			yield return CupheadTime.WaitForSeconds(this, attackDelay);
			AudioManager.Play("train_skeleton_hand_slap");
			leftHand.Slap();
			rightHand.Slap();
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.skeleton.slapHoldTime);
			Out();
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.skeleton.appearDelay);
			RandomizeLocations();
		}
	}

	private IEnumerator die_cr()
	{
		Animator handAnimator = rightHand.GetComponent<Animator>();
		head.Die();
		rightHand.Die();
		leftHand.Die();
		AudioManager.Play("train_skeleton_hand_death");
		emitAudioFromObject.Add("train_skeleton_hand_death");
		yield return handAnimator.WaitForAnimationToEnd(this, "Death");
		head.EndDeath();
		rightHand.EndDeath();
		leftHand.EndDeath();
		yield return CupheadTime.WaitForSeconds(this, 1f);
		if (this.OnDeathEvent != null)
		{
			this.OnDeathEvent();
		}
	}
}
