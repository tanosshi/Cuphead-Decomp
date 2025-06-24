using UnityEngine;

public class MouseLevelCanCatapultProjectile : BasicProjectile
{
	public MouseLevelCanCatapultProjectile CreateFromPrefab(Vector2 pos, float rotation, float speed, char c)
	{
		MouseLevelCanCatapultProjectile mouseLevelCanCatapultProjectile = base.Create(pos, rotation, speed) as MouseLevelCanCatapultProjectile;
		mouseLevelCanCatapultProjectile.Set(c);
		return mouseLevelCanCatapultProjectile;
	}

	protected override void RandomizeVariant()
	{
	}

	private void Set(char c)
	{
		int num = 0;
		switch (c)
		{
		default:
			num = 0;
			break;
		case 'n':
			num = 1;
			break;
		case 'g':
			num = 2;
			SetParryable(true);
			break;
		case 'p':
			num = 3;
			break;
		case 'c':
			num = 4;
			break;
		}
		SetInt(Integers.Variant, num);
	}
}
