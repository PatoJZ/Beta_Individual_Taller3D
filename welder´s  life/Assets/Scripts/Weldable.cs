// Weldable.cs
using UnityEngine;

public class Weldable : MonoBehaviour
{
    public float weldThreshold = 0.1f;
    public Material highlightMaterial;
    private Material originalMaterial;
    private new Renderer renderer;

    void Start()
    {
        renderer = GetComponent<Renderer>();
        originalMaterial = renderer.material;
    }

    public Vector3[] GetVertices()
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider == null) return null;

        Vector3[] vertices = new Vector3[8];
        Vector3 center = collider.center;
        Vector3 extents = collider.size * 0.5f;

        Vector3 localFrontTopLeft = new Vector3(-extents.x, extents.y, -extents.z) + center;
        Vector3 localFrontTopRight = new Vector3(extents.x, extents.y, -extents.z) + center;
        Vector3 localFrontBottomLeft = new Vector3(-extents.x, -extents.y, -extents.z) + center;
        Vector3 localFrontBottomRight = new Vector3(extents.x, -extents.y, -extents.z) + center;
        Vector3 localBackTopLeft = new Vector3(-extents.x, extents.y, extents.z) + center;
        Vector3 localBackTopRight = new Vector3(extents.x, extents.y, extents.z) + center;
        Vector3 localBackBottomLeft = new Vector3(-extents.x, -extents.y, extents.z) + center;
        Vector3 localBackBottomRight = new Vector3(extents.x, -extents.y, extents.z) + center;

        vertices[0] = transform.TransformPoint(localFrontTopLeft);
        vertices[1] = transform.TransformPoint(localFrontTopRight);
        vertices[2] = transform.TransformPoint(localFrontBottomLeft);
        vertices[3] = transform.TransformPoint(localFrontBottomRight);
        vertices[4] = transform.TransformPoint(localBackTopLeft);
        vertices[5] = transform.TransformPoint(localBackTopRight);
        vertices[6] = transform.TransformPoint(localBackBottomLeft);
        vertices[7] = transform.TransformPoint(localBackBottomRight);

        return vertices;
    }

    public void Highlight(bool enable)
    {
        renderer.material = enable ? highlightMaterial : originalMaterial;
    }
}