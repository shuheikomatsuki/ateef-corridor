using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;

public class HumanMoving : MonoBehaviourPunCallbacks
{
    private float moveSpeed = 0.05f; // 移動速度
    private float rotationSmoothTime = 0.1f; // 回転の滑らかさ
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private float targetAngle;
    private float currentAngle;
    private float rotationVelocity;
    private bool isDead;
    private bool isGameStarted = false;

    private Animator animator;

    private const string ALIVE_NUM_STRING = "aliveNum";

    private HumanHealth humanHealth;
    private GameObject gameManager;
    private CustomProperty customProperty;
    private VictoryTextManager victoryTextManager;
    private DebuffManager debuffManager;
    private FlashLightManager flashLightManager;
    private NetWorkConnection netWorkConnection;
    private AudioManager audioManager;
    private TimerManager timerManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isDead = false;
        animator = GetComponent<Animator>();

        humanHealth = GetComponent<HumanHealth>();
        gameManager = GameObject.Find("GameManager");
        customProperty = gameManager.GetComponent<CustomProperty>();
        victoryTextManager = gameManager.GetComponent<VictoryTextManager>();
        netWorkConnection = gameManager.GetComponent<NetWorkConnection>();
        audioManager = gameManager.GetComponent<AudioManager>();
        debuffManager = GetComponent<DebuffManager>();
        timerManager = gameManager.GetComponent<TimerManager>();

        Transform flashLightTransform = this.transform.GetChild(3);
        flashLightManager = flashLightTransform.GetComponent<FlashLightManager>();
    }

    // Update is called once per frame
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
            flashLightManager.ResetFlashLight();
            // Debug.Log("人間は動ける!!");
        }
    }

    private void HandleInput()
    {
        //  死んでいるなら動けない
        if (isDead)
        {
            return;
        }

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
        //  死んでいるなら動けない
        if (isDead)
        {
            return;
        }

        // 入力がある場合、移動する
        if (moveDirection.magnitude >= 0.1f)
        {
            // Vector3 forwardMove = transform.forward * moveDirection.magnitude * moveSpeed * Time.fixedDeltaTime;
            Vector3 forwardMove = transform.forward * moveDirection.magnitude * moveSpeed;
            transform.position += forwardMove;

            //  Animation
            animator.SetBool("walk", true);
        }
        else
        {
            //  動いていない時は、 "Idle" アニメーション
            animator.SetBool("walk", false);
        }
    }

    //  Ghostとの衝突を確認
    void OnCollisionEnter(Collision collision)
    {
        //壁との衝突を検出
        if (collision.gameObject.CompareTag("Ghost") && !isDead)
        {
            isDead = true;
            animator.SetBool("death", true);
            humanHealth.SetCurrentHumanHpZero();
            if (!photonView.IsMine)
            {
                return;
            }
            photonView.RPC(nameof(NotifyServerOfDeath), RpcTarget.MasterClient);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead)
        {
            return;
        }
        if (other.gameObject.CompareTag("Battery_Yellow"))
        {
            flashLightManager.ReplaceYellowBattery();
        }
        if (other.gameObject.CompareTag("Battery_Blue"))
        {
            flashLightManager.ReplaceBlueBattery();
        }
        if (other.gameObject.CompareTag("AttackZone"))
        {
            flashLightManager.UnableToUseFlashLight();
            debuffManager.ShowDebuffParticle();
            Debug.Log("AttackZoneを接触!");
        }
        if (!photonView.IsMine)
        {
            return;
        }
        if (other.gameObject.CompareTag("OutsideZone"))
        {
            Debug.Log("OutsideZoneと接触");
            audioManager.PlayApproachingAudio();
        }
        if (other.gameObject.CompareTag("InsideZone"))
        {
            Debug.Log("InsideZoneと接触");
            audioManager.IncreaseApproachingAudioVolume();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (other.gameObject.CompareTag("InsideZone"))
        {
            audioManager.ResetApproachingAudioVolume();
        }
        if (other.gameObject.CompareTag("OutsideZone"))
        {
            audioManager.StopApproachingAudio();
        }
    }

    public void StandUp()
    {
        if (!isDead)
        {
            return;
        }
        isDead = false;
        animator.SetBool("death", false);
        if (!photonView.IsMine)
        {
            return;
        }
        photonView.RPC(nameof(NotifyServerOfRevival), RpcTarget.MasterClient);
        flashLightManager.ResetFlashLight();
    }

    public bool CheckDeath()
    {
        return isDead;
    }

    [PunRPC]
    private void NotifyServerOfDeath()
    {
        customProperty.DecreaseAliveHumans();
    }

    [PunRPC]
    private void NotifyServerOfRevival()
    {
        customProperty.IncreaseAliveHumans();
    }
}
