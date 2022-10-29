using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Child : MonoBehaviour
{
    [Header("Capturing")]
    [SerializeField] Transform bar;
    [SerializeField] float captureSpeed;
    [SerializeField] float delayBeforeAnim;
    [SerializeField] float animSpeed;

    [Header("Speeds")]
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;

    [Header("Path")]
    [SerializeField] int currentPathIndex; // Should be closest point but oh well 
    [SerializeField] float minDisBeforeMeet;
    [SerializeField] float checkRadius;
    [SerializeField] LayerMask terrainMask;

    [Header("Animation")]
    [SerializeField] Texture[] front;
    [SerializeField] Texture[] turnToAsh;

    private GameManager gm;
    private List<Vector3> path;
    private float captureAmount;

    private Vector3 runAwayVec;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(captureAmount > 0)
        {
            if(captureAmount <= 1)
            {
                bar.transform.localScale = new Vector3(captureAmount, 0.2f, 1);
                RunAway();
            }
            else
            {
                // Fully Captured 
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
            // Turn to spectar 
            bar.localScale = Vector3.zero;
            gm.ConsumeChild();
            StartCoroutine(TurnToAshCo());
            // Follow witch 
        }

        return childCaptured;
    }

    private IEnumerator TurnToAshCo()
    {
        MiniAnimator mini = this.GetComponent<MiniAnimator>();
        mini.frames = turnToAsh;
        mini.currentIndex = 0;
        mini.active = false;
        mini.PlayAnimation(); // Set first frame 

        yield return new WaitForSeconds(delayBeforeAnim);

        mini.TimePerFram = animSpeed;
        mini.active = true;

        float totalTime = animSpeed * (turnToAsh.Length + 1);
        yield return new WaitForSeconds(totalTime);
        mini.active = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, checkRadius);
    }
}
