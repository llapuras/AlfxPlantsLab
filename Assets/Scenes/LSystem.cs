using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class LSystem : MonoBehaviour
{
    [Header("awsl")]
    public bool debug = true;
    public Dictionary<char, string> rules = new Dictionary<char, string>();
    [Range(0, 6)]
    public int iterations = 4; //大于x的话Unity会炸，在我的电脑上x=8(仅供参考
    public string input = "F";
    public string rule = "F+F+F+F";
    public float angle = 45;
    private string output;

    [Header("Random Setting")]
    public bool isRandom = false;
    public Vector2 randomRange = new Vector2(-45,45);

    public string result;

    List<point> points = new List<point>();
    List<GameObject> branches = new List<GameObject>();

    public GameObject cylinder;

    public GameObject stem;
    public GameObject petal;
    public GameObject leaf;


    void Start()
    {
        GenerateTree();
    }

    public void GenerateTree()
    {
        rules.Clear();
        points.Clear();
        foreach (GameObject obj in branches)
        {
            //Destroy(obj);
            DestroyImmediate(obj);
        }
        branches.Clear();

        // 自定义生成Rules，具体规则和示例可以参照用户指南→http://paulbourke.net/fractals/lsys/
        rules.Add('F', rule);

        // Apply rules for i interations
        output = input;
        for (int i = 0; i < iterations; i++)
        {
            output = applyRules(output);
        }
        result = output;
        determinePoints(output);
        CreateCylinders();
    }

    string applyRules(string p_input)
    {
        StringBuilder sb = new StringBuilder();
        foreach (char c in p_input)
        {
            if (rules.ContainsKey(c))
            {
                sb.Append(rules[c]);
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    struct point
    {
        public point(Vector3 rP, Vector3 rA, float rL) { Point = rP; Angle = rA; BranchLength = rL; }
        public Vector3 Point;
        public Vector3 Angle;
        public float BranchLength;
    }

    void determinePoints(string p_input)
    {
        Stack<point> returnValues = new Stack<point>();
        point lastPoint = new point(Vector3.zero, Vector3.zero, 1f);
        returnValues.Push(lastPoint);

        foreach (char c in p_input)
        {
            switch (c)
            {
                case 'F': 
                    points.Add(lastPoint);

                    point newPoint = new point(lastPoint.Point + new Vector3(0, lastPoint.BranchLength, 0), lastPoint.Angle, 1f);
                    newPoint.BranchLength = lastPoint.BranchLength - 0.02f;
                    if (newPoint.BranchLength <= 0.0f) newPoint.BranchLength = 0.001f;

                    newPoint.Angle.y = lastPoint.Angle.y;

                    //add random
                    if (isRandom == true)
                    {
                        newPoint.Angle.y += UnityEngine.Random.Range(randomRange.x, randomRange.y);
                    }

                    newPoint.Point = pivot(newPoint.Point, lastPoint.Point, new Vector3(newPoint.Angle.x, 0, 0));
                    newPoint.Point = pivot(newPoint.Point, lastPoint.Point, new Vector3(0, newPoint.Angle.y, 0));

                    points.Add(newPoint);
                    lastPoint = newPoint;
                    break;
                case '+': // Rotate +30
                    lastPoint.Angle.x += angle;
                    break;
                case '[': // Save State
                    returnValues.Push(lastPoint);
                    break;
                case '-': // Rotate
                    lastPoint.Angle.x += -angle;
                    break;
                case ']': // Load Saved State
                    lastPoint = returnValues.Pop();
                    break;
            }
        }
    }

    void CreateCylinders()
    {
        for (int i = 0; i < points.Count; i += 2)
        {
            CreateCylinder(points[i], points[i + 1], 0.1f);
        }
    }

    Vector3 pivot(Vector3 point1, Vector3 point2, Vector3 angles)
    {
        Vector3 dir = point1 - point2;
        dir = Quaternion.Euler(angles) * dir;
        point1 = dir + point2;
        return point1;
    }

    void CreateCylinder(point point1, point point2, float radius)
    {
        GameObject newCylinder = (GameObject)Instantiate(cylinder);
        newCylinder.SetActive(true);
        float length = Vector3.Distance(point2.Point, point1.Point);
        radius = radius * length;

        Vector3 scale = new Vector3(radius, length / 2.0f, radius);
        newCylinder.transform.localScale = scale;

        newCylinder.transform.position = point1.Point;
        newCylinder.transform.Rotate(point2.Angle);

        newCylinder.transform.parent = this.transform;

        branches.Add(newCylinder);
    }

    void Update()
    {
        if (debug)
        {
            for (int i = 0; i < points.Count; i += 2)
            {
                Debug.DrawLine(points[i].Point, points[i + 1].Point, Color.red);
            }
        }
    }
}