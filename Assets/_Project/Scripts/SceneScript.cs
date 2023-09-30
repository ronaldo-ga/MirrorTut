using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QuickStart
{
    public class SceneScript : NetworkBehaviour
    {
        public TextMeshProUGUI canvasStatusText;
        public PlayerScript playerScript;
        public SceneReference sceneReference;

        [SyncVar(hook = nameof(OnStatusTextChanged))]
        public string statusText;
        public TextMeshProUGUI canvasAmmoText;

        void OnStatusTextChanged(string _Old, string _New)
        {
            canvasStatusText.text = statusText;
        }

        public void ButtonSendMessage()
        {
            if (playerScript != null)
            {
                playerScript.CmdSendPlayerMessage();
            }
        }

        public void ButtonChangeScene()
        {
            if (isServer)
            {
                UnityEngine.SceneManagement.Scene scene = SceneManager.GetActiveScene();

                if (scene.name == "MyScene")
                {
                    NetworkManager.singleton.ServerChangeScene("MyOtherScene");
                }
                else
                {
                    NetworkManager.singleton.ServerChangeScene("MyScene");
                }
            }
            else
            {
                Debug.Log("Your are not Host.");
            }
        }

        public void UIAmmo(int _value)
        {
            canvasAmmoText.text = "Ammo: " + _value;
        }
    }
}
