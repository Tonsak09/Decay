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
    [SerializeField] GameObject particleFx;

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
    [SerializeField] Texture[] back;
    [SerializeField] Texture[] right;
    [SerializeField] Texture[] left;
    [SerializeField] Texture[] turnToAsh;
    [SerializeField] Transform displayQuad;

    [Header("Sounds")]
    [SerializeField] AudioClip toDust;
    [SerializeField] AudioClip eyesLand;
    [SerializeField] AudioClip castSound;


    private MiniAnimator mini;

    private GameManager gm;
    private SoundManager sm;

    public Transform pathParent;
    private List<Vector3> path;
    private float captureAmount;

    private Vector3 moveVec;
    private bool bouncing;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindObjectOfType<GameManager>();
        sm = GameObject.FindObjectOfType<SoundManager>();
        mini = this.GetComponent<MiniAnimator>();
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

                MovingSprites();
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
            MovingSprites();
        }

       
    }

    private void RunAway()
    {
        this.transform.position += moveVec * runSpeed * Time.deltaTime;
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, checkRadius, terrainMask);

        if(colliders.Length > 0)
        {
            moveVec = (this.transform.position - colliders[0].ClosestPoint(this.transform.position)).normalized;
        }

    }

    private void MovingSprites()
    {
        // Always moving so always update 
        if (Mathf.Abs(moveVec.x) > Mathf.Abs(moveVec.z))
        {
            if (moveVec.x > 0)
            {
                // Right
                mini.frames = right;
                displayQuad.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                // Left
                mini.frames = left;
                displayQuad.localScale = new Vector3(1, 1, 1);
            }
        }
        else
        {
            if (moveVec.z > 0)
            {
                // Back
                mini.frames = back;
                displayQuad.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                // Forward 
                mini.frames = front;
                displayQuad.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    /// <summary>
    /// Lets the character walk Around the given path 
    /// </summary>
    private void FollowPath()
    {
        Vector3 pos = pathParent.position + pathParent.TransformDirection(path[currentPathIndex]);
        moveVec = (pos - this.transform.position).normalized;

        // TODO: Spline 
        if(Vector3.Distance(this.transform.position, pos) < minDisBeforeMeet)
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

        this.transform.position += moveVec * walkSpeed * Time.deltaTime;
    }

    /// <summary>
    /// When creating a child set its path to follow a set of points 
    /// </summary>
    /// <param name="_path"></param>
    public void SetPath(List<Vector3> _path, Transform parentPatrol, int _index)
    {
        path = _path;
        pathParent = parentPatrol;
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

        if (captureAmount > 0 && bouncing == false)
        {
            bouncing = true;
            sm.PlaySoundFX(castSound, this.transform.position, "CAST", 1, 1, 1);
            moveVec = (this.transform.position - witch.position).normalized;
        }

        if (childCaptured)
        {
            // Turn to spectar 
            bar.localScale = Vector3.zero;
            gm.ConsumeChild();
            pathParent.GetComponent<Patrol>().RemoveChild(this);

            Instantiate(particleFx, this.transform.position, Quaternion.Euler(-90, 0, 0));
            displayQuad.GetComponent<Renderer>().material.mainTexture = front[0];

            sm.PlaySoundFX(toDust, this.transform.position, "TODUST");
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
        sm.PlaySoundFX(eyesLand, this.transform.position, "EYES");
        mini.active = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, checkRadius);
    }
}
