using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deformer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 castPosition = transform.position;
        Debug.DrawRay(castPosition, transform.forward, Color.cyan);

        Ray raycast = new Ray(castPosition, transform.forward);
        RaycastHit hitInfo;

        if (Physics.Raycast(raycast, out hitInfo, 10f))
        {
            Mesh mesh = hitInfo.transform.GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;

            vertices[hitInfo.triangleIndex].x += mesh.vertices[hitInfo.triangleIndex].x * 0.01f;
            vertices[hitInfo.triangleIndex + 1].x += mesh.vertices[hitInfo.triangleIndex + 1].x * 0.01f;
            vertices[hitInfo.triangleIndex + 2].x += mesh.vertices[hitInfo.triangleIndex + 2].x * 0.01f;

            mesh.vertices = vertices;
        }
    }
}
