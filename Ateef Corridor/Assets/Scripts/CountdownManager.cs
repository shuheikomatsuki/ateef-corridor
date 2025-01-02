using TMPro;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using System.Collections;

public class CountdownManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI hitTheGhostText;
    private NetWorkConnection netWorkConnection;
    public TextMeshProUGUI tKeyText;
    public TextMeshProUGUI yKeyText;

    private bool textSet = false;

    void Start()
    {
        netWorkConnection = GetComponent<NetWorkConnection>();
        SetAlpha(0f);
    }

    void Update()
    {
        SetStartingText();
    }

    private void SetAlpha(float alpha)
    {
        Color color = hitTheGhostText.color;
        color.a = alpha;
        hitTheGhostText.color = color;
    }

    private IEnumerator FadeInTextOverTime(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float alpha = elapsedTime / duration;
            SetAlpha(alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SetAlpha(1f);
        StartCoroutine(CallEraseText(1f));
    }

    private IEnumerator CallEraseText(float delay)
    {
        yield return new WaitForSeconds(delay);
        EraseText();
    }

    private void SetStartingText()
    {
        if (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom || textSet)
        {
            return;
        }
        //  ------------------------------------ テスト -------------------------------------------------
        if (netWorkConnection.Rpc_GetCountdownTimer() == 0)
        //  ------------------------------------ テスト -------------------------------------------------
        {
            countdownText.text = "";
            tKeyText.text = "";
            yKeyText.text = "";
            hitTheGhostText.text = "Hit the ghost with your light!";
            StartCoroutine(FadeInTextOverTime(3f));
            textSet = true;
        }
        else
        {
            //  ------------------------------------ テスト -------------------------------------------------
            countdownText.text = "Starting in " + netWorkConnection.Rpc_GetCountdownTimer();
            //  ------------------------------------ テスト -------------------------------------------------
            hitTheGhostText.text = "";
        }
    }

    private void EraseText()
    {
        hitTheGhostText.text = "";
    }
}
