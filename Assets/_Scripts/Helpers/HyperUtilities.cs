using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;

public static class HyperUtilities
{
    private static readonly string[] fileTypes = new string[]
    {
        ".asset",
        ".prefab",
    };

    // private static string WhereIs(string fileName, FileType fileType)
    // {
    //     string[] guids = AssetDatabase.FindAssets(fileName);
    //
    //     foreach (string guid in guids)
    //     {
    //         string path = AssetDatabase.GUIDToAssetPath(guid);
    //         if (path.EndsWith(fileTypes[(int)fileType]))
    //         {
    //             return path;
    //         }
    //     }
    //     Debug.Log($"Could not find the asset: {fileName}");
    //     return "";
    // }

    // public static object GetAsset(string fileName)
    // {
    //     var path = WhereIs(fileName, FileType.Asset);
    //     var file = (object)AssetDatabase.LoadAssetAtPath(path, typeof(object));
    //     return file;
    // }

    public static List<T> FindInterfacesOfType<T>()
    {
        List<T> interfaces = new List<T>();

        GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (var rootGameObject in rootGameObjects)
        {
            T[] childrenInterfaces = rootGameObject.GetComponentsInChildren<T>();

            foreach (var childInterface in childrenInterfaces)
            {
                interfaces.Add(childInterface);
            }
        }

        return interfaces;
    }
}
