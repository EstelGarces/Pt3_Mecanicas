using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PalancaManager : MonoBehaviour
{
    public Sprite palancaActivada;
    public Sprite palancaDesactivada;

    private SpriteRenderer palanca1;

    private GameObject plataformaO;
    private Animator animator;
    private bool activated = false;

    // Start is called before the first frame update
    void Start()
    {
        palanca1 = GetComponent<SpriteRenderer>();
        plataformaO = GameObject.FindGameObjectWithTag("PlataformaO");
        plataformaO.gameObject.SetActive(false);

        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!activated)
        {
            if (collision.gameObject.CompareTag("Personaje"))
            {
                palanca1.sprite = palancaActivada;
                plataformaO.SetActive(true);
                plataformaO.GetComponent<Animator>().SetTrigger("isActive");
                activated = true;
            }
        }
    }
}
