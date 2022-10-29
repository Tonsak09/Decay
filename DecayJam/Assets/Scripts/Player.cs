using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameManager manager;
    [Header("Visauls")]
    [SerializeField] Transform displayQuad;
    [SerializeField] Material playerMaterial;
    [SerializeField] Texture front;
    [SerializeField] Texture back;
    [SerializeField] Texture right;
    [SerializeField] Texture left;

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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        SpellPositioning();
    }

    private void SpellPositioning()
    {
        if (Input.GetMouseButton(0))
        {
            //create a ray cast and set it to the mouses cursor position in game
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxDis, spellMask))
            {
                spell.transform.position = hit.point + offset;
                Collider[] colliders = Physics.OverlapSphere(hit.point, spellRadiusMax * spellSizeLerp, childMask);

                // Goes through list and captures every child 
                for (int i = 0; i < colliders.Length; i++)
                {
                    colliders[i].GetComponent<Child>().CaptureChild(this.transform);
                }

                spellSizeLerp = Mathf.Clamp01(spellSizeLerp + Time.deltaTime * appearSpeed);

            }
            else
            {
                spellSizeLerp = Mathf.Clamp01(spellSizeLerp - Time.deltaTime * disappearSpeed);
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
        if (horizontal != 0)
        {
            // Horizontal Movement 

            if (horizontal < 0)
            {
                // Right texture 
                playerMaterial.mainTexture = right;
                displayQuad.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                // Left texture 
                playerMaterial.mainTexture = left;
                displayQuad.localScale = new Vector3(1, 1, 1);
            }
        }

        if (vertical != 0)
        {
            // Vertical Movement 

            if (vertical < 0)
            {
                // Forward texture 
                playerMaterial.mainTexture = front;
                displayQuad.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                // Backward texture 
                playerMaterial.mainTexture = back;
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
