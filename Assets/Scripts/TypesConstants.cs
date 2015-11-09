using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Knot{
	public Vector2 position;
	public Vector2 M0,M1;
	public float t,c,b;
	public GameObject KnotBody;
	public GameObject myParentSpline;
	public KnotBodyScript knotBodyScript;
	public Knot (Vector2 _position,float _t,float _c,float _b,GameObject _go){
		t = _t; c= _c; b = _b;
		position = _position;
		myParentSpline = _go;
		KnotBody = GameObject.Instantiate(Resources.Load("KnotBodyPrefab"),_position,Quaternion.identity) as GameObject;
		knotBodyScript = KnotBody.GetComponent<KnotBodyScript>();
		knotBodyScript.SetMyMaster(this);
	}
	public bool DeleteMe(bool deleteWithoutRestriction){
		var _parentSpline = myParentSpline.GetComponent<Spline>();
		if(_parentSpline.ReturnKnotsCount() <= 3 && !deleteWithoutRestriction)
			return false;
		GameObject.Destroy(KnotBody);
		_parentSpline.RemoveKnot(this);
		return true;
	}
}
public class NearestLineData{
	public Knot begin,end;
	public float distance;
	public int insertIndex;
	public GameObject spline;
	public NearestLineData(GameObject _spline){
		distance = 999;
		spline = _spline;
	}

}

public enum CamStates{
	none,
	pressed,
	drag,
	click
}


public static class TypesConstants{
/// UI
	public const float PDTToggle = .05f,deltaAspect = 40f,closeDistanceToAddKnot = 10f; //root values

}
