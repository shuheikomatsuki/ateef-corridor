using UnityEngine;
using Photon.Pun;
using System.Collections;

public class GhostHealth : MonoBehaviourPunCallbacks
{
    private float maxHealth = 100f;  // 最大HP
    private float currentHealth;
    private bool isLit = false;    // ライトに照らされているか
    private float damagePerSecond = 5f;  // 照射中のHP減少速度
    private bool isDead = false;

    const string GHOST_HP_STRING = "gHP";

    private GameObject gameManager;
    private CustomProperty customProperty;
    private GhostAppearance ghostAppearance;
    private VictoryTextManager victoryTextManager;
    private AudioManager audioManager;

    private const string GHOST_COLOR_PROPERTY_NAME = "_MainColor";

    void Start()
    {
        currentHealth = maxHealth;  // 初期HPを設定
        gameManager = GameObject.Find("GameManager");
        customProperty = gameManager.GetComponent<CustomProperty>();
        victoryTextManager = gameManager.GetComponent<VictoryTextManager>();
        audioManager = gameManager.GetComponent<AudioManager>();
        ghostAppearance = GetComponent<GhostAppearance>();
    }

    [PunRPC]
    public void Rpc_TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);  // HPが0未満にならないよう制限

        // customProperty.DecreaseGhostHealth(GHOST_HP_STRING, currentHealth);

        ghostAppearance.HandleCoroutine();

        if (currentHealth <= 0)
        {
            // Die();
            photonView.RPC(nameof(Die), RpcTarget.AllBufferedViaServer);
        }
    }

    [PunRPC]
    private void Die()
    {
        Debug.Log("Ghost Defeated!");
        isDead = true;
        victoryTextManager.SetHumanWinText();
        audioManager.PlayHumansWinAudio();
        audioManager.StopBattleAudio();
        audioManager.StopApproachingAudio();
        PhotonNetwork.Destroy(gameObject);
    }

    public float GetGhostHealth()
    {
        return currentHealth;
    }
    
    public bool IsLifeZero()
    {
        return isDead;
    }
}