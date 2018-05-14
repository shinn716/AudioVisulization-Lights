using UnityEngine;
using System.Collections;

public class ExampleClass : MonoBehaviour
{
    LineRenderer lr;

	[Header("LineRender Setting")]
	public int VertexSize = 4;
	public float Radius = 5f;
	public Material LineMat;

	float angle;
    float px, py;

	Vector3[] pos;


    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
		angle = (float)360 / VertexSize;

		pos = new Vector3[VertexSize+1];

		//draw ();
    }

    void draw(){

		angle = (float) 360 / VertexSize;
		lr.SetVertexCount (VertexSize + 1);

		for(int i=0; i<VertexSize+1; i++){
			
			if(i==VertexSize){
				
				px = transform.position.x + Radius * Mathf.Cos (Mathf.Deg2Rad * (VertexSize) * angle);
				py = transform.position.y + Radius * Mathf.Sin (Mathf.Deg2Rad * (VertexSize) * angle);   

				//pos [i] = new Vector3 (x, y, 0);

				lr.SetPosition(i, new Vector3(px, py, transform.position.z));
            }else{
				

				px = transform.position.x + Radius * Mathf.Cos (Mathf.Deg2Rad * i * angle);
				py = transform.position.y + Radius * Mathf.Sin (Mathf.Deg2Rad * i * angle) ;

				//pos [i] = new Vector3 (x, y, 0);

				lr.SetPosition(i, new Vector3(px, py, transform.position.z));
            }

			lr.material = LineMat;
		}
	}


	void Update(){
		draw();


		//for(int i=0; i<pos.Length; i++){
		//	rotate (lr.GetPosition(i), lr.GetPosition(i), Time.time);
		//}

	}

}