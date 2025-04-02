using UnityEngine;

public class WeldLineController : MonoBehaviour
{
    private LineRenderer lineRenderer;

    void Start()
    {
        // Crear componente din�micamente
        lineRenderer = gameObject.AddComponent<LineRenderer>();

        // Configurar propiedades b�sicas
        lineRenderer.material = Resources.Load<Material>("WeldMaterial");
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0; // Inicialmente sin puntos

        // Configurar para que siga la f�sica
        lineRenderer.useWorldSpace = true;

        // Opcional: Efecto de brillo
        lineRenderer.material.EnableKeyword("_EMISSION");
        lineRenderer.material.SetColor("_EmissionColor", Color.yellow);
    }

    // Actualizar puntos din�micamente (ej. durante la soldadura)
    public void UpdateLine(Vector3[] points)
    {
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }

    // Para tu sistema de soldadura progresiva
    public void AddPoint(Vector3 newPoint)
    {
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newPoint);
    }

    public void ClearLine()
    {
        lineRenderer.positionCount = 0;
    }
}