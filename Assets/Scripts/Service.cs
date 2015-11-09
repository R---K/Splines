using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public static class Service{
	static Rect insideRect;
	//static Vector3 []Corners;// = new Vector3[4];
	public static void Initialization(){
		CalcHotZoneRect();
		//Check();
	}
	public static bool IsMouseInsideArea(){
		Vector3 position = Input.mousePosition+Vector3.up*Screen.height;
		if(insideRect.Contains(position))
			return true;
		return false;
	}

	static void CalcHotZoneRect(){
		Vector3 []Corners = new Vector3[4];
		GameObject _go = GameObject.Find ("Canvas/Area");
		_go.GetComponent<RectTransform>().GetWorldCorners(Corners);

		Vector2 position,size;
		position = RectTransformUtility.WorldToScreenPoint(null,Corners[1]);
		size = RectTransformUtility.WorldToScreenPoint(null,Corners[3])-position;
		insideRect =  new Rect(position.x,position.y,size.x,size.y*-1);
	}
	static void Check(){
		Vector3 []Corners = new Vector3[4];
		GameObject _go = GameObject.Find ("Canvas/Area");
		_go.GetComponent<RectTransform>().GetWorldCorners(Corners);
		// Debug.Log("top left corner =>>"+ RectTransformUtility.WorldToScreenPoint(null,Corners[1]).ToString());
	}
	public static Mesh CreateMesh(Vector2 []_points,float thin,Mesh OldMesh){
		Object.Destroy(OldMesh);
		Mesh TargetMesh =  new Mesh();
		Vector3 [] points = new Vector3[_points.Length];
		for(int i = 0;i < points.Length;i++)
			points[i] = new Vector3(_points[i].x,_points[i].y,0);
		CombineInstance[] combineInstances = new CombineInstance[points.Length-1];
		for(int i = 0;i < points.Length-1;i++)
			{
				Vector3 w,h;
				w = points[i+1]-points[i];
				w = w.normalized*(w.magnitude+thin);
				h = new Vector3(w.y*-1,w.x).normalized*thin;
				combineInstances[i].mesh = Quad(points[i],w,h);
				combineInstances[i].transform = Matrix4x4.identity;
			}
		TargetMesh.CombineMeshes(combineInstances);
		for (int i = 0;i < points.Length-1;i++) 
			Object.Destroy(combineInstances[i].mesh);
		return TargetMesh;
	}
	public static Mesh Quad(Vector3 origin, Vector3 width, Vector3 length)
	{
		var normal = Vector3.Cross(length, width).normalized;
		var mesh = new Mesh
		{
			vertices = new[] { origin, origin + length, origin + length + width, origin + width },
			normals = new[] { normal, normal, normal, normal },
			uv = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) },
			triangles = new[] { 0, 1, 2, 0, 2, 3}
		};
		return mesh;
	}
	public static float CalculateDistance(Vector2 _x1,Vector2 _x2, Vector2 _d){
		float 	dx1 = (_d-_x1).magnitude,
		dx2 = (_d-_x2).magnitude,
		x2x1 = (_x2-_x1).magnitude;

		float d = Mathf.Sqrt( dx1*dx1 - ( (dx1*dx1-dx2*dx2+x2x1*x2x1) / (2*x2x1) ) *  ( (dx1*dx1-dx2*dx2+x2x1*x2x1) / (2*x2x1) ) );
		return d;
	
	}
}
