using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public bool isDetected;

    [SerializeField]
    private int velocidad = 4;
    private bool canChangeDirection =  true;
    private float size;
    private GameObject personaje;
    private bool direccion = true;

    private Transform plataformaActual = null;

    // Start is called before the first frame update
    void Start()
    {
        isDetected = false;
        personaje = GameObject.FindGameObjectWithTag("Personaje");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDetected)
        {
            CheckChangeDirection();
            transform.Translate(Vector2.left * velocidad * Time.deltaTime * (direccion ? 1 : -1));

            RaycastHit2D detectedLeft = Physics2D.Raycast(transform.position, Vector2.left, 10f, LayerMask.GetMask("Jugador"));
            if (detectedLeft.collider != null)
            {
                isDetected = true;
                direccion = true;
                velocidad = Mathf.Abs(velocidad);
                Voltear(1);
            }
            RaycastHit2D detectedRight = Physics2D.Raycast(transform.position, Vector2.right, 10f, LayerMask.GetMask("Jugador"));
            if (detectedRight.collider != null)
            {
                isDetected = true;
                direccion = false;
                velocidad = Mathf.Abs(velocidad);
                Voltear(-1);
            }
        }
        else
        {
            transform.Translate(Vector2.left * velocidad * Time.deltaTime * (direccion ? 1 : -1));

            RaycastHit2D detectedLeft = Physics2D.Raycast(transform.position, Vector2.left, 10f, LayerMask.GetMask("Jugador"));
            RaycastHit2D detectedRight = Physics2D.Raycast(transform.position, Vector2.right, 10f, LayerMask.GetMask("Jugador"));
            if (detectedLeft.collider == null && detectedRight.collider == null)
            {
                isDetected = false;
            }
        }
        IsFalling();
    }

    void IsFalling()
    {
        RaycastHit2D detectedBottom = Physics2D.Raycast(transform.position, Vector2.down, 1.3f, LayerMask.GetMask("Ground"));
        if (detectedBottom.collider == null)
        {
            canChangeDirection = false;
        }
        else
        {
            canChangeDirection = true; 
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        size = collision.contacts[0].normal.y;        
        if(collision.gameObject.tag == "Personaje")
        {
            if (size < -0.5)
            {
                personaje.GetComponent<Rigidbody2D>().velocity = Vector2.up * 6;
                velocidad = 0;
                this.GetComponent<Animator>().SetBool("isDead", true);
                MuerteEnemigo();
            }
            else
            {
                if (personaje.GetComponent<PlayerManager>().GetVulnerability())
                {
                    //personaje.GetComponent<Animator>().SetTrigger("isDead");
                    personaje.GetComponent<PlayerManager>().SubstractLives();
                    personaje.GetComponent<PlayerManager>().SetVulnerability();
                }
            }
        }
        if (canChangeDirection) 
        {
            if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            { 
                direccion = !direccion;
                Voltear(direccion ? 1 : -1);
            }
        }

        if (collision.gameObject.CompareTag("PlataformaMovil"))
        {
            plataformaActual = collision.transform;
            transform.parent = plataformaActual;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlataformaMovil"))
        {
            transform.parent = null;
            plataformaActual = null;
        }
    }

    private void Voltear(int direccion)
    {
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * direccion, transform.localScale.y, transform.localScale.z);
    }

    private void CheckChangeDirection()
    {
        if (canChangeDirection)
        {
            RaycastHit2D detected;
            if (direccion)
            {
                detected = Physics2D.Raycast(transform.position, Vector2.left, 0.5f, LayerMask.GetMask("Ground"));
            }
            else
            {
                detected = Physics2D.Raycast(transform.position, Vector2.right, 0.5f, LayerMask.GetMask("Ground"));
            }
            if (detected)
            {

                direccion = !direccion;
                Voltear(direccion ? 1 : -1);
            }
        }
    }
    
    public void MuerteEnemigo()
    {
        personaje.GetComponent<PlayerManager>().SumScore(50);
        Invoke("DestroyS", 0.35f);
    }
    private void DestroyS()
    {
        Destroy(this.gameObject);
    }
}
