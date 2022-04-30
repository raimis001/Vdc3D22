using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkController : MonoBehaviour
{
    public Transform chunk;
    public float chunkSpeed = 5;

    public GameObject[] prefabs;

    int currentChunk = 0;

    readonly List<GameObject> chunkList = new List<GameObject>();

    private void Start()
    {
        CreateChunk(0);
        CreateChunk(1);
    }

    void Update()
    {
        if (!PlayerController.IsLive)
            return;

        chunk.Translate(0, 0, -Time.deltaTime * chunkSpeed);

        int c = Mathf.FloorToInt(Mathf.Abs(chunk.position.z / 200f));
        if (c != currentChunk)
        {
            ChangeChunk(c);
        }
    }

    void ChangeChunk(int c)
    {
        GameObject g;
        if (chunkList.Count > 0)
        {
            g = chunkList[0];
            Destroy(g);
            chunkList.RemoveAt(0);
        }

        CreateChunk(c + 1);

        currentChunk = c;
    }

    void CreateChunk(int c)
    {
        GameObject p = prefabs[Random.Range(0, prefabs.Length)];

        GameObject g = Instantiate(p, chunk);
        Vector3 pos = Vector3.zero;
        pos.z = c * 200;

        g.transform.localPosition = pos;
        chunkList.Add(g);
    }
}
