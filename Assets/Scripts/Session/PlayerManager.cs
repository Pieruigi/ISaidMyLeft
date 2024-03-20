using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ISML
{
    public class PlayerManager : SingletonPersistent<PlayerManager>
    {
        public static UnityAction<Player> OnPlayerAdded;
        public static UnityAction<Player> OnPlayerRemoved;

        [SerializeField]
        GameObject playerPrefab;

        public Player LocalPlayer { get; private set; }

        List<Player> players = new List<Player>();
        public ICollection<Player> Players
        {
            get { return players.AsReadOnly(); }
        }

        private void OnEnable()
        {
            SessionManager.OnPlayerJoinedEvent += HandleOnPlayerJoined;
            SceneManager.sceneLoaded += HandleOnSceneLoaded;
            
        }

       

        private void OnDisable()
        {
            SessionManager.OnPlayerJoinedEvent -= HandleOnPlayerJoined;
            SceneManager.sceneLoaded -= HandleOnSceneLoaded;

        }

        private void HandleOnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                LocalPlayer.InGame = true;
            }
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
                        player.InGame = false;
                        player.IsCharacter = runner.IsSharedModeMasterClient ? true : false;
                    });
        }

    

        public void AddPlayer(Player player)
        {
            players.Add(player);
            if (player.HasStateAuthority)
                LocalPlayer = player;

            OnPlayerAdded?.Invoke(player);
        }

        public void RemovePlayer(Player player)
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

            OnPlayerRemoved?.Invoke(player);
        }
    }

}
