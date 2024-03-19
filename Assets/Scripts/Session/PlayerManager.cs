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

        private void OnEnable()
        {
            SessionManager.OnPlayerJoinedEvent += HandleOnPlayerJoined;
        }

        private void OnDisable()
        {
            SessionManager.OnPlayerJoinedEvent -= HandleOnPlayerJoined;
        }

        private void HandleOnPlayerJoined(NetworkRunner runner, PlayerRef playerRef)
        {
            if (!runner.IsSharedModeMasterClient)
                return;
           
            // Spawn player
            runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, playerRef);
        }
    }

}
