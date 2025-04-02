using UnityEngine;
using UnityEngine.XR;

public class ObjectGrabber : MonoBehaviour
{
    public Transform handRight;
    private GameObject heldObjectRight = null;
    public GameObject weldingToolPrefab;
    private GameObject weldingToolInstance = null;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Agarrar con la mano derecha
        {
            TryPickUpObject(ref heldObjectRight, handRight);
        }
        if (Input.GetKeyDown(KeyCode.R)) // Soltar objeto derecho
        {
            DropObject(ref heldObjectRight);
        }
        if (Input.GetKeyDown(KeyCode.T)) // Tecla para equipar soldador
        {

            if (weldingToolInstance == null && heldObjectRight == null)
            {
                weldingToolInstance = Instantiate(weldingToolPrefab, handRight);
           
                weldingToolInstance.transform.SetParent(handRight);
                weldingToolInstance.transform.localPosition = Vector3.zero;
                weldingToolInstance.transform.localRotation = Quaternion.identity;
                weldingToolInstance.GetComponent<Rigidbody>().isKinematic = true;

            }
            else if (weldingToolInstance != null)
            {
                Destroy(weldingToolInstance);
                weldingToolInstance = null;
            }
        }

    }

    void TryPickUpObject(ref GameObject heldObject, Transform hand)
    {
        if (heldObject != null) return; // Ya tiene un objeto en esa mano

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 2f))
        {
            if (hit.collider.CompareTag("Pickable")) // Verifica la etiqueta
            {
                heldObject = hit.collider.gameObject;
                heldObject.transform.SetParent(hand);
                heldObject.transform.localPosition = Vector3.zero;
                heldObject.transform.localRotation = Quaternion.identity;
                heldObject.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }

    void DropObject(ref GameObject heldObject)
    {
        if (heldObject == null) return;

        heldObject.transform.SetParent(null);
        heldObject.GetComponent<Rigidbody>().isKinematic = false;
        heldObject = null;
    }
}

