using UnityEditor;
using UnityEngine;
using System.Collections;

namespace RockStudio
{
    [CustomEditor(typeof(Handle))]
    public class HandleEditor : Editor
    {
        private void OnScene(SceneView sceneview)
        {
            Handle myObj = (Handle)target;
            Handles.color = new Color(0.15f, 0.75f, 1f);

            if (myObj != null) Handles.matrix = myObj.transform.localToWorldMatrix;
            if (myObj != null) Handles.DrawWireCube(Vector3.zero, Vector3.one);
        }

        void OnEnable()
        {
            SceneView.duringSceneGui -= OnScene;
            SceneView.duringSceneGui += OnScene;
        }
    }
}
