using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

namespace ISML
{
    public class FloorPatrolController : MonoBehaviour
    {
        [SerializeField]
        List<FloorTile> tiles = new List<FloorTile>();

        [SerializeField]
        bool pingPong = false;

        [SerializeField]
        float speed = 2;

        [SerializeField]
        int startingId = 0;

        [SerializeField]
        bool reverseOnStart = false;

        int currentId = 0;
        int currentDirection = 1;
        float currentTime;
        float time = 0;

        private void Awake()
        {
            currentId = startingId;
            currentDirection = reverseOnStart ? -1 : 1;
            time = 1f / speed;
            currentTime = time;

        }
        // Update is called once per frame
        void Update()
        {
            currentTime -= Time.deltaTime;
            if(currentTime < 0 ) 
            {
                currentTime += time;

                // Reset the current tile 
                tiles[currentId].SetState(TileState.White);
                // Get the next tile
                if(currentId == 0)
                {
                    if(currentDirection < 0)
                    {
                        if (pingPong)
                        {
                            currentDirection = 1;
                            currentId++;
                        }
                        else
                        {
                            currentId = tiles.Count - 1;
                        }
                    }
                    else
                    {
                        currentId++;
                    }
                }
                else
                {
                    if(currentId == tiles.Count - 1) 
                    {
                        if (currentDirection > 0)
                        {
                            if (pingPong)
                            {
                                currentDirection = -1;
                                currentId--;
                            }
                            else
                            {
                                currentId = 0;
                            }
                        }
                        else
                        {
                            currentId--;
                        }
                    }
                    else
                    {
                        if(currentDirection > 0)
                            currentId++;
                        else
                            currentId--;
                        
                    }
                }
                tiles[currentId].SetState(TileState.Red);
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
            if(tile == tiles[currentId])
                tile.SetState(TileState.Red);
        }

    }

}
