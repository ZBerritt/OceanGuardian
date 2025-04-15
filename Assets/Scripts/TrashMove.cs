using System;
using UnityEngine;

public class TrashMove : MonoBehaviour
{
    private float speed;
    private MiniGameManager manager;
    private float destroyX = -2.2f;

    public void Init(float moveSpeed, MiniGameManager mgr)
    {
        speed = moveSpeed;
        manager = mgr;
    }

    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x < destroyX)
        {
            manager.removeTrashFromQueue(gameObject);
        }
    }
}
