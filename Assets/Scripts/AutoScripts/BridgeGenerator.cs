using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class BridgeGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] PlanksPrefab;
    public GameObject SupportPrefab;
    public GameObject RopePrefab;

    [Header("Meshes")]
    public Mesh PlankMeshPreview;
    public Mesh SupportMeshPreview;
    public Mesh RopeMeshPreview;

    [Header("Posiciones")]
    public Vector3 pos1;
    public Vector3 pos2;
    public Vector3 posSoporte1;
    public Vector3 posSoporte2;
    public Vector3 posSoporte3;
    public Vector3 posSoporte4;
    public Vector3 RopeOffset;

    [Header("Variables")]
    [Range(0.05f, 10f)] public float PlankSeparation;
    [Range(0.05f, 10f)] public float SupportSeparation;
    [Range(0.005f, 1f)] public float RopeSeparation;

    public void GenerateBridge()
    {
        var Father = new GameObject("Bridge");
        Father.transform.position = (pos2 - pos1)/2 + pos1;
        var Distance = Vector3.Distance(pos1, pos2);

        //planks
        Rigidbody PreviousPlankRB = null;
        for (float i = 0; i < Distance; i += PlankSeparation)
        {
            var obj = Instantiate(PlanksPrefab[Random.Range(0, PlanksPrefab.Length)], Father.transform);
            var Quat = Quaternion.FromToRotation(obj.transform.forward, pos2 - pos1);
            obj.transform.SetPositionAndRotation(pos1 + (pos2 - pos1).normalized * i
                ,Quaternion.Euler(new Vector3(0, Quat.eulerAngles.y + 180 * Mathf.Round(Random.Range(0, 2)), 0)));
            if (PreviousPlankRB == null)
            {
                obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                PreviousPlankRB = obj.GetComponent<Rigidbody>();
                continue;
            }
            obj.GetComponent<FixedJoint>().connectedBody = PreviousPlankRB;
            PreviousPlankRB = obj.GetComponent<Rigidbody>();
        }
        if (PreviousPlankRB == null) 
            return;
        PreviousPlankRB.constraints = RigidbodyConstraints.FreezeAll;
        PreviousPlankRB = PreviousPlankRB.GetComponent<Rigidbody>();

        //Suports
        Instantiate(SupportPrefab, pos1 + (posSoporte1 - pos1).normalized * SupportSeparation
            , Quaternion.Euler(new Vector3(0, Random.Range(-180, 180), 0)), Father.transform);
        Instantiate(SupportPrefab, pos1 + (posSoporte2 - pos1).normalized * SupportSeparation
            , Quaternion.Euler(new Vector3(0, Random.Range(-180, 180), 0)), Father.transform);
        Instantiate(SupportPrefab, pos2 + (posSoporte3 - pos2).normalized * SupportSeparation
            , Quaternion.Euler(new Vector3(0, Random.Range(-180, 180), 0)), Father.transform);
        Instantiate(SupportPrefab, pos2 + (posSoporte4 - pos2).normalized * SupportSeparation
            , Quaternion.Euler(new Vector3(0, Random.Range(-180, 180), 0)), Father.transform);

        //Rope 1
        var RopePos1 = pos1 + (posSoporte1 - pos1).normalized * SupportSeparation + RopeOffset;
        var RopePos2 = pos2 + (posSoporte3 - pos2).normalized * SupportSeparation + RopeOffset;
        Distance = Vector3.Distance(RopePos1, RopePos2);
        Rigidbody PreviousRopeRB = null;
        for (float i = 0; i < Distance; i += RopeSeparation)
        {
            var obj = Instantiate(RopePrefab, Father.transform);
            var Quat = Quaternion.FromToRotation(obj.transform.forward, RopePos2 - RopePos1);
            obj.transform.SetPositionAndRotation(RopePos1 + (RopePos2 - RopePos1).normalized * i
                , Quaternion.Euler(new Vector3(0, Quat.eulerAngles.y, 0)));
            if (PreviousRopeRB == null)
            {
                obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                PreviousRopeRB = obj.GetComponent<Rigidbody>();
                continue;
            }
            obj.GetComponent<HingeJoint>().connectedBody = PreviousRopeRB;
            PreviousRopeRB = obj.GetComponent<Rigidbody>();
        }
        if (PreviousRopeRB == null)
            return;
        PreviousRopeRB.constraints = RigidbodyConstraints.FreezeAll;

        //Rope 1
        RopePos1 = pos1 + (posSoporte2 - pos1).normalized * SupportSeparation + RopeOffset;
        RopePos2 = pos2 + (posSoporte4 - pos2).normalized * SupportSeparation + RopeOffset;
        Distance = Vector3.Distance(RopePos1, RopePos2);
        PreviousRopeRB = null;
        for (float i = 0; i < Distance; i += RopeSeparation)
        {
            var obj = Instantiate(RopePrefab, Father.transform);
            var Quat = Quaternion.FromToRotation(obj.transform.forward, RopePos2 - RopePos1);
            obj.transform.SetPositionAndRotation(RopePos1 + (RopePos2 - RopePos1).normalized * i
                , Quaternion.Euler(new Vector3(0, Quat.eulerAngles.y, 0)));
            if (PreviousRopeRB == null)
            {
                obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                PreviousRopeRB = obj.GetComponent<Rigidbody>();
                continue;
            }
            obj.GetComponent<HingeJoint>().connectedBody = PreviousRopeRB;
            PreviousRopeRB = obj.GetComponent<Rigidbody>();
        }
        if (PreviousRopeRB == null)
            return;
        PreviousRopeRB.constraints = RigidbodyConstraints.FreezeAll;
        PreviousRopeRB = PreviousRopeRB.GetComponent<Rigidbody>();

        gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(pos1, 1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(pos2, 1.5f);
        Gizmos.color = Color.white;
        var Distance = Vector3.Distance(pos1, pos2);
        var meshScale = new Vector3(1.5f, 1f, 1f);
        for (float i = 0; i < Distance; i += PlankSeparation)
        {
            if (PlankMeshPreview == null)
                Gizmos.DrawSphere(pos1 + (pos2 - pos1).normalized * i, 0.5f);
            else
                Gizmos.DrawMesh(
                    PlankMeshPreview,
                    pos1 + (pos2 - pos1).normalized * i,
                    Quaternion.Euler(
                        new Vector3(
                            0,
                            Quaternion.FromToRotation(Vector3.forward, pos2 - pos1).eulerAngles.y,
                            0)
                        )
                    , meshScale);
        }
        if(SupportMeshPreview == null)
        {
            Gizmos.DrawCube(pos1 + (posSoporte1 - pos1).normalized * SupportSeparation, Vector3.one);
            Gizmos.DrawCube(pos1 + (posSoporte2 - pos1).normalized * SupportSeparation, Vector3.one);
            Gizmos.DrawCube(pos2 + (posSoporte3 - pos2).normalized * SupportSeparation, Vector3.one);
            Gizmos.DrawCube(pos2 + (posSoporte4 - pos2).normalized * SupportSeparation, Vector3.one);
        }
        else
        {
            Gizmos.DrawMesh(SupportMeshPreview, pos1 + (posSoporte1 - pos1).normalized * SupportSeparation, Quaternion.identity);
            Gizmos.DrawMesh(SupportMeshPreview, pos1 + (posSoporte2 - pos1).normalized * SupportSeparation, Quaternion.identity);
            Gizmos.DrawMesh(SupportMeshPreview, pos2 + (posSoporte3 - pos2).normalized * SupportSeparation, Quaternion.identity);
            Gizmos.DrawMesh(SupportMeshPreview, pos2 + (posSoporte4 - pos2).normalized * SupportSeparation, Quaternion.identity);
        }
        if(RopeMeshPreview == null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pos1 + (posSoporte1 - pos1).normalized * SupportSeparation + RopeOffset, 0.2f);
            Gizmos.DrawSphere(pos2 + (posSoporte3 - pos2).normalized * SupportSeparation + RopeOffset, 0.2f);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(pos1 + (posSoporte2 - pos1).normalized * SupportSeparation + RopeOffset, 0.2f);
            Gizmos.DrawSphere(pos2 + (posSoporte4 - pos2).normalized * SupportSeparation + RopeOffset, 0.2f);
            Gizmos.color = Color.white;
        }
        else
        {
            var SuportPoint1 = pos1 + (posSoporte1 - pos1).normalized * SupportSeparation + RopeOffset;
            var SuportPoint3 = pos2 + (posSoporte3 - pos2).normalized * SupportSeparation + RopeOffset;
            var firstSeparation = Vector3.Distance(SuportPoint1, SuportPoint3);
            var meshRotation = Quaternion.FromToRotation(Vector3.up, SuportPoint3 - SuportPoint1);
            meshScale = new Vector3(0.2f, 0.3f, 0.2f);
            for (float i = 0; i < firstSeparation; i += RopeSeparation)
            {
                Gizmos.DrawMesh(RopeMeshPreview, SuportPoint1 + (SuportPoint3 - SuportPoint1).normalized * i, meshRotation, meshScale);
            }
            var SuportPoint2 = pos1 + (posSoporte2 - pos1).normalized * SupportSeparation + RopeOffset;
            var SuportPoint4 = pos2 + (posSoporte4 - pos2).normalized * SupportSeparation + RopeOffset;
            var SecondSeparation = Vector3.Distance(SuportPoint1, SuportPoint3);
            meshRotation = Quaternion.FromToRotation(Vector3.up, SuportPoint4 - SuportPoint2);
            for (float i = 0; i < SecondSeparation; i += RopeSeparation)
            {
                Gizmos.DrawMesh(RopeMeshPreview, SuportPoint2 + (SuportPoint4 - SuportPoint2).normalized * i, meshRotation, meshScale);
            }
        }
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(BridgeGenerator))]
public class BridgeGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        BridgeGenerator generator = (BridgeGenerator)target;
        GUILayout.Space(10);
        if(GUILayout.Button("Create"))
        {
            generator.GenerateBridge();
            Debug.Log("Bridge created succesfully");
        }

    }

    private void OnSceneGUI()
    {
        BridgeGenerator generator = (BridgeGenerator)target;

        generator.pos1 = Handles.PositionHandle(generator.pos1, Quaternion.identity);
        generator.pos2 = Handles.PositionHandle(generator.pos2, Quaternion.identity);
        Handles.DrawDottedLine(generator.pos1, generator.pos2, 4.0f);
        generator.posSoporte1 = Handles.PositionHandle(generator.posSoporte1, Quaternion.identity);
        Handles.DrawLine(generator.pos1, generator.posSoporte1);
        generator.posSoporte2 = Handles.PositionHandle(generator.posSoporte2, Quaternion.identity);
        Handles.DrawLine(generator.pos1, generator.posSoporte2);
        generator.posSoporte3 = Handles.PositionHandle(generator.posSoporte3, Quaternion.identity);
        Handles.DrawLine(generator.pos2, generator.posSoporte3);
        generator.posSoporte4 = Handles.PositionHandle(generator.posSoporte4, Quaternion.identity);
        Handles.DrawLine(generator.pos2, generator.posSoporte4);
        generator.RopeOffset = Handles.PositionHandle(generator.RopeOffset + generator.pos1, Quaternion.identity) - generator.pos1;
        Handles.DrawLine(generator.pos1, generator.RopeOffset + generator.pos1);
    }

    
}
#endif