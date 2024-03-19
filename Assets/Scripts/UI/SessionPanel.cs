using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ISML.UI
{
    public class SessionPanel : MonoBehaviour
    {
        [SerializeField]
        Button buttonCreate;

        [SerializeField]
        Button buttonBack;


        private void OnEnable()
        {
            // Disable buttons
            SetInteractableAll(false);
            SessionManager.OnPlayerJoinedEvent += HandleOnPlayerJoined;
            SessionManager.OnShutdownEvent += HandleOnShutdown;
            SessionManager.OnJoinedToSessionLobbyEvent += HandleOnJoinedToSessionLobby;
            SessionManager.OnJoinToSessionLobbyFailedEvent += HandleOnJoinToSessionLobbyFailed;
            SessionManager.OnStartSessionFailed += HandleOnStartSessionFailed;
            SessionManager.Instance?.JoinSessionLobby();
            
        }

        private void OnDisable()
        {
            SessionManager.OnPlayerJoinedEvent -= HandleOnPlayerJoined;
            SessionManager.OnShutdownEvent -= HandleOnShutdown;
            SessionManager.OnJoinedToSessionLobbyEvent -= HandleOnJoinedToSessionLobby;
            SessionManager.OnJoinToSessionLobbyFailedEvent -= HandleOnJoinToSessionLobbyFailed;
            SessionManager.OnStartSessionFailed -= HandleOnStartSessionFailed;
        }

        private void HandleOnStartSessionFailed()
        {
            SetInteractableAll(true);
        }

        private void HandleOnJoinToSessionLobbyFailed()
        {
            GetComponentInParent<MainMenu>().ShowMainPanel();
        }

        private void HandleOnJoinedToSessionLobby()
        {
            // We joined the session lobby, enable buttons back
            SetInteractableAll(true);
        }

        private void HandleOnConnectedToServer(NetworkRunner arg0)
        {
            SetInteractableAll(true);
        }

        private void HandleOnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.LocalPlayer.Equals(player))
            {
                GetComponentInParent<MainMenu>().ShowLobbyPanel();
            }
        }

        private void HandleOnShutdown(NetworkRunner arg0, ShutdownReason arg1)
        {
            GetComponentInParent<MainMenu>().ShowMainPanel();
        }

        void SetInteractableAll(bool value)
        {
            buttonCreate.interactable = value;
            buttonBack.interactable = value;
        }

        public void CreateGameSession()
        {
            SetInteractableAll(false);
            SessionManager.Instance.CreateSession(false);
        }
    }

}
