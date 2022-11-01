using System.Diagnostics;
using System.Linq;
using BepInEx.Configuration;
using Kitchen;
using KitchenData;
using Unity.Entities;
using UnityEngine;

namespace MorePlayers.Config;

public static class ConfigHelper
{
    private static ConfigEntry<int> _maxPlayers;

    public static void SetUp(ConfigFile config)
    {
        _maxPlayers = config.Bind("Players", "Max players", 4, "How many players should be able to join your lobbies.");
    }

    public static int getMaxPlayers()
    {
        return _maxPlayers.Value;
    }
}