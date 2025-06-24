public class DragonLevelPeashot : BasicProjectile
{
	private int _color;

	public int color
	{
		get
		{
			return _color;
		}
		set
		{
			_color = value;
			SetColor();
		}
	}

	private void SetColor()
	{
		animator.SetInteger("Color", _color);
		if (color == 2)
		{
			SetParryable(true);
		}
	}
}
