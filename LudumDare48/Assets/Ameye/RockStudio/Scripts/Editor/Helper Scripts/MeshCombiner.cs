using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RockStudio
{
    public class MeshCombiner : MonoBehaviour
    {
        //DONE
        public static GameObject Combine(string combinedMeshName, GameObject parentObject, bool MultipleMaterials)
        {
            // Lists to hold the filters and materials of the meshes we want to combine
            List<MeshFilter> filters = new List<MeshFilter>();
            List<Material> materials = new List<Material>();

            // Loop through the children of the parent object and get the filter/renderer of each child
            foreach (Transform childTransform in parentObject.GetComponentInChildren<Transform>())
            {
                foreach (MeshFilter childFilter in childTransform.GetComponentsInChildren<MeshFilter>())
                {
                    MeshRenderer renderer = childFilter.transform.GetComponent<MeshRenderer>();

                    if (childFilter.sharedMesh == null) continue; // Skip if the child has no mesh component
                    else filters.Add(childFilter); // Add the filter to the list of filters we created earlier

                    // Loop through the submeshes of each child
                    for (int i = 0; i < childFilter.sharedMesh.subMeshCount; i++)
                    {
                        Material[] sharedMaterials;

                        //TODO: simplify and understand this shit
                        if (renderer == null) sharedMaterials = null; // If the child has no renderer component, there are no materials
                        else sharedMaterials = renderer.sharedMaterials; // Add the material of the current child to sharedMaterials

                        if (sharedMaterials != null && i < sharedMaterials.Length) materials.Add(sharedMaterials[i]); // Add the shared materials to the materials
                        else materials.Add(null);
                    }
                }
            }

            if (MultipleMaterials)
            {
                // Create a mesh
                Mesh combinedMesh = Combine(filters); // Combine all the mesh filters
                combinedMesh.name = combinedMeshName;
                Undo.RegisterCreatedObjectUndo(combinedMesh, "Combine");

                // Create a gameobject
                GameObject combinedGameObject = new GameObject(combinedMeshName);
                Undo.RegisterCreatedObjectUndo(combinedGameObject, "Create");
                combinedGameObject.AddComponent<MeshFilter>().sharedMesh = combinedMesh;
                combinedGameObject.AddComponent<MeshRenderer>().sharedMaterials = materials.ToArray();
                combinedGameObject.transform.position = GetCorrectPosition(combinedMesh);
                Undo.RegisterCreatedObjectUndo(combinedGameObject, "Combine");

                return combinedGameObject;
            }

            else
            {
                CombineInstance[] combine = new CombineInstance[filters.ToArray().Length];

                for (int i = 0; i < filters.ToArray().Length; i++)
                {
                    if (filters[i].sharedMesh == null) continue;
                    combine[i].mesh = filters[i].sharedMesh;
                    combine[i].transform = filters[i].transform.localToWorldMatrix;
                }

                // Create a mesh
                Mesh combinedMesh = new Mesh();
                combinedMesh.name = combinedMeshName;
                combinedMesh.CombineMeshes(combine, true, true); // The default 'combine meshes' function
                Undo.RegisterCreatedObjectUndo(combinedMesh, "Combine");

                // Create a gameobject
                GameObject combinedGameObject = new GameObject(combinedMeshName);
                Undo.RegisterCreatedObjectUndo(combinedGameObject, "Create");
                combinedGameObject.AddComponent<MeshFilter>().sharedMesh = combinedMesh;
                combinedGameObject.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard")); ; //Add this back in when adding in mesh combiner
                combinedGameObject.transform.position = GetCorrectPosition(combinedMesh);
                Undo.RegisterCreatedObjectUndo(combinedGameObject, "Combine");

                return combinedGameObject;
            }
        }

        public static Vector3 GetCorrectPosition(Mesh mesh)
        {
            Vector3[] verts = mesh.vertices;
            Vector3 center = mesh.bounds.center;

            for (int v = 0; v < verts.Length; v++)
                verts[v] -= center;

            mesh.vertices = verts;
            mesh.RecalculateBounds();
            return center;
        }

        public static Mesh Combine(IEnumerable<MeshFilter> meshes)
        {
            Mesh combinedMesh = new Mesh();

            List<Color[]> colors = new List<Color[]>();
            List<Color32[]> colors32 = new List<Color32[]>();
            List<Vector2[]> uv = new List<Vector2[]>();

            List<Vector3[]> vertices = new List<Vector3[]>();
            List<Vector3[]> normals = new List<Vector3[]>();
            List<Vector4[]> tangents = new List<Vector4[]>();

            List<int[]> indices = new List<int[]>();
            List<MeshTopology> topology = new List<MeshTopology>();


            int vertOffset = 0;

            // Loop through all the meshfilters
            foreach (MeshFilter filter in meshes)
            {
                Mesh mesh = filter.sharedMesh;
                Vector3[] verts = mesh.vertices;
                Vector3[] norms = mesh.normals;

                // Loop through all the vertices
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    verts[i] = filter.transform.TransformPoint(verts[i]);
                    norms[i] = filter.transform.TransformDirection(norms[i]);
                }

                vertices.Add(verts);
                colors.Add(mesh.colors == null || mesh.colors.Length < 1 ? Fill<Color>(mesh.vertexCount) : mesh.colors);
                colors32.Add(mesh.colors32 == null || mesh.colors32.Length < 1 ? Fill<Color32>(mesh.vertexCount) : mesh.colors32);
                normals.Add(norms);
                tangents.Add(mesh.tangents == null || mesh.tangents.Length < 1 ? Fill<Vector4>(mesh.vertexCount) : mesh.tangents);
                uv.Add(mesh.uv == null || mesh.uv.Length < 1 ? Fill<Vector2>(mesh.vertexCount) : mesh.uv);


                // Loop through the submeshes and add the indices
                for (int i = 0; i < mesh.subMeshCount; i++)
                {
                    int[] ind = mesh.GetIndices(i);

                    for (int l = 0; l < ind.Length; l++)
                        ind[l] += vertOffset;

                    indices.Add(ind);
                    topology.Add(mesh.GetTopology(i));
                }

                vertOffset += mesh.vertexCount;
            }

            // Create a new mesh and assign all the verts, colors, norms, tangs,...
            combinedMesh = new Mesh();
            combinedMesh.vertices = vertices.SelectMany(x => x).ToArray();
            combinedMesh.colors = colors.SelectMany(x => x).ToArray();
            combinedMesh.colors32 = colors32.SelectMany(x => x).ToArray();
            combinedMesh.normals = normals.SelectMany(x => x).ToArray();
            combinedMesh.tangents = tangents.SelectMany(x => x).ToArray();
            combinedMesh.uv = uv.SelectMany(x => x).ToArray();
            combinedMesh.subMeshCount = indices.Count;

            for (int i = 0; i < indices.Count; i++)
            {
                combinedMesh.SetIndices(indices[i], topology[i], i);
            }

            // Take into account the vertex limit
            if (vertOffset > 64999)
                combinedMesh = null;

            return combinedMesh;
        }

        private static T[] Fill<T>(int count)
        {
            T[] val = new T[count];
            for (int i = 0; i < count; i++)
                val[i] = default(T);
            return val;
        }
    }
}
