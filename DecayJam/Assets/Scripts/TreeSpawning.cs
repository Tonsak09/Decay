using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TreeSpawning : MonoBehaviour
{
    [SerializeField] Vector3 start;
    [SerializeField] Vector3 end;

    [SerializeField] List<GameObject> treeSets;
    [SerializeField] int count;
    [SerializeField] bool spawn;
    [SerializeField] bool cleaup;

    [Header("Transforms")]
    [SerializeField] Vector3 scales;
    [SerializeField] Vector3 rots;

    public List<GameObject> trees;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(spawn)
        {
            spawn = false;
            CleanUp();
            SpawnTrees();
        }

        if(cleaup)
        {
            cleaup = false;
            CleanUp();
        }
    }

    private void CleanUp()
    {
        for (int i = trees.Count - 1; i >=  0; i--)
        {
            DestroyImmediate(trees[i]);
            trees.RemoveAt(i);
        }
    }

    private void SpawnTrees()
    {
        float dis = Vector3.Distance(start, end);

        float spacing = dis / count;
        Vector3 vec = (end - start).normalized;
        for (int i = 0; i < count; i++)
        {
            GameObject temp = Instantiate(treeSets[Random.Range(0, treeSets.Count)], start + vec * spacing * i, Quaternion.Euler(rots));
            temp.transform.localScale = scales;
            trees.Add(temp);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(start, 0.1f);
        Gizmos.DrawWireSphere(end, 0.1f);
        Gizmos.DrawLine(start, end);
    }
}
