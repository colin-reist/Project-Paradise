using UnityEngine;

public class ChestHandler : MonoBehaviour, Iinteractible
{
    private bool _isOpened;
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

    public void BecomeTarget()
    {
        keySpriteRenderer.enabled = true;
    }

    public void RemoveTarget()
    {
        CloseChest();
        keySpriteRenderer.enabled = false;
    }
}
