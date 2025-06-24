using UnityEngine;

public class LevelAudio : AbstractMonoBehaviour
{
	public static LevelAudio Create()
	{
		return Object.Instantiate(Level.Current.LevelResources.levelAudio);
	}
}
