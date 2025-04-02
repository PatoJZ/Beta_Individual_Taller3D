using System.Collections.Generic;
using UnityEngine;

public class WeldingTool : MonoBehaviour
{
    [Header("Configuración Básica")]
    public float weldRange = 2f;
    public float weldThreshold = 0.3f;
    public float weldTime = 1.5f;
    public string weldableTag = "Weldable";

    [Header("Referencias")]
    public Transform toolTip;
    public Material weldMaterial;
    public GameObject weldPointPrefab; // Prefab para puntos de unión
    public LineRenderer weldPreview;

    [Header("Efectos")]
    public ParticleSystem weldingParticles;
    //public AudioSource weldingAudio;
    public Light weldingLight;

    [Header("Configuración de Luz")]
    public float minLightIntensity = 2f;
    public float maxLightIntensity = 8f;
    public float lightFlickerSpeed = 10f;
    public Color lightColor1 = new Color(1, 0.6f, 0.2f); // Naranja
    public Color lightColor2 = new Color(1, 0.9f, 0.4f); // Amarillo

    // Variables de estado
    private List<GameObject> activeWeldPoints = new List<GameObject>();
    public List<Weldable> nearbyWeldables = new List<Weldable>();
    private LineRenderer currentWeldLine;
    public float currentWeldProgress;
    public bool isWelding;
    public Vector3? weldStartPoint;

    void Start()
    {
        weldPreview.positionCount = 2;
        weldPreview.enabled = false;

        // Inicializar luz
        if (weldingLight != null)
        {
            weldingLight.enabled = false;
            weldingLight.intensity = maxLightIntensity;
        }
    }

    void Update()
    {
        UpdateNearbyObjects();
        ShowConnectionPoints();

        if (Input.GetMouseButtonDown(0)) StartWeld();
        if (Input.GetMouseButton(0)) UpdateWeld();
        if (Input.GetMouseButtonUp(0)) StopWeld();
    }

    void UpdateNearbyObjects()
    {
        nearbyWeldables.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(toolTip.position, weldRange);
        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag(weldableTag) && col.TryGetComponent<Weldable>(out var weldable))
            {
                nearbyWeldables.Add(weldable);
            }
        }
    }

    void ShowConnectionPoints()
    {
        ClearOldWeldPoints();

        foreach (var pair in GetWeldablePairs())
        {
            Vector3[] points = GetConnectionPoints(pair.Item1, pair.Item2);
            CreateVisualMarkers(points);
        }
    }

    void ClearOldWeldPoints()
    {
        foreach (var point in activeWeldPoints)
        {
            if (point != null) Destroy(point);
        }
        activeWeldPoints.Clear();
    }

    IEnumerable<(Weldable, Weldable)> GetWeldablePairs()
    {
        for (int i = 0; i < nearbyWeldables.Count; i++)
        {
            for (int j = i + 1; j < nearbyWeldables.Count; j++)
            {
                yield return (nearbyWeldables[i], nearbyWeldables[j]);
            }
        }
    }

    Vector3[] GetConnectionPoints(Weldable a, Weldable b)
    {
        List<Vector3> points = new List<Vector3>();

        // 1. Conexión entre vértices cercanos
        foreach (var v1 in a.GetVertices())
        {
            foreach (var v2 in b.GetVertices())
            {
                if (Vector3.Distance(v1, v2) < weldThreshold)
                {
                    points.Add((v1 + v2) * 0.5f);
                }
            }
        }

        // 2. Conexión entre centros si no hay vértices cercanos
        if (points.Count == 0 && Vector3.Distance(a.transform.position, b.transform.position) < weldRange * 1.5f)
        {
            points.Add(FindClosestSurfacePoint(a, b.transform.position));
            points.Add(FindClosestSurfacePoint(b, a.transform.position));
        }

        return points.ToArray();
    }

    Vector3 FindClosestSurfacePoint(Weldable target, Vector3 fromPoint)
    {
        // Versión simplificada - en una implementación real usar Raycast
        return target.transform.position + (fromPoint - target.transform.position).normalized * 0.5f;
    }

    void CreateVisualMarkers(Vector3[] points)
    {
        foreach (var point in points)
        {
            var marker = Instantiate(weldPointPrefab, point, Quaternion.identity);
            marker.transform.localScale = Vector3.one * Mathf.Clamp(1 / Vector3.Distance(toolTip.position, point), 0.1f, 0.3f);
            activeWeldPoints.Add(marker);
        }
    }

    void StartWeld()
    {
        if (TryGetClosestWeldPoint(out Vector3 point))
        {
            weldStartPoint = point;
            currentWeldLine = CreateNewWeldLine();
            isWelding = true;
            currentWeldProgress = 0;

            //weldingAudio.Play();
            weldingParticles.Play();
            if (weldingLight != null) weldingLight.enabled = true;
        }
    }

    bool TryGetClosestWeldPoint(out Vector3 point)
    {
        point = Vector3.zero;
        if (activeWeldPoints.Count == 0) return false;

        float minDistance = float.MaxValue;
        foreach (var marker in activeWeldPoints)
        {
            float dist = Vector3.Distance(toolTip.position, marker.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                point = marker.transform.position;
            }
        }
        return minDistance < weldRange;
    }

    LineRenderer CreateNewWeldLine()
    {
        GameObject lineObj = new GameObject("WeldLine");
        var lr = lineObj.AddComponent<LineRenderer>();
        lr.material = weldMaterial;
        lr.startWidth = lr.endWidth = 0.05f;
        lr.positionCount = 2;
        return lr;
    }

    void UpdateWeld()
    {
        if (!isWelding || !weldStartPoint.HasValue) return;

        currentWeldProgress += Time.deltaTime;
        UpdateWeldVisuals();

        if (currentWeldProgress >= weldTime)
        {
            CompleteWeld();
        }
    }

    void UpdateWeldVisuals()
    {
        // Actualizar línea temporal
        currentWeldLine.SetPosition(0, weldStartPoint.Value);
        currentWeldLine.SetPosition(1, toolTip.position);

        // Efectos dinámicos
        float progress = currentWeldProgress / weldTime;

        // Control de luz con efectos
        if (weldingLight.enabled)
        {
            // Efecto de parpadeo
            float noise = Mathf.PerlinNoise(Time.time * lightFlickerSpeed, 0);
            weldingLight.intensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, noise);

            // Cambio de color
            float colorShift = Mathf.PingPong(Time.time * 0.5f, 1);
            weldingLight.color = Color.Lerp(lightColor1, lightColor2, colorShift);
        }

        // Control de partículas
        var main = weldingParticles.main;
        main.startSpeed = Mathf.Lerp(1, 3, progress);
    }

    void CompleteWeld()
    {
        if (TryGetClosestWeldPoint(out Vector3 endPoint))
        {
            // Crear conexión física
            CreateWeldJoint(weldStartPoint.Value, endPoint);

            // Línea final
            currentWeldLine.SetPosition(1, endPoint);
            AddColliderToLine(currentWeldLine);
        }

        ResetWelding();
    }

    void CreateWeldJoint(Vector3 start, Vector3 end)
    {
        // Encontrar los objetos más cercanos a los puntos de soldadura
        Weldable startObj = FindNearestWeldable(start);
        Weldable endObj = FindNearestWeldable(end);

        if (startObj != null && endObj != null && startObj != endObj)
        {
            FixedJoint joint = startObj.gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = endObj.GetComponent<Rigidbody>();
            joint.breakForce = 1000f;
        }
    }

    Weldable FindNearestWeldable(Vector3 point)
    {
        Weldable closest = null;
        float minDist = float.MaxValue;

        foreach (var weldable in nearbyWeldables)
        {
            float dist = Vector3.Distance(weldable.transform.position, point);
            if (dist < minDist)
            {
                minDist = dist;
                closest = weldable;
            }
        }
        return closest;
    }

    void AddColliderToLine(LineRenderer line)
    {
        // Crear un collider aproximado para la línea
        GameObject colliderObj = new GameObject("WeldCollider");
        colliderObj.transform.SetParent(line.transform);
        var collider = colliderObj.AddComponent<CapsuleCollider>();

        Vector3 start = line.GetPosition(0);
        Vector3 end = line.GetPosition(1);

        collider.height = Vector3.Distance(start, end);
        collider.radius = 0.05f;
        collider.center = Vector3.Lerp(start, end, 0.5f) - line.transform.position;
        collider.direction = 2; // Eje Z
        colliderObj.transform.LookAt(end);
    }

    void StopWeld()
    {
        if (!isWelding) return;
        ResetWelding();
    }

    void ResetWelding()
    {
        isWelding = false;
        weldStartPoint = null;
        currentWeldProgress = 0;

        //weldingAudio.Stop();
        weldingParticles.Stop();
        if (weldingLight != null) weldingLight.enabled = false;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(toolTip.position, weldThreshold);
    }
}