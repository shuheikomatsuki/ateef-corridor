using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.Mathematics;
using System.Collections;
using System.Collections.Generic;

public class GhostSpecialAttack : MonoBehaviourPunCallbacks
{
    private int totalNumberOfHumans;

    private GameObject gameManager;
    private AudioManager audioManager;

    private Vector3[] playersPosition;
    private List<GameObject> attackZones = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager");
        audioManager = gameManager.GetComponent<AudioManager>();
        playersPosition = new Vector3[5];
    }

    IEnumerator CallFunctionAfterDelay(float delay, Vector3 playerPosition)
    {
        yield return new WaitForSeconds(delay);
        InstatiateAttackZone(playerPosition);
        audioManager.PlayAttackZoneAudio();
    }

    public void SpecialAttack()
    {
        totalNumberOfHumans = PhotonNetwork.CurrentRoom.PlayerCount - 1;
        for (int i = 1; i <= totalNumberOfHumans; i++)
        {
            GameObject playerObject = GameObject.Find("Player" + i + "(Clone)");
            if (playerObject != null)
            {
                Vector3 playerPosition = playerObject.transform.position;
                GameObject attackZoneEffect = PhotonNetwork.Instantiate("AttackZoneEffect", playerPosition, quaternion.identity);
                StartCoroutine(CallFunctionAfterDelay(2f, playerPosition));
            }
        }
        // Debug.Log("Special Attack開始!");
    }

    private void InstatiateAttackZone(Vector3 playerPosition)
    {
        GameObject attackZonet = PhotonNetwork.Instantiate("AttackZone", playerPosition, quaternion.identity);
    }
}
