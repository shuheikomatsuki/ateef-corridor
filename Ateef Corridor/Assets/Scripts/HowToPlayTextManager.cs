using TMPro;
using UnityEngine;
using Photon.Pun;

public class HowToPlayTextManager : MonoBehaviourPun
{
    private bool isShowingText = true;
    public TextMeshProUGUI howToUseText;

    void Start()
    {
        howToUseText.text = "";
    }

    void Update()
    {
        if (!PhotonNetwork.InRoom)
        {
            return;
        }
        if (PhotonNetwork.IsMasterClient)
        {
            howToUseText.text = "Space 3秒長押し → 特殊攻撃,   LeftShift 長押し → 高速移動";
        }
        else
        {
            howToUseText.text = "Space → 懐中電灯 On/Off";
        }
        if (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if (isShowingText)
        {
            howToUseText.text = "Space 3秒長押し → 特殊攻撃,   LeftShift 長押し → 高速移動";
        }
    }
}
