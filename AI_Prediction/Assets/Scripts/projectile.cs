using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class projectile : MonoBehaviour
{

    public Slider angle;
    public Slider angle2;
    public Slider force;
    public Text text;
    public Vector3 collision_position;
    public Vector3 inicial_position;
    public Vector3 actual_position;
    public bool ready;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(transform.position.x, 0.5f,transform.position.z); 
        inicial_position = transform.position;
        collision_position = Vector3.zero;
        ready = false;
    }

    // Update is called once per frame
    void Update()
    {
        actual_position = transform.position;
    }


    public void launch(float angleX, float angleY, float force)
    {
        
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.freezeRotation = false;
        transform.rotation = new Quaternion(angleX, angleY, 0, 1);
        rb.AddForce(transform.forward * force, ForceMode.Impulse);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "goal" || other.transform.tag == "target")
        {
            collision_position = transform.position;
            GetComponent<Rigidbody>().freezeRotation = true;
            
             Rigidbody rb = GetComponent<Rigidbody>();
            

            if (other.transform.tag == "target")
            {
                transform.position = inicial_position;
                rb.velocity = Vector3.zero;
                ready = true;       
                text.text = "Completado";
            }

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Respawn" && inicial_position == Vector3.zero) inicial_position = transform.position;
    }

}
