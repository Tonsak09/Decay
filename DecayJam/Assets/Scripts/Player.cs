using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameManager manager;
    [Header("Visauls")]
    [SerializeField] Transform displayQuad;

    [SerializeField] Transform finger;
    [SerializeField] Vector3 fingerOffsetPressed;
    [SerializeField] Vector3 fingerOffsetUnPressed;
    [SerializeField] Renderer fingerRend;
    [SerializeField] Texture unpressed;
    [SerializeField] Texture pressed;

    [Header("0%")]
    [SerializeField] Texture[] front0;
    [SerializeField] Texture[] back0;
    [SerializeField] Texture[] right0;
    [SerializeField] Texture[] left0;

    [Header("25%")]
    [SerializeField] Texture[] front25;
    [SerializeField] Texture[] back25;
    [SerializeField] Texture[] right25;
    [SerializeField] Texture[] left25;

    [Header("50%")]
    [SerializeField] Texture[] front50;
    [SerializeField] Texture[] back50;
    [SerializeField] Texture[] right50;
    [SerializeField] Texture[] left50;

    [Header("75%")]
    [SerializeField] Texture[] front75;
    [SerializeField] Texture[] back75;
    [SerializeField] Texture[] right75;
    [SerializeField] Texture[] left75;


    [Header("Controls")]
    [SerializeField] float speed;
    [SerializeField] float checkRadius;
    [SerializeField] float checkDis;
    [SerializeField] LayerMask terrainMask;

    [Header("Spell")]
    [SerializeField] Transform spell;

    [SerializeField] float spellRadiusMax;
    [SerializeField] LayerMask childMask;

    [Space]
    [SerializeField] float appearSpeed;
    [SerializeField] float disappearSpeed;
    [SerializeField] AnimationCurve spellSizeCurve;
    private float spellSizeLerp;

    [Header("Animation")]
    [SerializeField] float animSpeed;

    [Space]
    [SerializeField] LayerMask spellMask;
    [SerializeField] float maxDis;
    [SerializeField] Vector3 offset;

    private MiniAnimator mini;
    private SoundManager sm;

    public float startTime { get; set; }

    public bool active;

    // Start is called before the first frame update
    void Start()
    {
        mini = this.GetComponent<MiniAnimator>();
        sm = GameObject.FindObjectOfType<SoundManager>();
        active = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(active)
        {
            Movement();
            SpellPositioning();
        }
    }

    private void SpellPositioning()
    {

        //create a ray cast and set it to the mouses cursor position in game
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDis, spellMask))
        {
            Vector3 point = new  Vector3(hit.point.x, 0, hit.point.z);

            // Putting Spell down 
            if (Input.GetMouseButton(0))
            {
                spell.transform.position = point + offset;
                Collider[] colliders = Physics.OverlapSphere(point, spellRadiusMax * spellSizeLerp, childMask);

                // Goes through list and captures every child 
                for (int i = 0; i < colliders.Length; i++)
                {
                    colliders[i].GetComponent<Child>().CaptureChild(this.transform);
                    //sm.PlaySoundFX(castSound, this.transform.position, "CAST", 1, 1, 3);
                }

                spellSizeLerp = Mathf.Clamp01(spellSizeLerp + Time.deltaTime * appearSpeed);
                fingerRend.material.mainTexture = pressed;
                finger.position = point + fingerOffsetPressed;
            }
            else
            {
                // Not clicking 
                spellSizeLerp = Mathf.Clamp01(spellSizeLerp - Time.deltaTime * disappearSpeed);
                fingerRend.material.mainTexture = unpressed;
                finger.position = hit.point + fingerOffsetUnPressed;
            }

        }
        else
        {
            spellSizeLerp = Mathf.Clamp01(spellSizeLerp - Time.deltaTime * disappearSpeed);
        }

        
        

        spell.localScale = Vector3.one * spellSizeCurve.Evaluate(spellSizeLerp);
    }

    private void Movement()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        Vector3 direction = (new Vector3(horizontal, 0, vertical)).normalized;
        if (Physics.CheckSphere(this.transform.position + direction * checkDis, checkRadius, terrainMask))
        {
            return;
        }

        this.transform.position += direction * speed * Time.deltaTime;

        if(direction == Vector3.zero)
        {
            vertical = -1;
        }
        if (manager.TimeLeft <= (0.25f * startTime))
        {
            SetSprite(vertical, horizontal, front75, back75, right75, left75);
        }
        else if (manager.TimeLeft <= (0.50f * startTime))
        {
            SetSprite(vertical, horizontal, front50, back50, right50, left50);
        }
        else if (manager.TimeLeft <= (0.75f * startTime))
        {
            SetSprite(vertical, horizontal, front25, back25, right25, left25);
        }
        else
        {
            SetSprite(vertical, horizontal, front0, back0, right0, left0);
        }
    }

    private void SetSprite(float vertical, float horizontal, Texture[] front, Texture[] back, Texture[] right, Texture[] left)
    {
        if (horizontal != 0)
        {
            // Horizontal Movement 

            if (horizontal < 0)
            {
                // Right texture 
                mini.frames = right;
                displayQuad.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                // Left texture 
                mini.frames = left;
                displayQuad.localScale = new Vector3(1, 1, 1);
            }
        }

        if (vertical != 0)
        {
            // Vertical Movement 

            if (vertical < 0)
            {
                // Forward texture 
                mini.frames = front;
                displayQuad.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                // Backward texture 
                mini.frames = back;
                displayQuad.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDis, spellMask))
        {
            spell.transform.position = hit.point + offset;
            Gizmos.DrawWireSphere(hit.point + offset, spellRadiusMax * spellSizeLerp);
        }


        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        Vector3 direction = (new Vector3(horizontal, 0, vertical)).normalized;
        Gizmos.DrawWireSphere((this.transform.position + direction * checkDis), checkRadius);

    }
}
