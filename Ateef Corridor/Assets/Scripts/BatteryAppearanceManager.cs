using UnityEngine;
using Photon.Pun;
using Unity.Mathematics;
using System.Collections;

public class BatteryAppearanceManager : MonoBehaviourPun
{
    private int randomNum;
    private int timer;
    private bool hasInstatiated = false;
    private TimerManager timerManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timerManager = GetComponent<TimerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        timer = timerManager.GetRemainingTimer();
        if ((timer % 60 == 0) && (timer % 150 != 0) && !hasInstatiated)
        {
            Vector3 position = new Vector3(0, 0, 0);
            PhotonNetwork.Instantiate("Battery_Yellow", position, quaternion.identity);
            hasInstatiated = true;
            StartCoroutine(nameof(enumerator), 5f);
        }
        if (timer == 150 && !hasInstatiated)
        {
            Vector3 position = new Vector3(0, 0, 0);
            PhotonNetwork.Instantiate("Battery_Blue", position, quaternion.identity);
            hasInstatiated = true;
        }
    }

    private IEnumerator enumerator(float delay)
    {
        yield return new WaitForSeconds(delay);
        hasInstatiated = false;
    }
}
