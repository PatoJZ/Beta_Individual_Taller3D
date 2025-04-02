using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeldingUI : MonoBehaviour
{
    [Header("References")]
    public WeldingTool weldingTool;
    public Camera playerCamera;

    [Header("UI Elements")]
    public Image crosshair;
    public Image progressCircle;
    public TMP_Text objectNameText;
    public TMP_Text instructionText;
    public TMP_Text weldStatusText;

    [Header("Colors")]
    public Color readyColor = Color.green;
    public Color weldingColor = Color.yellow;
    public Color errorColor = Color.red;

    void Update()
    {
        UpdateUIElements();
    }

    void UpdateUIElements()
    {
        UpdateCrosshair();
        UpdateProgress();
        UpdateInstructionText();
    }

    void UpdateCrosshair()
    {
        // Cambiamos por la nueva lógica de detección
        bool isValidTarget = weldingTool.nearbyWeldables.Count > 0;
        crosshair.color = isValidTarget ? readyColor : errorColor;

        // Opcional: Cambiar tamaño según distancia
        if (isValidTarget)
        {
            float distance = Vector3.Distance(
                playerCamera.transform.position,
                weldingTool.toolTip.position
            );
            crosshair.transform.localScale = Vector3.one * Mathf.Clamp(1 / distance, 0.5f, 1.5f);
        }
        else
        {
            crosshair.transform.localScale = Vector3.one;
        }
    }

    void UpdateProgress()
    {
        progressCircle.fillAmount = weldingTool.currentWeldProgress / weldingTool.weldTime;
        progressCircle.color = Color.Lerp(weldingColor, readyColor, progressCircle.fillAmount);
    }

    void UpdateInstructionText()
    {
        if (weldingTool.isWelding)
        {
            instructionText.text = $"Soldando... {progressCircle.fillAmount * 100:F0}%";
            weldStatusText.text = weldingTool.weldStartPoint.HasValue ?
                "Arrastra al punto destino" :
                "Selecciona punto inicial";
        }
        else
        {
            instructionText.text = "Click para comenzar a soldar";
            weldStatusText.text = weldingTool.nearbyWeldables.Count > 0 ?
                "Objetos soldables detectados" :
                "Acerca objetos para soldar";
        }
    }
}