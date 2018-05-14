using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShinnUtil{

	public class RotateAround : MonoBehaviour {

		//public float speed = 1;

		[Header("Lookat")]
		public bool lookatst = false;
		public Transform target;



		[Header("Rotate Self Setting")]
		[SerializeField] float px;
		[SerializeField] float py;
		[SerializeField] float pz;
		[SerializeField] float speed = .5f;

		[Header("Random Rotate")]
		[SerializeField] bool RandSt = false;
		[SerializeField] Vector2 RotatePxRange;
		[SerializeField] Vector2 RotatePyRange;
		[SerializeField] Vector2 RotatePzRange;

		[Header("Noise Rotate")]
		[SerializeField] bool NoiseSt = false;

		float NoiseSeed1;
		float NoiseSeed2;
		float NoiseSeed3;

		void Start(){

			if (NoiseSt) {
				NoiseSeed1 = Random.value;
				NoiseSeed2 = Random.value;
				NoiseSeed3 = Random.value;
			}
		}

		void FixedUpdate () {
			transform.RotateAround(new Vector3 (	
				px * Mathf.PerlinNoise (Time.time * speed, NoiseSeed1),
				py * Mathf.PerlinNoise (Time.time * speed, NoiseSeed2),
				pz * Mathf.PerlinNoise (Time.time * speed, NoiseSeed3) ),
				Vector3.up, speed * Time.deltaTime);

			if(lookatst)
				transform.LookAt (target);
		}
	}

}