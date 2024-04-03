using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace ISML
{
    public class FloorTimedController : MonoBehaviour
    {
        [SerializeField]
        List<FloorTile> tiles;

        [SerializeField]
        float time = 2.5f;

        [SerializeField]
        TileState initialState;

        [SerializeField]
        TileState finalState;


        float switchPulseTime = 1.5f;
        float currentTime = 0;
        TileState currentState;

        private void Awake()
        {
            currentTime = time;
            currentState = initialState;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            currentTime -= Time.deltaTime;
            if(currentTime < 0 )
            {
                currentTime += time;
                currentState = currentState == initialState ? finalState : initialState;
                foreach (var tile in tiles)
                    tile.PulseAndChangeStateRpc(currentState, switchPulseTime);

            }
        }

        private void OnEnable()
        {
            FloorTile.OnSpawned += HandleOnTileSpawned;
        }

        private void OnDisable()
        {
            FloorTile.OnSpawned -= HandleOnTileSpawned;
        }

        private void HandleOnTileSpawned(FloorTile tile)
        {
            if (!tiles.Contains(tile))
                return;

            tile.SetState(currentState);
        }
    }

}
