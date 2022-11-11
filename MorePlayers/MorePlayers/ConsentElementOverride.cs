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
using KitchenData;
using Kitchen.Modules;
using MorePlayers.Config;
using System.Diagnostics;
using Shapes;
using UnityEngine;

namespace MorePlayers
{
    [HarmonyPatch(typeof(ConsentElement), "Update")]
    public class ConsentElementOverridePatch_Update
    {
        [HarmonyPrefix]
        public static bool Prefix(ConsentElement __instance)
        {
            Traverse CE = Traverse.Create(__instance);
            Traverse Progress = CE.Field("Progress");

            float progressSpeed = 0f;

            Dictionary<int, bool> consents = CE.Field("Consents").GetValue<Dictionary<int, bool>>();
            int NumConsents = 0;
            foreach (KeyValuePair<int, bool> consent in consents)
            {
                if (consent.Value)
                {
                    NumConsents++;
                }
            }
            //UnityEngine.Debug.Log(NumConsents);
            if ((float)NumConsents / consents.Count >= ConfigHelper.getNeededPlayerConfirmation())
            {
                progressSpeed = (float)NumConsents / consents.Count;
            }
            //UnityEngine.Debug.Log(progressSpeed);
            
            //CE.Method("GetProgressSpeed").GetValue<float>();
            bool flag = progressSpeed <= 0f;
            if (flag)
            {
                Progress.SetValue(Progress.GetValue<float>() - 2f * Time.unscaledDeltaTime);
            }
            else
            {
                Progress.SetValue(Progress.GetValue<float>() + progressSpeed * Time.unscaledDeltaTime);
            }
            CE.Property("IsCompleted").SetValue(Progress.GetValue<float>() >= 1f);
            Progress.SetValue(Mathf.Clamp01(Progress.GetValue<float>()));
            CE.Method("UpdateBar").GetValue();

            return false;
        }
    }
}
