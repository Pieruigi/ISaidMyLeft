using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ISML
{
    public class CharacterManager : SingletonPersistent<CharacterManager>
    {
        [SerializeField]
        GameObject prefab;

        //public GameObject Character { get; set; }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleOnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleOnSceneLoaded;
        }

        private void HandleOnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"Spawning character....:{scene.buildIndex}");
            if(scene.buildIndex > 0)
            {
                if (PlayerManager.Instance.LocalPlayer.Runner.IsSharedModeMasterClient && !PlayerController.Instance)
                    PlayerManager.Instance.LocalPlayer.Runner.SpawnAsync(prefab, Vector3.zero, Quaternion.identity, PlayerManager.Instance.LocalPlayer.Runner.LocalPlayer);
            }
            else
            {
                
                //Character = null;
            }
            
        }



       
    }

}
