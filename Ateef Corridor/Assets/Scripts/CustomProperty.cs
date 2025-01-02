using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class CustomProperty : MonoBehaviourPunCallbacks
{
    private int totalPlayerCount = 0;
    private int numberOfAliveHumans = 0;
    // private bool shouldRewriteGhostHptext;

    private GameObject gameManager;
    private VictoryTextManager victoryTextManager;
    private AudioManager audioManager;
    private TimerManager timerManager;
    
    private const string ALIVE_NUM_STRING = "aliveNum";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager");
        victoryTextManager = gameManager.GetComponent<VictoryTextManager>();
        timerManager = GetComponent<TimerManager>();

        // shouldRewriteGhostHptext = false;
        audioManager = GetComponent<AudioManager>();
    }

    void Update()
    {
        Debug.Log("NumberOfAliveHumans = " + numberOfAliveHumans);
        Debug.Log("TotalPlayerCount = " + totalPlayerCount);
    }

    // public override void OnJoinedRoom()
    // {
    //     photonView.RPC(nameof(SetNumberOfPlayers), RpcTarget.AllBuffered);
    // }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        photonView.RPC(nameof(Rpc_UpdatePlayerCounts), RpcTarget.AllBuffered);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        photonView.RPC(nameof(Rpc_UpdatePlayerCounts), RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void Rpc_UpdatePlayerCounts()
    {
        totalPlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        numberOfAliveHumans = totalPlayerCount - 1;
    }

    public System.Collections.IEnumerator PlayGhostWinsAudioAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        audioManager.PlayGhostWinsAudio();
    }

    public System.Collections.IEnumerator SetHowManyAliveHumansText(float delay)
    {
        yield return new WaitForSeconds(delay);
        // victoryTextManager.HowManyAliveHumans(numberOfAliveHumans);
        photonView.RPC(nameof(victoryTextManager.HowManyAliveHumans), RpcTarget.AllBuffered, numberOfAliveHumans);
    }

    [PunRPC]
    public void DecreaseAliveHumans()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        photonView.RPC(nameof(Rpc_DecreaseAliveHumans), RpcTarget.AllBuffered);
        if (numberOfAliveHumans <= 0)
        {
            timerManager.StopRemainingTimer();
        }
        StartCoroutine(nameof(SetHowManyAliveHumansText), 2f);
    }

    [PunRPC]
    public void Rpc_DecreaseAliveHumans()
    {
        // StartCoroutine(nameof(SetHowManyAliveHumansText), 2f);
        if (PhotonNetwork.IsMasterClient)
        {
            numberOfAliveHumans--;
            // Debug.Log("生存人数が" + numberOfAliveHumans + "に減少...");
        }
        // victoryTextManager.HowManyAliveHumans(numberOfAliveHumans);
        // StartCoroutine(nameof(SetHowManyAliveHumansText), 2f);
        if (numberOfAliveHumans == 0)
        {
            audioManager.StopBattleAudio();
            audioManager.StopApproachingAudio();
            StartCoroutine(nameof(PlayGhostWinsAudioAfterDelay), 3f);
        }
    }

    public void IncreaseAliveHumans()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        photonView.RPC(nameof(Rpc_IncreaseAliveHumans), RpcTarget.AllBuffered);
        StartCoroutine(nameof(SetHowManyAliveHumansText), 2f);
    }

    [PunRPC]
    public void Rpc_IncreaseAliveHumans()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            numberOfAliveHumans++;
            // Debug.Log("生存人数が" + numberOfAliveHumans + "に増加!");
        }
    }

    [PunRPC]
    public int Rpc_GetNumberOfAliveHumans()
    {
        return numberOfAliveHumans;
    }
}
