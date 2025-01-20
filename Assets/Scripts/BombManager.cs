using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class BombManager : MonoBehaviour
{
    private Animator animator;

    public float velocidad = 2f;
    public Vector2 direccionMovimiento;
    private Transform objetivo;          // Transform del jugador
    public GameObject explosionEffect;


    //private SpriteRenderer spriteRenderer;
    //private Collider2D collider2D;
    public float magfuerzaLanzamiento = 10f;
    private GameObject personaje;

    // Start is called before the first frame update
    public void SetObjetivo(Transform objetivoJugador)
    {
        objetivo = objetivoJugador;

        // Calcular la dirección hacia el objetivo
        if (objetivo != null)
        {
            Vector3 direccion = objetivo.position - transform.position;
            direccionMovimiento = direccion.normalized; // Normalizar para obtener la dirección unitaria
        }

    }
    void Start()
    {
        animator = this.GetComponent<Animator>();
        personaje = GameObject.FindGameObjectWithTag("Personaje");
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Personaje")
        {
            if (personaje.GetComponent<PlayerManager>().GetVulnerability())
            {
                Explosion();
                personaje.GetComponent<PlayerManager>().SubstractLives();
                personaje.GetComponent<PlayerManager>().SubstractLives();
                personaje.GetComponent<PlayerManager>().SetVulnerability();
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Explosion();
        }
    }

    public void LanzarBomba(Rigidbody2D rb)
    {
        if (objetivo != null) 
        {
            Vector2 direccionHaciaJugador = (objetivo.position - transform.position).normalized;

            float fuerzaCurvaturaY = 2f;
            Vector2 fuerzaLanzamiento = new Vector2(direccionHaciaJugador.x, fuerzaCurvaturaY).normalized * magfuerzaLanzamiento;

            rb.velocity = fuerzaLanzamiento;
            rb.gravityScale = 1f;
        }
    }

    private void Explosion()
    {
        Destroy(this.gameObject);
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Animator explosionAnimator = explosion.GetComponent<Animator>();
            if (explosionAnimator != null)
            {
                float explosionDuration = explosionAnimator.GetCurrentAnimatorStateInfo(0).length;
                Destroy(explosion, explosionDuration);
            }
            else
            {
                Destroy(explosion, 1f); // Valor por defecto si no hay animador
            }
        }

    }

    
}
