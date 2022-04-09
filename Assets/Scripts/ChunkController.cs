using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkController : MonoBehaviour
{
    public Transform chunk;
    public float chunkSpeed = 5;

    void Update()
    {
        if (!PlayerController.IsLive)
            return;

        chunk.Translate(0, 0, -Time.deltaTime * chunkSpeed);
    }
}
