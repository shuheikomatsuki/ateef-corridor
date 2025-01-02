using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class FlashLightManager : MonoBehaviourPunCallbacks
{
    private float range = 2f;                 // 懐中電灯の射程距離（1m）
    private float angle = 30f;                // 光の視野角（左右15°で合計30°）
    private float damagePerSecond = 10f;      // ダメージ量
    private float recoveringPerSecond = 20f;  // 照射中のHP増加速度
    private float batteryDrainPerSecond = 4f;
    private float maxBatteryOfFlashLight = 100f;
    private float batteryOfFlashLight;
    private bool isFlashLightBroken = false;
    private float debuffDuration = 10f;

    public LayerMask ghostLayer;             // 判定対象のレイヤー（Ghost）
    public LayerMask humanLayer;
    
    private Light flashLight;
    private HumanMoving humanMoving;
    private bool isFlashLightOn = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        batteryOfFlashLight = maxBatteryOfFlashLight;

        Transform flashLightTransform = this.transform.GetChild(1);
        flashLight = flashLightTransform.GetComponent<Light>();
        flashLight.enabled = false;
        humanMoving = this.transform.GetParentComponent<HumanMoving>();
        // if (humanMoving == null)
        // {
        //     Debug.LogError("humanMoving == null");
        // }
    }

    // Update is called once per frame
    void Update()
    {
        DecreaseBattery();
        CheckForGhosts();
        RecoverHuman();
        if (Input.GetKeyDown(KeyCode.Space) && photonView.IsMine && !humanMoving.CheckDeath())
        {
            ToggleFlashLight();
        }
        if (humanMoving.CheckDeath())
        {
            flashLight.enabled = false;
            isFlashLightOn = false;
        }
    }

    IEnumerator CallFunctionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EnableToUseFlashLight();
    }

    public void UnableToUseFlashLight()
    {
        isFlashLightBroken = true;
        flashLight.enabled = false;
        StartCoroutine(CallFunctionAfterDelay(debuffDuration));
        // Debug.Log("懐中電灯が壊れた!");
    }

    public void EnableToUseFlashLight()
    {
        isFlashLightBroken = false;
        // Debug.Log("懐中電灯が正常化した!");
    }

    public void ResetFlashLight()
    {
        isFlashLightBroken = false;
        batteryOfFlashLight = maxBatteryOfFlashLight;
    }

    private void ToggleFlashLight()
    {
        if (batteryOfFlashLight > 0 && !isFlashLightBroken)
        {
            photonView.RPC(nameof(RpcToggleFlashLight), RpcTarget.All);
        }
    }

    [PunRPC]
    public void RpcToggleFlashLight()
    {
        isFlashLightOn = !isFlashLightOn;
        flashLight.enabled = isFlashLightOn;
    }

    public void ReplaceYellowBattery()
    {
        batteryOfFlashLight += 20;
        if (batteryOfFlashLight >= maxBatteryOfFlashLight)  //バッテリーが100を超えた場合
        {
            batteryOfFlashLight = maxBatteryOfFlashLight;
        }
    }
    public void ReplaceBlueBattery()
    {
        batteryOfFlashLight = maxBatteryOfFlashLight;
    }
    
    private void DecreaseBattery()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (flashLight.enabled && batteryOfFlashLight > 0 && !isFlashLightBroken)
        {
            batteryOfFlashLight -= batteryDrainPerSecond * Time.deltaTime;
            batteryOfFlashLight = Mathf.Clamp(batteryOfFlashLight, 0, maxBatteryOfFlashLight);  // Batteryが0未満にならないよう制限
            if (batteryOfFlashLight == 0)
            {
                // Debug.Log("バッテリーが0");
                flashLight.enabled = false;
            }
        }
    }

    private void CheckForGhosts()
    {
        if (batteryOfFlashLight == 0 || !flashLight.enabled || isFlashLightBroken)
        {
            return;
        }

        // 懐中電灯の位置と前方を取得
        Vector3 flashlightPosition = transform.position;
        Vector3 flashlightForward = transform.forward;

        // 範囲内のすべてのコライダーを取得
        Collider[] colliders = Physics.OverlapSphere(flashlightPosition, range, ghostLayer);

        foreach (Collider collider in colliders)
        {
            // 対象の位置との方向ベクトルを計算
            Vector3 directionToTarget = (collider.transform.position - flashlightPosition).normalized;

            // 対象との角度を計算
            float angleToTarget = Vector3.Angle(flashlightForward, directionToTarget);

            // 視野角内かつ射程内の場合、ダメージを与える
            if (angleToTarget <= angle)
            {
                GhostHealth ghostHealth = collider.GetComponent<GhostHealth>();
                if (ghostHealth != null)
                {
                    //  ------------------------------------------------------- テスト --------------------------------------------------------------
                    // ghostHealth.TakeDamage(damagePerSecond * Time.deltaTime);  // ダメージを与える
                    // photonView.RPC(nameof(ghostHealth.Rpc_TakeDamage), RpcTarget.AllBufferedViaServer, damagePerSecond * Time.deltaTime);  // ダメージを与える
                    ghostHealth.Rpc_TakeDamage(damagePerSecond * Time.deltaTime);
                    //  ------------------------------------------------------- テスト --------------------------------------------------------------
                    // Debug.Log("ゴーストにダメージ!");
                }
            }
        }
    }

    
    private void RecoverHuman()
    {
        if (batteryOfFlashLight == 0 || !flashLight.enabled || isFlashLightBroken)
        {
            return;
        }

        // 懐中電灯の位置と前方を取得
        Vector3 flashlightPosition = transform.position;
        Vector3 flashlightForward = transform.forward;

        // 範囲内のすべてのコライダーを取得
        Collider[] colliders = Physics.OverlapSphere(flashlightPosition, range, humanLayer);

        foreach (Collider collider in colliders)
        {
            // 対象の位置との方向ベクトルを計算
            Vector3 directionToTarget = (collider.transform.position - flashlightPosition).normalized;

            // 対象との角度を計算
            float angleToTarget = Vector3.Angle(flashlightForward, directionToTarget);

            // 視野角内かつ射程内の場合、回復させる
            if (angleToTarget <= angle)
            {
                HumanHealth humanHealth = collider.GetComponent<HumanHealth>();
                if (humanHealth != null)
                {
                    //  ------------------------------------ テスト -------------------------------------------------
                    humanHealth.Recover(recoveringPerSecond * Time.deltaTime);  // 回復させる
                    //  ------------------------------------ テスト -------------------------------------------------
                }
            }
        }
    }
}
