using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordManager : MonoBehaviour
{
    public float misilTime = 0.35f;
    private float shootedTime = 0f;

    //public PlayerManager player;
    //public EnemyManager slime;
    //public MushroomManager mushroom;

    private GameObject player;
    private GameObject slime;
    private GameObject mushroom;
    private GameObject bomber;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Personaje");
        slime = GameObject.FindGameObjectWithTag("Enemy");
        mushroom = GameObject.FindGameObjectWithTag("Mushroom");
        bomber = GameObject.FindGameObjectWithTag("Bomber");
    }

    // Update is called once per frame
    void Update()
    {
        shootedTime += Time.deltaTime;
        //Debug.Log(shootedTime);
        if (misilTime <= shootedTime)
        {
            DestroySword();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Animator>().SetTrigger("isDead");
            collision.gameObject.GetComponent<EnemyManager>().MuerteEnemigo();
        }
        if (collision.gameObject.CompareTag("Mushroom"))
        {
            collision.gameObject.GetComponent<Animator>().SetTrigger("isDead");
            collision.gameObject.GetComponent<MushroomManager>().MuerteEnemigo();
        }
        if (collision.gameObject.CompareTag("Bomber"))
        {
            collision.gameObject.GetComponent<BomberManager>().GetHit();
            collision.gameObject.GetComponent<BomberManager>().SubstractLives();

        }
    }

    void DestroySword()
    {
        Destroy(this.gameObject);
    }

}
