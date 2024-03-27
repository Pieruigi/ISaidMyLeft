using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISML
{
    public class LevelController : Singleton<LevelController>
    {
        [SerializeField]
        Transform playerSpawnPoint; 

        public Transform PlayerSpawnPoint { get { return playerSpawnPoint; } }

        private void Start()
        {
            
        }

       
    }

}
