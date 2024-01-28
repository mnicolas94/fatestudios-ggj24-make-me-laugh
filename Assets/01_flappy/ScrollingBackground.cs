using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    Rigidbody2D fondo_rb;
    public float scroll_speed;
    public GameObject columns01,columns02;
    

    // Start is called before the first frame update
    void Start()
    {
        fondo_rb=GetComponent<Rigidbody2D>();
        scroll_speed = -1.5f;
        fondo_rb.velocity=new Vector2(scroll_speed,0);
    }

    // Update is called once per frame
    void Update()
    {
        RepositingBackground();
    {
    }


    void RepositingBackground()
    {
       if(transform.position.x< -20.48)
       {
        transform.Translate(Vector2.right*20.48f*2f);
        float offset=Random.Range(-2.5f,1f);
        columns01.transform.position= new Vector3(this.transform.position.x-5f,offset,this.transform.position.z);
        offset=Random.Range(-2.5f,1f);
        columns02.transform.position= new Vector3(this.transform.position.x+5f,offset,this.transform.position.z);
       }
    }

    }



}
