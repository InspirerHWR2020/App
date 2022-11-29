using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class ARTapToPlaceObject : MonoBehaviour
{
    public GameObject objectToPlace; 
    public GameObject placementIndicator;
    
    private ARRaycastManager arRaycastManager;
    private Pose placementPose;
    private bool placementPoseIsValid;

    private string database_url;
    private string assetbundle_storage_url;
    private AssetBundle downloaded_bundle;
    private List<BundleClass> availableBundles;
    

    void Start()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        
        database_url = "http://inspirer.lindstedt.berlin/database";
        assetbundle_storage_url = "http://inspirer.lindstedt.berlin/bundles";

        StartCoroutine(GetAllBundleInfo());
        // StartCoroutine(GetAssetBundle("streetlampone"));
    }

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
                    // this.objectToPlace = Resources.Load("SpaceZeta_StreetLamps/Prefabs/StreetLamp1_Short (Concrete)") as GameObject;

                    // donwlod assetbundle
                    StartCoroutine(GetAssetBundle(availableBundles[0]));
                }
            }
        }
    }

    void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        // arRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);
        
        // placementPoseIsValid = hits.Count > 0;
        placementPoseIsValid = arRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;
        }
    }

    void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else placementIndicator.SetActive(false);
    }
    
    void PlaceObject()
    {
        Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
    }

    private IEnumerator GetAllBundleInfo() {
        UnityWebRequest www = UnityWebRequest.Get(this.database_url + "/bundles");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        } else {
            Debug.Log("INSPIRER " + www.downloadHandler.text);
            // BundleClassList bundleClassList = JsonUtility.FromJson<BundleClassList>("{\"availableBundles\":" + www.downloadHandler.text + "}");
            List<BundleClass> bundleClassList = JsonConvert.DeserializeObject<List<BundleClass>>(www.downloadHandler.text);
            this.availableBundles = bundleClassList;
            Debug.Log("INSPIRER " + bundleClassList);
            Debug.Log("INSPIRER " + bundleClassList[0]);
            Debug.Log("INSPIRER " + bundleClassList[0].id);
            Debug.Log("INSPIRER " + bundleClassList[0].file_name);
            Debug.Log("INSPIRER " + bundleClassList[0].asset_name);
            Debug.Log("INSPIRER " + bundleClassList[0].display_name);
        }
    }

    private IEnumerator GetAssetBundle(BundleClass online_bundle) {
        Debug.Log("INSPIRER " + online_bundle);
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(this.assetbundle_storage_url + "/" + online_bundle.file_name);
        Debug.Log("INSPIRER " + online_bundle.file_name);
        yield return www.SendWebRequest();
 
        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        }
        else {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
            this.objectToPlace = bundle.LoadAsset(online_bundle.asset_name) as GameObject;
        }
    }

    [Serializable]
    public class BundleClass {
        public int id { get; set; }
        public string file_name { get; set; }
        public string display_name { get; set; }
        public string asset_name { get; set; }
    }
}
