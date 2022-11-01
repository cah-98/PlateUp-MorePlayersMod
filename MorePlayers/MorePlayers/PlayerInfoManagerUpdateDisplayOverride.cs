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
    [HarmonyPatch(typeof(PlayerInfoManager), "UpdateDisplay")]
    public class UpdateDisplayOverridePatch_UpdateDisplay
	{
		[HarmonyPrefix]
		public static bool Prefix(PlayerInfoManager __instance)
		{
			Traverse PIM = Traverse.Create(__instance);

			List<PlayerInfo> list = PIM.Property("Players").GetValue<IPlayers>().All();
			bool flag = list.Count < Config.ConfigHelper.getMaxPlayers();
			foreach (PlayerInfo playerInfo in list)
			{
				bool flag2 = playerInfo.JoinProgress < PlayerInfoManager.DisplayJoinGrace && playerInfo.JoinProgress > -0.5f;
				if (!flag2)
				{
					bool flag3 = !PIM.Field("Modules").GetValue<Dictionary<int,PlayerElement>>().ContainsKey(playerInfo.ID);
					if (flag3)
					{
						PlayerElement playerElement = UnityEngine.Object.Instantiate<PlayerElement>((PlayerElement)ModuleDirectory.Main.GetPrefab<PlayerElement>(), __instance.Container, true);
						playerElement.transform.localRotation = Quaternion.identity;
						PIM.Field("Modules").GetValue<Dictionary<int, PlayerElement>>().Add(playerInfo.ID, playerElement);
					}
					PIM.Field("Modules").GetValue<Dictionary<int, PlayerElement>>()[playerInfo.ID].SetPlayer(playerInfo.ID);
				}
			}
			bool flag4 = flag && !PIM.Field("Modules").GetValue<Dictionary<int, PlayerElement>>().ContainsKey(0);
			if (flag4)
			{
				PlayerElement playerElement2 = UnityEngine.Object.Instantiate<PlayerElement>((PlayerElement)ModuleDirectory.Main.GetPrefab<PlayerElement>(), __instance.Container, true);
				playerElement2.transform.localRotation = Quaternion.identity;
				PIM.Field("Modules").GetValue<Dictionary<int, PlayerElement>>().Add(0, playerElement2);
				PIM.Field("Modules").GetValue<Dictionary<int, PlayerElement>>()[0].SetJoinPrompt();
			}
			PIM.Field("RemoveList").GetValue<HashSet<int>>().Clear();
			foreach (KeyValuePair<int, PlayerElement> keyValuePair in PIM.Field("Modules").GetValue<Dictionary<int, PlayerElement>>())
			{
				PlayerInfo playerInfo2 = PIM.Property("Players").GetValue<IPlayers>().Get(keyValuePair.Key);
				bool flag5 = (!PIM.Property("Players").GetValue<IPlayers>().Has(keyValuePair.Key) || (playerInfo2.JoinProgress <= PlayerInfoManager.DisplayJoinGrace && playerInfo2.JoinProgress >= -0.5f)) && (!flag || keyValuePair.Key != 0);
				if (flag5)
				{
					keyValuePair.Value.Destroy();
					PIM.Field("RemoveList").GetValue<HashSet<int>>().Add(keyValuePair.Key);
				}
			}
			foreach (int key in PIM.Field("RemoveList").GetValue<HashSet<int>>())
			{
				PIM.Field("Modules").GetValue<Dictionary<int, PlayerElement>>().Remove(key);
			}
			PIM.Method("SetPositions").GetValue();

			return false;
		}
	}
}
