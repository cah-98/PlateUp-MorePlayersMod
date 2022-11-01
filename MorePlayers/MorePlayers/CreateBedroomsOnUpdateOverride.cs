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
using Unity.Collections;
using Unity.Entities;

namespace MorePlayers
{
    [HarmonyPatch(typeof(CreateBedrooms), "OnUpdate")]
    public class CreateBedroomsOnUpdateOverridePatch_OnUpdate
	{
		[HarmonyPostfix]
		public static void Postfix(CreateBedrooms __instance)
		{
			Traverse CB = Traverse.Create(__instance);
			
			NativeArray<Entity> nativeArray = CB.Field("Players").GetValue<EntityQuery>().ToEntityArray(Allocator.Temp);
			List<Vector3> list = new List<Vector3>
			{
				new Vector3(9f, 0f, 5f),
				new Vector3(9f, 0f, 2f),
				new Vector3(9f, 0f, -3f),
				new Vector3(9f, 0f, -6f),
			};

			float xCoord = 9f;
			float zCoordOdd = -6f;
			float zCoordEven = 5f;
			for (int i = 4; i < Config.ConfigHelper.getMaxPlayers(); i++)
            {
				if (i % 12 == 0)
				{
					xCoord += 4.25f;
					zCoordEven = 5f;
					zCoordOdd = -6f;
				}

				if (i == 4)
                {
					zCoordEven += 3f;
					list.Add(new Vector3(xCoord, 0f, zCoordEven));
				}
				else if (i % 2 == 0)
                {
					zCoordEven += 3f;
					list.Add(new Vector3(xCoord, 0f, zCoordEven));
                }else
                {
					zCoordOdd -= 3f;
					list.Add(new Vector3(xCoord, 0f, zCoordOdd));
                }
            }
			Vector3[] array = list.ToArray();
			for (int i = 4; i < Config.ConfigHelper.getMaxPlayers(); i++)
			{
				var cbCreateAssigned = CB.Method("CreateAssigned", i, GameData.Main.Get<Appliance>(AssetReference.Bed), array[i] + new Vector3(0f, 0f, 1f), Vector3.forward);

				Entity target = cbCreateAssigned.GetValue<Entity>(i, GameData.Main.Get<Appliance>(AssetReference.Bed), array[i] + new Vector3(0f, 0f, 1f), Vector3.forward);
				Entity entity = cbCreateAssigned.GetValue<Entity>(i, GameData.Main.Get<Appliance>(AssetReference.InteractionProxy), array[i] + new Vector3(0f, 0f, 0f), Vector3.forward);

				__instance.EntityManager.AddComponentData<CInteractionProxy>(entity, new CInteractionProxy
				{
					Target = target,
					IsActive = true
				});

				cbCreateAssigned.GetValue<Entity>(i, GameData.Main.Get<Appliance>(AssetReference.OutfitStation), array[i] + new Vector3(-2f, 0f, 1f), Vector3.forward);
				cbCreateAssigned.GetValue<Entity>(i, GameData.Main.Get<Appliance>(AssetReference.CosmeticStation), array[i] + new Vector3(-2f, 0f, -1f), Vector3.back);
				cbCreateAssigned.GetValue<Entity>(i, GameData.Main.Get<Appliance>(AssetReference.ColourStation), array[i] + new Vector3(-3f, 0f, 1f), Vector3.forward);
				cbCreateAssigned.GetValue<Entity>(i, GameData.Main.Get<Appliance>(AssetReference.OccupationIndicator), array[i] + new Vector3(1f, 0f, 0f), Vector3.forward);

				CB.Method("PlaceSpawnMarker", i, array[i] + new Vector3(-1f, 0f, 0f)).GetValue(i, array[i] + new Vector3(-1f, 0f, 0f));

				int j = 0;
				foreach (Entity entity2 in nativeArray)
				{
					if (j > 4)
                    {
						bool flag = i == CB.Method("GetComponent", entity2).GetValue<CPlayer>(entity2).Index;
						if (flag)
						{
							__instance.EntityManager.SetComponentData<CPosition>(entity2, new CPosition(array[i] + new Vector3(-1f, 0f, 0f)));
							break;
						}
					}
					j++;
				}
			}
			nativeArray.Dispose();

			Vector3 botWallToRemove = new Vector3(5f,0f,-7.5f);
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
