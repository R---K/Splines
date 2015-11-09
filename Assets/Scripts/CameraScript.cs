using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {
	//float pressDownTimer;



	Camera cameraComp;
	//==========>>>>

	Vector3 savedMousePosition;
	// Use this for initialization
	void Start () {
		cameraComp = GetComponent<Camera>();
		Application.targetFrameRate = -1;
	}
	
	// Update is called once per frame
	void Update () {
		//ManageMouseButs();

	}
	void ManageMouseButs(){
		/*if(Input.GetMouseButton(0)){
			if(Service.IsMouseInsideArea()){
					pressDownTimer +=Time.deltaTime;
					if(pressDownTimer > PDTToggle){
						if(camState != CamStates.pressed)
							savedMousePosition = Input.mousePosition;
						camState = CamStates.pressed;
				}
			}
		}

		if(Input.GetMouseButtonUp(0)){
			camState = CamStates.none;
			pressDownTimer = 0;
		}*/
	

	}

	public void CamMove(){
			if(savedMousePosition == Vector3.zero)
				savedMousePosition = Input.mousePosition;
			Vector3 _deltaVect = cameraComp.ScreenToWorldPoint(Input.mousePosition)-cameraComp.ScreenToWorldPoint(savedMousePosition);
			transform.position -= _deltaVect;
			savedMousePosition = Input.mousePosition;
	}
	public void CamStopMoving(){
		savedMousePosition = Vector3.zero;
	}

}
