using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardPlacement : MonoBehaviour
{   
    [SerializeField] private Camera PlayerCamera;
    [SerializeField] private LayerMask PlacementCollideMask;
    [SerializeField] private LayerMask PlacementCheckMask;
    private GameObject CurrentPlacingWizard;
    
    void Start()
    {
        
    }

    void Update()
    {
        if(CurrentPlacingWizard != null)
        {
            Ray camray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit HitInfo;
            if (Physics.Raycast(camray, out  HitInfo, 100f, PlacementCollideMask))
            {
                CurrentPlacingWizard.transform.position = HitInfo.point;
            }

            if (Input.GetMouseButtonDown(0) && HitInfo.collider.gameObject != null)
            {
                if (!HitInfo.collider.gameObject.CompareTag("PlaceNotAllowed"))
                {
                    
                    BoxCollider WizardCollider = CurrentPlacingWizard.gameObject.GetComponent<BoxCollider>();
                    WizardCollider.isTrigger = true;
                    Vector3 BoxCenter = CurrentPlacingWizard.gameObject.transform.position + WizardCollider.center;
                    Vector3 HalfExtents = WizardCollider.size / 2;

                    if (!Physics.CheckBox(BoxCenter, HalfExtents, Quaternion.identity, PlacementCheckMask, QueryTriggerInteraction.Ignore))
                    {
                        WizardCollider.isTrigger = false;
                        CurrentPlacingWizard = null;
                    }

                }
                
            }
        }
    }

    public void SetWizardToPlace(GameObject wizard)
    {
        CurrentPlacingWizard = Instantiate(wizard, Vector3.zero, Quaternion.identity);
    }
}
