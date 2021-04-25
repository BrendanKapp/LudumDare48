using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RockStudio
{
    public class SculptureCreator : MonoBehaviour
    {

        public static void GenerateComposition(int NumberOfVertices, Vector3 pos, Vector3 nor, Material rockmat)
        {
            if (GameObject.Find("Sculpture in progress") != null)
            {
                GameObject rockstructure = new GameObject("Sculpture");
                Undo.RegisterCreatedObjectUndo(rockstructure, "Create");

                Random.state = Random.state;
                var seed = Random.Range(0, 5000);
                Random.InitState(seed);

                foreach (Transform child in GameObject.Find("Sculpture in progress").transform)
                {
                    List<Vector3> VertexList = new List<Vector3>(NumberOfVertices);
                    pos = child.localPosition;
                    Vector3 scale = child.localScale;
                    Vector3 rot = child.localEulerAngles;
                    for (int i = 0; i < NumberOfVertices; i++) VertexList.Add((new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 0.5f));
                    CreateSculpture(VertexList, pos, Vector3.zero, rot, scale, rockmat);
                }
            }
        }

        public static void CreateSculpture(IEnumerable<Vector3> Vertices, Vector3 pos, Vector3 nor, Vector3 rot, Vector3 scale, Material rockmat)
        {
            GameObject rock = new GameObject("Temporary Sculpture");
            Undo.RegisterCreatedObjectUndo(rock, "Create");

            rock.transform.position = pos;
            rock.transform.eulerAngles = rot;
            rock.transform.localScale = scale;
            rock.transform.LookAt(pos - nor);
            rock.transform.parent = GameObject.Find("Sculpture").transform;

            MeshFilter meshFilter = (MeshFilter)rock.AddComponent(typeof(MeshFilter));  //Add a mesh filter.
            MeshRenderer renderer = rock.AddComponent(typeof(MeshRenderer)) as MeshRenderer; //Add a mesh renderer.

            if (renderer != null) renderer.material = rockmat;

            meshFilter.sharedMesh = RockCreation.CreateMesh(Vertices);
            meshFilter.sharedMesh.name = "Temporary Sculpture";

            Mesh lowpolymesh = RockMeshProcessor.MakeLowPoly(rock.transform, "Temporary Sculpture");
            RockMeshProcessor.BoxUV(lowpolymesh, rock.transform);
        }

        public static void GenerateCompositionFill(int NumberOfVertices, Vector3 pos, Vector3 nor, Material rockmat)
        {
            List<Vector3> VertexList = new List<Vector3>(NumberOfVertices);
            VertexList.Clear();
            
            Random.state = Random.state;
            var seed = Random.Range(0, 5000);
            Random.InitState(seed);

            if (GameObject.Find("Sculpture in progress") != null)
            {
                GameObject rockstructure = new GameObject();
                Undo.RegisterCreatedObjectUndo(rockstructure, "Create");
                rockstructure.name = "Sculpture";

                foreach (Transform child in GameObject.Find("Sculpture in progress").transform)
                {
                    Vector3 pos1 = child.localPosition;
                    Vector3 scale = child.localScale;
                    Vector3 rot = child.localEulerAngles;

                    for (int i = 0; i < NumberOfVertices; i++) VertexList.Add(new Vector3(Random.Range(-scale.x * 0.5f, scale.x * 0.5f) + pos1.x, Random.Range(-scale.y * 0.5f, scale.y * 0.5f) + pos1.y, Random.Range(-scale.z * 0.5f, scale.z * 0.5f) + pos1.z));
                }
            }
            CreateSculptureFill(VertexList, pos, nor, rockmat); //Create the rock.
        }

        public static void CreateSculptureFill(IEnumerable<Vector3> Vertices, Vector3 pos, Vector3 nor, Material rockmat)
        {
            GameObject rock = new GameObject("Temporary Sculpture");
            Undo.RegisterCreatedObjectUndo(rock, "Create");
            rock.transform.position = pos;
            rock.transform.LookAt(pos - nor);
            rock.transform.parent = GameObject.Find("Sculpture").transform;
            Selection.activeGameObject = rock;

            MeshFilter meshFilter = (MeshFilter)rock.AddComponent(typeof(MeshFilter));  //Add a mesh filter.
            MeshRenderer renderer = rock.AddComponent(typeof(MeshRenderer)) as MeshRenderer; //Add a mesh renderer.

            if (renderer != null) renderer.material = rockmat;

            meshFilter.sharedMesh = RockCreation.CreateMesh(Vertices);
            meshFilter.sharedMesh.name = "Temporary Sculpture";

            Mesh lowpolymesh = RockMeshProcessor.MakeLowPoly(rock.transform, "Temporary Sculpture");
            RockMeshProcessor.BoxUV(lowpolymesh, rock.transform);
        }
    }
}
