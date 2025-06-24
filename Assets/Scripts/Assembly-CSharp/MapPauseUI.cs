public class MapPauseUI : LevelPauseGUI
{
	protected override bool CanPause
	{
		get
		{
			if (base.state != State.Animating && MapDifficultySelectStartUI.Current.CurrentState == AbstractMapSceneStartUI.State.Inactive && MapConfirmStartUI.Current.CurrentState == AbstractMapSceneStartUI.State.Inactive && MapBasicStartUI.Current.CurrentState == AbstractMapSceneStartUI.State.Inactive)
			{
				return true;
			}
			return false;
		}
	}
}
