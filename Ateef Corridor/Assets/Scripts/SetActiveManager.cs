using UnityEngine;

public class SetActiveManager : MonoBehaviour
{
    public GameObject gameObject;

    // Update is called once per frame
    void Update()
    {
       gameObject = GameObject.Find("Player(Clone)");
       if (gameObject != null)
       {
        Debug.Log("オブジェクト: " + gameObject.name);
       }
       else 
       {
        Debug.Log("オブジェクトなし...");
       }
       gameObject.SetActive(true);
    }
}
