using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;

namespace ISML
{
    public class Player: NetworkBehaviour
    {


        [UnitySerializeField]
        [Networked] 
        public NetworkString<_16> Name {  get; private set; }

        ChangeDetector changeDetector;


        private void Update()
        {
            foreach(var propertyName in changeDetector.DetectChanges(this, out var previous, out var current))
            {
                switch(propertyName)
                {
                    case nameof(Name):
                        Debug.Log($"Player - Name changed from {previous} to {current}");
                        break;
                }
            }
        }

        public override void Spawned()
        {
            base.Spawned();

            if (HasInputAuthority)
            {
                Name = AccountManager.Instance.UserName;
            }

            changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            
        }

        
    }

}
