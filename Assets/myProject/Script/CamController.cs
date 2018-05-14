using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour {


	public GameObject[] cam;
	public Vector2 ChangeTimeRange;

	int index;
	int changetime;
	float timevalue;


	void Start () {
		index = Random.Range (0, cam.Length);
		changetime = Random.Range ((int) ChangeTimeRange.x, (int) ChangeTimeRange.y);

		for (int i = 0; i < cam.Length; i++)
			cam [i].SetActive (false);

		cam [0].SetActive (true);
	}
	
	void Update () {
		timevalue += Time.deltaTime;
		int seconds = (int)timevalue % 60;
		if(seconds > changetime){
			timevalue = 0;
			changetime = Random.Range ((int) ChangeTimeRange.x, (int) ChangeTimeRange.y);

			for (int i = 0; i < cam.Length; i++)
				cam [i].SetActive (false);
			index = Random.Range (0, cam.Length);
			cam [index].SetActive (true);
		}
	}
}
