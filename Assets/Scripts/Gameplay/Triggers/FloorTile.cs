using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace ISML
{
    /// <summary>
    /// White: you can walk, nothing happens
    /// Red: you die if you walk in
    /// Green: it's like a button, you must walk in to activate something
    /// Orange: walk but with different rules ( you must walk backward or strafing, depending on the tile symbol... crouch can be enabled )
    /// Yellow: just reset some other tiles
    /// </summary>
    public enum TileState: byte { White, Red, Green, Yellow, Orange }

    public class FloorTile : NetworkBehaviour
    {
        public static UnityAction<FloorTile> OnTileEnter;
        public static UnityAction<FloorTile> OnSpawned;

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

            SetColors();

            OnSpawned?.Invoke(this);
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
                        SetColors();
                        break;
                }
            }
        }

        void SetColors()
        {
            ColorController[] cs = GetComponentsInChildren<ColorController>();
            foreach (var c in cs)
            {
                Debug.Log($"Setting color state:{State}");
                c.SetColor(State);
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

        [Rpc(sources: RpcSources.StateAuthority, RpcTargets.All)]
        public async void PulseAndChangeStateRpc(TileState state, float delay)
        {
            ColorController[] cs = GetComponentsInChildren<ColorController>();
            foreach (var c in cs)
            {
                Debug.Log($"Setting color state:{State}");
                c.Pulse(delay);
            }
            await Task.Delay(System.TimeSpan.FromSeconds(delay));
            SetState(state);
        }
    }

}
