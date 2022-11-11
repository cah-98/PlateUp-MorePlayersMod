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
    private static ConfigEntry<float> _neededPlayerConfirmation;

    public static void SetUp(ConfigFile config)
    {
        _maxPlayers = config.Bind("Players", "Max players", 4, "How many players should be able to join your lobbies.");
        _neededPlayerConfirmation = config.Bind("Players", "Player confirmation count", 1.0f, "How many players that need to confirm a vote to move on.");
    }

    public static int getMaxPlayers()
    {
        return _maxPlayers.Value;
    }

    public static float getNeededPlayerConfirmation()
    {
        return _neededPlayerConfirmation.Value;
    }
}