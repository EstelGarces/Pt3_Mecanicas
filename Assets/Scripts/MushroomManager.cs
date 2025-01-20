using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomManager : MonoBehaviour
{
    public bool isDetected;
    public bool isMoving;
    public bool isSeen;
    public Transform raycastOrigin;

    [SerializeField]
    private int velocidad = 4;
    private bool canChangeDirection = true;
    private float size;
    private GameObject personaje;
    private bool direccion = true;
    private Animator animator;

    private Transform plataformaActual = null;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();

        isDetected = false;
        personaje = GameObject.FindGameObjectWithTag("Personaje");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(raycastOrigin.position, Vector2.left * 5f, Color.red); // Raycast izquierda
        Debug.DrawRay(raycastOrigin.position, Vector2.right * 5f, Color.blue); // Raycast derecha

        if (!isDetected)
        {
            CheckChangeDirection();

            RaycastHit2D detectedLeft = Physics2D.Raycast(raycastOrigin.position, Vector2.left, 8f, LayerMask.GetMask("Jugador"));
            if (detectedLeft.collider != null)
            {
                isDetected = true;
                direccion = true;
                animator.SetBool("isMoving", true);
                velocidad = Mathf.Abs(velocidad);
                Voltear(-1);
            }
            RaycastHit2D detectedRight = Physics2D.Raycast(raycastOrigin.position, Vector2.right, 8f, LayerMask.GetMask("Jugador"));
            if (detectedRight.collider != null)
            {
                isDetected = true;
                direccion = false;
                animator.SetBool("isMoving", true);
                velocidad = Mathf.Abs(velocidad);
                Voltear(1);
            }
        }
        else
        {
            RaycastHit2D detectedLeft = Physics2D.Raycast(raycastOrigin.position, Vector2.left, 8f, LayerMask.GetMask("Jugador"));
            RaycastHit2D detectedRight = Physics2D.Raycast(raycastOrigin.position, Vector2.right, 8f, LayerMask.GetMask("Jugador"));
            if (detectedLeft.collider == null && detectedRight.collider == null)
            {
                isDetected = false;
                animator.SetBool("isMoving", false);

            }
        }

        if (animator.GetBool("isMoving"))
        {
            transform.Translate(Vector2.left * velocidad * Time.deltaTime * (direccion ? 1 : -1));
        }

        IsFalling();
    }

    void IsFalling()
    {
        RaycastHit2D detectedBottom = Physics2D.Raycast(raycastOrigin.position, Vector2.down, 1.3f, LayerMask.GetMask("Ground"));
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
        if (collision.gameObject.tag == "Personaje")
        {
            if (size < -0.5)
            {
                personaje.GetComponent<Rigidbody2D>().velocity = Vector2.up * 10;
                isSeen = true;
                animator.SetBool("isSeen", true);
                animator.SetBool("isMoving", false);
                Invoke("Move", 1f);
            }
            else
            {
                if (personaje.GetComponent<PlayerManager>().GetVulnerability())
                {
                    personaje.GetComponent<PlayerManager>().SubstractLives();
                    personaje.GetComponent<PlayerManager>().SubstractLives();
                    personaje.GetComponent<PlayerManager>().SetVulnerability();
                }
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

    void Move()
    {
        isSeen = false;
        animator.SetBool("isSeen", false);
        animator.SetBool("isMoving", true);
            
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
                detected = Physics2D.Raycast(raycastOrigin.position, Vector2.left, 0.5f, LayerMask.GetMask("Ground"));
            }
            else
            {
                detected = Physics2D.Raycast(raycastOrigin.position, Vector2.right, 0.5f, LayerMask.GetMask("Ground"));
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
        personaje.GetComponent<PlayerManager>().SumScore(100);
        Invoke("DestroyM", 0.35f);
    }
    private void DestroyM()
    {
        Destroy(this.gameObject);
    }
}