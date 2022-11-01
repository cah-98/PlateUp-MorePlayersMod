using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Kitchen.NetworkSupport;
using Steamworks;
using Steamworks.Data;
using Kitchen;
using KitchenData;
using Kitchen.Modules;
using UnityEngine;

namespace MorePlayers
{
    [HarmonyPatch(typeof(MultiplayerMenu), "Redraw")]
    public class RedrawOverridePatch_Redraw
	{
		[HarmonyPrefix]
		public static bool Prefix(MultiplayerMenu __instance, int hide_player_id = -1)
		{
			GlobalLocalisation BaseLoc = GameData.Main.GlobalLocalisation;
			Traverse MM = Traverse.Create(__instance);
			Traverse Perms = MM.Field("Permissions");

			__instance.ModuleList.Clear();
			Perms.SetValue(new Option<NetworkPermissions>(new List<NetworkPermissions>
			{
				NetworkPermissions.Private,
				NetworkPermissions.InviteOnly,
				NetworkPermissions.Open
			}, Session.GetNetworkPermissions(), new List<string>
			{
				BaseLoc.Name(NetworkPermissions.Private),
				BaseLoc.Name(NetworkPermissions.InviteOnly),
				BaseLoc.Name(NetworkPermissions.Open)
			}, null));

			Perms.GetValue<Option<NetworkPermissions>>().OnChanged += delegate (object _, NetworkPermissions f)
			{
				Session.SetNetworkPermissions(f);
			};
			Option<NetworkPermissions> PermValues = Perms.GetValue<Option<NetworkPermissions>>();
			MM.Method("AddSelect", PermValues.Names, new Action<int>(PermValues.SetChosen), PermValues.Chosen).GetValue(PermValues.Names, new Action<int>(PermValues.SetChosen), PermValues.Chosen);
			//this.AddSelect<NetworkPermissions>(this.Permissions);
			int j = 0;
			bool flag = hide_player_id != -1;
			foreach (PlayerInfo playerInfo in Players.Main.All())
			{
				bool flag2 = hide_player_id == playerInfo.ID;
				if (!flag2)
				{
					j++;
					PlayerRowElement playerRowElement = MM.Method("DrawPlayerRow",playerInfo,false).GetValue<PlayerRowElement>(playerInfo,false);
				}
			}
			while (j < Config.ConfigHelper.getMaxPlayers())
			{
				MM.Method("AddButton", BaseLoc["MENU_EMPTY_PLAYER_SLOT"], delegate (int i){}, 0, 0.6f, 0.05f).GetValue<ButtonElement>(BaseLoc["MENU_EMPTY_PLAYER_SLOT"], delegate (int i) { }, 0, 0.6f, 0.05f).SetSelectable(false);
				j++;
			}
			//MM.Method("AddActionButton", BaseLoc["MENU_OPEN_INVITE_OVERLAY"], PauseMenuAction.OpenInvitePanel).GetValue(BaseLoc["MENU_OPEN_INVITE_OVERLAY"], PauseMenuAction.OpenInvitePanel);

			MM.Method("AddButton", BaseLoc["MENU_OPEN_INVITE_OVERLAY"], delegate (int i)
			{
				MM.Method("RequestAction", PauseMenuAction.OpenInvitePanel).GetValue(PauseMenuAction.OpenInvitePanel);
			}, 0, 1f, 0.2f).GetValue(BaseLoc["MENU_OPEN_INVITE_OVERLAY"], delegate (int i)
			{
				MM.Method("RequestAction", PauseMenuAction.OpenInvitePanel).GetValue(PauseMenuAction.OpenInvitePanel);
			}, 0, 1f, 0.2f);

			MM.Method("AddButton", BaseLoc["MENU_BACK_SETTINGS"], delegate (int i)
			{
				MM.Method("RequestAction", PauseMenuAction.Back).GetValue(PauseMenuAction.Back);
			}, 0, 1f, 0.2f).GetValue(BaseLoc["MENU_BACK_SETTINGS"], delegate (int i)
			{
				MM.Method("RequestAction", PauseMenuAction.Back).GetValue(PauseMenuAction.Back);
			}, 0, 1f, 0.2f);

			return false;
		}
	}
}
