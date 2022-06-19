using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacement : MonoBehaviour
{
    public GameObject ObjectToSpawn;

    private ARPlaneManager planeManager;
    private ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private bool objectWasPlaced;

    private Transform selectedObject;

    void Start()
    {
        planeManager = GetComponent<ARPlaneManager>();
        raycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        if (!Input.GetMouseButton(0)) return;

        // First touch
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitObject))
            {
                selectedObject = hitObject.transform.CompareTag("ARObject") ? hitObject.transform : null;
            }
        }

        if (raycastManager.Raycast(Input.mousePosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            if (!selectedObject)
                selectedObject = Instantiate(ObjectToSpawn, hitPose.position, hitPose.rotation).transform;
            else
            {
                selectedObject.transform.position = hitPose.position;
            }
        }
    }

}
