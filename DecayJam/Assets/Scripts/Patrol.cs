using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{

    [SerializeField] List<Vector3> path;
    [SerializeField] List<Child> children;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < children.Count; i++)
        {
            int index = 0;
            float sqrtMag = 9999;
            // Finds closest point 
            for (int j = 0; j < path.Count; j++)
            {
                float tempDis = (path[j] - children[i].transform.position).sqrMagnitude;
                if (tempDis < sqrtMag)
                {
                    index = j;
                    sqrtMag = tempDis;
                }
            }

            children[i].SetPath(path, index);
        }
    }
    
    /// <summary>
    /// Adds a child randomly to some point along the path 
    /// </summary>
    public void AddChild()
    {
        
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
    }
}
