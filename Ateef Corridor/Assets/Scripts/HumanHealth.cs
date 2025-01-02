using UnityEngine;
using Photon.Pun;

public class HumanHealth : MonoBehaviourPun
{
    private float maxHealth = 100f;  // 最大HP
    private float currentHealth;
    private bool isLit = false;    // ライトに照らされているか

    private bool isDead;

    private CustomProperty customProperty;
    
    const string GHOST_HP_STRING = "hHP";

    private GameObject gameManager;
    private HumanMoving humanMoving;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        humanMoving = GetComponent<HumanMoving>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Recover(float amount)
    {
        if (!humanMoving.CheckDeath())
        {
            return;
        }
        Debug.Log("回復中");
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);  // HPが0未満にならないよう制限
        if (currentHealth == 100)
        {
            Revive();  // HPが100になった場合
        }
    }

    private void Revive()
    {
        humanMoving.StandUp();
    }

    public void SetCurrentHumanHpZero()
    {
        currentHealth = 0;
    }
}
