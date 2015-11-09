using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {

	public static Main instance;
	public Rect _rect;
	public GameObject tmpObject;

	public List <GameObject> splines = new List<GameObject>();
	public void AddNewSpline(){
		splines.Add( GameObject.Instantiate(Resources.Load("Spline"),Vector3.zero,Quaternion.identity) as GameObject);
	}
	// Use this for initialization
	void Awake(){
		Screen.SetResolution(800,480,false);
		instance = this;
	}
	void Start () {
		Service.Initialization();
		AddNewSpline();

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	/*void CheckMeshCreate(){
		Vector3 [] Points = {new Vector3(0,0,0), new Vector3(100,100,0),new Vector3(200,0,0)};
		tmpObject.GetComponent<MeshFilter>().mesh = Service.CreateMesh(Points,5,tmpObject.GetComponent<MeshFilter>().mesh);
	}*/
	public void DebugScript(){
		Debug.Log("DebugScript");
	}
}
