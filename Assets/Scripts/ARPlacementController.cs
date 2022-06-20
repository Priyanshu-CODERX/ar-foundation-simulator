using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;

[RequireComponent(typeof(ARRaycastManager))]
public class ARPlacementController : MonoBehaviour
{
    public XROrigin sessionOrigin;

    public GameObject ARObject;
    public GameObject placementIndicator;
    private GameObject spawnedObject;
    private Pose PlacementPose;
    private ARRaycastManager aRRaycastManager;
    private bool placementPoseIsValid = false;

    public GameObject PlaceObjectButton;

    private ARPlaneManager planeManager;

    void Start()
    {
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    // need to update placement indicator, placement pose and spawn 
    void Update()
    {
        /*if (spawnedObject == null && placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            ARPlaceObject();
        }*/


        UpdatePlacementPose();
        UpdatePlacementIndicator();


    }
    void UpdatePlacementIndicator()
    {
        if (spawnedObject == null && placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    void UpdatePlacementPose()
    {
        var screenCenter = sessionOrigin.Camera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            PlacementPose = hits[0].pose;

            // Calibrate Camera Orientation
            var cameraForward = sessionOrigin.Camera.transform.forward;
            var cameraOrientation = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            PlacementPose.rotation = Quaternion.LookRotation(cameraOrientation);
        }
    }

    public void ARPlaceObject()
    {
        spawnedObject = Instantiate(ARObject, PlacementPose.position, PlacementPose.rotation);
        //placementIndicator.SetActive(false);

        foreach(var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }

        PlaceObjectButton.SetActive(false);

    }
}
