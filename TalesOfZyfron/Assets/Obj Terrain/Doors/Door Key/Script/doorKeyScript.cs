using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorKeyScript : MonoBehaviour
{
    public GameObject key; // Référence à l'objet spécifique
    public string cameraTag = "Camera"; // Tag de la caméra à utiliser
    public float maxDistance = 3f; // Distance maximale pour appuyer sur la touche C
    private Camera mainCamera; // Référence à la caméra
    private bool HaveKey = false;
    public GameObject DoorLeft;
    public GameObject DoorRight;
    private Animator DoorKLController;
    private Animator DoorKRController;


    void Start()
    {
        DoorKLController = DoorLeft.GetComponent<Animator>();
        DoorKRController = DoorRight.GetComponent<Animator>();
   
    }

    private void Update()
    {
        // Vérifier si le joueur appuie sur la touche C
        if (Input.GetKeyDown(KeyCode.C))
        {
            // Vérifier si la caméra n'a pas été trouvée et que le joueur est à une certaine distance de l'objet
            if (mainCamera == null)
            {
                // Rechercher la caméra par son tag
                GameObject cameraObject = GameObject.FindWithTag(cameraTag);
                if (cameraObject != null)
                {
                    mainCamera = cameraObject.GetComponent<Camera>();
                }
            }

            // Si la caméra a été trouvée, continuer avec la logique de clic
            if (mainCamera != null)
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit) && !HaveKey && hit.collider.gameObject == key && Vector3.Distance(mainCamera.transform.position, key.transform.position) <= maxDistance)
                {
                    Debug.Log("Une clé a été recupérée");
                    HaveKey = true;
                }
                else if(HaveKey && Physics.Raycast(ray, out hit) && (hit.collider.gameObject == DoorLeft && Vector3.Distance(mainCamera.transform.position, DoorLeft.transform.position) <= maxDistance || hit.collider.gameObject == DoorRight && Vector3.Distance(mainCamera.transform.position, DoorRight.transform.position) <= maxDistance))
                {
                    OpenDoor();
                 
               
                }
            }
        }
    }
  

    private void OpenDoor()
    {
            DoorKLController.SetBool("Open",!DoorKLController.GetBool("Open"));
            DoorKRController.SetBool("Open",!DoorKRController.GetBool("Open"));
    }
}
