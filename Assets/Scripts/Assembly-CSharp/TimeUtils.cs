using System;

public static class TimeUtils
{
	public static int GetCurrentSecond()
	{
		DateTime dateTime = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Utc);
		return (int)(DateTime.UtcNow - dateTime).TotalSeconds;
	}
}
