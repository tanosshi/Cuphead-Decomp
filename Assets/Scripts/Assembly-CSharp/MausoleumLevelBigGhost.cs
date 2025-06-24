using UnityEngine;

public class MausoleumLevelBigGhost : MausoleumLevelGhostBase
{
	[SerializeField]
	private MausoleumLevelRegularGhost regGhost;

	[SerializeField]
	private FlyingBlimpLevelSpawnRadius smallRoot1;

	[SerializeField]
	private FlyingBlimpLevelSpawnRadius smallRoot2;

	private LevelProperties.Mausoleum.BigGhost properties;

	private GameObject urn;

	private float rotation;

	public MausoleumLevelBigGhost Create(Vector2 position, float rotation, float speed, LevelProperties.Mausoleum.BigGhost properties, GameObject urn)
	{
		MausoleumLevelBigGhost mausoleumLevelBigGhost = base.Create(position, rotation, speed) as MausoleumLevelBigGhost;
		mausoleumLevelBigGhost.rotation = rotation;
		mausoleumLevelBigGhost.properties = properties;
		mausoleumLevelBigGhost.urn = urn;
		return mausoleumLevelBigGhost;
	}

	public override void OnParry(AbstractPlayerController player)
	{
		Vector2 vector = smallRoot1.transform.position;
		Vector2 vector2 = vector;
		Vector2 vector3 = new Vector2(Random.value * (float)(Rand.Bool() ? 1 : (-1)), Random.value * (float)(Rand.Bool() ? 1 : (-1)));
		vector = vector2 + vector3.normalized * smallRoot1.radius * Random.value;
		Vector2 vector4 = smallRoot2.transform.position;
		Vector2 vector5 = vector4;
		Vector2 vector6 = new Vector2(Random.value * (float)(Rand.Bool() ? 1 : (-1)), Random.value * (float)(Rand.Bool() ? 1 : (-1)));
		vector4 = vector5 + vector6.normalized * smallRoot2.radius * Random.value;
		Vector3 vector7 = urn.transform.position - (Vector3)vector;
		Vector3 vector8 = urn.transform.position - (Vector3)vector4;
		MausoleumLevelRegularGhost mausoleumLevelRegularGhost = regGhost.Create(vector, MathUtils.DirectionToAngle(vector7), properties.littleGhostSpeed) as MausoleumLevelRegularGhost;
		mausoleumLevelRegularGhost.GetParent(parent);
		MausoleumLevelRegularGhost mausoleumLevelRegularGhost2 = regGhost.Create(vector4, MathUtils.DirectionToAngle(vector8), properties.littleGhostSpeed) as MausoleumLevelRegularGhost;
		mausoleumLevelRegularGhost2.GetParent(parent);
		mausoleumLevelRegularGhost.transform.SetScale(0.7f, 0.7f, 0.7f);
		mausoleumLevelRegularGhost2.transform.SetScale(0.7f, 0.7f, 0.7f);
		base.OnParry(player);
		Object.Destroy(base.gameObject);
	}
}
