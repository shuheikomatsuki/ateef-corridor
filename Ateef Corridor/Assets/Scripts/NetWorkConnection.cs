using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UIElements;
using Unity.Mathematics;
using System.Collections;
using TMPro;

public class NetWorkConnection : MonoBehaviourPunCallbacks
{
    private int myViewId;
    private GameObject obj;
    public GameObject battleAudio;
    public TextMeshProUGUI yourRoleText;

    private float countdownTimer = 10f;
    private float currentTimer = 10f;
    private bool isGameStarted = false;
    private bool shouldPlayBattleBgm = false;
    private bool hasAudioPlayed = false;
    private bool isCounting = false;

    private CustomProperty customProperty;
    private AudioManager audioManager;
    private TimerManager timerManager;

    void Start()
    {
        customProperty = GetComponent<CustomProperty>();
        audioManager = GetComponent<AudioManager>();
        timerManager = GetComponent<TimerManager>();
        yourRoleText.text = "";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        if (Input.GetKeyDown(KeyCode.T) && PhotonNetwork.InRoom)
        {
            photonView.RPC(nameof(Rpc_SetIsCounting), RpcTarget.AllBufferedViaServer);          //  isCounting = true;
            photonView.RPC(nameof(CountdownForStartingGame), RpcTarget.AllBufferedViaServer);   //  MasterClient のみ Coroutine を呼び出す
        }
        if (Input.GetKeyDown(KeyCode.Y) && PhotonNetwork.InRoom)
        {
            photonView.RPC(nameof(Rpc_SetIsCounting), RpcTarget.AllBufferedViaServer);          //  isCounting = false;
            StopCoroutine(nameof(CountdownCoroutine));
        }
        DisConnect();
    }

    public override void OnConnectedToMaster()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 5;
        PhotonNetwork.JoinOrCreateRoom("AteefCorridorRoom", new RoomOptions(), TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        // Debug.Log("入室");
        if (isGameStarted)
        {
            PhotonNetwork.Disconnect();
            return;
        }
        AssignRole();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        currentTimer = countdownTimer;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        currentTimer = countdownTimer;
    }

    private IEnumerator CountdownCoroutine()
    {
        while (currentTimer > 0)
        {
            yield return new WaitForSeconds(1f);
            currentTimer--;
            photonView.RPC(nameof(Rpc_UpdateTimer), RpcTarget.AllBufferedViaServer, currentTimer);
        }
        if (currentTimer <= 0)
        {
            photonView.RPC(nameof(Rpc_StartGameSequence), RpcTarget.AllBufferedViaServer);
        }
    }

    private IEnumerator StartGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartGame();
    }

    private IEnumerator PlayBattleBgm(float delay)
    {
        yield return new WaitForSeconds(delay);
        audioManager.PlayBattleAudio();
        hasAudioPlayed = true;
    }

    [PunRPC]
    public void CountdownForStartingGame()
    {
        //  MasterClient のみが Coroutine を呼び出す
        if (!isCounting || !PhotonNetwork.IsMasterClient)
        {
            return;
        }
        StartCoroutine(nameof(CountdownCoroutine));
    }

    [PunRPC]
    private void Rpc_SetIsCounting()
    {
        isCounting = !isCounting;
    }

    [PunRPC]
    private void Rpc_StartGameSequence()
    {
        if (currentTimer <= 0 && !hasAudioPlayed)
        {
            audioManager.PlayGameStartedAudio();
            shouldPlayBattleBgm = true;
            hasAudioPlayed = true;
            StartCoroutine(PlayBattleBgm(2f));
            StartCoroutine(StartGameAfterDelay(4f));
        }
    }

    [PunRPC]
    private void Rpc_UpdateTimer(float timerValue)
    {
        currentTimer = timerValue;
    }

    private void StartGame()
    {
        isGameStarted = true;
        timerManager.StartRemainingTimer();
        // photonView.RPC(nameof(timerManager.StartRemainingTimer), RpcTarget.MasterClient);
    }

    private void AssignRole()
    {
        Quaternion rotation = Quaternion.Euler(0, 180, 0); 
        // Debug.Log("総員 = " + PhotonNetwork.CountOfPlayers);

        if (PhotonNetwork.CountOfPlayers == 1)
        {
            PhotonNetwork.Instantiate("Ghost", new Vector3(0f, 0f, 0f), rotation);
            yourRoleText.text = "あなたの役割: ghost";
        }
        if (PhotonNetwork.CountOfPlayers == 2)
        {
            PhotonNetwork.Instantiate("Player1", new Vector3(-9.5f, 0f, 5.5f), rotation);
            yourRoleText.text = "あなたの役割: red";
        }
        if (PhotonNetwork.CountOfPlayers == 3)
        {
            PhotonNetwork.Instantiate("Player2", new Vector3(9.5f, 0f, 5.5f), rotation);
            yourRoleText.text = "あなたの役割: blue";
        }
        if (PhotonNetwork.CountOfPlayers == 4)
        {
            PhotonNetwork.Instantiate("Player3", new Vector3(-9.5f, 0f, -5.5f), quaternion.identity);
            yourRoleText.text = "あなたの役割: yellow";
        }
        if (PhotonNetwork.CountOfPlayers == 5)
        {
            PhotonNetwork.Instantiate("Player4", new Vector3(9.5f, 0f, -5.5f), quaternion.identity);
            yourRoleText.text = "あなたの役割: green";
        }
    }

    private void DisConnect()
    {
        if (!PhotonNetwork.IsConnected)
        {
            return;
        }
        if (Input.GetKey(KeyCode.Alpha8) && Input.GetKey(KeyCode.Alpha9))
        {
            PhotonNetwork.Disconnect();
        }
    }

    [PunRPC]
    public int Rpc_GetCountdownTimer()
    {
        return (int) Mathf.Ceil(currentTimer);
    }

    public bool ShouldPlayBattleBgm()
    {
        return shouldPlayBattleBgm;
    }

    public bool IsGameStarted()
    {
        return isGameStarted;
    }
}