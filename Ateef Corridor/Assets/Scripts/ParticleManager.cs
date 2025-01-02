using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ParticleManager : MonoBehaviourPunCallbacks
{
    GhostMoving ghostMoving;
    public ParticleSystem particleSystem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ghostMoving = GetComponent<GhostMoving>();
    }

    // Update is called once per frame
    void Update()
    {
        ShowParticle();
    }

    private void ShowParticle()
    {
        if (!photonView.IsMine) return;

        if (ghostMoving.GetTimer() > 0 && ghostMoving.GetTimer() < 3)
        {
            if (!particleSystem.gameObject.activeSelf)
            {
            //   photonView.RPC("RPC_SetParticleActive", RpcTarget.All, true);
            photonView.RPC(nameof(RPC_SetParticleActive), RpcTarget.All, true);
            }
        }
        else
        {
            if (particleSystem.gameObject.activeSelf)
            {
                // photonView.RPC("RPC_SetParticleActive", RpcTarget.All, false);
                photonView.RPC(nameof(RPC_SetParticleActive), RpcTarget.All, false);
            }
        }
    }

    [PunRPC]
    private void RPC_SetParticleActive(bool isActive)
    {
        particleSystem.gameObject.SetActive(isActive);
    }
}
