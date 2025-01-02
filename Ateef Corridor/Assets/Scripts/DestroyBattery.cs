using UnityEngine;
using System.Collections;
using Photon.Pun;

public class DestroyBattery : MonoBehaviourPun
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(nameof(enumerator), 29f);
    }

    // Update is called once per frame
    void Update()
    {

    }
    

    private IEnumerator enumerator(float delay)
    {
        yield return new WaitForSeconds(delay);
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
