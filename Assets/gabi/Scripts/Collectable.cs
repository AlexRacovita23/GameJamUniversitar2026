using UnityEngine;

public class Collectable : MonoBehaviour
{ 
    public void CollectItem()
    {
        Debug.Log("Collecting item: " + gameObject.name);
        // Add any additional logic for collecting the item here (e.g., updating score, playing sound, etc.)
        Destroy(gameObject);
    }
}
