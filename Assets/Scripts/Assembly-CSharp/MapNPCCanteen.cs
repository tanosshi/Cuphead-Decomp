using UnityEngine;

public class MapNPCCanteen : MonoBehaviour
{
	[SerializeField]
	private int dialoguerVariableID = 13;

	private void Start()
	{
		if (PlayerData.Data.IsUnlocked(PlayerId.PlayerOne, Weapon.plane_weapon_bomb) && (PlayerManager.Multiplayer || PlayerData.Data.IsUnlocked(PlayerId.PlayerTwo, Weapon.plane_weapon_bomb)))
		{
			Dialoguer.SetGlobalFloat(dialoguerVariableID, 1f);
		}
		AddDialoguerEvents();
	}

	private void OnDestroy()
	{
		RemoveDialoguerEvents();
	}

	public void AddDialoguerEvents()
	{
		Dialoguer.events.onMessageEvent += OnDialoguerMessageEvent;
	}

	public void RemoveDialoguerEvents()
	{
		Dialoguer.events.onMessageEvent -= OnDialoguerMessageEvent;
	}

	private void OnDialoguerMessageEvent(string message, string metadata)
	{
		if (!(message == "CanteenWeaponTwo"))
		{
			return;
		}
		MapEventNotification.Current.ShowTooltipEvent(TooltipEvent.Canteen);
		if (!PlayerData.Data.IsUnlocked(PlayerId.PlayerOne, Weapon.plane_weapon_bomb))
		{
			PlayerData.Data.Gift(PlayerId.PlayerOne, Weapon.plane_weapon_bomb);
			if (!PlayerData.Data.Loadouts.GetPlayerLoadout(PlayerId.PlayerOne).HasEquippedSecondarySHMUPWeapon)
			{
				PlayerData.Data.Loadouts.GetPlayerLoadout(PlayerId.PlayerOne).MustNotifySwitchSHMUPWeapon = true;
			}
			PlayerData.Data.Loadouts.GetPlayerLoadout(PlayerId.PlayerOne).HasEquippedSecondarySHMUPWeapon = true;
		}
		if (!PlayerData.Data.IsUnlocked(PlayerId.PlayerTwo, Weapon.plane_weapon_bomb))
		{
			PlayerData.Data.Gift(PlayerId.PlayerTwo, Weapon.plane_weapon_bomb);
			if (!PlayerData.Data.Loadouts.GetPlayerLoadout(PlayerId.PlayerTwo).HasEquippedSecondarySHMUPWeapon)
			{
				PlayerData.Data.Loadouts.GetPlayerLoadout(PlayerId.PlayerTwo).MustNotifySwitchSHMUPWeapon = true;
			}
			PlayerData.Data.Loadouts.GetPlayerLoadout(PlayerId.PlayerTwo).HasEquippedSecondarySHMUPWeapon = true;
		}
		Dialoguer.SetGlobalFloat(dialoguerVariableID, 1f);
		PlayerData.SaveCurrentFile();
	}
}
