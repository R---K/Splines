using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spline : MonoBehaviour {
	MeshFilter _meshFilter,_lineFilter;
	int fidelity = 10;
	// Use this for initialization

	public List<Knot> knots;
	public int ReturnKnotsCount(){ return knots.Count;}
	public void AddKnot(Vector2 _position,float _t,float _c,float _b,GameObject _go){
		knots.Add(new Knot(_position,_t,_c,_b,_go));
	}
	public void InsertKnot(Vector2 _position,float _t,float _c,float _b,GameObject _go,int _index){
		knots.Insert(_index, new Knot(_position,_t,_c,_b,_go));
		Redraw();
	}
	public void RemoveKnot(Knot _knot){
		if(!knots.Contains(_knot))
			return;
		knots.Remove(_knot);

		
	}
	void Awake(){
		_meshFilter = GetComponent<MeshFilter>();
		knots = new List<Knot>();
		AddKnot(new Vector2(100,100),0,0,0,gameObject);
		AddKnot(new Vector2(200,200),0,0,0,gameObject);
		AddKnot(new Vector2(300,100),0,0,0,gameObject);


		Redraw();
	}
	void Start(){
		SetMyColor(new Color(Random.Range (0,1f),Random.Range (0,1f),Random.Range (0,1f)),new Color(1f,.2f,.2f));
	}
	public void Redraw(){

		_meshFilter.mesh = Service.CreateMesh(GetPointsFromKnots(0),3,_meshFilter.mesh);
		GameObject _go = transform.GetChild(0).gameObject;
		//Debug.Log(_go.name);
		_lineFilter = _go.GetComponent<MeshFilter>();
		_lineFilter.mesh = Service.CreateMesh(GetPointsFromKnots(1),3,_lineFilter.mesh);


	}
	Vector2 []GetPointsFromKnots(int _value){
		CalculateTangets();
		Vector2 []points = new Vector2[(knots.Count-1)*fidelity+1];
		for(int _curKnot = 0 ;_curKnot<knots.Count-1; _curKnot++){
			for(int i = 0;i < fidelity;i++){
				if(_value == 0)
					points[_curKnot*fidelity+i] = GetCurrentPointsBias(knots[_curKnot],knots[_curKnot+1],(float)i/fidelity);
				if(_value == 1)
					points[_curKnot*fidelity+i] = GetCurrentPointSimpleLine(knots[_curKnot],knots[_curKnot+1],(float)i/fidelity);
			}
		}
		if(_value == 0)
			points[(knots.Count-1)*fidelity] = GetCurrentPointsBias(knots[knots.Count-2],knots[knots.Count-1],1);
		if(_value == 1)
			points[(knots.Count-1)*fidelity] = GetCurrentPointSimpleLine(knots[knots.Count-2],knots[knots.Count-1],1);
		return points;
	}
	Vector3 GetCurrentPointSimpleLine(Knot from,Knot to,float _t){
			// line
		Vector2 result = to.position-from.position;
		result = from.position + result.normalized*result.magnitude*_t;

		return result;
	}
	Vector3 GetCurrentPointsBias(Knot from,Knot to,float _t){
		Vector2 P0= from.position,P1=to.position;
		Vector2 M0,M1;
		M0 = from.M1;
		M1 = to.M0;

		Vector2 result =	(2*(_t*_t*_t) - 3*(_t*_t) + 1)*P0+
							((_t*_t*_t)-2*(_t*_t)+_t)*M0+
							(-2*(_t*_t*_t)+3*(_t*_t))*P1+
							((_t*_t*_t)-(_t*_t) )* M1;
		//result = from.position + result.normalized*result.magnitude*_t;
		
		return result;
	}

	void CalculateTangets(){

		for(int i = 1;i < knots.Count-1;i++)
			{
				float 	t = knots[i].t,
						b = knots[i].b,
						c = knots[i].c;
				knots[i].M0 = 	(1-t)*(1+b)*(1+c)/2*(knots[i].position-knots[i-1].position)+
								(1-t)*(1-b)*(1-c)/2*(knots[i+1].position-knots[i].position);


				knots[i].M1 = 	(1-t)*(1+b)*(1-c)/2*(knots[i].position-knots[i-1].position)+
								(1-t)*(1-b)*(1+c)/2*(knots[i+1].position-knots[i].position);
			
		}


	}
	public void SetMyColor(Color _splineColor,Color _lineColor){
			Renderer splineRenderer = GetComponent<Renderer>(), 
				lineRenderer = transform.GetChild(0).GetComponent<Renderer>();

			Material _mat = new Material(splineRenderer.material);
			Object.Destroy(splineRenderer.material);
			_mat.color = _splineColor;
			splineRenderer.material = _mat;
			
			
			_mat = new Material(lineRenderer.material);
			_mat.color = _lineColor;
			Object.Destroy(lineRenderer.material);
			lineRenderer.material = _mat;

	}
	public NearestLineData GetNearestLineData(Vector2 _position){
		NearestLineData nearestLineData = new NearestLineData(gameObject);

		for(int i = 0;i < knots.Count-1;i++)
			{
				float distance = CalculateDistance(knots[i].position,knots[i+1].position,_position);
				Debug.Log(distance);
				if(distance < nearestLineData.distance)
					{
						nearestLineData.distance = distance;
						nearestLineData.begin = knots[i];
						nearestLineData.end = knots[i+1];
						nearestLineData.insertIndex = i+1;
					}
			}

		return nearestLineData;
	}
	public float CalculateDistance(Vector2 _x1,Vector2 _x2, Vector2 _d){
		float 	dx1 = (_d-_x1).magnitude,
		dx2 = (_d-_x2).magnitude,
		x2x1 = (_x2-_x1).magnitude;
		Debug.Log("ddd");
		float d = Mathf.Sqrt( dx1*dx1 - ( (dx1*dx1-dx2*dx2+x2x1*x2x1) / (2*x2x1) ) *  ( (dx1*dx1-dx2*dx2+x2x1*x2x1) / (2*x2x1) ) );
		return d;
	}

	void OnDestroy(){
		int knotsCount = knots.Count;
		for(int i = 0;i < knotsCount;i++)
			knots[0].DeleteMe(true);
		Main.instance.splines.Remove(gameObject);
	}
}
