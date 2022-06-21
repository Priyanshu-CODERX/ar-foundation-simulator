using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;

public class MultiARPlacement : MonoBehaviour
{
    public XROrigin sessionOrigin;

    public GameObject placementIndicator;
    private GameObject spawnedObject;
    private Pose PlacementPose;
    private ARRaycastManager aRRaycastManager;
    private bool placementPoseIsValid = false;

    private ARPlaneManager planeManager;

    void Start()
    {
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
    }
    void UpdatePlacementIndicator()
    {
        placementIndicator.SetActive(true);
        placementIndicator.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation);
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

    public void ARPlaceObject(GameObject AugmentableObject)
    {
        spawnedObject = Instantiate(AugmentableObject, PlacementPose.position, PlacementPose.rotation);

        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
    }
}
