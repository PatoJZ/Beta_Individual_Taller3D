using UnityEngine;

public class WeldingLight : MonoBehaviour
{
    private Light weldingLight;
    public float minIntensity = 2f;
    public float maxIntensity = 8f;
    public float flickerSpeed = 10f;

    void Start()
    {
        weldingLight = GetComponent<Light>();
        weldingLight.enabled = false;
    }

    void Update()
    {
        if (weldingLight.enabled)
        {
            // Efecto de parpadeo aleatorio (como una antorcha real)
            float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0);
            weldingLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
            
            // Cambio sutil de color
            float colorShift = Mathf.PingPong(Time.time * 0.5f, 1);
            weldingLight.color = Color.Lerp(
                new Color(1, 0.6f, 0.2f), // Naranja
                new Color(1, 0.9f, 0.4f), // Amarillo
                colorShift
            );
        }
    }

    public void ToggleLight(bool state)
    {
        weldingLight.enabled = state;
        if (state) 
        {
            weldingLight.intensity = maxIntensity;
        }
    }
}