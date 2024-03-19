using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ISML.UI
{
    public class SessionList : MonoBehaviour
    {
        [SerializeField]
        GameObject itemPrefab;

        [SerializeField]
        Transform content;

        [SerializeField]
        Button buttonPrev;

        [SerializeField]
        Button buttonNext;

        [SerializeField]
        List<SessionItem> sessionItems = new List<SessionItem>();

        private void OnEnable()
        {
            ClearAll();
            SessionManager.OnSessionListUpdatedEvent += HandleOnSessionListUpdated;
        }

        private void OnDisable()
        {
            ClearAll();
            SessionManager.OnSessionListUpdatedEvent -= HandleOnSessionListUpdated;
        }

        private void HandleOnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessions)
        {
            ClearAll();

            foreach (SessionInfo session in sessions)
            {
                Debug.Log($"Session:{session.Name}");
                // Create a new session item
                GameObject item = Instantiate(itemPrefab, content);
                SessionItem sItem = item.GetComponent<SessionItem>();
                sItem.Init(session);
                sessionItems.Add(sItem);
            }
        }

        public void SetInteractable(bool value)
        {
            foreach(SessionItem session in sessionItems)
            {
                session.SetInteractable(value);
            }
        }

        public void ClearAll()
        {
            int count = sessionItems.Count;
            for(int i = 0; i < count; i++)
            {
                Destroy(sessionItems[i].gameObject);
            }
            sessionItems.Clear();
        }
    }

}
