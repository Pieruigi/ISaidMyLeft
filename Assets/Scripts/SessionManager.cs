using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace ISML
{
    public class SessionManager : Singleton<SessionManager>, INetworkRunnerCallbacks
    {
        public static UnityAction<NetworkRunner, PlayerRef> OnPlayerJoinedEvent;
        public static UnityAction<NetworkRunner, PlayerRef> OnPlayerLeftEvent;
        public static UnityAction<NetworkRunner, ShutdownReason> OnShutdownEvent;


        public const int MaxPlayers = 4;

        NetworkSceneManagerDefault sceneManager;

        List<UnityAction<NetworkRunner, PlayerRef>> onPlayerJoinedCallbacks = new List<UnityAction<NetworkRunner, PlayerRef>>();
        List<UnityAction<NetworkRunner, ShutdownReason>> onShutdownCallbacks = new List<UnityAction<NetworkRunner, ShutdownReason>>();

        bool shutdown = false;
    
        protected override void Awake()
        {
            base.Awake();
            sceneManager = GetComponent<NetworkSceneManagerDefault>();
        }


        async void StartSession(StartGameArgs args)
        {
            NetworkRunner runner = GetComponent<NetworkRunner>();
            if(!runner)
                runner = gameObject.AddComponent<NetworkRunner>();
                

            var result = await runner.StartGame(args);

            if(result.Ok)
            {
                Debug.Log($"Session started:{runner.SessionInfo.Name}");
            }
            else
            {
                Debug.Log("Start session failed");
            }
        }


        public void CreateSession(bool isPrivate)
        {
            StartGameArgs args = new StartGameArgs()
            {

                GameMode = GameMode.Shared,
                //SessionName = "",
                MatchmakingMode = Fusion.Photon.Realtime.MatchmakingMode.FillRoom,
                PlayerCount = MaxPlayers,
                SceneManager = sceneManager,
                DisableNATPunchthrough = true,
                IsVisible = !isPrivate
            };

            StartSession(args);
        }

        public void QuitSession()
        {
            var runner = GetComponent<NetworkRunner>();
            runner.Shutdown(false);
        }

        public async void JoinSessionLobby()
        {
            NetworkRunner runner = GetComponent<NetworkRunner>();
            if (!runner || shutdown)
            {
                if (shutdown)
                {
                    shutdown = false;
                    DestroyImmediate(runner);
                }

                runner = gameObject.AddComponent<NetworkRunner>();
            }
                


            var result = await runner.JoinSessionLobby(SessionLobby.Shared);

            if (result.Ok)
            {
                Debug.Log($"Joined to session lobby");
            }
            else
            {
                Debug.Log("Join to session lobby failed");
            }
        }

        public async Task LeaveSessionLobby()
        {
            await GetComponent<NetworkRunner>()?.Shutdown(false);
        }

        #region fusion callbacks
        public void OnConnectedToServer(NetworkRunner runner)
        {
            //throw new NotImplementedException();
            Debug.Log("Connected to server");
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log($"Player {player.PlayerId} joined the session {runner.SessionInfo.Name}");

            OnPlayerJoinedEvent(runner, player);
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log($"Player {player.PlayerId} left the session {runner.SessionInfo.Name}");
            OnPlayerLeftEvent(runner, player);
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
            
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {
            
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            shutdown = true;
            Destroy(runner);
            OnShutdownEvent(runner, shutdownReason);
            
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            
        }
        #endregion

    }

}
