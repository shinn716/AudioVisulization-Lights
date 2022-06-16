using UnityEngine;
using UnityEngine.UI;
using System.Collections;
//using UnityStandardAssets.ImageEffects;

public class ParticleSea : MonoBehaviour {

	public PropagationType propagation;
	public Camera cam;
	public bool render;
	public new ParticleSystem particleSystem;
	//public Text particlesText;
	public Vector3 meshOffset;
	public int x_meshResolution;
	public int y_meshResolution;
	public float x_meshSpacing;
	public float y_meshSpacing;
	public float meshHeightScale;
	public float perlinNoiseScale;
	public float posRandomness;
	public Vector2 perlinNoiseOffset;
	public Gradient colorsGradient;
	public bool useGradientColors;
	public Color defaultColor;
	public float x_randomLimit;
	public float y_randomLimit;

	private Vector3[] positions;
	private bool colorsAssigned = false;
	private ParticleSystem.Particle[] particlesArray;
	private float xPos;
	private float yPos;
	private float zPos;
	public float xAnimSpeed;
	public float yAnimSpeed;
	private int frameCount;
	private bool hasAnythingChanged;


	[Space]
	public AudioManager audioManager;
	public Type type = new Type();

	public enum Type
    {
		Spectrum,
		Wave
	}



	[ContextMenu("SetMode1")]
	public void SetMode1()
    {
		type = Type.Spectrum;
		Spacing(.145f);
		HeightModifier(1.5f);
		Resolution(160);
		PerlinNoiseScale(0);
		XAnimSpeed(0);
		YAnimSpeed(0);
	}

	[ContextMenu("SetMode2")]
	public void SetMode2()
	{
		type = Type.Wave;
		Spacing(.145f);
		HeightModifier(.75f);
		Resolution(200);
		//PerlinNoiseScale(0);
		XAnimSpeed(.5f);
		YAnimSpeed(.125f);
	}


	public enum PropagationType {
		Regular,
		Random
	}

	public void PerlinNoiseScale(float noise) {
		if(noise > 0f)
			perlinNoiseScale = noise * 100;
	}

	public void HeightModifier(float height) {
		if(height > 0f)
			meshHeightScale = height * 3;

		//print("HeightModifier " + meshHeightScale);
	}

	public void Spacing(float spacing) {
		hasAnythingChanged = true;
		if(spacing > 0f) {
			x_meshSpacing = spacing;
			y_meshSpacing = spacing;
        }

		//print("Spacing " + spacing);
	}

	public void Render(bool isActive) {
		render = isActive;
	}

	public void UseGradient(bool isActive) {
		useGradientColors = isActive;
		colorsAssigned = false;
	}

	//public void Bloom(bool isActive) {
	//	cam.GetComponent<BloomOptimized>().enabled = isActive;
	//}

	//public void Vignetting(bool isActive) {
	//	cam.GetComponent<VignetteAndChromaticAberration>().enabled = isActive;
	//}

	public void XAnimSpeed(float speed) {
		xAnimSpeed = speed;
	}

	public void YAnimSpeed(float speed) {
		yAnimSpeed = speed;
	}

	IEnumerator Animate() {
		while(true) {
			perlinNoiseOffset = new Vector2(perlinNoiseOffset.x + xAnimSpeed, perlinNoiseOffset.y + yAnimSpeed);
			yield return new WaitForSeconds(0.02f);
		}
	}

	public void Resolution(float i) {
		x_meshResolution = (int) i;
		y_meshResolution = (int) i;
		particlesArray = new ParticleSystem.Particle[x_meshResolution * y_meshResolution];
		positions = new Vector3[x_meshResolution * y_meshResolution];
		RegenerateRandomPositionsArray(x_meshResolution * y_meshResolution);
		particleSystem.maxParticles = x_meshResolution * y_meshResolution;
		//particlesText.text = "Particles count: "+(y_meshResolution * y_meshResolution).ToString();
		hasAnythingChanged = true;
		//print("Resolution " + i);
	}

	void Start() {
		particlesArray = new ParticleSystem.Particle[x_meshResolution * y_meshResolution];
		positions = new Vector3[x_meshResolution * y_meshResolution];
		RegenerateRandomPositionsArray(x_meshResolution * y_meshResolution);
		StartCoroutine(Animate());
		colorsAssigned = false;
		hasAnythingChanged = true;
        //particlesText.text = "Particles count: 40000";

		if(type == Type.Spectrum)
	        SetMode1();
		else
			SetMode2();
	}


	public float height = 0;
	public float speed = 5;
	float zposLerp;
	bool m_coolDown = false;
	Vector2 randVec;
	//float temp;

	void CoolDown()
    {
		m_coolDown = false;
	}
	

	void Update()
	{
		if (type == Type.Wave)
		{
			if (Mathf.Clamp01(audioManager.GetSpectrumData[0] * 10) > .7f && !m_coolDown)
			{
				m_coolDown = true;
				height = 1 - Mathf.Clamp01(audioManager.GetSpectrumData[0] * 10);
			}
			if (m_coolDown && zposLerp > (height - .1f))
			{
				m_coolDown = false;
                PerlinNoiseScale(zposLerp);
			}

			if (height <= 0)
				height = .01f;
			zposLerp = Mathf.Lerp(zposLerp, height, Time.deltaTime * speed);
		}

		if (render && frameCount % 2 == 0)
		{
			if (hasAnythingChanged) meshOffset = new Vector3(x_meshSpacing * x_meshResolution * -0.5f, y_meshSpacing * y_meshResolution * -0.5f, 0f);
			hasAnythingChanged = false;
			particleSystem.GetParticles(particlesArray);

			for (int i = 0; i < x_meshResolution; i++)
			{
				for (int j = 0; j < y_meshResolution; j++)
				{
					xPos = (perlinNoiseOffset.x + i) / perlinNoiseScale;
					yPos = (perlinNoiseOffset.y + j) / perlinNoiseScale;
					zPos = Mathf.PerlinNoise(xPos + randVec.x, yPos + randVec.y);

					//Set position based on these three
					if (type == Type.Wave)
					{
						particlesArray[i * x_meshResolution + j].position = new Vector3(i * x_meshSpacing + meshOffset.x, j * y_meshSpacing + meshOffset.y, zPos * meshHeightScale + zposLerp);
					}
                    else
                    {
                        float val2 = Mathf.Clamp01(audioManager.GetSpectrumData[i + 1] * 10);
                        particlesArray[i * x_meshResolution + j].position = new Vector3(i * x_meshSpacing + meshOffset.x, j * y_meshSpacing + meshOffset.y, zPos * meshHeightScale * val2);
					}

					if (useGradientColors) particlesArray[i * x_meshResolution + j].color = colorsGradient.Evaluate(zPos);
					else if (!colorsAssigned) particlesArray[i * x_meshResolution + j].color = defaultColor;
					colorsAssigned = true;
				}
			}
			particleSystem.SetParticles(particlesArray, particlesArray.Length);
		}
		frameCount++;
		colorsAssigned = true;
	}

	private void RegenerateRandomPositionsArray(int arraySize) {
		for(int i = 0; i < arraySize; i++) {
			positions[i] = new Vector3(Random.Range(-x_randomLimit, x_randomLimit), Random.Range(-y_randomLimit, y_randomLimit), 0);
		}
	}
}
