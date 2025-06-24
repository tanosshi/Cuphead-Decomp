using System.Collections;
using UnityEngine;

public class DicePalaceCigarLevelCigar : LevelProperties.DicePalaceCigar.Entity
{
	[Space(5f)]
	[SerializeField]
	private GameObject leftAshTray;

	[SerializeField]
	private GameObject rightAshTray;

	[Space(5f)]
	[SerializeField]
	private GameObject leftAsh;

	[SerializeField]
	private GameObject rightAsh;

	[Space(5f)]
	[SerializeField]
	private Transform leftSpawnPointFacingRight;

	[SerializeField]
	private Transform leftSpawnPointFacingLeft;

	[SerializeField]
	private Transform rightSpawnPointFacingLeft;

	[SerializeField]
	private Transform rightSpawnPointFacingRight;

	[Space(5f)]
	[SerializeField]
	private Transform smokeSpawnPoint;

	[SerializeField]
	private Effect smokeA;

	[SerializeField]
	private Effect smokeB;

	[SerializeField]
	private CollisionChild collisionChild;

	[Space(10f)]
	[SerializeField]
	private DicePalaceCigarLevelCigarSpit spitPrefab;

	[SerializeField]
	private Transform spitSpawnPoint;

	[SerializeField]
	private DicePalaceCigarLevelCigaretteGhost ghostPrefab;

	[SerializeField]
	private Transform ghostSpawnPoint;

	[SerializeField]
	private float ghostOffset;

	private bool isVisible;

	private bool onRightSpawn;

	private bool isFiring;

	private bool facingBack;

	private int spitAttackCountIndex;

	private int spitAttackDirectionIndex;

	private int ghostAttackDelayIndex;

	private int ghostSpawnPositionIndex;

	private int counter;

	private int maxCounter;

	private DamageReceiver damageReceiver;

	private DamageDealer damageDealer;

	protected override void Awake()
	{
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		collisionChild.OnPlayerCollision += OnCollisionPlayer;
		base.Awake();
	}

	protected override void Update()
	{
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
		base.Update();
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
	}

	public override void LevelInit(LevelProperties.DicePalaceCigar properties)
	{
		onRightSpawn = true;
		isFiring = false;
		rightAsh.SetActive(false);
		spitAttackCountIndex = Random.Range(0, properties.CurrentState.spiralSmoke.attackCount.Split(',').Length);
		spitAttackDirectionIndex = Random.Range(0, properties.CurrentState.spiralSmoke.rotationDirectionString.Split(',').Length);
		ghostAttackDelayIndex = Random.Range(0, properties.CurrentState.cigaretteGhost.attackDelayString.Split(',').Length);
		ghostSpawnPositionIndex = Random.Range(0, properties.CurrentState.cigaretteGhost.spawnPositionString.Split(',').Length);
		base.LevelInit(properties);
		Level.Current.OnIntroEvent += OnIntroEnd;
		Level.Current.OnWinEvent += OnDeath;
		StartCoroutine(intro_cr());
	}

	private IEnumerator intro_cr()
	{
		AudioManager.PlayLoop("dice_palace_cigar_intro_start_loop");
		emitAudioFromObject.Add("dice_palace_cigar_intro_start_loop");
		yield return CupheadTime.WaitForSeconds(this, 2f);
		base.animator.SetTrigger("Continue");
		yield return null;
	}

	private void StopIntroLoop()
	{
		AudioManager.Stop("dice_palace_cigar_intro_start_loop");
	}

	private void OnIntroEnd()
	{
		StartCoroutine(attack_cr());
		StartCoroutine(ghostAttack_cr());
	}

	private IEnumerator attack_cr()
	{
		while (true)
		{
			GetComponent<BoxCollider2D>().enabled = true;
			maxCounter = int.Parse(base.properties.CurrentState.spiralSmoke.attackCount.Split(',')[spitAttackCountIndex]);
			isFiring = true;
			while (isFiring)
			{
				if (counter <= maxCounter)
				{
					yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.spiralSmoke.hesitateBeforeAttackDelay);
					counter++;
					base.animator.SetTrigger("IsAttacking");
					yield return base.animator.WaitForAnimationToEnd(this, "Attack");
					yield return null;
					continue;
				}
				isFiring = false;
				counter = 0;
				break;
			}
			spitAttackCountIndex++;
			if (spitAttackCountIndex >= base.properties.CurrentState.spiralSmoke.attackCount.Split(',').Length)
			{
				spitAttackCountIndex = 0;
			}
			spitAttackDirectionIndex++;
			if (spitAttackDirectionIndex >= base.properties.CurrentState.spiralSmoke.rotationDirectionString.Split(',').Length)
			{
				spitAttackDirectionIndex = 0;
			}
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.cigar.warningDelay);
			base.animator.SetTrigger("OnStateChange");
			yield return base.animator.WaitForAnimationToEnd(this, "Teleport_End");
			yield return null;
		}
	}

	private void TeleportSFX()
	{
		AudioManager.Play("dice_palace_cigar_teleport");
		emitAudioFromObject.Add("dice_palace_cigar_teleport");
	}

	private void AttackSFX()
	{
		AudioManager.Play("dice_palace_cigar_attack");
		emitAudioFromObject.Add("dice_palace_cigar_attack");
	}

	private void SwitchSides()
	{
		leftAsh.SetActive(!onRightSpawn);
		rightAsh.SetActive(onRightSpawn);
		onRightSpawn = !onRightSpawn;
		if (!facingBack)
		{
			base.transform.Rotate(Vector3.up, 180f);
		}
		if (onRightSpawn)
		{
			base.transform.position = rightSpawnPointFacingRight.position;
		}
		else
		{
			base.transform.position = leftSpawnPointFacingRight.position;
		}
		StartCoroutine(finish_teleport_cr());
	}

	private void Rotate()
	{
		AbstractPlayerController next = PlayerManager.GetNext();
		if (next.transform.position.x < base.transform.position.x && !onRightSpawn)
		{
			base.transform.position = leftSpawnPointFacingLeft.position;
			base.transform.Rotate(Vector3.up, 180f);
			facingBack = true;
		}
		else if (next.transform.position.x > base.transform.position.x && onRightSpawn)
		{
			base.transform.position = rightSpawnPointFacingLeft.position;
			base.transform.Rotate(Vector3.up, 180f);
			facingBack = true;
		}
		else
		{
			base.transform.Rotate(Vector3.up, 0f);
			facingBack = false;
		}
	}

	private void CheckIfBackward()
	{
		if (facingBack)
		{
			base.transform.Rotate(Vector3.up, 180f);
			if (onRightSpawn)
			{
				base.transform.position = rightSpawnPointFacingRight.position;
			}
			else
			{
				base.transform.position = leftSpawnPointFacingRight.position;
			}
			facingBack = false;
		}
	}

	private IEnumerator finish_teleport_cr()
	{
		AudioManager.PlayLoop("dice_palace_cigar_teleport_warning_loop");
		emitAudioFromObject.Add("dice_palace_cigar_teleport_warning_loop");
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.cigar.warningDelay);
		VOXTeleportWarning();
		AudioManager.Stop("dice_palace_cigar_teleport_warning_loop");
		AudioManager.Play("dice_palace_cigar_teleport_end");
		emitAudioFromObject.Add("dice_palace_cigar_teleport_end");
		base.animator.SetTrigger("Continue");
		yield return null;
	}

	private void SpitAttack()
	{
		bool flag = false;
		flag = ((!facingBack) ? onRightSpawn : (!onRightSpawn));
		AbstractProjectile abstractProjectile = spitPrefab.Create(spitSpawnPoint.position, (int)base.transform.eulerAngles.y);
		if (base.properties.CurrentState.spiralSmoke.rotationDirectionString.Split(',')[spitAttackDirectionIndex][0] == '1')
		{
			abstractProjectile.GetComponent<DicePalaceCigarLevelCigarSpit>().InitProjectile(base.properties, true, flag);
		}
		else
		{
			abstractProjectile.GetComponent<DicePalaceCigarLevelCigarSpit>().InitProjectile(base.properties, false, flag);
		}
	}

	private IEnumerator ghostAttack_cr()
	{
		while (true)
		{
			float spawnPosx = Random.Range(ghostSpawnPoint.transform.position.x - ghostOffset, ghostSpawnPoint.transform.position.x + ghostOffset);
			yield return CupheadTime.WaitForSeconds(this, float.Parse(base.properties.CurrentState.cigaretteGhost.attackDelayString.Split(',')[ghostAttackDelayIndex]));
			AbstractProjectile proj = ghostPrefab.Create(new Vector2(spawnPosx, ghostSpawnPoint.transform.position.y));
			proj.GetComponent<DicePalaceCigarLevelCigaretteGhost>().InitGhost(base.properties);
			ghostAttackDelayIndex++;
			if (ghostAttackDelayIndex >= base.properties.CurrentState.cigaretteGhost.attackDelayString.Split(',').Length)
			{
				ghostAttackDelayIndex = 0;
			}
			ghostSpawnPositionIndex++;
			if (ghostSpawnPositionIndex >= base.properties.CurrentState.cigaretteGhost.spawnPositionString.Split(',').Length)
			{
				ghostSpawnPositionIndex = 0;
			}
		}
	}

	private void SmokeAB()
	{
		smokeA.Create(smokeSpawnPoint.transform.position);
		smokeB.Create(smokeSpawnPoint.transform.position);
	}

	private void SmokeB()
	{
		smokeB.Create(smokeSpawnPoint.transform.position);
	}

	private void SwitchLayer()
	{
		GetComponent<SpriteRenderer>().sortingLayerName = SpriteLayer.Map.ToString();
		GetComponent<SpriteRenderer>().sortingOrder = 200;
	}

	protected override void OnDestroy()
	{
		StopAllCoroutines();
		base.OnDestroy();
	}

	private void OnDeath()
	{
		AudioManager.Play("dice_palace_cigar_death");
		emitAudioFromObject.Add("dice_palace_cigar_death");
		VOXDeath();
		StopAllCoroutines();
		GetComponent<Collider2D>().enabled = false;
		base.animator.SetTrigger("OnDeath");
	}

	private void DeathSFX()
	{
		AudioManager.Play("dice_palace_cigar_death_end");
		emitAudioFromObject.Add("dice_palace_cigar_death_end");
	}

	private void VOXIntro()
	{
		AudioManager.Play("cigar_vox_intro");
		emitAudioFromObject.Add("cigar_vox_intro");
	}

	private void VOXDeath()
	{
		AudioManager.Play("cigar_vox_death");
		emitAudioFromObject.Add("cigar_vox_death");
	}

	private void VOXTeleport()
	{
		AudioManager.Play("cigar_vox_pre_teleport");
		emitAudioFromObject.Add("cigar_vox_pre_teleport");
	}

	private void VOXTeleportWarning()
	{
		AudioManager.Play("cigar_vox_warning");
		emitAudioFromObject.Add("cigar_vox_warning");
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.DrawLine(new Vector2(ghostSpawnPoint.transform.position.x - ghostOffset, ghostSpawnPoint.transform.position.y), new Vector2(ghostSpawnPoint.transform.position.x + ghostOffset, ghostSpawnPoint.transform.position.y));
	}
}
