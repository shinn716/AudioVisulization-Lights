using UnityEngine;
using System.Collections;

public class drawRender : MonoBehaviour
{
	[Header("Start Circle")]
    public float StartRadius = 3f;
	public float step=1f;
	int num=120;


	public GameObject myPrefab;
	public AudioClip myClip;

    private LineRenderer LineDrawer;
	float angle;

	bool once=false;
	float totalDistance=17.8f;
	GameObject[] ellipse;
	AudioSource myAudio;
	int ellipseNum=20;
	float[] target;

	bool b_endState=false;
	bool b_endOnce=false;

	public ParticleSystem PS1;
	public ParticleSystem PS2;

    void Start ()
    {       
        LineDrawer = GetComponent<LineRenderer>();
		LineDrawer.material = new Material(Shader.Find("Sprites/Default"));

		//----Part of audio visualization
		myAudio = GetComponent<AudioSource>();
		myAudio.clip =  myClip;
		myAudio.Play();
		
		target = new float[ellipseNum];
    }

    void Update ()
    {    

		if(b_endState){
			print("end radius: " + StartRadius);
			
			for(int i=0; i<ellipseNum; i++)
				ellipse[i].GetComponent<ellipseScript>().fixPos(0, -5f, 0);
    	    
			
			if(StartRadius<0){
				step=0;
			} else{
				drawStart();
			}
			
		}else{
			
			if(StartRadius>30){
				print(">30");
				updateEllipse();
			} else{
    	    	drawStart();
			}
			if(StartRadius==30 && once==false){
				print("START");			
				addEllipse(ellipseNum);
				once=true;
			} 

		}

		if (!myAudio.isPlaying && b_endOnce==false)
        {
			// print("END");
			b_endOnce=true;
			step=-step;
			b_endState=true;	
			myAudio.Stop();
			print("END-step " +  step);

			PS1.enableEmission = false;
			PS2.enableEmission = false;
        }

		//----Test
		if(Input.GetKeyDown(KeyCode.R)){
			Application.LoadLevel (0);
		}
		
    }

	void updateEllipse(){
		float[] spectrum = AudioListener.GetSpectrumData(1024,0,FFTWindow.Hamming);
		print("Length " + ellipse.Length + " spectrum " + spectrum.Length);	

		for(int i=0; i<ellipseNum; i++){
			float prec = i / (float)ellipseNum;
			float x = -8.9f + totalDistance/(ellipseNum*2) + prec*totalDistance;
			float y = 0f;

			spectrum[i]*=10f;
			target[i] += (spectrum[i]-target[i])*0.6f;
			ellipse[i].GetComponent<ellipseScript>().fixPos(x, y, target[i]);
        }

	}

	void addEllipse(int tmp){

		for(int i=0; i<tmp; i++){
			GameObject go = (GameObject) Instantiate(myPrefab);
		}
		ellipse = GameObject.FindGameObjectsWithTag("ellipses");
	}

	 void drawStart(){
		
		angle = (float) 360/num;
		LineDrawer.SetVertexCount(num+1);
		for(int i=0; i<num+1; i++){
            if(i==num){
				float x = StartRadius* Mathf.Cos(Mathf.Deg2Rad *0);
				float y = StartRadius* Mathf.Sin(Mathf.Deg2Rad *0);                
                LineDrawer.SetPosition(i, new Vector3(x, y, 20));
            }else{
				float x = StartRadius* Mathf.Cos(Mathf.Deg2Rad *i*angle);
				float y = StartRadius* Mathf.Sin(Mathf.Deg2Rad *i*angle);
                LineDrawer.SetPosition(i, new Vector3(x, y, 20));
            }
		}

		StartRadius += Time.fixedDeltaTime * step;
	}

	
}