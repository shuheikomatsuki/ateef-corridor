using UnityEngine;
using System.Collections;
using TMPro;
using Photon.Pun;

public class TimerManager : MonoBehaviourPun
{
    private const int maxTimer = 300;
    private int remainingTimer;
    public TextMeshProUGUI remainingTimerText;
    
    private VictoryTextManager victoryTextManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        remainingTimer = maxTimer;
        victoryTextManager = GetComponent<VictoryTextManager>();
    }

    // Update is called once per frame
    void Update()
    {  
    }

    [PunRPC]
    private IEnumerator RemainingTimerCoroutine()
    {
        while (remainingTimer > 0)
        {
            yield return new WaitForSeconds(1f);
            remainingTimer--;
            if (remainingTimer % 60 < 10)
            {
                remainingTimerText.text = "  " + (remainingTimer / 60) + ":0" + (remainingTimer % 60);
            }
            else
            {
                remainingTimerText.text = "  " + (remainingTimer / 60) + ":" + (remainingTimer % 60);
            }
        }
        if (remainingTimer <= 0)
        {
            remainingTimerText.text = "終了!";
            victoryTextManager.SetTimeUpText();
        }
    }

    [PunRPC]
    public void StartRemainingTimer()
    {
        StartCoroutine(nameof(RemainingTimerCoroutine));
    }

    [PunRPC]
    public void StopRemainingTimer()
    {
        StopCoroutine(nameof(RemainingTimerCoroutine));
    }

    public int GetRemainingTimer()
    {
        return remainingTimer;
    }

}
