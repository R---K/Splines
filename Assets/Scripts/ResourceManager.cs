using UnityEngine;
using System.Collections;

public class ResourceManager : MonoBehaviour {
	public static ResourceManager instance;
	public Material[] materials;
	void Awake(){
		instance = this;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
