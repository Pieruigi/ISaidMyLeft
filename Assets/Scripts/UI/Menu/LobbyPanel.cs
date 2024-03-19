using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISML.UI
{
    public class LobbyPanel : MonoBehaviour
    {
        private void OnEnable()
        {
            SessionManager.OnPlayerLeftEvent += HandleOnPlayerLeft;
            SessionManager.OnShutdownEvent += HandleOnShutdown;
            
        }

        private void OnDisable()
        {
            SessionManager.OnPlayerLeftEvent -= HandleOnPlayerLeft;
            SessionManager.OnShutdownEvent -= HandleOnShutdown;
            
        }

        private void HandleOnShutdown(NetworkRunner runner, ShutdownReason reason)
        {
            GetComponentInParent<MainMenu>().ShowSessionPanel();
        }

        private void HandleOnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            
        }
    }

}
