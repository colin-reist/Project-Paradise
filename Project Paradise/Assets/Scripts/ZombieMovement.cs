using System;
using UnityEngine;

public class ZombieMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GameObject playerToFollow;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Transform playerPosition = playerToFollow?.GetComponent<Transform>();
    }
}
