using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public GameObject gameManager;
    private NetWorkConnection netWorkConnection;
    private bool isGameStarted = false;

    public AudioSource battleAudioSource;
    public AudioSource humanDieAudioSource;
    public AudioSource gameStartedAudio;
    public AudioSource hitTheLightAudio;
    public AudioSource runningAudio;
    public AudioSource humansWinAudio;
    public AudioSource ghostWinsAudio;
    public AudioSource chargingAudio;
    public AudioSource specialAttackingAudio;
    public AudioSource attackZoneAudio;
    public AudioSource approachingAudio;
    public AudioSource speedMovingAudio;

    void Start()
    {
        netWorkConnection = gameManager.GetComponent<NetWorkConnection>();
    }

    void Update()
    {
    }

    public void PlayBattleAudio()
    {
        battleAudioSource.Play();
    }

    public void StopBattleAudio()
    {
        battleAudioSource.Stop();
    }

    public void DecreaseBattleAudioVolume()
    {
        battleAudioSource.volume = 0f;
    }

    public void ResetBattleAudioVolume()
    {
        battleAudioSource.volume = 0.4f;
    }

    public void PlayHumanDieAudio()
    {
        humanDieAudioSource.Play();
    }

    public void PlayGameStartedAudio()
    {
        gameStartedAudio.Play();
    }

    public void PlayHitTheLightAudio()
    {
        hitTheLightAudio.Play();
    }

    public void PlayRunningAudio()
    {
        runningAudio.Play();
    }

    public void StopRunningAudio()
    {
        runningAudio.Stop();
    }

    public void PlayHumansWinAudio()
    {
        humansWinAudio.Play();
    }

    public void PlayGhostWinsAudio()
    {
        ghostWinsAudio.Play();
    }

    public void PlayChargingAudio()
    {
        chargingAudio.Play();
    }

    public void StopChargingAudio()
    {
        chargingAudio.Stop();
    }
    
    public void PlaySpecialAttackingAudio()
    {
        specialAttackingAudio.Play();
    }

    public void PlayAttackZoneAudio()
    {
        attackZoneAudio.Play();
    }

    public void PlayApproachingAudio()
    {
        approachingAudio.Play();
        approachingAudio.volume = 0.15f;
    }

    public void StopApproachingAudio()
    {
        approachingAudio.Stop();
        approachingAudio.volume = 0.15f;
    }

    public void IncreaseApproachingAudioVolume()
    {
        approachingAudio.volume = 0.5f;
    }

    public void ResetApproachingAudioVolume()
    {
        approachingAudio.volume = 0.15f;
    }

    public void PlaySpeedMovingAudio()
    {
        speedMovingAudio.Play();
    }

    public void StopSpeedMovingAudio()
    {
        speedMovingAudio.Stop();
    }
}
