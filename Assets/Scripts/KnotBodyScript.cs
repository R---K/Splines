using UnityEngine;
using System.Collections;

public class KnotBodyScript : MonoBehaviour {
	Knot myMaster;
	// Use this for initialization
	public void SetMyMaster(Knot _k){	myMaster  = _k;	}
	public Knot GetMyMaster(){ return myMaster;}
	public void SetActiveSkin(bool _state){
		if(_state)
			GetComponent<Renderer>().material = ResourceManager.instance.materials[1];
		else
			GetComponent<Renderer>().material = ResourceManager.instance.materials[0];
	}
	public void SetNewPosition(Vector3 _newPos){
		//Debug.Log("New Position: "+_newPos.ToString());
		transform.position = _newPos;
		myMaster.position = new Vector2(_newPos.x,_newPos.y);
		myMaster.myParentSpline.GetComponent<Spline>().Redraw();
	}
}
