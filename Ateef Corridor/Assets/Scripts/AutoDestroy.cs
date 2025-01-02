using UnityEngine;
using Photon.Pun;

public class AutoDestroy : MonoBehaviourPun
{
    private float setActiveTime = 3.0f;
    private float lifeTime = 6.0f;
    
    void Start()
    {
        if (photonView.IsMine)
        {
            Invoke(nameof(DestroySelf), lifeTime);
        }
    }

    private void SetActive()
    {
        this.gameObject.SetActive(true);
    }

    private void DestroySelf()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
