using UnityEngine;

public class MausoleumLevelCircleGhost : MausoleumLevelGhostBase
{
	private float rotationSpeed;

	private float rotation;

	private float setDirection;

	private Transform rotationBase;

	protected override bool DestroyedAfterLeavingScreen
	{
		get
		{
			return false;
		}
	}

	public virtual AbstractCollidableObject Create(Vector2 position, Vector2 urnPosition, float rotation, float speed, float rotationSpeed)
	{
		MausoleumLevelCircleGhost mausoleumLevelCircleGhost = base.Create(position, rotation, speed) as MausoleumLevelCircleGhost;
		mausoleumLevelCircleGhost.rotationSpeed = rotationSpeed;
		mausoleumLevelCircleGhost.rotationBase = new GameObject("CircleGhostBase").transform;
		mausoleumLevelCircleGhost.rotationBase.position = urnPosition;
		mausoleumLevelCircleGhost.rotation = rotation;
		mausoleumLevelCircleGhost.transform.parent = mausoleumLevelCircleGhost.rotationBase;
		return mausoleumLevelCircleGhost;
	}

	protected override void Start()
	{
		base.Start();
		bool flag = Rand.Bool();
		setDirection = ((!flag) ? (-360) : 360);
		GetComponent<SpriteRenderer>().flipY = flag;
	}

	protected override void Move()
	{
		base.transform.localPosition += (Vector3)MathUtils.AngleToDirection(rotation) * Speed * CupheadTime.FixedDelta;
		rotationBase.AddEulerAngles(0f, 0f, rotationSpeed * setDirection * CupheadTime.FixedDelta);
	}

	public override void OnParry(AbstractPlayerController player)
	{
		base.OnParry(player);
		AudioManager.Play("mausoleum_circle_ghost_death");
		emitAudioFromObject.Add("mausoleum_circle_ghost_death");
		Object.Destroy(rotationBase.gameObject);
	}
}
