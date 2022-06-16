using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    public float timer = 0;

    [Header("Main")]
    public float randRange = 1.5f;
    public float lineWidth = .5f;
    public float radius { get; set; } = 1f;

    [Header("Child")]
    public float chilRandRange = 1.5f;
    public float chilLineWidth = .5f;
    //public float childRadius { get; set; } = 2f;
    //public int SetCount { get; set; } = 120;

    List<Vector3> cvertices = new List<Vector3>();
    List<Vector3> cverticesLerp = new List<Vector3>();

    List<Vector3> vertices = new List<Vector3>();
    List<Vector3> verticesLerp = new List<Vector3>();


    LineRenderer childLine;
    LineRenderer circle;
    int num = 60;          //120
    float angle;
    float px, py;
    float updateTime;

    public void Init(int count)
    {
        num = count;
        childLine = transform.GetChild(0).GetComponent<LineRenderer>();
        childLine.startWidth = chilLineWidth;
        childLine.endWidth = chilLineWidth;

        circle = GetComponent<LineRenderer>();
        angle = (float) 360 / num;

        circle.startWidth = lineWidth;
        circle.endWidth = lineWidth;

        DrawCircle();
    }

    void LateUpdate()
    {
        updateTime += Time.deltaTime;
        if (updateTime > timer)
        {
            updateTime = 0;
            for (int i = 0; i < vertices.Count; i++)
            {
                float value = Random.Range(-randRange, randRange) + radius;
                Vector2 circlepara = new Vector2(Mathf.Cos(Mathf.Deg2Rad * i * angle + Time.time), Mathf.Sin(Mathf.Deg2Rad * i * angle + Time.time));
                float x = value * circlepara.x;
                float y = value * circlepara.y;
                vertices[i] = new Vector3(x, y);

                float cvalue = Random.Range(-randRange, randRange) + radius * 1.5f;
                float cx = cvalue * circlepara.x;
                float cy = cvalue * circlepara.y;
                cvertices[i] = new Vector3(cx, cy);
            }
        }
        UpdateVertex();
    }

    void DrawCircle()
    {
        circle.positionCount = num + 1;
        childLine.positionCount = num + 1;
        for (int i = 0; i < num + 1; i++)
        {
            float x = px + radius * Mathf.Cos(Mathf.Deg2Rad * i * angle);
            float y = py + radius * Mathf.Sin(Mathf.Deg2Rad * i * angle);
            circle.SetPosition(i, new Vector3(x, y, 0));
            vertices.Add(new Vector3(x, y, 0));
            verticesLerp.Add(new Vector3(x, y, 0));

            //float cx = px + radius * 1.5f * Mathf.Cos(Mathf.Deg2Rad * i * angle);
            //float cy = py + radius * 1.5f * Mathf.Sin(Mathf.Deg2Rad * i * angle);
            childLine.SetPosition(i, new Vector3(x * 1.5f, y * 1.5f, 0));
            cvertices.Add(new Vector3(x * 1.5f, y * 1.5f, 0));
            cverticesLerp.Add(new Vector3(x * 1.5f, y * 1.5f, 0));
        }
    }

    void UpdateVertex()
    {
        for (int i = 0; i < num + 1; i++)
        {
            verticesLerp[i] = Vector3.Lerp(verticesLerp[i], vertices[i], Time.deltaTime * 10);
            circle.SetPosition(i, verticesLerp[i]);

            cverticesLerp[i] = Vector3.Lerp(cverticesLerp[i], cvertices[i], Time.deltaTime * 10);
            childLine.SetPosition(i, cverticesLerp[i]);
        }
    }

    public void SetLineMat(float val)
    {
        if (circle != null)
            circle.material.SetFloat("_Cutoff", val);
    }

}
