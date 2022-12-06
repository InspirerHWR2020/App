using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

using Newtonsoft.Json;

public class ARTapToPlaceObject : MonoBehaviour
{
    /// object to be placed with PlaceObject()
    public GameObject objectToPlace;
    /// object that diesplays where the objectToPlace will be placed
    public GameObject placementIndicator;
    /// info to objectToPlace
    private BackendConnection.BundleClass objectToPlaceInfo;
    
    private ARRaycastManager arRaycastManager;
    private Pose placementPose;
    private bool placementPoseIsValid;

    /// instance of the BackendConnection class used to  communicate with the backend
    private BackendConnection backendConnector = new BackendConnection();
    /// list of all available bundles from the backend
    private List<BackendConnection.BundleClass> availableBundles;

    /// PreFab for UI list object of the available bundles
    public GameObject listObjectPrefab;
    

    /// <summary>
    /// Start is called before the first frame update
    /// <para>It initializes the ARRaycastManager and the BackendConnection and requests the list of all available objects.</para>
    /// </summary>
    void Start()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();

        // request information about all available bundles from backend
        this.ReloadAllObjectInfo();

        // Listener for SearchImageInput Text
        GameObject.Find("SearchImageInput").GetComponent<TMP_InputField>().onValueChanged.AddListener(delegate { this.FilterObjects(GameObject.Find("SearchImageInput").GetComponent<TMP_InputField>().text); });
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

        if (Input.touchCount == 1 && placementPoseIsValid)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                PlaceObject();
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
    /// Places the objectToPlace at the placementPose and adjusts it rotation.
    /// </summary>
    void PlaceObject()
    {
        // adjusting the rotation of the objectToPlace
        Pose pose = new Pose(placementPose.position, placementPose.rotation);
        Vector3 rot = pose.rotation.eulerAngles;
        rot = new Vector3(rot.x, rot.y + 180, rot.z);
        if (this.objectToPlaceInfo != null) 
        {
            rot = new Vector3(rot.x + this.objectToPlaceInfo.custom_rotation_x, rot.y + this.objectToPlaceInfo.custom_rotation_y, rot.z + this.objectToPlaceInfo.custom_rotation_z);
        }
        pose.rotation = Quaternion.Euler(rot);

        // needed for loaded gltf objects to not be displayed in the wrong position 
        if (this.objectToPlaceInfo.gltf == true) 
        {
            this.objectToPlace.SetActive(true);
        }
        // placing the object
        Instantiate(objectToPlace, pose.position, pose.rotation);
        if (this.objectToPlaceInfo.gltf == true) 
        {
            this.objectToPlace.SetActive(false);
        }
    }

    /// <summary>
    /// Loads the requested object from the backend if it is not already loaded.
    /// Otherwise it sets the objectToPlace to the requested object.
    /// </summary>
    void LoadObject(BackendConnection.BundleClass objectToPlaceInfo)
    {
        // object already loaded
        if (objectToPlaceInfo.loaded_object != null) 
        {
            this.objectToPlace = objectToPlaceInfo.loaded_object;
            this.objectToPlaceInfo = objectToPlaceInfo;
            return;
        }

        // load object
        StartCoroutine(backendConnector.getObjectFromBackend(objectToPlaceInfo, (GameObject objectToPlace) => {
            this.objectToPlace = objectToPlace;
            this.objectToPlaceInfo = objectToPlaceInfo;
            this.objectToPlaceInfo.loaded_object = objectToPlace;
        }));
    }

    /// <summary>
    /// Requests the list of all available objects from the backend.
    /// </summary>
    void ReloadAllObjectInfo() {
        Debug.Log("INSPIRER - ReloadAllObjectInfo");
        StartCoroutine(backendConnector.getAvailableBundlesInfo((List<BackendConnection.BundleClass> availableBundles) => {
            this.availableBundles = availableBundles;

            Debug.Log("INSPIRER available bundle count: " + this.availableBundles.Count);

            foreach (BackendConnection.BundleClass bundle in this.availableBundles) 
            {
                Debug.Log("INSPIRER bundle: " + bundle.file_name);
                GameObject uiElement = GameObject.Instantiate(this.listObjectPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                uiElement.name = "ListObject_" + bundle.id;
                uiElement.transform.SetParent(GameObject.Find("ObjectsCanvas").transform, false);
                GameObject.Find(uiElement.name + "/ObjectName").GetComponent<TextMeshProUGUI>().text = bundle.display_name;
                uiElement.GetComponent<Button>().onClick.AddListener(() => {
                    LoadObject(bundle);
                    GameObject.Find("User InterfaceCanvas").GetComponent<Animator>().SetTrigger("Show");
                });
            }
        }));
    }

    /// <summary>
    /// Searches the availableBundles for the one with the given name returns a list with the ids of the objects that match the given name.
    /// </summary>
    /// <param name="name">The name of the object to search for.</param>
    /// <returns>A list with the ids of the objects that match the given name.</returns>
    private List<int> SearchAvailableBundles(string name) {
        List<int> ids = new List<int>();
        if (name == "")
        {
            foreach (BackendConnection.BundleClass bundle in this.availableBundles) {
                ids.Add(bundle.id);
            }
            return ids;
        }
        foreach (BackendConnection.BundleClass bundle in this.availableBundles) {
            if (bundle.display_name.ToLower().Contains(name.ToLower())) {
                ids.Add(bundle.id);
            }
        }
        return ids;
    }


    /// <summary>
    /// Searches the ListObjects for the one with the given name and filters the ObjectsCanvas by the given name.
    /// </summary>
    /// <param name="name">The name of the ListObject to search for.</param>
    public void FilterObjects(string name) {
        Debug.Log("FilterObjects: " + name);
        List<int> bundleIds = SearchAvailableBundles(name);
        foreach (Transform child in GameObject.Find("ObjectsCanvas").transform) {
            if (child.name.StartsWith("ListObject_")) {
                if (bundleIds.Contains(int.Parse(child.name.Substring(11)))) {  // 11 = "ListObject_".Length
                    child.gameObject.SetActive(true);
                } else {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }
}
