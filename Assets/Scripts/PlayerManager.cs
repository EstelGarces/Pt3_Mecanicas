using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerManager : MonoBehaviour
{

    public float velocidad = 4.5f;
    public float jumpForce = 6.5f;
    public float tiempoMaximoSalto = 0.5f;
    public float tiempoSaltoActual = 0f;
    public LayerMask groundLayer;

    public float radio = 0.4f;


    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    public bool isActive = false;

    //Salto
    public bool isJump = false;
    private bool btnJump = false;
    private int numJump = 0;    

    //private bool botonSaltopresionado = false;
    //private int direction = 1;
    private float originalXScale;
    //private float xVelcity;
    private bool isVulnerable = true;
    private float vulnerabilityTime = 0f;
    private Animator animator;

    private Transform plataformaActual = null;
    float horizontal;

    //Vidas 
    public GameObject livePrefab;
    public Transform liveContainer;
    private List<GameObject> lives = new List<GameObject>();

    private int maxlives = 5;
    private int currentLives;

    //Spawnpoints
    public Transform playerTransform;
    private Vector3 lastSavedPosition;
    public bool checkpoint = false;
    private CheckPointManager lastCheckpoint;

    //Score
    public TextMeshProUGUI scoreNum;
    private int score;

    //Pegar
    public GameObject espadaPrefab;
    public Transform puntoOrigenD;
    public Transform puntoOrigenA;

    //PowerUps
    public TextMeshProUGUI orbNum;
    private int numOrb;


    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody2D>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        originalXScale = transform.localScale.x;
        currentLives = 3;
        UpdateLivesUI();
        lastSavedPosition = playerTransform.position;
        score = 0;
        numOrb = 0;
        scoreNum.text = score.ToString();
        orbNum.text = "x" + numOrb.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Orbes " + numOrb);

        MovimientoPj();
        SaltoPersonaje();
        CheckVulnerability();
        if (!enSuelo()) 
        {
            isJump = true;
            animator.SetBool("isJump", true);
        }
        else
        {
            isJump = false;
            animator.SetBool("isJump", false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            InstanciarEspada();
        }
    }

    private void MovimientoPj()
    {
        horizontal = Input.GetAxisRaw("Horizontal");


        Vector2 movimiento = new Vector2(horizontal, 0) * velocidad * Time.deltaTime;
        transform.Translate(movimiento);

        if (horizontal == 0) 
        { 
            isActive = false;
            animator.SetBool("isActive", false);
        }
        else
        {
            isActive = true;
            animator.SetBool("isActive", true);

            if (horizontal < 0)
            {
                spriteRenderer.flipX = true; // izquierda
            }
            else if (horizontal > 0)
            {
                spriteRenderer.flipX = false; // derecha
            }
        }
    }

    private void SaltoPersonaje()
    {
        if (Input.GetKeyDown(KeyCode.Space) )
        {
            if (enSuelo())
            {
                Jump();
            }else if(numJump < 1 && numOrb > 1)
            {
                numJump++;
                Jump();
            }
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            btnJump = false;

        }
        if (!btnJump && enSuelo()) 
        {
            isJump = false;
            animator.SetBool("isJump", false);
            numJump = 0;
        }
    }

    private void Jump()
    {
        animator.SetBool("isJump", true);
        isJump = true;
        btnJump = true;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private bool enSuelo()   
    {
        float anchoPj = GetComponent<Collider2D>().bounds.extents.x;
        Vector2 origenCentro = new Vector2(transform.position.x, transform.position.y);

        RaycastHit2D hitCentro = Physics2D.Raycast(origenCentro, Vector2.down, 1.1f, groundLayer);
        Collider2D suelo = Physics2D.OverlapCircle(origenCentro, radio, groundLayer);

        Debug.DrawRay(origenCentro, Vector2.down * 1.1f, Color.green);
        return hitCentro.collider != null || suelo != null;
    }

    //Contador de Orbes
    public void SumOrb()
    {
        numOrb++;
        orbNum.text = "x" + numOrb.ToString();
        if(numOrb > 2)
        {
            SumScore(150);
        }
    }
    private void InstanciarEspada()
    {
        if(numOrb>0)
        {
            if (espadaPrefab != null && puntoOrigenA != null || puntoOrigenD != null)
            {
                animator.SetBool("hit", true);

                bool estaMirandoIzquierda = spriteRenderer.flipX;

                // Selecciona el punto de origen según la dirección
                Transform puntoOrigenSeleccionado = estaMirandoIzquierda ? puntoOrigenA : puntoOrigenD;

                // Calcula la rotación de la espada según la dirección
                Quaternion rotacionEspada = estaMirandoIzquierda ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;

                // Instancia la espada en el punto de origen seleccionado
                GameObject espada = Instantiate(espadaPrefab, puntoOrigenSeleccionado.position, rotacionEspada);


                Invoke("NoHit", 0.35f);
            }
            else
            {
                Debug.LogWarning("EspadaPrefab o PuntoOrigen no está asignado.");
            } 
        }
    }

    public void NoHit()
    {
        animator.SetBool("hit", false);
    }
    public bool GetVulnerability()
    {
        return isVulnerable;
    }
    public void SetVulnerability()
    {
        isVulnerable = false;
        vulnerabilityTime = Time.time;
    }

    private void CheckVulnerability()
    {
        if (!isVulnerable && (Time.time - vulnerabilityTime) > 3f) 
        {
            isVulnerable = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlataformaMovil"))
        {
            plataformaActual = collision.transform;
            transform.parent = plataformaActual;
        }
        if (collision.gameObject.CompareTag("Puerta"))
        {
            SceneManager.LoadScene("WinScene");
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

    //vidas
    public void AddLives()
    {
        if(currentLives < maxlives)
        {
            currentLives++;
            UpdateLivesUI();
        }
    }

    public int GetLives()
    {
        return currentLives;
    }

    public void SubstractLives()
    {
        if(currentLives > 0)
        {
            currentLives--;
            animator.SetBool("isDead", true);
            UpdateLivesUI();
            if (score < 250)
            {
                score = 0;
                scoreNum.text = score.ToString();
            }
            else
            {
                RemoveScore();
            }
            StartCoroutine(RespawnPlayer());
        }
        else
        {
            MuertePersonaje();
        }
    }

    private void UpdateLivesUI()
    {
        foreach(GameObject live in lives)
        {
            Destroy(live);
        }
        lives.Clear();
        for (int i = 0; i < currentLives; i++)
        {
            GameObject live = Instantiate(livePrefab, liveContainer);
            lives.Add(live);
        }
    }

    //Spawnpoint
    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(0.4f);
        animator.SetBool("isDead", false);
        if (lastCheckpoint != null)
        {
            lastSavedPosition = lastCheckpoint.transform.position;
        }
        playerTransform.position = lastSavedPosition;
    }

    public void SetCheckpoint(CheckPointManager checkpoint)
    {
        if (lastCheckpoint != null)
        {
            lastCheckpoint.AciveCheckpoint();
        }

        lastCheckpoint = checkpoint;
    }
    private void SavePlayerPosition()
    {
        lastSavedPosition = playerTransform.position;
    }

    //Score
    public void SumScore(int points)
    {
        score = score + points; 
        scoreNum.text = score.ToString();
    }

    public int GetScore() { return score; }
    public void RemoveScore()
    {
        score = score - 250;
        scoreNum.text = score.ToString();
    }


    private void MuertePersonaje()
    {
        SceneManager.LoadScene("LostScene");
    }
}
