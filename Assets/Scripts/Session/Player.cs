using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISML
{
    public class Player: NetworkBehaviour
    {
        
        [UnitySerializeField]
        [Networked] 
        public NetworkString<_16> Name {  get; set; }

        [UnitySerializeField]
        [Networked]
        public NetworkBool HelperOnly { get; set; }

        [UnitySerializeField]
        [Networked]
        public NetworkBool Ready { get; set; }

        [UnitySerializeField]
        [Networked]
        public NetworkBool IsCharacter { get; set; }


        [UnitySerializeField]
        [Networked]
        public NetworkBool InGame { get; set; }

        ChangeDetector changeDetector;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            foreach (var propertyName in changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                switch (propertyName)
                {
                    //case nameof(Name):
                    //    var nameReader = GetPropertyReader<NetworkString<_16>>(propertyName);
                    //    var (namePrev, nameCurr) = nameReader.Read(previousBuffer, currentBuffer);
                    //    Debug.Log($"Player - Name changed from {namePrev} to {nameCurr}");
                    //    break;
                    case nameof(Ready):
                        var readyReader = GetPropertyReader<NetworkBool>(propertyName);
                        var (readyPrev, readyCurr) = readyReader.Read(previousBuffer, currentBuffer);
                        Debug.Log($"Player - Ready changed from {readyPrev} to {readyCurr}");
                        break;
                }
            }
        }

        public override void Spawned()
        {
            base.Spawned();
            
            changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            PlayerManager.Instance.AddPlayer(this);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {

            base.Despawned(runner, hasState);
           
            PlayerManager.Instance.RemovePlayer(this);
        }


        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SetIsCharacterRpc(NetworkBool value)
        {
            IsCharacter = value;
        }
    }

}
