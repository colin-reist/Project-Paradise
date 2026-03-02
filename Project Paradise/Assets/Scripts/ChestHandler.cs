using UnityEngine;
using UnityEngine.InputSystem;

public class ChestHandler : MonoBehaviour, Iinteractible
{
    private bool _isPlayerNear = false;
    private bool _isOpened = false;
    private GameObject _chestUiInstance;

    [Header("Inventory")]
    [SerializeField] private GameObject prefab;
    
    [Header("General")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private SpriteRenderer keySpriteRenderer;
    
    private Camera _cam;

    private void Awake()
    {
        _cam = Camera.main;
    }

    void Start()
    {
        keySpriteRenderer.enabled = false;
    }

    // Called when another collider enter in contact with this Rigidbody2D (2D physics)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            _isPlayerNear = true;
            keySpriteRenderer.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            _isPlayerNear = false;
            CloseChest();
            keySpriteRenderer.enabled = false;
        }
    }

    void Update()
    {
        
    }
    
    private void OpenChest()
    {
        Debug.Log("Chest opened!");
        Destroy(_chestUiInstance);

        if (!prefab)
        {
            Debug.LogWarning("Chest-inventory prefab not found in Resources/Prefabs.");
            return;
        }

        Debug.Log("Chest opened!");
        float targetZ = 0f;
        Vector3 spawnPos;
        if (_cam)
        {
            float distance = Mathf.Abs(_cam.transform.position.z - targetZ);
            spawnPos = _cam.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, distance));
            spawnPos.z = targetZ;
        }
        else
        {
            spawnPos = new Vector3(0f, 0f, targetZ);
        }

        _chestUiInstance = Instantiate(prefab, spawnPos, Quaternion.identity,  _cam.transform);
        _chestUiInstance.transform.localScale = new Vector3(1f, 1f, 1f) * 8f;
        _isOpened = true;
    }

    private void CloseChest()
    {
        Destroy(_chestUiInstance);
        _chestUiInstance = null;
        _isOpened = false;
    }

    public void Interact()
    {
        if (_isOpened)
            CloseChest();
        else
            OpenChest();
    }
}
