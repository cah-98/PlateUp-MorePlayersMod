using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MorePlayers.Config;

namespace MorePlayers;

[BepInPlugin(Guid, Name, Version)]
[BepInProcess("PlateUp.exe")]
public class MorePlayers : BaseUnityPlugin
{
    private const string Guid = "MorePlayers";
    private const string Name = "MorePlayers";
    private const string Version = "1.0.0";
    
    internal static ManualLogSource Log;

    private void Awake()
    {
        Log = Logger;
        
        var harmony = new Harmony(Guid);
        harmony.PatchAll();

        Log.LogMessage("Loaded MorePlayers version: " + Version);
        
        ConfigHelper.SetUp(Config);
    }
}