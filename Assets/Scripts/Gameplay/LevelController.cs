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

                SwitchPlayerAndReload();

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
            await CameraFader.Instance.FadeIn(1f);

            PlayerController.Instance.SetNormalState();

           
        }

     
        public async void SwitchPlayerAndReload()
        {
            if (Runner.IsSharedModeMasterClient)
            {
                Player currentCharacter = PlayerManager.Instance.Players.Where(p => p.IsCharacter).First();
                Player nextCharacter = PlayerManager.Instance.Players.Where(p => !p.IsCharacter).First();

                currentCharacter.SetIsCharacterRpc(false);
                nextCharacter.SetIsCharacterRpc(true);

                await Task.Delay(System.TimeSpan.FromSeconds(1f));

                await Runner.LoadScene(SceneRef.FromIndex(1), LoadSceneMode.Single);
            }


        }


    }

}
