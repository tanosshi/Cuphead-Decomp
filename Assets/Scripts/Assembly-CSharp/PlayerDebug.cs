public class PlayerDebug
{
	public static bool Enabled = true;

	public static void Enable()
	{
		Enabled = true;
	}

	public static void Disable()
	{
		Enabled = false;
	}

	public static void Toggle()
	{
		Enabled = !Enabled;
	}
}
