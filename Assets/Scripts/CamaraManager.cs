using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraManager : MonoBehaviour
{
    public Transform player; // Referencia al jugador
    public Vector3 offset;   // Offset para ajustar la posición de la cámara respecto al jugador
    public float smoothSpeed = 0.125f; // Velocidad de interpolación de la cámara

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 desiredPosition = player.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
