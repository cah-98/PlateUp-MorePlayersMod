using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Kitchen.NetworkSupport;
using Steamworks;
using Steamworks.Data;
using Kitchen;
using UnityEngine;
using System.Reflection;

namespace MorePlayers
{
    [HarmonyPatch(typeof(SteamPlatform), "CreateNewLobby")]
    public class CreateNewLobbyOverridePatch_CreateNewLobby
    {
		[HarmonyPostfix]
		public static void Postfix(Action<bool, Lobby> callback, SteamPlatform __instance)
		{
			// You can return true if you want the original to be called.

			SteamMatchmaking.OnLobbyCreated += delegate (Result result, Lobby lobby)
			{
				lobby.MaxMembers = Config.ConfigHelper.getMaxPlayers();
			};

			// Let's do nothing, and not call the original method.
			//return false;
		}
	}
}
