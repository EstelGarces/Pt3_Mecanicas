using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbsManager : MonoBehaviour
{
    private Animator animator;

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
                player.SumOrb();
                Invoke("DestroyO", 0.35f);
            }
        }
    }

    private void DestroyO()
    {
        Destroy(this.gameObject);
    }

}
