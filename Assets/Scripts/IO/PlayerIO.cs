
using UnityEngine;

public static class PlayerIO
{
    public static void SavePlayerState(string reality, Player player)
    {
        if (string.IsNullOrEmpty(reality))
        {
            Debug.LogError("Reality is null or em empty, cannot save player state.");
            return;
        }
        if (player == null)
        {
            Debug.LogError("Null player, cannot save player state.");
            return;
        }

        // Make path.
        string path = OutputUtils.RealitySaveDirectory + reality + OutputUtils.PlayerSaveDirectory + player.Name + OutputUtils.PlayerStateFile;
        Debug.Log("Saving state of player '" + player.Name + "' to '" + path + "'...");

        // Make save data.
        PlayerSaveData sd = new PlayerSaveData();
        sd.Set(player);

        // Save to file.
        OutputUtils.ObjectToFile(sd, path);
    }

    public static void LoadPlayerState(string reality, Player player)
    {
        if (string.IsNullOrEmpty(reality))
        {
            Debug.LogError("Reality is null or em empty, cannot save player state.");
            return;
        }
        if (player == null)
        {
            Debug.LogError("Null player, cannot save player state.");
            return;
        }

        // Make path.
        string path = OutputUtils.RealitySaveDirectory + reality + OutputUtils.PlayerSaveDirectory + player.Name + OutputUtils.PlayerStateFile;
        Debug.Log("Loading state of player '" + player.Name + "' from '" + path + "'...");

        // Load save data.
        PlayerSaveData sd = InputUtils.FileToObject<PlayerSaveData>(path);

        if (sd == null)
            return;

        // Apply to player.
        sd.Apply(player);
    }
}

public class PlayerSaveData
{
    public float X;
    public float Y;
    public float Health;
    public float MaxHealth;

    public void Set(Player player)
    {
        if (player == null)
            return;

        X = player.transform.position.x;
        Y = player.transform.position.y;

        Health = player.Health.GetHealth();
        MaxHealth = player.Health.GetMaxHealth();
    }

    public void Apply(Player player)
    {
        if (player == null)
            return;

        player.transform.position = new Vector3(X, Y, 0f);
        player.Health.Server_SetMaxHealth(MaxHealth);
        player.Health.Server_SetHealth(Health);
    }
}