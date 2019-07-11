using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goal : MonoBehaviour
{
    [SerializeField]
    GameObject original_target;

    [SerializeField]
    public projectile ball;

    GameObject target;
    [SerializeField]
    public GameObject[] obstacles;

    public Vector3 distance;
    public static Vector3 Target_position;
    public static Vector3[] obstacles_Position;
    public static Vector3[] obstacles_scale;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Instantiate(original_target, new Vector3(100,100,100), Quaternion.Euler(-90, 0, 0));

        Target_position = new Vector3(Random.Range(-7, 7), Random.Range(2, 7), 49);

        obstacles_Position = new Vector3[4];
        obstacles_scale = new Vector3[4];
        int it = 0;
        foreach(GameObject obstacle in  obstacles)
        {
            obstacle.transform.localScale = new Vector3(Random.Range(3,10), Random.Range(2, 7), Random.Range(0.3f, 2));
            obstacle.transform.position = new Vector3(Random.Range(-10.5f + obstacle.transform.localScale.x/2, 10.5f- obstacle.transform.localScale.x / 2), Random.Range(0.5f+ obstacle.transform.localScale.y / 2, 7 - obstacle.transform.localScale.y / 2), obstacle.transform.position.z);
            
            obstacles_Position[it] = obstacle.transform.position;
            obstacles_scale[it] = obstacle.transform.localScale;
            it++;
        }

    }

    // Update is called once per frame
    void Update()
    {
        for(int it=0; it <obstacles.Length;it++)
        {
            obstacles[it].transform.position = obstacles_Position[it] + transform.position;
            obstacles[it].transform.localScale = obstacles_scale[it];
        }
        target.transform.position = Target_position + transform.position;
        if (ball.GetComponent<Rigidbody>().velocity.magnitude < 1 && (ball.transform.position != ball.inicial_position || ball.collision_position != Vector3.zero) || ball.transform.position.y < 0)
        {
            if (ball.GetComponent<TrailRenderer>()) ball.GetComponent<TrailRenderer>().enabled = false;
            if (ball.collision_position != Vector3.zero)
                 distance = target.transform.position - ball.collision_position;
            else
                distance = target.transform.position - ball.actual_position;
            if (ball.GetComponent<TrailRenderer>().enabled == false)
            {
                ball.transform.position = ball.inicial_position;
                ball.GetComponent<TrailRenderer>().enabled = true;
            }
                
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.freezeRotation = true;
        }
    }

    public void launch(float angleX,float angleY,float force){ ball.launch(angleX, angleY, force); }
}
