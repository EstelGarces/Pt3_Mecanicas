using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberManager : MonoBehaviour
{
    public bool isDetected;
    public bool isHit;
    public bool isAtack;

    public GameObject bombPrefab;
    public Transform puntoDisparoD;
    public Transform puntoDisparoA;

    public float velocidadProyectil = 10f;
    public float tiempoDisparo = 4.5f;
    public float refrescoDisparo = 0f;
    public Transform raycastOrigin;
    private SpriteRenderer spriteRenderer;

    private Animator animator;
    [SerializeField]
    private GameObject personaje;

    private Transform plataformaActual = null;

    private int maxLives = 5;
    private int lives = 5;
    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        isDetected = false;

        personaje = GameObject.FindGameObjectWithTag("Personaje");
        spriteRenderer = this.GetComponent<SpriteRenderer>();

        lives = maxLives;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDetected)
        {
            Debug.DrawRay(raycastOrigin.position, Vector2.left * 10f, Color.red); // Raycast izquierda
            Debug.DrawRay(raycastOrigin.position, Vector2.right * 10f, Color.blue); // Raycast derecha

            RaycastHit2D detectedRight = Physics2D.Raycast(raycastOrigin.position, Vector2.right, 10f, LayerMask.GetMask("Jugador"));
            RaycastHit2D detectedLeft = Physics2D.Raycast(raycastOrigin.position, Vector2.left, 10f, LayerMask.GetMask("Jugador"));

            if (detectedRight.collider != null || detectedLeft.collider != null)
            {
                isDetected = true;
                refrescoDisparo = 6f;
            }
        }

        if (isDetected)
        {
            RaycastHit2D detectedLeft = Physics2D.Raycast(raycastOrigin.position, Vector2.left, 10f, LayerMask.GetMask("Jugador"));
            if (detectedLeft.collider != null)
            {
                spriteRenderer.flipX = true; // izquierda
            }
            RaycastHit2D detectedRight = Physics2D.Raycast(raycastOrigin.position, Vector2.right, 10f, LayerMask.GetMask("Jugador"));
            if (detectedRight.collider != null)
            {
                spriteRenderer.flipX = false; // derecha
            }

            if (refrescoDisparo > tiempoDisparo)
            {
                Disparar();
                refrescoDisparo = 0f;
            }
            refrescoDisparo += Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlataformaMovil"))
        {
            plataformaActual = collision.transform;
            transform.parent = plataformaActual;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Personaje")
        {
            if (personaje.GetComponent<PlayerManager>().GetVulnerability())
            {
                personaje.GetComponent<PlayerManager>().SubstractLives();
                personaje.GetComponent<PlayerManager>().SetVulnerability();
            }
        }

        if (collision.gameObject.CompareTag("PlataformaMovil"))
        {
            transform.parent = null;
            plataformaActual = null;
        }
    }

    void Disparar()
    {
        if (bombPrefab != null && puntoDisparoA != null || puntoDisparoD != null)
        {
            animator.SetBool("isAtack", true);
            Invoke("NoAtack", 0.35f);
            Invoke("LanzarBomba", 0.55f);
        }
    }

    private void LanzarBomba()
    {
        bool estaMirandoIzquierda = spriteRenderer.flipX;
        Transform puntoOrigenSeleccionado = estaMirandoIzquierda ? puntoDisparoA : puntoDisparoD;
        Quaternion rotacionBomba = estaMirandoIzquierda ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;

        GameObject bomb = Instantiate(bombPrefab, puntoOrigenSeleccionado.position, rotacionBomba);

        BombManager bombManager = bomb.GetComponent<BombManager>();
        Rigidbody2D rb = bomb.GetComponent<Rigidbody2D>(); // Obtener el Rigidbody2D de la bomba

        bombManager.SetObjetivo(personaje.transform);
        bombManager.LanzarBomba(rb);

    }
    public void NoHit()
    {
        animator.SetBool("isHit", false);
    }

    public void NoAtack()
    {
        animator.SetBool("isAtack", false);
    }

    public int GetLives()
    {
        return lives;
    }

    public void GetHit()
    {

    }
    public void SubstractLives()
    {
        animator.SetBool("isHit", true);
        lives--;
        Invoke("NoHit", 0.35f);
        if (lives == 0) 
        {
            MuerteEnemigo();
        }
    }
    public void MuerteEnemigo()
    {
        personaje.GetComponent<PlayerManager>().SumScore(300);
        this.gameObject.GetComponent<BoxCollider2D>().enabled = false;  
        Invoke("DestroyM", 0.35f);
    }
    private void DestroyM()
    {
        Destroy(this.gameObject);
    }


}
