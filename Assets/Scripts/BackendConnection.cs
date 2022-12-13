using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Siccity.GLTFUtility;

/// <summary>
/// This class is used to communicate with the backend. 
/// <para>It features retrieving a list of all available objects and the objects itself.</para>
/// </summary>
public class BackendConnection
{
    /// url to the postgrest REST-API for the postgreSQL database
    private string database_url = "";
    /// url to the file server that hosts the asset bundles and gltf files
    private string assetbundle_storage_url = "";

    public BackendConnection() {
        this.loadConfig();
    }

    /// <summary>
    /// Loads the configuration for backend access from Settings.cs.
    /// </summary>
    public void loadConfig() {
        Debug.Log("INSPIRER loading backend config");

        this.database_url = Settings.backendDatabaseUrl;
        this.assetbundle_storage_url = Settings.backendAssetsUrl;

        Debug.Log("INSPIRER successfully loaded backend config");
    }

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
        Debug.Log("INSPIRER downloading asset from: " + this.assetbundle_storage_url + "/" + online_bundle_info.file_name);
        
        if (online_bundle_info.gltf == false) {
            Debug.Log("INSPIRER downloading NON-gltf file");
            UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(this.assetbundle_storage_url + "/" + online_bundle_info.file_name);
            yield return www.SendWebRequest();
    
            if (www.result != UnityWebRequest.Result.Success) {
                Debug.Log(www.error);
            }
            else {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
                callback(bundle.LoadAsset(online_bundle_info.asset_name) as GameObject);
            }
        } else {
            Debug.Log("INSPIRER downloading gltf file");
            
            UnityWebRequest www = UnityWebRequest.Get(this.assetbundle_storage_url + "/" + online_bundle_info.file_name);
            yield return www.SendWebRequest();
    
            if (www.result != UnityWebRequest.Result.Success) {
                Debug.Log(www.error);
            } else {
                // save the downloaded file to the temporary directory
                string path = Application.temporaryCachePath + "/" + online_bundle_info.file_name;
                System.IO.File.WriteAllBytes(path, www.downloadHandler.data);

                // download the accompanying files (textures, etc.)
                string directory_name = online_bundle_info.file_name.Substring(0, online_bundle_info.file_name.IndexOf("."));
                foreach (string add_file_name in online_bundle_info.additional_files) {
                    UnityWebRequest add_www = UnityWebRequest.Get(this.assetbundle_storage_url + "/" + directory_name + "/" + add_file_name);
                    add_www.SendWebRequest();
                    if (add_www.result != UnityWebRequest.Result.Success) {
                        Debug.Log(add_www.error);
                    } else {
                        System.IO.File.WriteAllBytes(Application.temporaryCachePath + "/" + add_file_name, add_www.downloadHandler.data);
                    }
                }

                GameObject result = Importer.LoadFromFile(path);
                callback(result);
                //GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
                
                result.SetActive(false);
            }
        }
    }

    /// <summary>
    /// This class is used to store the information about an assetbundle.
    /// </summary>
    [Serializable]
    public class BundleClass {
        /// Unique identifier of the bundle
        public int id { get; set; }
        /// Name of the asset bundle or gltf file
        public string file_name { get; set; }
        /// Name to show in the UI
        public string display_name { get; set; }
        /// Name of the prefab in the asset bundle
        public string asset_name { get; set; }
        /// True if the bundle is a gltf file, false if it is an asset bundle
        public bool gltf { get; set; }
        /// List of additional files that are required for the gltf file (textures, etc.)
        public string[] additional_files { get; set; }
        /// Additional rotation of the object on the x axis
        public int custom_rotation_x { get; set; }
        /// Additional rotation of the object on the y axis
        public int custom_rotation_y { get; set; }
        /// Additional rotation of the object on the z axis
        public int custom_rotation_z { get; set; }
        /// GameObject already loaded from the backend
        public GameObject loaded_object { get; set; } = null;
    }
}