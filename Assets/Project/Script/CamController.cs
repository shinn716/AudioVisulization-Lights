using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class CamController : MonoBehaviour {

	public AudioSource AS;
	public GameObject partices;
	public GameObject partices2;
	public GameObject[] cam;
	public Vector2 ChangeTimeRange;
	public Text author;
	public Image end;


	float lifeCount = 12000 * 2;

	int index;
	int changetime;
	float timevalue;

	bool once = false;
	bool startRecord = false;
	bool AuthorFade = false;
	bool EndFade = false;

	ParticleSystem ps;
	public Color lerpcolor = new Color(.96f, .63f, 0, 1);

	void Start ()
    {
		ps = partices.GetComponent<ParticleSystem>();
		index = Random.Range (0, cam.Length);
		//changetime = Random.Range ((int) ChangeTimeRange.x, (int) ChangeTimeRange.y);
		changetime = 20;

		for (int i = 0; i < cam.Length; i++)
			cam [i].SetActive (false);
		cam [0].SetActive (true);

		Cursor.visible = true;

		RecordStartAsync();
	}

    private void FixedUpdate()
    {
		if (startRecord)
		{
			lifeCount-=1.25f;
			Time.timeScale = lifeCount / (12000 * 2);
        }
	}

    async void Update ()
    {
		if (startRecord)
		{
			if (!once)
			{
				once = true;
				cam[0].GetComponent<ShinnUtil.NoiseCamera>().enabled = true;
			}
			timevalue += Time.deltaTime;
			int seconds = (int)timevalue % 60;
			if (seconds > changetime)
			{
				//print("change " + index);
				timevalue = 0;
				changetime = Random.Range((int)ChangeTimeRange.x, (int)ChangeTimeRange.y);


				//index = Random.Range(0, cam.Length);
				if(index >= cam.Length-1)
                	index = 0;
                else
					index++;

				for (int i = 0; i < cam.Length; i++)
					cam[i].SetActive(false);
				cam[index].SetActive(true);
			}


			lerpcolor = Color.Lerp(lerpcolor, new Color(.2f, .4f, 1), Time.deltaTime * .012f/2);
			//print(lerpcolor);
			var col = ps.colorOverLifetime;
			//col.enabled = true;
			Gradient grad = new Gradient();
			grad.SetKeys(new GradientColorKey[] { new GradientColorKey(lerpcolor, 0f), new GradientColorKey(Color.white, 1f) }, new GradientAlphaKey[] { new GradientAlphaKey(1, 0f), new GradientAlphaKey(0, 1f) });
			col.color = grad;
		}

		//if (Input.GetKeyDown(KeyCode.Space))
  //      {
		//	RecordStart();
		//}

		if (AuthorFade)
		{
			author.color = Color.Lerp(author.color, Color.white, 1 * Time.deltaTime);

			if (author.color.a > .95f)
			{
				author.color = Color.white;

				await Task.Delay(5000);
				EndFade = true;
				AuthorFade = false;
			}
		}

        if (EndFade)
        {
			end.color = Color.Lerp(end.color, Color.black, .5f * Time.deltaTime);

			if (end.color.a > .95f)
			{
				end.color = Color.black;
				EndFade = false;
                partices.SetActive(false);
			}
		}
	}

	async Task RecordStartAsync()
    {
		Cursor.visible = false;
		startRecord = true;
		partices.SetActive(true);
		AS.Play();

		await Task.Delay(120000 * 1);
		partices2.SetActive(true);
		await Task.Delay(120000 * 2);
		startRecord = false;
		AuthorFade = true;
		AS.Stop();
	}
}

