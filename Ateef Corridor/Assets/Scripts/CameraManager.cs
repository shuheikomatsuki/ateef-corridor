using UnityEngine;
using Photon.Pun;

public class CameraManager : MonoBehaviourPun
{
    public Camera camera;
    private bool dontCheck = false;

    // Update is called once per frame
    void Update()
    {
        if (dontCheck || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
        {
            return;
        }
        if (PhotonNetwork.IsMasterClient)
        {
            camera.cullingMask = -1;
            dontCheck = true;
        }
    }
}
