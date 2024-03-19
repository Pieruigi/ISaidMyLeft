using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISML
{
    public class AccountManager : SingletonPersistent<AccountManager>
    {
        string[] fakeNames = new string[] { "Pippo", "Pluto", "Topolino", "Paperino", "Qui", "Quo", "Qua" };

        string userName;
        public string UserName
        {
            get { return userName; }
        }

        private void Start()
        {
            Login();
        }

        void Login()
        {
            userName = fakeNames[Random.Range(0, fakeNames.Length)];
        }
    }

}
