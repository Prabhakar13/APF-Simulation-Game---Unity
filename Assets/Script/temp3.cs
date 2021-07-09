/*

    static float[] x_cord = { (float)(-15.0), (float)(15.0), (float)(-15.0), (float)(-15.0) };
    List<float> x_cord_list = new List<float>(x_cord);
    static float[] y_cord = { (float)(7.0), (float)(-7.0), (float)(-7.0), (float)(7.0) };
    List<float> y_cord_list = new List<float>(y_cord);

*/



using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class temp3 : MonoBehaviour 
{
    public float speed;
    private Rigidbody2D myRigidbody;
    private Vector3 change;
    private List<Rigidbody2D> components = new List<Rigidbody2D>();
    private int index;
    private float x;
    private float y;
    private float prev_x;
    private float prev_y;
    private int stuck;
    private float destx, desty, threshold;
    private int iter;


    private float KREP, D_REP;
    private float KATT, D_ATT;
    private float V_MAX;

    static float[] x_cord = { (float)(-15.0), (float)(15.0), (float)(-15.0), (float)(-15.0) };
    List<float> x_cord_list = new List<float>(x_cord);
    static float[] y_cord = { (float)(7.0), (float)(-7.0), (float)(-7.0), (float)(7.0) };
    List<float> y_cord_list = new List<float>(y_cord);


    // Start is called before the first frame update
    void Start()
    {


        // Initialize Hyper-Parameters
        D_REP = (float)4.0;
        D_ATT = (float)2.0;
        KREP = (float)10.0;
        KATT = (float)2.0;
        V_MAX = (float)2.0;


        iter = 0;
        threshold = (float)0.1;
        myRigidbody = GetComponent<Rigidbody2D>();
        x = myRigidbody.transform.position.x;
        y = myRigidbody.transform.position.y;
        prev_x = myRigidbody.transform.position.x;
        prev_y = myRigidbody.transform.position.y;
        stuck = 0;
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            Rigidbody2D temp = go.GetComponent(typeof(Rigidbody2D)) as Rigidbody2D;
            if (temp != null)
                components.Add(temp);
        }

        int i = 0;
        foreach (Rigidbody2D obj in components)
        {
            float xx, yy;
            xx = obj.transform.position.x;
            yy = obj.transform.position.y;
            if (x == xx && y == yy)
                index = i;
            i = i + 1;
        }

    }


    float get_dist(float a, float b, float c, float d)
    {
        float ret = (float)Math.Pow((Math.Pow(a - c, 2) + Math.Pow(b - d, 2)), 0.5);
        return ret;
    }

    // Update is called once per frame
    void Update()
    {
        destx = x_cord_list[iter];
        desty = y_cord_list[iter];


        x = myRigidbody.transform.position.x;
        y = myRigidbody.transform.position.y;

        if (x == prev_x && y == prev_y)
            stuck = stuck + 1;
        else
        {
            prev_x = x;
            prev_y = y;
            stuck = 0;
        }
        change = Vector3.zero;


        int i = 0;
        float dist;
        float F_Att_x, F_Att_y;

        //Goal Seeking
        dist = get_dist(x, y, destx, desty);

        if (dist <= threshold)
        {
            iter = iter + 1;
            if (iter == x_cord_list.Count)
                iter = 0;
        }

        if (dist <= D_ATT)
        {
            F_Att_x = -KATT * (x - destx);
            F_Att_y = -KATT * (y - desty);
        }
        else
        {
            F_Att_x = (-KATT * D_ATT * (x - destx)) / dist;
            F_Att_y = (-KATT * D_ATT * (y - desty)) / dist;
        }

        //Obstacle Avoidance
        float F_Rep_x = (float)0.0;
        float F_Rep_y = (float)0.0;
        foreach (Rigidbody2D obj in components)
        {
            if (i == index)
                continue;
            float xx, yy;
            xx = obj.transform.position.x;
            yy = obj.transform.position.y;

            dist = get_dist(x, y, xx, yy);
            if (dist <= D_REP)
            {
                float dist_mul = (float)((1 / dist) - (1 / D_REP));
                F_Rep_x = F_Rep_x + (float)((KREP * dist_mul * (x - xx)) / Math.Pow(dist, 3));
                F_Rep_y = F_Rep_y + (float)((KREP * dist_mul * (y - yy)) / Math.Pow(dist, 3));
            }
            i = i + 1;
        }

        float F_Res_x = F_Att_x + F_Rep_x;
        float F_Res_y = F_Att_y + F_Rep_y;


        float F_Mag = (float)Math.Pow(Math.Pow(F_Res_x, 2) + Math.Pow(F_Res_y, 2), 0.5);


        if (F_Mag != 0)
        {
            change.x = (float)(V_MAX * (F_Res_x / F_Mag));
            change.y = (float)(V_MAX * (F_Res_y / F_Mag));
        }
        else
        {
            change.x = 0;
            change.y = 0;
        }

        change.z = 0;

        if(stuck>=20)
        {
            //Debug.Log(" Perbutation at - Index " + index);
            change.x = (float)(Random.Range(-5.0f, 5.0f));
            change.y = (float)(Random.Range(-5.0f, 5.0f));
            MoveCharacter();
        }

        if (change != Vector3.zero)
        {
            MoveCharacter();
        }
    }

    void MoveCharacter()
    {
        myRigidbody.MovePosition(
            transform.position + change * speed * Time.deltaTime
            );
    }
}



