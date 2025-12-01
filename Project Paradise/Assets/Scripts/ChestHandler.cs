using UnityEngine;

public class ChestHandler : MonoBehaviour
{
    private bool isPlayerNear = false;
    private bool isOpened = false;
    private GameObject chestUiInstance;

    [SerializeField] private string playerTag = "Player";
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    // Called when another collider enter in contact with this Rigidbody2D (2D physics)
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            isPlayerNear = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            isPlayerNear = false;
            CloseChest();
        }
    }

    void Update()
    {
        if (!isPlayerNear)
        {
            return;
        }

        if (Input.GetKeyDown(interactKey))
        {
            if (isOpened)
                CloseChest();
            else
                OpenChest();
        }
    }

    private void OpenChest()
    {
        if (chestUiInstance != null)
        {
            Destroy(chestUiInstance);
        }

        GameObject prefab = Resources.Load<GameObject>("Prefabs/Chest-inventory");
        if (prefab == null)
        {
            Debug.LogWarning("Chest-inventory prefab not found in Resources/Prefabs.");
            return;
        }

        Debug.Log("Chest opened!");
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

        chestUiInstance = Instantiate(prefab, spawnPos, Quaternion.identity);
        chestUiInstance.transform.localScale = new Vector3(1f, 1f, 1f) * 18f;
        isOpened = true;
    }

    private void CloseChest()
    {
        if (chestUiInstance != null)
        {
            Destroy(chestUiInstance);
            chestUiInstance = null;
        }

        isOpened = false;
    }
}
