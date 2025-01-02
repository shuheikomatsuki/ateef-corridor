using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class GhostMoving : MonoBehaviourPunCallbacks
{
    private float moveSpeed = 0.05f; // 移動速度
    private float rotationSmoothTime = 0.1f; // 回転の滑らかさ
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private float targetAngle;
    private float currentAngle;
    private float rotationVelocity;
    private float timerForSpecialAttack = 0;
    private float speedUp = 1f;
    private bool specialAtaccking;

    private int defaultLayer;
    private int ghostLayer;

    private Animator animator;

    Transform ghostObject;

    private GameObject gameManager;
    private CustomProperty customProperty;
    private NetWorkConnection netWorkConnection;
    private GhostHealth ghostHealth;
    private GhostAppearance ghostAppearance;
    private GhostSpecialAttack ghostSpecialAttack;
    private GameObject humanGameObject;
    private AudioManager audioManager;
    private TimerManager timerManager;

    private bool isGameStarted = false;

    void Start()
    {
        ghostObject = this.transform.GetChild(0);
        defaultLayer = LayerMask.NameToLayer("Default");
        ghostLayer = LayerMask.NameToLayer("Ghost");

        ghostHealth = GetComponent<GhostHealth>();
        ghostAppearance = GetComponent<GhostAppearance>();
        ghostSpecialAttack = GetComponent<GhostSpecialAttack>();

        gameManager = GameObject.Find("GameManager");
        customProperty = gameManager.GetComponent<CustomProperty>();
        netWorkConnection = gameManager.GetComponent<NetWorkConnection>();
        audioManager = gameManager.GetComponent<AudioManager>();
        timerManager = gameManager.GetComponent<TimerManager>();
    }

    void Update()
    {
        if (timerManager.GetRemainingTimer() <= 0)
        {
            return;
        }
        EnableToMove();
        if (!photonView.IsMine || !isGameStarted)
        {
            return;
        }
        HandleInput();
        SpecialAttack();
        HandleLayerChange(); // レイヤー変更処理
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine || !isGameStarted)
        {
            return;
        }
        Move();
    }

    private void EnableToMove()
    {
        if (isGameStarted)
        {
            return;
        }
        if (netWorkConnection.IsGameStarted())
        {
            isGameStarted = true;
            // photonView.RPC(nameof(ghostHealth.RpcSetGhostLayer), RpcTarget.All);
            photonView.RPC(nameof(ghostAppearance.RpcSetGhostLayer), RpcTarget.All);
            // Debug.Log("ゴーストは動ける");
        }
    }

    private void HandleInput()
    {
        // 入力を取得
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // 入力方向を計算 (normalizedはベクトルを長さ1に正規化する)
        moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // プレイヤーが移動する場合のみ方向を計算
        if (moveDirection.magnitude >= 0.1f)
        {
            // ターゲットの角度を計算（回転方向を正しく修正）
            // 座標(-1,0)の場合はπラジアン(180°)であり、それを180°に変換
            targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

            // 現在の角度を滑らかに目標角度に近づける
            currentAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);

            // 回転を適用
            transform.rotation = Quaternion.Euler(0f, currentAngle, 0f);
        }
    }

    private void Move()
    {
        if (specialAtaccking)
        {
            return;
        }
        
        speedUp = 1f;
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            audioManager.PlaySpeedMovingAudio();
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speedUp = 1.5f;
        }
        else
        {
            audioManager.StopSpeedMovingAudio();
        }

        // 入力がある場合、移動する
        if (moveDirection.magnitude >= 0.1f)
        {
            Vector3 forwardMove = transform.forward * moveDirection.magnitude * moveSpeed;
            transform.position += forwardMove * speedUp;
        }
    }

    private void SpecialAttack()
    {
        //  Audio管理
        if (Input.GetKeyDown(KeyCode.Space))
        {
            audioManager.PlayChargingAudio();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            audioManager.StopChargingAudio();
        }

        //  特署攻撃の処理
        if (Input.GetKey(KeyCode.Space))
        {
            specialAtaccking = true;
            timerForSpecialAttack += Time.deltaTime; // 押し続けている間、タイマーを加算
        }
        else
        {
            specialAtaccking = false;
            timerForSpecialAttack = 0f;
        }
        if (timerForSpecialAttack > 3)
        {
            ghostSpecialAttack.SpecialAttack();
            audioManager.PlaySpecialAttackingAudio();
            timerForSpecialAttack = 0;
            // Debug.Log("特殊攻撃!");
        }
    }

    public float GetTimer()
    {
        return timerForSpecialAttack;
    }

    private void HandleLayerChange()
    {
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            // photonView.RPC(nameof(ghostHealth.RpcSetDefaultLayer), RpcTarget.All);
            photonView.RPC(nameof(ghostAppearance.RpcSetDefaultLayer), RpcTarget.All);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            // photonView.RPC(nameof(ghostHealth.RpcSetGhostLayer), RpcTarget.All);
            photonView.RPC(nameof(ghostAppearance.RpcSetGhostLayer), RpcTarget.All);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // photonView.RPC(nameof(ghostHealth.RpcSetDefaultLayer), RpcTarget.All);
            photonView.RPC(nameof(ghostAppearance.RpcSetDefaultLayer), RpcTarget.All);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            // photonView.RPC(nameof(ghostHealth.RpcSetGhostLayer), RpcTarget.All);
            photonView.RPC(nameof(ghostAppearance.RpcSetGhostLayer), RpcTarget.All);
        }
    }

    private IEnumerator enumerator(float delay)
    {
        yield return new WaitForSeconds(delay);
        // photonView.RPC(nameof(ghostHealth.RpcSetGhostLayer), RpcTarget.All);
        photonView.RPC(nameof(ghostAppearance.RpcSetGhostLayer), RpcTarget.All);
    }

    //  Playerとの衝突を確認
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {   
            if (this.gameObject.tag != "Ghost")
            {
                // Debug.Log("このGameObjectのTagは、Ghostではありません");
                return;
            }
            photonView.RPC(nameof(ghostAppearance.RpcSetDefaultLayer), RpcTarget.AllViaServer);
            StartCoroutine(enumerator(3f));
            audioManager.PlayHumanDieAudio();
            audioManager.DecreaseBattleAudioVolume();
            StartCoroutine(nameof(CallFunctionAfterDelay), 3f);
        }
    }

    private IEnumerator CallFunctionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        audioManager.ResetBattleAudioVolume();
    }
}
