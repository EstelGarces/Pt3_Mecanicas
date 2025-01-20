using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartManager : MonoBehaviour
{
    private Animator animator;
    private bool activated = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Personaje"))
        {
            animator.SetBool("isCollected", true);

            PlayerManager player = collision.GetComponent<PlayerManager>();
            if (player != null)
            {
                player.AddLives();
                Invoke("DestroyO", 0.35f);
            }
        }
    }

    private void DestroyO()
    {
        Destroy(this.gameObject);
    }
}
