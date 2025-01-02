using UnityEngine;
using Photon.Pun;

public class AutoSetActive : MonoBehaviourPun
{
    private GameObject targetObject;
    private float setActiveTime = 3.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        Transform zoneTransform = this.transform.GetChild(0);
        targetObject = zoneTransform.gameObject;
        if (photonView.IsMine)
        {
            Invoke(nameof(EnableCapsulCollider), setActiveTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void EnableCapsulCollider()
    {
        if (targetObject != null)
        {
            CapsuleCollider capsuleCollider = targetObject.GetComponent<CapsuleCollider>();
            if (capsuleCollider != null)
            {
                capsuleCollider.enabled = true;
                Debug.Log("カプセルコライダー設置完了");
            }
            else 
            {
                Debug.LogError("No CapsuleCollider.");
            }
        }
        else
        {
            Debug.LogError("ターゲットなし...");
        }
    }
}
