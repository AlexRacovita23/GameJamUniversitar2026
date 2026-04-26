using System;
using UnityEngine;

public class TempleController : MonoBehaviour
{
    [Header("Animation values")]
    [SerializeField] private float riseSpeed = 0.1f;
    [SerializeField] private float maxHeight = 1.09f;

    public GameObject finalCard;

    public bool isRising = false;

    // Update is called once per frame
    void Update()
    {
        if (!isRising)
            return;

        if (transform.position.y < maxHeight)
        {
            transform.position += Vector3.up * riseSpeed * Time.deltaTime;
            AudioManager.Instance.PlayEarthquake();
        }

        if (transform.position.y >= maxHeight)
        {
            transform.position = new Vector3(transform.position.x, maxHeight, transform.position.z);
            AudioManager.Instance.StopEarthquake();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Temple triggered by: " + other.name);
        if (other.CompareTag("Player"))
        {
            if (transform.position.y < maxHeight)
            {
                Debug.Log("Temple is not fully risen yet. Current height: " + transform.position.y);
                return;
            }
            Debug.Log("Temple activated! Game Over!");
            finalCard.SetActive(true);
            other.GetComponentInParent<PlayerMovement>()?.ChangeCoursorState();
            // Time.timeScale = 0f; // Pause the game
        }
    }
}
