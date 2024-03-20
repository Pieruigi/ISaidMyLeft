using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISML
{
    public class PlayerManager : SingletonPersistent<PlayerManager>
    {
        [SerializeField]
        GameObject playerPrefab;

        public Player LocalPlayer { get; private set; }

        List<Player> players = new List<Player>();

        private void OnEnable()
        {
            SessionManager.OnPlayerJoinedEvent += HandleOnPlayerJoined;
            Player.OnPlayerSpawned += HandleOnPlayerSpawned;
            Player.OnPlayerDespawned += HandleOnPlayerDespawned;
        }

       

        private void OnDisable()
        {
            SessionManager.OnPlayerJoinedEvent -= HandleOnPlayerJoined;
            Player.OnPlayerSpawned -= HandleOnPlayerSpawned;
            Player.OnPlayerDespawned -= HandleOnPlayerDespawned;
        }

        private void HandleOnPlayerJoined(NetworkRunner runner, PlayerRef playerRef)
        {
            // In shared mode each player spawns their own networked objects
            if(runner.LocalPlayer == playerRef)
                runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, playerRef, 
                    (r, o) => 
                    { 
                        Player player = o.GetComponent<Player>();
                        player.Name = AccountManager.Instance.UserName;
                        player.HelperOnly = false;
                    });
        }

        private void HandleOnPlayerDespawned(Player player)
        {
            if (player.HasStateAuthority)
            {
                players.Clear();
                LocalPlayer = null;
            }
            else
            {
                players.Remove(player);
            }
            
        }

        private void HandleOnPlayerSpawned(Player player)
        {
            players.Add(player);
            if(player.HasStateAuthority)
                LocalPlayer = player;
        }
    }

}
