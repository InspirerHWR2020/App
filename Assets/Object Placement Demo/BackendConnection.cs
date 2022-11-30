using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

/// <summary>
/// This class is used to communicate with the backend. 
/// <para>It features retrieving a list of all available objects and the objects itself.</para
/// </summary>
public class BackendConnection
{
    private string database_url = "http://inspirer.lindstedt.berlin/database";
    private string assetbundle_storage_url = "http://inspirer.lindstedt.berlin/bundles";

    /// <summary>
    /// Retrieves the information about all available bundles from the backend and returns it with a callback function.
    /// </summary>
    public IEnumerator getAvailableBundlesInfo(System.Action<List<BundleClass>> callback) {
        UnityWebRequest www = UnityWebRequest.Get(this.database_url + "/bundles");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        } else {
            List<BundleClass> bundleClassList = JsonConvert.DeserializeObject<List<BundleClass>>(www.downloadHandler.text);
            callback(bundleClassList);
        }
    }

    /// <summary>
    /// Downloads the assetbundle associated with the information given as first parameter
    /// from the backend, extracts the object and returns it with a callback function.
    /// </summary>
    public IEnumerator getObjectFromBackend(BundleClass online_bundle_info, System.Action<GameObject> callback) {
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(this.assetbundle_storage_url + "/" + online_bundle_info.file_name);
        yield return www.SendWebRequest();
 
        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        }
        else {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
            callback(bundle.LoadAsset(online_bundle_info.asset_name) as GameObject);
        }
    }

    /// <summary>
    /// This class is used to store the information about an assetbundle.
    /// </summary>
    [Serializable]
    public class BundleClass {
        public int id { get; set; }
        public string file_name { get; set; }
        public string display_name { get; set; }
        public string asset_name { get; set; }
    }
}