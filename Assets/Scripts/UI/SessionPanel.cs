using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISML.UI
{
    public class SessionPanel : MonoBehaviour
    {
        private void OnEnable()
        {
            SessionManager.OnPlayerJoinedEvent += HandleOnPlayerJoined;

            SessionManager.Instance?.JoinSessionLobby();
            
        }

        private void OnDisable()
        {
            SessionManager.OnPlayerJoinedEvent -= HandleOnPlayerJoined;
        }

        private void HandleOnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.LocalPlayer.Equals(player))
            {
                GetComponentInParent<MainMenu>().ShowLobbyPanel();
            }
        }
    }

}
