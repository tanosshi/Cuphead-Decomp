using System.Collections;
using UnityEngine;

public class FlyingBirdLevelHeart : AbstractCollidableObject
{
	private const float ProjectileOffsetX = 72f;

	private const float ScaleRate = 1.75f;

	private const float ScaleStartPosition = 100f;

	private const float InitialScale = 0.5f;

	private const float TargetScale = 1f;

	[SerializeField]
	private Effect puffFX;

	[SerializeField]
	private BasicProjectile projectilePrefab;

	[SerializeField]
	private SpriteRenderer[] renderers;

	private bool isActive;

	private int mainShootIndex;

	private int projectileMainIndex;

	private int projectileSubIndex;

	private Animator thisAnimator;

	private LevelProperties.FlyingBird properties;

	private bool faceRight;

	public void InitHeart(LevelProperties.FlyingBird properties)
	{
		this.properties = properties;
		mainShootIndex = Random.Range(0, properties.CurrentState.heart.shootString.Length);
		projectileMainIndex = Random.Range(0, properties.CurrentState.heart.numOfProjectiles.Length);
		thisAnimator = GetComponent<Animator>();
	}

	public void StartHeartAttack()
	{
		AbstractPlayerController next = PlayerManager.GetNext();
		faceRight = next.transform.position.x > base.transform.position.x;
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].flipX = faceRight;
		}
		base.gameObject.SetActive(true);
		StartCoroutine(accend_cr());
	}

	private IEnumerator accend_cr()
	{
		isActive = true;
		float start = base.transform.position.y;
		FireSpreadshot();
		while (base.transform.position.y < start + properties.CurrentState.heart.heartHeight)
		{
			base.transform.position += Vector3.up * properties.CurrentState.heart.movementSpeed * CupheadTime.Delta;
			if (base.transform.localScale.x < 1f)
			{
				base.transform.localScale += Vector3.one * 1.75f * CupheadTime.Delta;
			}
			else
			{
				base.transform.localScale = Vector3.one * 1f;
			}
			yield return null;
			if (properties.CurrentHealth <= 0f)
			{
				isActive = false;
				break;
			}
		}
		while (base.transform.position.y > start)
		{
			base.transform.position += Vector3.down * properties.CurrentState.heart.movementSpeed * CupheadTime.Delta;
			if (base.transform.position.y < start + 100f)
			{
				if (base.transform.localScale.x > 0.5f)
				{
					base.transform.localScale -= Vector3.one * 1.75f * CupheadTime.Delta;
				}
				else
				{
					base.transform.localScale = Vector3.one * 0.5f;
				}
			}
			yield return null;
			if (properties.CurrentHealth <= 0f)
			{
				start = Level.Current.Ground - 150;
			}
		}
		base.transform.SetPosition(null, start);
		base.gameObject.SetActive(false);
	}

	private void FireSpreadshot()
	{
		StartCoroutine(spreadShot_cr());
	}

	private void SpawnFX()
	{
		puffFX.Create(base.transform.position);
	}

	private IEnumerator spreadShot_cr()
	{
		AbstractPlayerController player = PlayerManager.GetNext();
		string[] shootString = properties.CurrentState.heart.shootString[mainShootIndex].Split(',');
		int shootIndex = Random.Range(0, shootString.Length);
		for (int j = 0; j < properties.CurrentState.heart.shotCount; j++)
		{
			string[] projectileString = properties.CurrentState.heart.numOfProjectiles[projectileMainIndex].Split(',');
			int projectiles = 0;
			float projectileDelay = 0f;
			if (player == null || player.IsDead)
			{
				player = PlayerManager.GetNext();
			}
			int.TryParse(projectileString[projectileSubIndex], out projectiles);
			float.TryParse(shootString[shootIndex], out projectileDelay);
			yield return CupheadTime.WaitForSeconds(this, projectileDelay);
			thisAnimator.SetTrigger("Attack");
			yield return CupheadTime.WaitForSeconds(this, 0.125f);
			float directionX = player.transform.position.x - base.transform.position.x;
			float directionY = player.transform.position.y - base.transform.position.y;
			AudioManager.Play("level_flyingbird_stretcher_regurgitate_projectile");
			emitAudioFromObject.Add("level_flyingbird_stretcher_regurgitate_projectile");
			for (int i = 0; i < projectiles; i++)
			{
				float floatAt = properties.CurrentState.heart.spreadAngle.GetFloatAt((float)i / ((float)projectiles - 1f));
				float num = properties.CurrentState.heart.spreadAngle.max / 2f;
				floatAt -= num;
				float num2 = Mathf.Atan2(directionY, directionX) * 57.29578f;
				if (faceRight && (num2 < -90f || num2 > 90f))
				{
					num2 = 180f - num2;
				}
				else if (!faceRight && num2 > -90f && num2 < 90f)
				{
					num2 = -180f - num2;
				}
				Vector3 vector = new Vector3(72f, 0f, 0f);
				vector *= (float)((!faceRight) ? 1 : (-1));
				projectilePrefab.Create(base.transform.position - vector, num2 + floatAt, properties.CurrentState.heart.projectileSpeed);
				shootIndex = (shootIndex + 1) % shootString.Length;
			}
			if (shootIndex < shootString.Length - 1)
			{
				shootIndex++;
			}
			else
			{
				mainShootIndex = (mainShootIndex + 1) % properties.CurrentState.heart.shootString.Length;
				shootIndex = 0;
			}
			if (projectileSubIndex < projectileString.Length - 1)
			{
				projectileSubIndex++;
			}
			else
			{
				projectileMainIndex = (projectileMainIndex + 1) % properties.CurrentState.heart.numOfProjectiles.Length;
				projectileSubIndex = 0;
			}
			yield return null;
		}
		isActive = false;
	}
}
