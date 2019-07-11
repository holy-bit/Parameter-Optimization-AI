using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AI : MonoBehaviour
{
    [SerializeField]
    Slider angleX;
    [SerializeField]
    Slider angleY;
    [SerializeField]
    Slider force;
    [SerializeField]
    Slider TimeScale;
    [SerializeField]
    GameObject target;

    [SerializeField]
    goal[] goals;

    [SerializeField]
    projectile projectile;

    [SerializeField]
    GameObject camera;

    Vector3 target_Position;
    Vector3 collision_position;
    Vector3 distance;
    int parameter = 1;

    bool track;

    float actualAngleX;
    float actualAngleY;
    float actualForce;
    public
    Text text;
    public
    Text text2;

    float stepMultiplier;
    float score;
    float auxScore;
    float bestAngleX;
    float bestAngleY;
    float bestForce;
    int bestParameter;
    int prevParameter;
    bool inertia;
    int fails;
    int paramsame;
    int completed;
    bool inertia_reset = false;

    float prevAngleX;
    float prevAngleY;
    float prevForce;

    bool flag = false;

    // Start is called before the first frame update
    void Start()
    {
        reset();
        TimeScale.value = 10;
        Time.timeScale = TimeScale.value;
        fails = 0;
        completed = 0;
        track = false;
        
    }

    public  void setTimeScale()
    {
        Time.timeScale = TimeScale.value;
    }

    // Update is called once per frame
    void Update()
    {

        int count = 0;
        if (parameter == 7)
        {
            flag = true;
            for (int i = 1; i < goals.Length; i++)
            {
                if (goals[i].ball.transform.position == goals[i].ball.inicial_position)
                {
                    count++;
                }
                if (count == 6)
                {
                    launchMainBall();
                }
                    
            }
        }
        if (parameter < 7 && goals[parameter].ball.transform.position == goals[parameter].ball.inicial_position && !track)
        {
            if (!flag)
            {
                Evaluate();
            }

        }
      


    }

    void launchMainBall()
    {
        flag = false;
        parameter = 0;

        checkBestMove();

        SwitchParamerter(0);

        if (goals[parameter].ball.transform.position == goals[parameter].ball.inicial_position &&
            !goals[parameter].ball.ready)
            goals[parameter].ball.launch(angleX.value / 100, angleY.value / 100, force.value);

        goals[parameter].ball.GetComponent<TrailRenderer>().enabled = true;




        if (goals[parameter].ball.ready)
        {
            completed++;
            reset();
            reset2();
           


        }
        
        parameter++;


    }

    public void reset()
    {
        angleX.value = Random.Range(angleX.minValue + 50, angleX.maxValue - 50);
        angleY.value = Random.Range(angleY.minValue + 10, angleY.maxValue -10);
        force.value = Random.Range(20, 40);
        goals[bestParameter].ball.ready = false;
        goals[0].ball.ready = false;
        bestParameter = 0;
        track = false;
        actualAngleX = angleX.value;
        actualAngleY = angleY.value;
        actualForce = force.value;
       
        score = auxScore = 100;
        //goals[bestparameter].ball.ready = false;
        camera.transform.position = new Vector3(0, 6.7f, -6.4f);

        stepMultiplier = 0.2f;
        text2.text = "Completados:" + completed;
        

    }

    public void reset2()
    {
        goal.Target_position = new Vector3(Random.Range(-7, 7), Random.Range(2, 7), 49);
        for (int it = 0; it < 4; it++)
        {
            goal.obstacles_scale[it] = new Vector3(Random.Range(3, 10), Random.Range(2, 7), Random.Range(0.3f, 2));
            goal.obstacles_Position[it] = new Vector3(Random.Range(-10.5f + goal.obstacles_scale[it].x / 2, 10.5f - goal.obstacles_scale[it].x / 2), Random.Range(0.5f + goal.obstacles_scale[it].y / 2, 7 - goal.obstacles_scale[it].y / 2), 10 * (it + 1));

        }
        TimeScale.value = 10;
        setTimeScale();
    }

    void Evaluate()
    {
        if (goals[parameter].ball.GetComponent<Rigidbody>().velocity == Vector3.zero || goals[parameter].ball.collision_position != Vector3.zero)
        {            
            goals[parameter].ball.collision_position = Vector3.zero;
            SwitchParamerter(parameter);

            if (parameter <= 6)
            {
                goals[parameter].ball.launch(angleX.value / 100, angleY.value / 100, force.value);
                parameter++;

            }           
            angleX.value = actualAngleX;
            angleY.value = actualAngleY;
            force.value = actualForce;
        }
        else flag = false;
    }

    void checkBestMove()
    {
        
        prevParameter = bestParameter;
        fails++;
        for (int i = 1; i < goals.Length; i++)
        {
            angleX.value = actualAngleX;
            angleY.value = actualAngleY;
            force.value = actualForce;
     
            if (auxScore > goals[i].distance.magnitude)
            {            
                bestParameter = i;           
                fails = 0;
                SwitchParamerter(i);
                bestAngleX = angleX.value;
                bestAngleY = angleY.value;
                bestForce = force.value;
                auxScore = goals[i].distance.magnitude;
                text.text = "Distancia:" + auxScore.ToString() + " AnguloX:" + bestAngleX.ToString() + " AnguloY" + bestAngleY.ToString() + " Force:" + bestForce.ToString();           
            }
            
        }

        Vector3 distance = goals[bestParameter].distance;

        if (score > auxScore)
        {
            if (inertia)
            {
                SwitchParamerter(0);
                inertia = false;
                paramsame = 0;
            }

            if (prevParameter == bestParameter)
            {
                paramsame++;
                if (paramsame == 4)
                {
                    inertia = true;

                }
            }
            stepMultiplier = Mathf.Max(0.2f * fails + (stepMultiplier * (goals[0].distance.magnitude / 50)), 0.4f);
        }
        else if (inertia)
        {
            if (!inertia_reset)
            {
                prevParameter = 0;
                prevAngleX = angleY.value;
                prevAngleY = angleX.value;
                prevForce = force.value;
                SwitchParamerter(bestParameter);
                actualAngleX = angleY.value;
                actualAngleY = angleX.value;
                actualForce = force.value;
                text.text = "Distancia:" + auxScore.ToString() + " AnguloX:" + bestAngleX.ToString() + " AnguloY" + bestAngleY.ToString() + " Force:" + bestForce.ToString();
                // inertia = false;
                stepMultiplier += 0.2f * fails + stepMultiplier * (distance.magnitude / 100);
                inertia_reset = true;
            }
            else
            {
                inertia_reset = false;
                inertia = false;
                paramsame = 0;
                actualAngleX = prevAngleX;
                actualAngleY = prevAngleY;
                actualForce = prevForce;

            }

        }

        score = auxScore;
        float auxstepmultiplier;
        if (stepMultiplier < 0.2)
            auxstepmultiplier = stepMultiplier * 10;
        else auxstepmultiplier = stepMultiplier;
        TimeScale.value = Mathf.Max(Mathf.Min(10 *(score / 5f),10), 1);
        setTimeScale();
        stepMultiplier += 0.2f * fails + (stepMultiplier * (goals[0].distance.magnitude / 50));

        Debug.Log("Step:" + stepMultiplier + "  Score:" + score.ToString());
        if (stepMultiplier > 40) reset();

    }



    void SwitchParamerter(int parameterP)
    {
        
        switch (parameterP)
        {
            case 1:
                force.value -= stepMultiplier * Mathf.Max((10 * (score / 100)), 0.1f);
                break;

            case 2:
                force.value += stepMultiplier * Mathf.Max((10 * (score / 100)), 0.1f);
                break;

            case 3:
                angleX.value -= stepMultiplier * Mathf.Max((10 * (score / 100)), 0.1f);
                break;

            case 4:
                angleX.value += stepMultiplier * Mathf.Max((10 * (score / 100)), 0.1f);
                break;

            case 5:
                angleY.value -= stepMultiplier * Mathf.Max((10 * (score / 100)), 0.1f);
                break;

            case 6:
                angleY.value += stepMultiplier * Mathf.Max((10 * (score / 100)), 0.1f); 
                break;

            default:           
                actualAngleX = bestAngleX;
                actualAngleY = bestAngleY;
                actualForce = bestForce;
                break;
        }
    }
}
