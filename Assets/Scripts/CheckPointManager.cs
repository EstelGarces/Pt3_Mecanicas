using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    private Animator animator;

    private bool isActive = false;
    private bool activated = false;
    private void Start()
    {
        animator = this.GetComponent<Animator>();
    }
    public void AciveCheckpoint()
    {
        isActive = true;
    }

    public bool IsActive()
    {
        return isActive;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activated)
        {
            if (collision.CompareTag("Personaje"))
            {
                animator.SetBool("isSaved", true);

                PlayerManager player = collision.GetComponent<PlayerManager>();
                if (player != null)
                {
                    player.SetCheckpoint(this);
                    player.SumScore(20);
                    activated = true;
                }
            }
        }
    }

}
