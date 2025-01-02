using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleImageManager : MonoBehaviour
{
    private int num;
    public GameObject[] gameObjects;
    [SerializeField] private GameObject titleTransitionText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // num = Random.Range(1, 9);
        // gameObjects[num].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            // gameObjects[num].SetActive(false);
            titleTransitionText.SetActive(false);
        }
    }
}
