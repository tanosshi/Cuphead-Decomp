using UnityEngine;
using UnityEngine.UI;

public class UserProfileDisplay : AbstractMonoBehaviour
{
	[SerializeField]
	private Image gamerPic;

	[SerializeField]
	private Text gamerTag;

	[SerializeField]
	private PlayerId player;

	[SerializeField]
	private GameObject root;

	[SerializeField]
	private bool showForMultipleUsersUnsupported;

	private OnlineUser currentPicUser;

	protected override void Awake()
	{
		base.Start();
		root.SetActive(false);
		gamerPic.gameObject.SetActive(false);
	}

	protected override void Update()
	{
		base.Update();
		if (OnlineManager.Instance.Interface.SupportsMultipleUsers || (showForMultipleUsersUnsupported && OnlineManager.Instance.Interface.SupportsUserSignIn))
		{
			OnlineUser user = OnlineManager.Instance.Interface.GetUser(player);
			if (user != null)
			{
				root.SetActive(true);
				string text = user.Name;
				if (gamerTag.text != text)
				{
					gamerTag.text = text;
				}
				Texture2D profilePic = OnlineManager.Instance.Interface.GetProfilePic(player);
				if (profilePic != null && currentPicUser != user)
				{
					Debug.Log("Setting gamerpic. size: " + profilePic.width);
					currentPicUser = user;
					Sprite sprite = Sprite.Create(profilePic, new Rect(0f, 0f, profilePic.width, profilePic.height), new Vector2(0.5f, 0.5f));
					gamerPic.sprite = sprite;
					gamerPic.gameObject.SetActive(true);
				}
				else if (profilePic == null)
				{
					gamerPic.gameObject.SetActive(false);
				}
			}
			else
			{
				root.SetActive(false);
			}
		}
		else
		{
			root.SetActive(false);
		}
	}
}
