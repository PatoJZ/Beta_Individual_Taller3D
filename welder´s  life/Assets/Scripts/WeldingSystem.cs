using UnityEngine;

public class WeldingSystem : MonoBehaviour
{
    public ParticleSystem sparksEffect; // Efecto de chispas
    private bool canWeld = false; // Indica si se puede soldar

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weldable")) // Asegúrate de que los objetos a soldar tengan la etiqueta "Metal"
        {
            canWeld = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Weldable"))
        {
            canWeld = false;
            sparksEffect.Stop(); // Detener las chispas al salir del objeto
        }
    }

    private void Update()
    {
        if (canWeld && Input.GetMouseButton(0)) // Detectar clic izquierdo
        {
            if (!sparksEffect.isPlaying)
                sparksEffect.Play();
        }
        else
        {
            sparksEffect.Stop();
        }
    }
}
