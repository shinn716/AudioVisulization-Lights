// Author: Tung-Chen_tsai
// Date: 20200904

using System.Collections;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kino;
using System.Threading.Tasks;
using UnityEngine.Playables;

public class AudioManager : MonoBehaviour
{
    public FFTWindow eFFTW = FFTWindow.Rectangular;
    public GameObject circlePrefab;

    [Header("Scene1")]
    public GameObject s1Group;
    public Circle circleBehavior_s1;
    AnalogGlitch analogGlitch1;

    [Header("Scene2")]
    public GameObject s2Group;
    public AnalogGlitch analogGlitch2;
    public Circle[] circleBehavior_s2;

    [Header("Scene3")]
    public GameObject s3Group;
    AnalogGlitch analogGlitch3;

    [Header("Scene4")]
    public ParticleSea particleSea;

    [Space]
    public Image ending;
    public Text author;

    public float[] GetSpectrumData { get; set; } = new float[1024];

    PlayableDirector playableDirector;
    AudioSource AS;
    List<GameObject> circleBehavior_s3 = new List<GameObject>();
    bool SceneInSt = false;
    bool m_scene4;
    bool m_end;

    void Start()
    {
        CreateS3();
        s1Group.SetActive(false);
        s2Group.SetActive(false);
        s3Group.SetActive(false);

        AS = GetComponent<AudioSource>();
        playableDirector = GetComponent<PlayableDirector>();
        analogGlitch1 = s1Group.transform.GetChild(0).GetComponent<AnalogGlitch>();
        //analogGlitch2 = s2Group.transform.GetChild(0).GetComponent<AnalogGlitch>();
        analogGlitch3 = s3Group.transform.GetChild(0).GetComponent<AnalogGlitch>();

        circleBehavior_s1.Init(120);
        for (int i = 0; i < circleBehavior_s2.Length; i++)
            circleBehavior_s2[i].Init(120);

        playableDirector.enabled = false;

        Invoke("GameStartAsync", 1);
    }

    // Update is called once per frame
    async void Update()
    {
        float[] spectrum = new float[1024];
        AudioListener.GetSpectrumData(spectrum, 0, eFFTW);
        GetSpectrumData = spectrum;
        //var spectrum = simpleSpectrum.spectrumInputData;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            await Task.Delay(100);
            s1Group.SetActive(true);
            s2Group.SetActive(false);
            s3Group.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            await Task.Delay(100);
            s1Group.SetActive(false);
            s2Group.SetActive(true);
            s3Group.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            await Task.Delay(100);
            s1Group.SetActive(false);
            s2Group.SetActive(false);
            s3Group.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            m_scene4 = !m_scene4;
            if (m_scene4)
                particleSea.SetMode1();
            else
                particleSea.SetMode2();
        }

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    GameStartAsync();
        //}

        if (s1Group.activeSelf)
        {
            AudioVisual1(spectrum);
        }

        if (s2Group.activeSelf)
        {
            AudioVisual2(spectrum);
        }

        if (s3Group.activeSelf)
        {
            AudioVisual3(spectrum);
        }

        //if (!AS.isPlaying && !m_end)
        //{
        //    m_end = true;
        //    End();
        //}
    }

    void SceneIn()
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "time", 2, "onupdate", "ScanLine", "easetype", iTween.EaseType.easeOutSine));
        iTween.ValueTo(gameObject, iTween.Hash("from", .5f, "to", 0, "time", 5, "onupdate", "ColorDrift", "easetype", iTween.EaseType.easeOutSine));
    }

    void ScanLine(float value)
    {
        analogGlitch1.scanLineJitter = value;
    }

    void ColorDrift(float value)
    {
        analogGlitch1.colorDrift = value;
        SceneInSt = true;
    }

    async Task GameStartAsync()
    {
        playableDirector.enabled = true;
        StartScene();
        AS.Play();

        await Task.Delay(2000);
        SceneIn();
    }

    void CreateS3()
    {
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 30; y++)
            {
                GameObject go = Instantiate(circlePrefab);
                //go.transform.GetChild(0).gameObject.SetActive(false);
                go.transform.SetParent(s3Group.transform);
                go.transform.localPosition = new Vector3(x * 5 - (32 * 5 / 2), y * 5 - (16 * 5 / 2), 10);
                go.GetComponent<Circle>().randRange = .2f;
                go.GetComponent<Circle>().Init(20);
                //go.GetComponent<CircleBehavior>().timer = .1f;
                circleBehavior_s3.Add(go);
            }
        }
    }

    void StartScene()
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "time", 1, "onupdate", "StartSceneProcess", "easetype", iTween.EaseType.easeOutExpo));
    }

    public void End()
    {
        AS.Stop();
        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", 2, "onupdate", "ShowAuthorProcess", "easetype", iTween.EaseType.easeOutSine, "Oncomplete", "EndScene"));
    }

    void EndScene()
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", 5, "delay", 2, "onupdate", "EndingProcess", "easetype", iTween.EaseType.easeOutSine));
    }



    void StartSceneProcess(float value)
    {
        ending.color = new Color(0, 0, 0, value);
    }

    void ShowAuthorProcess(float value)
    {
        author.color = new Color(1, 1, 1, value);
    }

    void EndingProcess(float value)
    {
        ending.color = new Color(0, 0, 0, value);
    }

    public void AudioVisual1(float[] singal)
    {
        float val = Mathf.Clamp01(1 - singal[0] * 20);
        circleBehavior_s1.radius = singal[0] * 100;
        circleBehavior_s1.SetLineMat(val);

        if (SceneInSt)
        {
            float val2 = Mathf.Clamp01(singal[0] * 5);
            analogGlitch1.scanLineJitter = val2;
        }
    }

    public void AudioVisual2(float[] singal)
    {
        for (int i = 0; i < 8; i++)
        {
            float limit = Mathf.Clamp(singal[i * 5] * 20, 0, 3);
            float val = Mathf.Clamp01(1 - singal[i * 5] * 20);
            circleBehavior_s2[i].radius = limit;
            circleBehavior_s2[i].SetLineMat(val);
        }

        float val2 = Mathf.Clamp01(singal[0] * 5);
        analogGlitch2.scanLineJitter = val2;
    }

    public void AudioVisual3(float[] singal)
    {
        for (int i = 0; i < circleBehavior_s3.Count; i++)
        {
            var circle = circleBehavior_s3[i].GetComponent<Circle>();
            float limit = Mathf.Clamp(singal[i] * 20 * 2, 0, 5);
            float val = Mathf.Clamp01(1 - singal[i] * 20);
            circle.radius = limit;
            circle.SetLineMat(val);
        }

        float val2 = Mathf.Clamp01(singal[0] * 5);
        analogGlitch3.scanLineJitter = val2;
        analogGlitch3.colorDrift = val2 / 3;
    }

    public void AudioVisual4(bool status)
    {
        if (status)
            particleSea.SetMode1();
        else
            particleSea.SetMode2();
    }

    #region for TimeLine
    public void EnableAudioVisual1()
    {
        s1Group.SetActive(true);
        s2Group.SetActive(false);
        s3Group.SetActive(false);
    }

    public void EnableAudioVisual2()
    {
        s1Group.SetActive(false);
        s2Group.SetActive(true);
        s3Group.SetActive(false);
    }

    public void EnableAudioVisual3()
    {
        s1Group.SetActive(false);
        s2Group.SetActive(false);
        s3Group.SetActive(true);
    }

    public void EnableAudioVisual4(bool status)
    {
        AudioVisual4(status);
    }

    #endregion
}
 