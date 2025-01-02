using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.InputSystem;
using System.Linq;
using TMPro;
using System.Collections;
// using UnityEditor.ShaderGraph;

public class VictoryTextManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI humansWinText;
    public TextMeshProUGUI howManyAliveText;
    private GhostHealth ghostHealth;
    private GameObject ghostObject;
    private GameObject gameManager;
    private CustomProperty customProperty;
    private bool dontSearchGhost;

    private const string ALIVE_NUM_STRING = "aliveNum";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        humansWinText.text = "";
        dontSearchGhost = false;
        gameManager = GameObject.Find("GameManager");
        customProperty = gameManager.GetComponent<CustomProperty>();
    }

    // Update is called once per frame
    void Update()
    {
        GetGhostGameobject();
        SetHumanWinText();
    }

    private IEnumerator enumerator(float delay)
    {
        yield return new WaitForSeconds(delay);
        howManyAliveText.text = "";
    }

    private void GetGhostGameobject()
    {
        if (dontSearchGhost)
        {
            return;
        }
        if (GameObject.Find("Ghost(Clone)") != null)
        {
            ghostObject = GameObject.Find("Ghost(Clone)");
            ghostHealth = ghostObject.GetComponent<GhostHealth>();
            dontSearchGhost = true;
        }
    }

    [PunRPC]
    public void HowManyAliveHumans(int value)
    {
        if (value == 0) {
            howManyAliveText.color = Color.red;
            howManyAliveText.text = "  Wiped Out...";
        }
        else if (value == 1)
        {
            howManyAliveText.text = value + " Player Left...";
        }
        else{
            howManyAliveText.text = value + " Players Left...";
        }
        StartCoroutine(nameof(enumerator), 3f);
    }

    public void SetHumanWinText()
    {
        if (ghostHealth == null)
        {
            return;
        }
        if (ghostHealth.IsLifeZero())
        {
            // Debug.Log("Ghostのライフは0よ!");
            humansWinText.text = "HUMANS WIN!";
            howManyAliveText.text = "";
        }
    }

    public void SetTimeUpText()
    {
        humansWinText.text = "HUMANS WIN!";
    }

    public void SetNullText()
    {
        howManyAliveText.text = "NULL";
    }
}
