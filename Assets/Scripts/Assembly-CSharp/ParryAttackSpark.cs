public class ParryAttackSpark : Effect
{
	public bool IsCuphead
	{
		set
		{
			animator.SetBool("IsCuphead", value);
		}
	}
}
