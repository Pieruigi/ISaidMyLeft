using Fusion;
using ISML.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ISML
{
    public enum LevelState : byte { NotReady, Ready }

    public class LevelController : NetworkBehaviour
    {
        public static LevelController Instance { get; private set; }

        [SerializeField]
        Transform playerSpawnPoint; 

        public Transform PlayerSpawnPoint { get { return playerSpawnPoint; } }

        [UnitySerializeField]
        [Networked]
        public LevelState State { get; private set; }

        ChangeDetector changeDetector;

        public bool IsSpawned { get; private set; }

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                Debug.Log("New scene...");
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        async void FakeReady()
        {
            await Task.Delay(1000);
            State = LevelState.Ready;
        }
       
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {

                ChangeMasterClient();

                /*ResetPlayer();*/
            }
            //if (Input.GetKeyDown(KeyCode.E))
            //{
            //    Reload();
            //}

            DetectChanges();
        }

        public override void Spawned()
        {
            base.Spawned();

            changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

            IsSpawned = true;
            FakeReady();
        }



        void DetectChanges()
        {
            if (changeDetector == null)
                return;
            foreach (var propertyName in changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                switch (propertyName)
                {
                    case nameof(State):
                        var stateReader = GetPropertyReader<LevelState>(propertyName);
                        var (statePrev, stateCurr) = stateReader.Read(previousBuffer, currentBuffer);
                        EnterNewState(statePrev, stateCurr);
                        break;

                }
            }
        }

        void EnterNewState(LevelState oldState, LevelState newState)
        {
            switch(newState)
            {
                case LevelState.Ready:
                    SetReady();
                    break;
               
            }
        }

        async void SetReady()
        {
            if (!SessionManager.Instance.IsSwitchingMasterClient)
            {
                await CameraFader.Instance.FadeIn(1f);

                PlayerController.Instance.SetNormalState();
            }
            else
            {
                SessionManager.Instance.IsSwitchingMasterClient = false;
                if (Runner.IsSharedModeMasterClient)
                {
                    Debug.Log("Reloading level...");
                    await Task.Delay(System.TimeSpan.FromSeconds(2f));
                    await Runner.LoadScene(SceneRef.FromIndex(1), LoadSceneMode.Single);
                }

            }
        }

        //async void Reload()
        //{
        //    await Task.Delay(System.TimeSpan.FromSeconds(5));

        //    Debug.Log("Reloading level");
        //    await Runner.LoadScene(SceneRef.FromIndex(1), LoadSceneMode.Single);
        //}

        public async void ChangeMasterClient()
        {
            //SessionManager.Instance.IsSwitchingMasterClient = true;

            //if (!Runner.IsSharedModeMasterClient) return;
            await Task.Delay(System.TimeSpan.FromSeconds(2f));
            if (Runner.IsSharedModeMasterClient)
            {
                Player p = PlayerManager.Instance.Players.Where(p => p != PlayerManager.Instance.LocalPlayer).First();

                // Set level state on switching

                await Task.Delay(System.TimeSpan.FromSeconds(2f));

                Runner.SetMasterClient(p.Object.InputAuthority);
            }
            else
            {
                //while(!Runner.IsSharedModeMasterClient) { await Task.Delay(500); }
                await Task.Delay(10000);
                await Runner.LoadScene(SceneRef.FromIndex(1), LoadSceneMode.Single);
            }
            

        }

        
    }

}
