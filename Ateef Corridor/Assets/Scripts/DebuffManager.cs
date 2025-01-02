using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class DebuffManager : MonoBehaviourPunCallbacks
{
    public ParticleSystem particleSystem;
    private float debuffDuration = 10f;

    IEnumerator CallFunctionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        RPC_HideDebuffParticle();
    }

    public void ShowDebuffParticle()
    {
        if (!photonView.IsMine) return;

        if (!particleSystem.gameObject.activeSelf)
        {
            photonView.RPC(nameof(RPC_ShowDebuffParticle), RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_ShowDebuffParticle()
    {
        particleSystem.gameObject.SetActive(true);
        StartCoroutine(CallFunctionAfterDelay(debuffDuration));
    }

    [PunRPC]
    private void RPC_HideDebuffParticle()
    {
        particleSystem.gameObject.SetActive(false);
    }
}
