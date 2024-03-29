using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ISML
{
    public class FloorPathController : MonoBehaviour
    {
        [SerializeField]
        List<FloorTile> tiles;

        [SerializeField]
        FloorTile enterTile;
        [SerializeField]
        FloorTile exitTile;
        [SerializeField]
        int columns;

        int rows;
        int spawnedCount = 0;

        private void Awake()
        {
            rows = tiles.Count / columns;
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.T))
                ResetTiles();
        }

        private void OnEnable()
        {
            foreach (var tile in tiles)
            {
                tile.OnSpawned += HandleOnTileSpawned;
            }
        }

        private void OnDisable()
        {
            foreach (var tile in tiles)
            {
                tile.OnSpawned -= HandleOnTileSpawned;
            }
        }

        private void HandleOnTileSpawned()
        {
            spawnedCount++;
            if(spawnedCount == tiles.Count)
            {
                ResetTiles();
            }
        }

        public void ResetTiles()
        {
            if (!PlayerManager.Instance.LocalPlayer.Runner.IsSharedModeMasterClient)
                return;
           
            // Set all the tiles as red
            for(int i=0; i<tiles.Count; i++)
            {
                tiles[i].SetState(TileState.Red);
            }
            
            // Start with the entering tile
            List<FloorTile> tilesToCheck = new List<FloorTile>(tiles);
            FloorTile tile = enterTile;
            tilesToCheck.Remove(tile);
            CheckTile(tile, ref tilesToCheck);
        }

        bool CheckTile(FloorTile tile, ref List<FloorTile> tilesToCheck)
        {
            // Set the current tile as walkable
            tile.SetState(TileState.White);
            // If is the exit tile then return true
            if (tile == exitTile)
                return true;
            // Not the exit tile, find an adiacent tile to continue
            int tileId = tiles.IndexOf(tile);
            List<FloorTile> candidates = new List<FloorTile>();
            // Left
            if(tileId % columns > 0 && tilesToCheck.Contains(tiles[tileId-1]))
                candidates.Add(tiles[tileId-1]);
            // Bottom
            if(tileId / columns > 0 && tilesToCheck.Contains(tiles[tileId-columns]))
                candidates.Add(tiles[tileId - columns]);
                
            // Right
            if (tileId % columns < columns - 1 && tilesToCheck.Contains(tiles[tileId + 1]))
                candidates.Add(tiles[tileId + 1]);
            // Front
            if(tileId / columns < rows - 1 && tilesToCheck.Contains(tiles[tileId + columns]))
                candidates.Add(tiles[tileId + columns]);

            foreach(var t in candidates)
                tilesToCheck.Remove(t);

           
            List<int> indices = new List<int>();
            for(int i=0; i<candidates.Count; i++)
                indices.Add(i);
            
            while(indices.Count > 0) 
            {
                int index = indices[UnityEngine.Random.Range(0, indices.Count)];
                indices.Remove(index);
                if (CheckTile(candidates[index], ref tilesToCheck))
                    return true;
            }

            tile.SetState(TileState.Red);
            return false;
            
                
        }

        
    }

}
