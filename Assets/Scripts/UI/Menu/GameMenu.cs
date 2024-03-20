using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ISML.UI
{
    public class GameMenu : MonoBehaviour
    {

        private void OnEnable()
        {
            SessionManager.OnShutdownEvent += HandleOnShutdown;
        }

        private void OnDisable()
        {
            SessionManager.OnShutdownEvent -= HandleOnShutdown;
        }

        private void HandleOnShutdown(NetworkRunner runner, ShutdownReason reason)
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }

        public void QuitGame()
        {
            SessionManager.Instance.Shutdown();
            
            
        }

    }

}
