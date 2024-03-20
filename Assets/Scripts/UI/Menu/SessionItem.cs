using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ISML.UI
{
    public class SessionItem : MonoBehaviour
    {

        [SerializeField]
        TMP_Text textName;

        [SerializeField]
        Button buttonPlay;

        SessionInfo sessionInfo;


        public void Init(SessionInfo sessionInfo) 
        {
            this.sessionInfo = sessionInfo;
            textName.text = sessionInfo.Name;
        }

        public void JoinSession()
        {
            Debug.Log($"Join session {sessionInfo.Name}");
            SessionManager.Instance.JoinSession(sessionInfo);
        }

        public void SetInteractable(bool value)
        {
            buttonPlay.interactable = value;
        }
    }

}
