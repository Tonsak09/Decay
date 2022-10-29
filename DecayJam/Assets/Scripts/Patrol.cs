using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{

    [SerializeField] float size;
    [SerializeField] List<Vector3> path;
    [SerializeField] List<Child> children;

    public float Size { get { return size; } }
    public bool needsChildren { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        SetUpChildren();
    }

    private void Update()
    {
        needsChildren = (children.Count <= 1);
    }

    /// <summary>
    /// Adds a child randomly to some point along the path 
    /// </summary>
    public void AddChild(Child child, float range)
    {
        children.Add(child);
        int index = Random.Range(0, path.Count);

        Vector3 pos = this.transform.position + this.transform.TransformDirection(path[index]);
        child.transform.position = pos + new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));
        child.pathParent = this.transform;

    }

    public void RemoveChild(Child child)
    {
        children.Remove(child);
    }

    public void SetUpChildren()
    {
        for (int i = 0; i < children.Count; i++)
        {
            int index = 0;
            float sqrtMag = 9999;
            // Finds closest point 
            for (int j = 0; j < path.Count; j++)
            {
                Vector3 pos = this.transform.position + this.transform.TransformDirection(path[j]);

                float tempDis = (pos - children[i].transform.position).sqrMagnitude;
                if (tempDis < sqrtMag)
                {
                    index = j;
                    sqrtMag = tempDis;
                }
            }

            children[i].SetPath(path, this.transform, index);
        }
    }

    /// <summary>
    /// Cleans up all chldren along this path 
    /// </summary>ee
    public void CleanUp()
    {
        
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < path.Count; i++)
        {
            Gizmos.color = Color.yellow;
            Vector3 pos = this.transform.position + this.transform.TransformDirection(path[i]);
            Gizmos.DrawWireSphere(pos, 0.1f);

            // Rendering path 
            Gizmos.color = Color.red;
            if (i + 1 < path.Count)
            {
                Gizmos.DrawLine(pos, this.transform.position + this.transform.TransformDirection(path[i + 1]));
            }
            else
            {
                Gizmos.DrawLine(pos, this.transform.position + this.transform.TransformDirection(path[0]));
            }
        }

        Gizmos.DrawWireSphere(this.transform.position, size);
    }
}
