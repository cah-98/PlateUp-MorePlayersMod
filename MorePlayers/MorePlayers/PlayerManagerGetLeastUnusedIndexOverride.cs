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
    [HarmonyPatch(typeof(PlayerManager), "GetLeastUnusedIndex")]
    public class GetLeastUnusedIndexOverridePatch_GetLeastUnusedIndex
	{
		[HarmonyPrefix]
		public static bool Prefix(PlayerManager __instance, ref int __result)
		{
			__result = -1;
			for (int i = 0; i < Config.ConfigHelper.getMaxPlayers(); i++)
			{
				bool flag = false;
				foreach (KeyValuePair<InputIdentifier, Player> keyValuePair in Traverse.Create(__instance).Field("Players").GetValue<Dictionary<InputIdentifier,Player>>())
				{
					int index = keyValuePair.Value.Index;
					bool flag2 = index == i;
					if (flag2)
					{
						flag = true;
						break;
					}
				}
				bool flag3 = !flag;
				if (flag3)
				{
					__result = i;
				}
			}
			
			return false;
		}
	}
}
