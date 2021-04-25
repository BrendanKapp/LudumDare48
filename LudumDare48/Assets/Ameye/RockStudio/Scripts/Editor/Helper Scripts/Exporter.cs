using UnityEngine;
using UnityEditor;
using UnityFBXExporter;

namespace RockStudio
{
    public class Exporter : MonoBehaviour
    {
        public static string ExportGameObject(GameObject gameObj, bool copyMaterials, bool copyTextures, string oldPath = null)
        {
            oldPath = RockStudio.DefaultPath;

            if(gameObj == null)
            {
                EditorUtility.DisplayDialog("Object is null", "Please select any GameObject to Export to FBX", "Okay");
                return null;
            }

            string newPath = GetNewPath(gameObj, oldPath);

            if(newPath != null && newPath.Length != 0)
            {
                bool isSuccess = FBXExporter.ExportGameObjToFBX(gameObj, newPath, copyMaterials, copyTextures);

                if(isSuccess)
                {
                    return newPath;
                }
                else
                    EditorUtility.DisplayDialog("Warning", "The extension probably wasn't an FBX file, could not export.", "Okay");
            }
            return null;
        }

        private static string GetNewPath(GameObject gameObject, string oldPath = null)
        {
            string name = gameObject.name;

            string newPath = null;
            if(oldPath == null)
                newPath = EditorUtility.SaveFilePanelInProject("Export as .fbx", name + ".fbx", "fbx", "Export " + name + " GameObject to a FBX file");
            else
            {
                if(oldPath.StartsWith("/Assets"))
                {
                    oldPath = Application.dataPath.Remove(Application.dataPath.LastIndexOf("/Assets"), 7) + oldPath;
                    oldPath = oldPath.Remove(oldPath.LastIndexOf('/'), oldPath.Length - oldPath.LastIndexOf('/'));
                }
                newPath = EditorUtility.SaveFilePanel("Export FBX File", oldPath, name + ".fbx", "fbx");
            }

            int assetsIndex = newPath.IndexOf("Assets");

            if(assetsIndex < 0)
                return null;

            if(assetsIndex > 0)
                newPath = newPath.Remove(0, assetsIndex);

            return newPath;
        }
        
        public static string ChangePath(string path)
        {
            string emptyString = "";
            path = EditorUtility.SaveFolderPanel("Choose Folder", "", "");
            if(string.IsNullOrEmpty(path)) return emptyString;
            path = FileUtil.GetProjectRelativePath(path);
            return path;
        }
    }
}
