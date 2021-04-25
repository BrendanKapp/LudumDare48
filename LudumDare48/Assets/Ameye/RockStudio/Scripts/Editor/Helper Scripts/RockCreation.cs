using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace RockStudio
{
    public class RockCreation : MonoBehaviour
    {
        public static void CreateRock(IEnumerable<Vector3> vertices, Vector3 pos, Vector3 nor, int RockOrientation, Material RockMaterial, string RockName, bool Rigidbody, int ColliderType, GameObject parent, bool showLog)
        {
            GameObject rock = new GameObject(RockName);
            Undo.RegisterCreatedObjectUndo(rock, "Create");

            rock.transform.position = pos;
            rock.transform.LookAt(pos - nor);
            rock.transform.parent = parent.transform;

            if (RockOrientation == 0) rock.transform.localEulerAngles = new Vector3(0, 0, 0);
            if (RockOrientation == 1) rock.transform.localEulerAngles = new Vector3(0, 0, 90);
            if (RockOrientation == 2) rock.transform.localEulerAngles = new Vector3(0, 90, 0);

            MeshFilter meshFilter = (MeshFilter)rock.AddComponent(typeof(MeshFilter));
            MeshRenderer renderer = rock.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

            if (renderer != null) renderer.material = RockMaterial;

            meshFilter.sharedMesh = CreateMesh(vertices);
            meshFilter.sharedMesh.name = RockName;

            Mesh lowpolymesh = RockMeshProcessor.MakeLowPoly(rock.transform, RockName);
            RockMeshProcessor.BoxUV(lowpolymesh, rock.transform);

            if (Rigidbody) rock.AddComponent(typeof(Rigidbody));

            if (ColliderType == 0) rock.AddComponent(typeof(BoxCollider));
            if (ColliderType == 1)
            {
                MeshCollider meshcollider = rock.AddComponent(typeof(MeshCollider)) as MeshCollider;
                meshcollider.convex = true;
                meshcollider.sharedMesh = rock.GetComponent<MeshFilter>().sharedMesh;
            }

            if(showLog) Debug.Log("Rock mesh saved with " + rock.GetComponent<MeshFilter>().sharedMesh.vertexCount + " vertices.");
        }

        public static Mesh CreateMesh(IEnumerable<Vector3> Vertices)
        {
            Mesh mesh = new Mesh { name = "RockMesh" };
            List<int> triangles = new List<int>();

            var vertices = Vertices.Select(x => new Vertex(x)).ToList();

            var result = MIConvexHull.ConvexHull.Create(vertices);
            mesh.vertices = result.Points.Select(x => x.ToVec()).ToArray();
            var xxx = result.Points.ToList();

            foreach (var face in result.Faces)
            {
                triangles.Add(xxx.IndexOf(face.Vertices[0]));
                triangles.Add(xxx.IndexOf(face.Vertices[1]));
                triangles.Add(xxx.IndexOf(face.Vertices[2]));
            }
            mesh.triangles = triangles.ToArray();

            mesh.RecalculateNormals();
            return mesh;
        }
    }
}