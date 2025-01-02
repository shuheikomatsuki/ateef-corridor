using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

public class GhostAppearance : MonoBehaviourPunCallbacks
{
    private int defaultLayer;
    private int ghostLayer;
    private int runningLayer;
    private float ghostAppearingTime = 4f;

    Coroutine resetLayerCoroutine;
    Transform ghostObject;
    Renderer ghostRenderer;
    Material ghostMaterial;

    private GameObject gameManager;
    private AudioManager audioManager;

    private bool isPlayingRunningAudio = false;

    private const string GHOST_COLOR_PROPERTY_NAME = "_MainColor";

    void Start()
    {
        defaultLayer = LayerMask.NameToLayer("Default");
        ghostLayer = LayerMask.NameToLayer("Ghost");
        runningLayer = LayerMask.NameToLayer("Running");

        ghostObject = this.transform.GetChild(0);
        ghostRenderer = ghostObject.GetComponent<Renderer>();
        ghostMaterial = ghostRenderer.material;
        ghostMaterial.SetColor(GHOST_COLOR_PROPERTY_NAME, Color.yellow);

        gameManager = GameObject.Find("GameManager");
        audioManager = gameManager.GetComponent<AudioManager>();
    }

    void Update()
    {
        if (ghostObject.gameObject.layer == runningLayer && !isPlayingRunningAudio)
        {
            audioManager.PlayRunningAudio();
            audioManager.PlayHitTheLightAudio();
            isPlayingRunningAudio = true;
        }
        if (ghostObject.gameObject.layer != runningLayer && isPlayingRunningAudio)
        {
            audioManager.StopRunningAudio();
            isPlayingRunningAudio = false;
        }
    }

    public void HandleCoroutine()
    {
        photonView.RPC(nameof(RpcSetRunningLayer), RpcTarget.AllViaServer);

        // 既存のコルーチンがあれば停止
        if (resetLayerCoroutine != null)
        {
            StopCoroutine(resetLayerCoroutine);
        }
        // 新しいコルーチンを開始
        resetLayerCoroutine = StartCoroutine(ResetLayerAfterDelay());
    }

    [PunRPC]
    private void ChangeColorToRed()
    {
        ghostMaterial.SetColor(GHOST_COLOR_PROPERTY_NAME, Color.red);
    }

    [PunRPC]
    private void ChangeColorToYellow()
    {
        ghostMaterial.SetColor(GHOST_COLOR_PROPERTY_NAME, Color.yellow);
    }

    [PunRPC]
    public void RpcSetDefaultLayer()
    {
        ghostObject.gameObject.layer = defaultLayer;
    }

    [PunRPC]
    public void RpcSetRunningLayer()
    {
        ghostObject.gameObject.layer = runningLayer;
        this.gameObject.tag = "Untagged";
        ChangeColorToRed();
    }

    [PunRPC]
    public void RpcSetGhostLayer()
    {
        ghostObject.gameObject.layer = ghostLayer;
        this.gameObject.tag = "Ghost";
        ChangeColorToYellow();
    }

    private IEnumerator ResetLayerAfterDelay()
    {
        yield return new WaitForSeconds(ghostAppearingTime);
        photonView.RPC(nameof(RpcSetGhostLayer), RpcTarget.AllViaServer);
        resetLayerCoroutine = null; // コルーチンをリセット
    }
}
