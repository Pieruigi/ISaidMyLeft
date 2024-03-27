using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ISML
{
    public enum TileState: byte { White, Red, Green, Yellow }

    public class FloorTile : NetworkBehaviour
    {
        public static UnityAction<FloorTile> OnTileEnter;

        [UnitySerializeField]
        [Networked]        
        public TileState State { get; private set; }

        ChangeDetector changeDetector;

        private void Start()
        {
            //ColorController[] cs = GetComponentsInChildren<ColorController>();
            //foreach (var c in cs)
            //{
            //    c.SetColor(0);
            //}
        }

        private void Update()
        {
            DetectChanges();
        }

        public override void Spawned()
        {
            base.Spawned();
            changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            
            ColorController[] cs = GetComponentsInChildren<ColorController>();
            foreach (var c in cs)
            {
                c.SetColor(State);
            }
        }

        void DetectChanges()
        {
            if(changeDetector == null)
                return;

            foreach (var propertyName in changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                switch (propertyName)
                {
                    //case nameof(Name):
                    //    var nameReader = GetPropertyReader<NetworkString<_16>>(propertyName);
                    //    var (namePrev, nameCurr) = nameReader.Read(previousBuffer, currentBuffer);
                    //    Debug.Log($"Player - Name changed from {namePrev} to {nameCurr}");
                    //    break;
                    case nameof(State):
                        var stateReader = GetPropertyReader<TileState>(propertyName);
                        var (statePrev, stateCurr) = stateReader.Read(previousBuffer, currentBuffer);
                        break;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Entering floor tile trigger...");
            if (other.CompareTag("Player"))
                OnTileEnter?.Invoke(this);
        }

        public void SetState(TileState state)
        {
            State = state;
        }
    }

}
