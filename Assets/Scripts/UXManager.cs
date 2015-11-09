using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UXManager : MonoBehaviour {
	public static UXManager instance;
	CameraScript MainCamScript;
	Camera MainCamera;
	Knot selectedKnot;
	float pressDownTimer;
	GameObject activeField;


	const string inputFieldPath = "Canvas/Panel/";

	float PDTToggle =  TypesConstants.PDTToggle;
	Vector3 savedMousePosition;
	CamStates camState = CamStates.none;
	// Use this for initialization
	void Awake(){
		instance = this;
		MainCamScript = Camera.main.GetComponent<CameraScript>();
		MainCamera = Camera.main.GetComponent<Camera>();
		if(MainCamScript == null || MainCamera == null){
			Debug.LogWarning("No Camera found");
			Debug.Break();
		}

	}
	void Start () {
		SetInputFields(false);
	}
	
	// Update is called once per frame
	void Update () {

		DebugKeyControl();
		ManageButtons();
		CameraZoom();
	}
	void CameraZoom(){
		if(Input.GetAxis("Mouse ScrollWheel") != 0){
			float _aspect = MainCamera.orthographicSize;
			if(Input.GetAxis("Mouse ScrollWheel") > 0){
				_aspect += TypesConstants.deltaAspect;
			}
			if(Input.GetAxis("Mouse ScrollWheel") < 0){
				_aspect -= TypesConstants.deltaAspect;
			}
			MainCamera.orthographicSize = Mathf.Clamp(_aspect,250,500);
		}
	}
	void ManageButtons(){
		ProcessLeftButton();
		ProcessRightButton();
	}
	void ProcessLeftButton(){
		if(Input.GetMouseButton(0) && Service.IsMouseInsideArea()){
			pressDownTimer +=Time.deltaTime;
			Knot _knot = GetReturnedKnot(Input.mousePosition);
			if(_knot != null){
				if(camState != CamStates.pressed){
					if(selectedKnot != _knot)
					{
						_knot.KnotBody.GetComponent<KnotBodyScript>().SetActiveSkin(true);
						if(selectedKnot != null)
							selectedKnot.KnotBody.GetComponent<KnotBodyScript>().SetActiveSkin(false);
					}
					selectedKnot = _knot;
					SetInputFields();
					if(pressDownTimer > PDTToggle && camState != CamStates.pressed){
						camState = CamStates.drag;
					}
				}
			}// knot != null
			else{// knot == null
				if(camState != CamStates.drag)
					if(pressDownTimer > PDTToggle){
						MainCamScript.CamMove();
						camState = CamStates.pressed;
					}
			}// knot == null
		}// mouseButtonpressedDown
		if(Input.GetMouseButtonUp(0)){
			pressDownTimer = 0;
			camState = CamStates.none;
			MainCamScript.CamStopMoving();
			pressDownTimer = 0;
		}
		if(pressDownTimer > PDTToggle && camState == CamStates.drag && Service.IsMouseInsideArea())
			MoveKnot(selectedKnot);
	}//void ProcessLeftButton()

	void ProcessRightButton(){
			if(Input.GetMouseButtonDown(1)){
				Knot _knot = GetReturnedKnot(Input.mousePosition);
				if(_knot == null)
				{
					AddKnot();
				} 
				else
				{
					if(selectedKnot != _knot)
						{
							_knot.KnotBody.GetComponent<KnotBodyScript>().SetActiveSkin(true);
							if(selectedKnot != null)
								selectedKnot.KnotBody.GetComponent<KnotBodyScript>().SetActiveSkin(false);
							}
							selectedKnot = _knot;
							SetInputFields();
							DeleteKnot(selectedKnot);
						}
				}


	}
	void DebugKeyControl(){

	}
	Knot GetReturnedKnot(Vector3 position){
		RaycastHit hit = new RaycastHit();
		Ray _ray = MainCamera.ScreenPointToRay(new Vector3(Input.mousePosition.x,Input.mousePosition.y,MainCamera.nearClipPlane));
		if(Physics.Raycast(_ray,out hit))
			if(hit.transform.tag == "Knot")
				return hit.transform.GetComponent<KnotBodyScript>().GetMyMaster();
		return null;
	}
	void MoveKnot(Knot _knot){
		Vector3 tmpVect = MainCamera.ScreenToWorldPoint(Input.mousePosition);

		_knot.knotBodyScript.SetNewPosition(new Vector3(tmpVect.x,tmpVect.y,_knot.KnotBody.transform.position.z));

	}
	public void ChangedValue(){

		if(activeField == null)
			return;
		var txtComponent = activeField.GetComponent<InputField>();
		string _value = txtComponent.text;
		Debug.Log(_value);
		float v;
		try{
			v = (float)Convert.ToDouble(_value);

		}
		catch(Exception _ex){
			Debug.Log(_ex.Message);
			txtComponent.text = "0.0";
			return;
		}
		v = Mathf.Clamp(v,-1,1);

		txtComponent.text =  (v == Mathf.Round(v)) ? Convert.ToString(v)+".0" : Convert.ToString(v);
		Debug.Log("true val: "+v.ToString()+ "  Changed Value" + txtComponent.text);
		if(selectedKnot!=null){
			switch (activeField.name){
				case "t_InputField": selectedKnot.t = v; break;
				case "b_InputField": selectedKnot.b = v; break;
				case "c_InputField": selectedKnot.c = v; break;
			}
			selectedKnot.myParentSpline.GetComponent<Spline>().Redraw();
		}

	}
	public void GetActiveField(BaseEventData _eventData){
		activeField = _eventData.selectedObject;
	}
	void SetInputFields(bool _state){
		InputField []allFields = GameObject.Find(inputFieldPath).GetComponentsInChildren<InputField>();
		foreach(InputField _f in allFields){
			_f.interactable = _state;
			if(_state == false)
				_f.text = "--";
		}
	}
	void SetInputFields(){
		if(selectedKnot == null)
			{
				SetInputFields(false);
			}
		else
			{
				SetInputFields(true);
				var field = GameObject.Find(inputFieldPath+"t_InputField").GetComponent<InputField>();
				field.text = Convert.ToString(selectedKnot.t);
				field = GameObject.Find(inputFieldPath+"b_InputField").GetComponent<InputField>();
				field.text = Convert.ToString(selectedKnot.b);
				field = GameObject.Find(inputFieldPath+"c_InputField").GetComponent<InputField>();
				field.text = Convert.ToString(selectedKnot.c);
			}
	}
	void DeleteKnot(Knot _knot){
		//_knot.KnotBody.GetComponent<KnotBodyScript>().SetActiveSkin(false);
		Spline _sp = _knot.myParentSpline.GetComponent<Spline>();
		if(_knot.DeleteMe(false)){
			selectedKnot = null;
			_sp.Redraw();
			SetInputFields();
		}
	}
	void AddKnot(){
		// calculate the nearest straight line
		NearestLineData nearestLine = new NearestLineData(null); 
		Vector3 vx = MainCamera.ScreenToWorldPoint(Input.mousePosition);
		Vector2 worldCoords = new Vector2(vx.x,vx.y);

		foreach(GameObject _sp in Main.instance.splines){

			NearestLineData nearestLineData = _sp.GetComponent<Spline>().GetNearestLineData(worldCoords);
			if(nearestLine.distance > nearestLineData.distance)
				nearestLine = nearestLineData;
		}
		if(nearestLine.distance < TypesConstants.closeDistanceToAddKnot)
			{
				nearestLine.spline.GetComponent<Spline>().InsertKnot(worldCoords ,0,0,0,nearestLine.spline,nearestLine.insertIndex);
			}

	}
	public void InsertNewSpline(){
		Main.instance.AddNewSpline();
	}
	public void DeleteSpline(){
		if(selectedKnot != null){
			Destroy(selectedKnot.myParentSpline);
			selectedKnot = null;
			SetInputFields();
		}
	}
	public void Exit(){
		Application.Quit();
	}
}
