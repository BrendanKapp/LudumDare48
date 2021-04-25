using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RockStudio
{
    public class MeshUtilities : MonoBehaviour
    {
        public static Vector3 GetRandomPointOnMesh(Mesh mesh)
        {
            int triangleCount = mesh.triangles.Length / 3;
            float[] sizes = new float[triangleCount];
            for (int i = 0; i < triangleCount; i++)
            {
                Vector3 va = mesh.vertices[mesh.triangles[i * 3 + 0]];
                Vector3 vb = mesh.vertices[mesh.triangles[i * 3 + 1]];
                Vector3 vc = mesh.vertices[mesh.triangles[i * 3 + 2]];

                sizes[i] = .5f * Vector3.Cross(vb - va, vc - va).magnitude;
            }

            float[] cumulativeSizes = new float[sizes.Length];
            float total = 0;

            for (int i = 0; i < sizes.Length; i++)
            {
                total += sizes[i];
                cumulativeSizes[i] = total;
            }

            float randomsample = Random.value * total;

            int triIndex = -1;

            for (int i = 0; i < sizes.Length; i++)
            {
                if (randomsample <= cumulativeSizes[i])
                {
                    triIndex = i;
                    break;
                }
            }

            if (triIndex == -1) Debug.LogError("triIndex should never be -1");

            Vector3 a = mesh.vertices[mesh.triangles[triIndex * 3 + 0]];
            Vector3 b = mesh.vertices[mesh.triangles[triIndex * 3 + 1]];
            Vector3 c = mesh.vertices[mesh.triangles[triIndex * 3 + 2]];

            float r = Random.value;
            float s = Random.value;

            if (r + s >= 1)
            {
                r = 1 - r;
                s = 1 - s;
            }

            Vector3 pointOnMesh = a + r * (b - a) + s * (c - a);
            return pointOnMesh;
        }

        public static bool ChildrenAllGameObjects(GameObject obj)
        {
            foreach (Transform child in obj.transform)
            {
                if (child.GetComponent<MeshFilter>() == null)
                    return false;
            }

            return true;
        }

        public static int GetVerts(GameObject obj)
        {
            int numberOfVerts = 0;

            foreach (Transform child in obj.transform)
            {
                numberOfVerts += child.gameObject.GetComponent<MeshFilter>().sharedMesh.vertexCount;

            }

            if (obj.gameObject.transform.childCount == 0 && obj.gameObject.GetComponent<MeshFilter>() != null) numberOfVerts = obj.gameObject.GetComponent<MeshFilter>().sharedMesh.vertexCount;

            return numberOfVerts;
        }

        public static void RemoveGameObject(string name)
        {
            if (GameObject.Find(name) != null) DestroyImmediate(GameObject.Find(name));
        }

        public static List<Vector3> GetRandomPointsWithinCube(int numberOfPoints, float width, float height,
            float depth)
        {
            List<Vector3> points = new List<Vector3>();

            for (int i = 0; i < numberOfPoints; i++)
                points.Add(new Vector3(Random.Range(-depth * 0.5f, depth * 0.5f),
                    Random.Range(-height * 0.5f, height * 0.5f),
                    Random.Range(-width * 0.5f, width * 0.5f)));

            return points;
        }

        public static List<Vector3> GetRandomPointsWithinSphere(int numberOfPoints, float radius)
        {
            List<Vector3> points = new List<Vector3>();

            for (int i = 0; i < numberOfPoints; i++) points.Add(Random.insideUnitSphere * radius);

            return points;
        }

        public static List<Vector3> GetRandomPointsWithinCrystal(int numberOfPoints, bool tetragonal, bool oneSided,
            float baseWidth, float baseHeight, float tipProtrusion, float tipFlatness)
        {
            List<Vector3> points = new List<Vector3>();

            if (tetragonal)
            {
                for (int i = 0; i < numberOfPoints; i++)
                    points.Add(new Vector3(Random.Range(-baseHeight, baseHeight),
                        Random.Range(-baseWidth, baseWidth),
                        Random.Range(-baseWidth, baseWidth)));
            }

            else
            {
                for (int i = 0; i < numberOfPoints; i++)
                    points.Add(new Vector3(Random.Range(-baseHeight, baseHeight),
                        Random.Range(-baseWidth * 1.732f, baseWidth * 1.732f),
                        Random.Range(-baseWidth, baseWidth)));
                for (int i = 0; i < numberOfPoints; i++)
                    points.Add(new Vector3(Random.Range(-baseHeight, baseHeight), 0,
                        Random.Range(-baseWidth * 1.732f, baseWidth * 1.732f)));
            }

            if (oneSided)
            {
                for (int i = 0; i < numberOfPoints; i++)
                    points.Add(new Vector3(Random.Range(-baseHeight, baseHeight + tipProtrusion),
                        Random.Range(-baseWidth * (tipFlatness / 10f), baseWidth * (tipFlatness / 10f)),
                        Random.Range(-baseWidth * 1.732f * (tipFlatness / 10f),
                            baseWidth * 1.732f * (tipFlatness / 10f))));
            }

            else
            {
                for (int i = 0; i < numberOfPoints; i++)
                    points.Add(new Vector3(
                        Random.Range(-baseHeight - tipProtrusion, baseHeight + tipProtrusion),
                        Random.Range(-baseWidth * (tipFlatness / 10f), baseWidth * (tipFlatness / 10f)),
                        Random.Range(-baseWidth * 1.732f * (tipFlatness / 10f),
                            baseWidth * 1.732f * (tipFlatness / 10f))));
            }

            return points;
        }

        public static List<Vector3> GetRandomPointsOnMesh(int numberOfPoints, Mesh mesh)
        {
            List<Vector3> points = new List<Vector3>();

            for (int i = 0; i < numberOfPoints; i++)
            {
                Vector3 p = MeshUtilities.GetRandomPointOnMesh(mesh);

                float min = 1f;
                float max = 1f;

                p *= Random.Range(min, max);
                points.Add(p);
            }

            return points;
        }
    }
}