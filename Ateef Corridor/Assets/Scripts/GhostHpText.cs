using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.InputSystem;
using System.Linq;
using TMPro;

public class GhostHpText : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI ghostHpText;
    private CustomProperty customProperty;
    private GhostHealth ghostHealth;

    private const string GHOST_HP_STRING = "gHP";

    private GameObject gameManager;
    private bool dontSearchGhost = false;
    private GameObject ghostObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager");

        if (gameManager != null)
        {
            customProperty = gameManager.GetComponent<CustomProperty>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!dontSearchGhost && PhotonNetwork.InRoom)
        {
            ghostObject = GameObject.Find("Ghost(Clone)");
            ghostHealth = ghostObject.GetComponent<GhostHealth>();
            dontSearchGhost = true;
        }

        if (ghostHealth == null)
        {
            return;
        }
        ghostHpText.text = ((int) Mathf.Ceil(ghostHealth.GetGhostHealth())).ToString();
    }
}
