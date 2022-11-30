using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Networking;

public class ARTapToPlaceObject : MonoBehaviour
{
    public GameObject objectToPlace; 
    public GameObject placementIndicator;
    
    private ARRaycastManager arRaycastManager;
    private Pose placementPose;
    private bool placementPoseIsValid;

    private BackendConnection backendConnector;
    private AssetBundle downloaded_bundle;
    private List<BackendConnection.BundleClass> availableBundles;
    

    /// <summary>
    /// Start is called before the first frame update
    /// <para>It initializes the ARRaycastManager and the BackendConnection and requests the list of all available objects.</para>
    /// </summary>
    void Start()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();

        this.backendConnector = new BackendConnection();

        // request information about all available bundles from backend
        StartCoroutine(backendConnector.getAvailableBundlesInfo((List<BackendConnection.BundleClass> availableBundles) => {
            this.availableBundles = availableBundles;
        }));
    }

    /// <summary>
    /// This method is executed every frame.
    /// <para>It updates the placement indicator and checks if and with how many fingers the user tapped the screen and acts accordingly.</para>
    /// <list type="bullet">
    ///   <item>One finger: Place the object.</item>
    ///   <item>Two fingers: Download the first object on the list.</item>
    /// </summary>
    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (Input.touchCount > 0) {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (placementPoseIsValid && Input.touchCount == 1)
                {
                    PlaceObject();
                }
                else if (Input.touchCount == 2)
                {
                    StartCoroutine(backendConnector.getObjectFromBackend(availableBundles[0], (GameObject objectToPlace) => {
                        this.objectToPlace = objectToPlace;
                        Debug.Log("INSPIRER " + this.objectToPlace);
                    }));
                }
            }
        }
    }

    /// <summary>
    /// Updates the position objectToPlace and the placement indicator will be placed.
    /// </summary>
    void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        
        placementPoseIsValid = arRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;
        }
    }

    /// <summary>
    /// Updates the placement indicator's position and rotation.
    /// </summary>
    void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else placementIndicator.SetActive(false);
    }
    
    /// <summary>
    /// Places the objectToPlace at the placementPose.
    /// </summary>
    void PlaceObject()
    {
        Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
    }
}
