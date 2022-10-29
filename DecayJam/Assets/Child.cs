using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Child : MonoBehaviour
{
    [Header("Capturing")]
    [SerializeField] Transform bar;
    [SerializeField] float captureSpeed;

    [Header("Speeds")]
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;

    [Header("Path")]
    [SerializeField] int currentPathIndex; // Should be closest point but oh well 
    [SerializeField] float minDisBeforeMeet;
    [SerializeField] float checkRadius;
    [SerializeField] LayerMask terrainMask;

    private List<Vector3> path;
    private float captureAmount;

    private Vector3 runAwayVec;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(captureAmount > 0)
        {
            bar.transform.localScale = new Vector3(captureAmount, 0.2f, 1);
            if(captureAmount <= 1)
            {
                RunAway();
            }
        }
        else
        {
            // Still not attacked 
            FollowPath();
        }
    }

    private void RunAway()
    {
        this.transform.position += runAwayVec * runSpeed * Time.deltaTime;
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, checkRadius, terrainMask);

        if(colliders.Length > 0)
        {
            runAwayVec = (this.transform.position - colliders[0].ClosestPoint(this.transform.position)).normalized;
        }

    }

    /// <summary>
    /// Lets the character walk Around the given path 
    /// </summary>
    private void FollowPath()
    {
        Vector3 vec = (path[currentPathIndex] - this.transform.position).normalized;

        // TODO: Spline 
        if(Vector3.Distance(this.transform.position, path[currentPathIndex]) < minDisBeforeMeet)
        {
            if(currentPathIndex + 1 < path.Count)
            {
                currentPathIndex++;
            }
            else
            {
                // Loops
                currentPathIndex = 0;
            }
        }

        this.transform.position += vec * walkSpeed * Time.deltaTime;
    }

    /// <summary>
    /// When creating a child set its path to follow a set of points 
    /// </summary>
    /// <param name="_path"></param>
    public void SetPath(List<Vector3> _path, int _index)
    {
        path = _path;
        currentPathIndex = _index;
    }

    public bool CaptureChild(Transform witch)
    {
        if(captureAmount > 1)
        {
            return true;
        }

        captureAmount += captureSpeed * Time.deltaTime;
        bool childCaptured = captureAmount >= 1;

        if(runAwayVec == Vector3.zero)
        {
            runAwayVec = (this.transform.position - witch.position).normalized;
        }

        if (childCaptured)
        {
            // Cease movment 

            // Turn to spectar 

            // Follow witch 
        }

        return childCaptured;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, checkRadius);
    }
}
