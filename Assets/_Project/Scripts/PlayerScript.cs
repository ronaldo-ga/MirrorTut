using Mirror;
using TMPro;
using UnityEngine;

namespace QuickStart
{
    public class PlayerScript : NetworkBehaviour
    {
        public TextMeshPro playerNameText;
        public GameObject floatingInfo;
        public GameObject[] weaponArray;

        private Material playerMaterialClone;
        private SceneScript sceneScript;
        private int selectedWeaponLocal = 1;

        [SyncVar(hook = nameof(OnNameChanged))]
        public string playerName;

        [SyncVar(hook = nameof(OnColorChanged))]
        public Color playerColor = Color.white;

        [SyncVar(hook = nameof(OnWeaponChanged))]
        public int activeWeaponSynced = 1;

        void Awake()
        {
            sceneScript = GameObject.FindObjectOfType<SceneScript>();

            foreach (var item in weaponArray)
                if (item != null)
                    item.SetActive(false);
        }

        void Update()
        {
            if (!isLocalPlayer)
            {
                floatingInfo.transform.LookAt(Camera.main.transform);
                return;
            }

            float moveX = Input.GetAxis("Horizontal") * Time.deltaTime * 110.0f;
            float moveZ = Input.GetAxis("Vertical") * Time.deltaTime * 4f;

            transform.Rotate(0, moveX, 0);
            transform.Translate(0, 0, moveZ);

            if (Input.GetButtonDown("Fire2"))
            {
                selectedWeaponLocal += 1;

                if (selectedWeaponLocal > weaponArray.Length)
                {
                    selectedWeaponLocal = 1;
                }

                CmdChangeActiveWeapon(selectedWeaponLocal);
            }
        }

        [Command]
        public void CmdSetupPlayer(string _name, Color _color)
        {
            playerName = _name;
            playerColor = _color;
            sceneScript.statusText = $"{playerName} joined.";
        }

        [Command]
        public void CmdSendPlayerMessage()
        {
            if (sceneScript)
            {
                sceneScript.statusText = $"{playerName} says hello {Random.Range(10, 99)}";
            }
        }

        [Command]
        public void CmdChangeActiveWeapon(int newIndex)
        {
            activeWeaponSynced = newIndex;
        }

        public override void OnStartLocalPlayer()
        {
            sceneScript.playerScript = this;

            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = new Vector3(0, 0, 0);

            floatingInfo.transform.localPosition = new Vector3(0, -0.3f, 0.6f);
            floatingInfo.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            string name = "Player" + Random.Range(100, 999);
            Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            CmdSetupPlayer(name, color);
        }

        void OnNameChanged(string _Old, string _New)
        {
            playerNameText.text = playerName;
        }

        void OnColorChanged(Color _Old, Color _New)
        {
            playerNameText.color = _New;
            playerMaterialClone = new Material(GetComponent<Renderer>().material);
            playerMaterialClone.color = _New;
            GetComponent<Renderer>().material = playerMaterialClone;
        }

        void OnWeaponChanged(int _Old, int _New)
        {
            if (0 < _Old && _Old < weaponArray.Length && weaponArray[_Old] != null)
            {
                weaponArray[_Old].SetActive(false);
            }

            if (0 < _New && _New < weaponArray.Length && weaponArray[_New] != null)
            {
                weaponArray[_New].SetActive(true);
            }
        }
    }
}