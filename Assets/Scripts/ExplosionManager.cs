using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : MonoBehaviour
{
    public float explosionTime = 1f;
    private float shootedTime = 0f;
    private GameObject personaje;

    // Start is called before the first frame update
    void Start()
    {
        personaje = GameObject.FindGameObjectWithTag("Personaje");
    }

    // Update is called once per frame
    void Update()
    {
        shootedTime += Time.deltaTime;
        if (explosionTime <= shootedTime)
        {
            DestroyExplosion();
        }
    }
    void DestroyExplosion()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Personaje")
        {
            if (personaje.GetComponent<PlayerManager>().GetVulnerability())
            {
                personaje.GetComponent<PlayerManager>().SubstractLives();
                personaje.GetComponent<PlayerManager>().SubstractLives();
                personaje.GetComponent<PlayerManager>().SetVulnerability();
            }
        }
    }
}
