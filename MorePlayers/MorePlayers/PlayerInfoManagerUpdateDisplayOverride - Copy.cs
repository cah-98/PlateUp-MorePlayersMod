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
    [HarmonyPatch(typeof(GameCreator), "Update")]
    public class UpdateOverridePatch_Update
	{
		[HarmonyPostfix]
		public static void Postfix(GameCreator __instance)
		{
			Vector3 botWallToRemove = new Vector3(5f, 0f, -7.5f);
			Vector3 topWallToRemove = new Vector3(5f, 0f, 6.5f);

			Collider[] colliders;
			if ((colliders = Physics.OverlapSphere(botWallToRemove, 1f /* Radius */)).Length > 1)
			{
				foreach (var collider in colliders)
				{
					var go = collider.gameObject; //This is the game object you collided with
					if (go.name.ToLower().Contains("wall section") && go.transform.position == botWallToRemove)
					{
						go.SetActive(false);
					}
				}
			}
			if ((colliders = Physics.OverlapSphere(topWallToRemove, 1f /* Radius */)).Length > 1)
			{
				foreach (var collider in colliders)
				{
					var go = collider.gameObject; //This is the game object you collided with
					if (go.name.ToLower().Contains("wall section") && go.transform.position == topWallToRemove)
					{
						go.SetActive(false);
					}
				}
			}
		}
	}
}
