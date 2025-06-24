using System.Collections;
using UnityEngine;

public class DicePalaceMainLevelKingDice : LevelProperties.DicePalaceMain.Entity
{
	[SerializeField]
	private Transform rightRoot;

	[SerializeField]
	private Transform leftRoot;

	[SerializeField]
	private DicePalaceMainLevelCard cardRegular;

	[SerializeField]
	private DicePalaceMainLevelCard cardPink;

	private DamageReceiver damageReceiver;

	private new Animator animator;

	private bool CuriousSFXActive;

	protected override void Start()
	{
		base.Start();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		damageReceiver.enabled = false;
		GetComponent<Collider2D>().enabled = false;
		animator = GetComponent<Animator>();
		Level.Current.OnWinEvent += OnDeath;
		AudioManager.Play("king_dice_intro");
		emitAudioFromObject.Add("king_dice_intro");
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
	}

	public void StartKingDiceBattle()
	{
		AudioManager.FadeBGMVolume(0f, 0.5f, true);
		AudioManager.Play("king_dice_trans");
		AudioManager.PlayBGMPlaylistManually(false);
		animator.SetBool("IsAttacking", true);
		animator.SetBool("IsBattling", true);
		LevelIntroAnimation levelIntroAnimation = LevelIntroAnimation.Create(null);
		levelIntroAnimation.Play();
		StartCoroutine(cards_cr());
	}

	private void RevealSFX()
	{
		AudioManager.Play("king_dice_reveal");
		emitAudioFromObject.Add("king_dice_reveal");
	}

	public void RevealDice()
	{
		DicePalaceMainLevel dicePalaceMainLevel = Level.Current as DicePalaceMainLevel;
		dicePalaceMainLevel.GameManager.RevealDice();
	}

	private IEnumerator cards_cr()
	{
		LevelProperties.DicePalaceMain.Cards p = base.properties.CurrentState.cards;
		int cardIndex = Random.Range(0, p.cardString.Length);
		string[] sideString = p.cardSideOrder.GetRandom().Split(',');
		int suitIndex = Random.Range(0, 3);
		int sideIndex = Random.Range(0, sideString.Length);
		bool onLeft = false;
		Vector3 rootPos = Vector3.zero;
		damageReceiver.enabled = true;
		GetComponent<Collider2D>().enabled = true;
		while (true)
		{
			string[] cardString = p.cardString[cardIndex].Split(',');
			if (sideString[sideIndex][0] == 'L')
			{
				onLeft = true;
				rootPos = leftRoot.transform.position;
			}
			else if (sideString[sideIndex][0] == 'R')
			{
				onLeft = false;
				rootPos = rightRoot.transform.position;
			}
			else
			{
				Debug.LogError("Invalid pattern string");
			}
			animator.SetBool("OnLeftAttack", onLeft);
			yield return animator.WaitForAnimationToEnd(this, (!onLeft) ? "Attack_Right" : "Attack_Left");
			AudioManager.PlayLoop("king_dice_march_loop");
			emitAudioFromObject.Add("king_dice_march_loop");
			StartCoroutine(kd_laugh_cr());
			for (int i = 0; i < cardString.Length; i++)
			{
				if (cardString[i][0] == 'R')
				{
					DicePalaceMainLevelCard dicePalaceMainLevelCard = cardRegular.Create(rootPos, p, onLeft, p.cardScale, suitIndex);
					dicePalaceMainLevelCard.transform.SetScale(onLeft ? 1 : (-1));
					dicePalaceMainLevelCard.GetComponent<SpriteRenderer>().sortingOrder = i;
					suitIndex = (suitIndex + 1) % 3;
				}
				else if (cardString[i][0] == 'P')
				{
					DicePalaceMainLevelCard dicePalaceMainLevelCard2 = cardPink.Create(rootPos, p, onLeft, p.cardScale, suitIndex);
					dicePalaceMainLevelCard2.transform.SetScale(onLeft ? 1 : (-1));
					dicePalaceMainLevelCard2.GetComponent<SpriteRenderer>().sortingOrder = i;
				}
				else
				{
					Debug.LogError("Invalid pattern string");
				}
				yield return CupheadTime.WaitForSeconds(this, p.cardDelay);
			}
			AudioManager.Stop("king_dice_march_loop");
			animator.SetBool("IsAttacking", false);
			yield return CupheadTime.WaitForSeconds(this, p.hesitate);
			animator.SetBool("IsAttacking", true);
			sideIndex = (sideIndex + 1) % sideString.Length;
			cardIndex = (cardIndex + 1) % p.cardString.Length;
			yield return null;
		}
	}

	private void AttackSFX()
	{
		AudioManager.PlayLoop("king_dice_attack");
		emitAudioFromObject.Add("king_dice_attack");
	}

	private void IntroSFX()
	{
		AudioManager.Play("king_dice_intro");
		emitAudioFromObject.Add("king_dice_intro");
	}

	private void VoxCurious()
	{
		AudioManager.Play("vox_curious");
		emitAudioFromObject.Add("vox_curious");
	}

	private void AttackSFXStop()
	{
		AudioManager.Stop("king_dice_attack");
	}

	private void OnDeath()
	{
		AudioManager.PlayLoop("king_dice_death");
		AudioManager.Play("vox_death");
		emitAudioFromObject.Add("vox_death");
		animator.SetTrigger("OnDeath");
		StopAllCoroutines();
		SpriteRenderer component = GetComponent<SpriteRenderer>();
		component.sortingLayerName = "Background";
		component.sortingOrder = 100;
		GetComponent<Collider2D>().enabled = false;
	}

	private IEnumerator kd_laugh_cr()
	{
		MinMax delay = new MinMax(1f, 3.5f);
		while (animator.GetBool("IsAttacking"))
		{
			AudioManager.Play("king_dice_attack_vox");
			emitAudioFromObject.Add("king_dice_attack_vox");
			while (AudioManager.CheckIfPlaying("king_dice_attack_vox"))
			{
				yield return null;
			}
			yield return delay.RandomFloat();
		}
	}
}
