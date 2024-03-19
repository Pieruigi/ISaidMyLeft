using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISML.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        GameObject mainPanel;

        [SerializeField]
        GameObject sessionPanel;

        [SerializeField]
        GameObject lobbyPanel;

        private void Awake()
        {
            
        }

        private void OnEnable()
        {
            ShowMainPanel();
        }

        void HideAll()
        {
            mainPanel.SetActive(false);
            sessionPanel.SetActive(false);
            lobbyPanel.SetActive(false);
        }

        public void ShowMainPanel()
        {
            HideAll();
            mainPanel.SetActive(true);
        }

        public void ShowSessionPanel()
        {
            HideAll();
            sessionPanel.SetActive(true);
        }

        public void ShowLobbyPanel()
        {
            HideAll();
            lobbyPanel.SetActive(true);
        }
    }

}
