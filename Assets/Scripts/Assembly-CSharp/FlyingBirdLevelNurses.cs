using System.Collections;
using UnityEngine;

public class FlyingBirdLevelNurses : AbstractCollidableObject
{
	private const string Regular = "R";

	private const string Parry = "P";

	[SerializeField]
	private AbstractProjectile pillPrefab;

	[SerializeField]
	private Transform shootRightPosRoot;

	[SerializeField]
	private Transform shootLeftPosRoot;

	[SerializeField]
	private GameObject spitFXLeft;

	[SerializeField]
	private GameObject spitFXRight;

	private FlyingBirdLevelBird parent;

	private bool leftSideShooting;

	private int typeIndex;

	private int attackIndex;

	private PlayerId target;

	private string[] pinkPattern;

	private int pinkIndex;

	public Transform[] nurses;

	private LevelProperties.FlyingBird.Nurses properties;

	public void InitNurse(LevelProperties.FlyingBird.Nurses properties, FlyingBirdLevelBird parent)
	{
		nurses = base.transform.GetChildTransforms();
		Transform[] array = nurses;
		foreach (Transform transform in array)
		{
			transform.gameObject.SetActive(true);
		}
		leftSideShooting = ((Random.Range(-1, 1) >= 0) ? true : false);
		this.properties = properties;
		this.parent = parent;
		typeIndex = Random.Range(0, properties.pinkString.Split(',').Length);
		attackIndex = Random.Range(0, properties.attackCount.Split(',').Length);
		pinkPattern = properties.pinkString.Split(',');
		pinkIndex = 0;
		Transform[] array2 = nurses;
		foreach (Transform transform2 in array2)
		{
			if (transform2.GetComponent<Collider2D>() != null)
			{
				transform2.GetComponent<Collider2D>().enabled = true;
			}
		}
		StartCoroutine(attack_cr());
	}

	private IEnumerator attack_cr()
	{
		bool shootLeft = Rand.Bool();
		bool multiplayer = ((PlayerManager.GetPlayer(PlayerId.PlayerTwo) != null) ? true : false);
		int numbOfLeftSideNurses = nurses.Length / 2;
		while (true)
		{
			int max = int.Parse(properties.attackCount.Split(',')[attackIndex]);
			for (int i = 0; i < max; i++)
			{
				if (i != 0)
				{
					yield return CupheadTime.WaitForSeconds(this, properties.attackRepeatDelay);
				}
				if (shootLeft)
				{
					base.animator.SetBool("ANurseATK", true);
				}
				else
				{
					base.animator.SetBool("BNurseATK", true);
				}
				shootLeft = !shootLeft;
				if (multiplayer)
				{
					target++;
					if (target > PlayerId.PlayerTwo)
					{
						target = PlayerId.PlayerOne;
					}
				}
			}
			leftSideShooting = !leftSideShooting;
			yield return CupheadTime.WaitForSeconds(this, properties.attackMainDelay);
			attackIndex++;
			if (attackIndex >= properties.attackCount.Split(',').Length)
			{
				attackIndex = 0;
			}
		}
	}

	private void ShootLeft()
	{
		spitFXLeft.SetActive(false);
		AbstractProjectile abstractProjectile = pillPrefab.Create(shootLeftPosRoot.position + base.transform.up.normalized * 0.1f);
		abstractProjectile.GetComponent<FlyingBirdLevelNursePill>().InitPill(properties, target, pinkPattern[pinkIndex] == "P");
		pinkIndex = (pinkIndex + 1) % pinkPattern.Length;
		base.animator.SetBool("ANurseATK", false);
		spitFXLeft.SetActive(true);
	}

	private void ShootRight()
	{
		spitFXRight.SetActive(false);
		AbstractProjectile abstractProjectile = pillPrefab.Create(shootRightPosRoot.position + base.transform.up.normalized * 0.1f);
		abstractProjectile.GetComponent<FlyingBirdLevelNursePill>().InitPill(properties, target, pinkPattern[pinkIndex] == "P");
		pinkIndex = (pinkIndex + 1) % pinkPattern.Length;
		base.animator.SetBool("BNurseATK", false);
		spitFXRight.SetActive(true);
	}

	private void ShootSFX()
	{
		AudioManager.Play("nurse_attack");
		emitAudioFromObject.Add("nurse_attack");
	}

	public void Die()
	{
		StopAllCoroutines();
	}
}
