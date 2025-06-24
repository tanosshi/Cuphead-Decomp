public class FrogsLevelShortRageBullet : BasicProjectile
{
	protected override void Die()
	{
		if (base.CanParry)
		{
			base.Die();
			move = true;
		}
	}
}
