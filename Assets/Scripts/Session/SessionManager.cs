using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace ISML
{
    public class SessionManager : SingletonPersistent<SessionManager>, INetworkRunnerCallbacks
    {
        public static UnityAction<NetworkRunner, PlayerRef> OnPlayerJoinedEvent;
        public static UnityAction<NetworkRunner, PlayerRef> OnPlayerLeftEvent;
        public static UnityAction<NetworkRunner, ShutdownReason> OnShutdownEvent;
        public static UnityAction OnJoinedToSessionLobbyEvent;
        public static UnityAction OnJoinToSessionLobbyFailedEvent;
        public static UnityAction OnStartSessionFailed;
        public static UnityAction<NetworkRunner, List<SessionInfo>> OnSessionListUpdatedEvent;

        public const int MaxPlayers = 4;


        NetworkSceneManagerDefault sceneManager;

        List<UnityAction<NetworkRunner, PlayerRef>> onPlayerJoinedCallbacks = new List<UnityAction<NetworkRunner, PlayerRef>>();
        List<UnityAction<NetworkRunner, ShutdownReason>> onShutdownCallbacks = new List<UnityAction<NetworkRunner, ShutdownReason>>();

        bool shutdown = false;
        NetworkRunner networkRunner;
        public NetworkRunner NetworkRunner
        {
            get { if (!networkRunner) networkRunner = GetComponent<NetworkRunner>(); return networkRunner; }
        }
    
        protected override void Awake()
        {
            base.Awake();
            sceneManager = GetComponent<NetworkSceneManagerDefault>();
        }

        void Update()
        {
            if(NetworkRunner == null || !NetworkRunner.IsSceneAuthority || NetworkRunner.SessionInfo == null || PlayerManager.Instance.Players.Count < 2) return;

            if (NetworkRunner.SessionInfo.IsOpen) // Game not started yet
            {
                foreach(var player in PlayerManager.Instance.Players)
                {
                    if (!player.Ready)
                        return;
                }

                // Start game
                NetworkRunner.SessionInfo.IsOpen = false;
                
                
                NetworkRunner.LoadScene(SceneRef.FromIndex(1), LoadSceneMode.Single);
            }
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
                OnStartSessionFailed?.Invoke();
            }
        }


        public void CreateSession(bool isPrivate)
        {
            StartGameArgs args = new StartGameArgs()
            {

                GameMode = GameMode.Shared,
                SessionName = $"{AccountManager.Instance.UserName}_{Guid.NewGuid()}",
                //MatchmakingMode = Fusion.Photon.Realtime.MatchmakingMode.,
                
                PlayerCount = MaxPlayers,
                SceneManager = sceneManager,
                DisableNATPunchthrough = true,
                IsVisible = !isPrivate

            };

            StartSession(args);
        }

        public void JoinSession(SessionInfo sessionInfo)
        {
            StartGameArgs args = new StartGameArgs()
            {

                GameMode = GameMode.Shared,
                SessionName = sessionInfo.Name
                             

            };

            StartSession(args);
        }

        public void Shutdown()
        {
            try
            {
                var runner = GetComponent<NetworkRunner>();
                runner.Shutdown(false);
            }
            catch (Exception ex) { }
            
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
                OnJoinedToSessionLobbyEvent?.Invoke();
            }
            else
            {
                Debug.Log("Join to session lobby failed");
                OnJoinToSessionLobbyFailedEvent?.Invoke();
            }
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
            Debug.Log($"Session list updated, session count: {sessionList.Count}");
            OnSessionListUpdatedEvent?.Invoke(runner, sessionList);
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            shutdown = true;
            
            Destroy(runner);
            OnShutdownEvent?.Invoke(runner, shutdownReason);
            
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            
        }
        #endregion

    }

}
