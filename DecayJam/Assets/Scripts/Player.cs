using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Visauls")]
    [SerializeField] Material playerMaterial;
    [SerializeField] Texture front;
    [SerializeField] Texture back;
    [SerializeField] Texture right;
    [SerializeField] Texture left;

    [Header("Controls")]
    [SerializeField] float speed;

    [Header("Spell")]
    [SerializeField] Transform spell;
    [SerializeField] float spellRadiusMax;
    [SerializeField] LayerMask spellLayerMask;
    [Space]
    [SerializeField] float appearSpeed;
    [SerializeField] float disappearSpeed;
    [SerializeField] AnimationCurve spellSizeCurve;
    public float spellSizeLerp;

    [Space]
    [SerializeField] LayerMask mask;
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
            if (Physics.Raycast(ray, out hit, maxDis, mask))
            {
                spell.transform.position = hit.point + offset;
                Collider[] colliders = Physics.OverlapSphere(hit.point, spellRadiusMax * spellSizeLerp, spellLayerMask);

                // Goes through list and captures every child 
                for (int i = 0; i < colliders.Length; i++)
                {
                    colliders[i].GetComponent<Child>().CaptureChild(this.transform);
                }
            }

            spellSizeLerp = Mathf.Clamp01(spellSizeLerp + Time.deltaTime * appearSpeed);
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

        if (vertical != 0)
        {
            // Vertical Movement 
            this.transform.position += this.transform.forward * speed * vertical * Time.deltaTime;

            if (vertical < 0)
            {
                // Forward texture 
                playerMaterial.mainTexture = front;
            }
            else
            {
                // Backward texture 
                playerMaterial.mainTexture = back;
            }
        }

        if (horizontal != 0)
        {
            // Horizontal Movement 
            this.transform.position += this.transform.right * speed * horizontal * Time.deltaTime;

            if (horizontal < 0)
            {
                // Right texture 
                playerMaterial.mainTexture = right;
            }
            else
            {
                // Left texture 
                playerMaterial.mainTexture = left;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDis, mask))
        {
            spell.transform.position = hit.point + offset;
            Gizmos.DrawWireSphere(hit.point + offset, spellRadiusMax * spellSizeLerp);
        }
    }
}
