public class VeggiesLevelSpit : BasicProjectile
{
	protected override void Die()
	{
		if (base.CanParry)
		{
			AudioManager.Play("level_veggies_potato_worm_explode");
		}
		base.Die();
	}
}
