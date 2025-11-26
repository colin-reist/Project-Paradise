using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

public class ChestHandler : MonoBehaviour
{
    private BoxCollider2D bc;
    private bool isPlayerNear = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
    }

    // Called when another collider enter in contact with this Rigidbody2D (2D physics)
    void OnTriggerEnter2D(Collider2D collision)
    {
        isPlayerNear = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        isPlayerNear = false;
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Chest opened!");
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Chest-inventory");
            if (prefab == null)
            {
                Debug.LogWarning("Chest-inventory prefab not found in Resources/Prefabs");
            }
            else
            {
                Camera cam = Camera.main;
                float targetZ = 0f; // desired world Z for the spawned object
                Vector3 spawnPos;
                if (cam != null)
                {
                    float distance = Mathf.Abs(cam.transform.position.z - targetZ);
                    spawnPos = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, distance));
                    spawnPos.z = targetZ;
                }
                else
                {
                    spawnPos = new Vector3(0f, 0f, targetZ);
                }

                Instantiate(prefab, spawnPos, Quaternion.identity);
            }
        }
    }
}
