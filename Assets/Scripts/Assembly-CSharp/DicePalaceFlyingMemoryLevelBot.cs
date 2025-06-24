using System.Collections;
using UnityEngine;

public class DicePalaceFlyingMemoryLevelBot : AbstractProjectile
{
	[SerializeField]
	private BasicProjectile projectile;

	private DicePalaceFlyingMemoryLevelGameManager gameManager;

	private LevelProperties.DicePalaceFlyingMemory.Bots properties;

	private DicePalaceFlyingMemoryLevelContactPoint currentPoint;

	private DicePalaceFlyingMemoryLevelContactPoint targetPoint;

	private AbstractPlayerController player;

	private DamageReceiver damageReceiver;

	private bool moveOnY;

	private bool reachedEnd;

	private float health;

	private int movement;

	private int movementIndex;

	private int directionIndex;

	private int setPosition;

	private float offsetEnd;

	private Vector3 targetPos;

	private string[] movementString;

	private string[] directionString;

	protected override void Awake()
	{
		base.Awake();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	public void Init(LevelProperties.DicePalaceFlyingMemory.Bots properties, DicePalaceFlyingMemoryLevelContactPoint startingPoint, bool moveOnY, float health, AbstractPlayerController player)
	{
		base.transform.position = startingPoint.transform.position;
		currentPoint = startingPoint;
		this.health = health;
		this.player = player;
		this.moveOnY = moveOnY;
		this.properties = properties;
		base.transform.SetScale(properties.botsScale, properties.botsScale);
		gameManager = DicePalaceFlyingMemoryLevelGameManager.Instance;
		targetPoint = gameManager.contactPoints[1, 1];
		movementString = properties.movementString.GetRandom().Split(',');
		directionString = properties.directionString.GetRandom().Split(',');
		movementIndex = Random.Range(0, movementString.Length);
		StartCoroutine(move_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		health -= info.damage;
		if (health < 0f)
		{
			Die();
		}
	}

	private void CalculateYTarget(bool followPlayer)
	{
		reachedEnd = false;
		if (followPlayer)
		{
			if (player.transform.position.y <= base.transform.position.y)
			{
				MoveUp();
			}
			else
			{
				MoveDown();
			}
		}
		else if (currentPoint.Ycoord <= (gameManager.contactDimY - 1) / 2)
		{
			MoveUp();
		}
		else
		{
			MoveDown();
		}
		if (reachedEnd)
		{
			targetPos.y = gameManager.contactPoints[currentPoint.Xcoord, setPosition].transform.position.y + offsetEnd;
			return;
		}
		targetPoint = gameManager.contactPoints[currentPoint.Xcoord, setPosition];
		targetPos = targetPoint.transform.position;
	}

	private void CalculateXTarget(bool followPlayer)
	{
		reachedEnd = false;
		if (followPlayer)
		{
			if (player.transform.position.x <= base.transform.position.x)
			{
				MoveRight();
			}
			else
			{
				MoveLeft();
			}
		}
		else if (currentPoint.Xcoord <= (gameManager.contactDimX - 1) / 2)
		{
			MoveRight();
		}
		else
		{
			MoveLeft();
		}
		if (reachedEnd)
		{
			targetPos.x = gameManager.contactPoints[setPosition, currentPoint.Ycoord].transform.position.x + offsetEnd;
			return;
		}
		targetPoint = gameManager.contactPoints[setPosition, currentPoint.Ycoord];
		targetPos = targetPoint.transform.position;
	}

	private IEnumerator move_cr()
	{
		bool followPlayer = false;
		Vector3 pos = base.transform.position;
		while (true)
		{
			int.TryParse(movementString[movementIndex], out movement);
			if (player == null || player.IsDead)
			{
				player = PlayerManager.GetNext();
			}
			GetMovement(followPlayer);
			if (moveOnY)
			{
				CalculateYTarget(followPlayer);
			}
			else
			{
				CalculateXTarget(followPlayer);
			}
			if (moveOnY)
			{
				while (base.transform.position.y != targetPos.y)
				{
					pos.y = Mathf.MoveTowards(base.transform.position.y, targetPos.y, properties.botsSpeed * (float)CupheadTime.Delta);
					base.transform.position = pos;
					yield return null;
				}
			}
			else
			{
				while (base.transform.position.x != targetPos.x)
				{
					pos.x = Mathf.MoveTowards(base.transform.position.x, targetPos.x, properties.botsSpeed * (float)CupheadTime.Delta);
					base.transform.position = pos;
					yield return null;
				}
			}
			if (reachedEnd)
			{
				OnDestroy();
			}
			else
			{
				currentPoint = targetPoint;
				moveOnY = !moveOnY;
				movementIndex = (movementIndex + 1) % movementString.Length;
				directionIndex = (directionIndex + 1) % directionString.Length;
			}
			yield return null;
		}
	}

	private bool GetMovement(bool followPlayer)
	{
		if (directionString[directionIndex][0] == 'N')
		{
			followPlayer = false;
		}
		else if (directionString[directionIndex][0] == 'P')
		{
			followPlayer = true;
		}
		return followPlayer;
	}

	private void MoveUp()
	{
		int num = currentPoint.Ycoord + movement;
		if (num > gameManager.contactDimY - 1)
		{
			setPosition = gameManager.contactDimY - 1;
			reachedEnd = true;
			offsetEnd = -200f;
		}
		else
		{
			setPosition = num;
			reachedEnd = false;
		}
	}

	private void MoveDown()
	{
		int num = currentPoint.Ycoord - movement;
		if (num < 0)
		{
			setPosition = 0;
			reachedEnd = true;
			offsetEnd = 200f;
		}
		else
		{
			setPosition = num;
			reachedEnd = false;
		}
	}

	private void MoveLeft()
	{
		int num = currentPoint.Xcoord - movement;
		if (num < 0)
		{
			setPosition = 0;
			reachedEnd = true;
			offsetEnd = -200f;
		}
		else
		{
			setPosition = num;
			reachedEnd = false;
		}
	}

	private void MoveRight()
	{
		int num = currentPoint.Xcoord + movement;
		if (num > gameManager.contactDimX - 1)
		{
			setPosition = gameManager.contactDimX - 1;
			reachedEnd = true;
			offsetEnd = 200f;
		}
		else
		{
			setPosition = num;
			reachedEnd = false;
		}
	}

	private IEnumerator shoot_bullets_cr()
	{
		while (true)
		{
			animator.Play("Bot_Warning");
			yield return CupheadTime.WaitForSeconds(this, properties.bulletWarningDuration);
			FireProjectile();
			player = PlayerManager.GetNext();
			animator.Play("Off");
			yield return null;
			yield return CupheadTime.WaitForSeconds(this, properties.bulletDelay);
			yield return null;
		}
	}

	private void FireProjectile()
	{
		if (player == null || player.IsDead)
		{
			player = PlayerManager.GetNext();
		}
		Vector3 vector = player.transform.position - base.transform.position;
		float rotation = MathUtils.DirectionToAngle(vector);
		projectile.Create(base.transform.position, rotation, properties.bulletSpeed);
	}

	protected override void Die()
	{
		StopAllCoroutines();
		GetComponent<SpriteRenderer>().enabled = false;
		OnDestroy();
		base.Die();
	}
}
