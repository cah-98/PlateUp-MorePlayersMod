using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Kitchen.NetworkSupport;
using Steamworks;
using Steamworks.Data;
using Kitchen;
using Controllers;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.LowLevel;

namespace MorePlayers
{
    [HarmonyPatch(typeof(InputSource), "Setup")]
    public class SetupOverridePatch_Setup
	{
		static InputSource instance;

		[HarmonyPrefix]
		public static bool Prefix(InputSource __instance, bool register_as_default = true)
		{
			// You can return true if you want the original to be called.
			//__instance.Setup(register_as_default);
			if (register_as_default)
			{
				InputSourceIdentifier.DefaultInputSource = __instance;
			}

			Dictionary<InputActionMap, Func<InputActionMap>> tempDict = new()
            {
				{
					Maps.NewGamepad(),
					new Func<InputActionMap>(Maps.NewGamepad)
				},
				{
					Maps.NewKeyboard(),
					new Func<InputActionMap>(Maps.NewKeyboard)
				}
			};
			Traverse.Create(__instance).Field("ActionMaps").SetValue(tempDict);
			__instance.RejectedDevices = new HashSet<int>();
			InputUser.listenForUnpairedDeviceActivity = Config.ConfigHelper.getMaxPlayers();

			instance = __instance;

			InputUser.onUnpairedDeviceUsed += NewDeviceUsedHelper;
			
			Traverse.Create(__instance).Field("Players").SetValue(new Dictionary<int, PlayerData>());
			foreach (InputUser inputUser in InputUser.all)
			{
				bool flag = !inputUser.valid;
				if (!flag)
				{
					inputUser.UnpairDevicesAndRemoveUser();
				}
			}
			InputSourceIdentifier.DefaultInputSource = __instance;
			// Let's do nothing, and not call the original method.
			return false;
		}

		private static void NewDeviceUsedHelper(InputControl control, InputEventPtr eventPtr)
		{
			Traverse.Create(instance).Method("NewDeviceUsed", control, eventPtr).GetValue(control, eventPtr);
		}
	}
}
