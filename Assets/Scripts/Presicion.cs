using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Presicion : MonoBehaviour
{
    
    bool subir;
    
    public float velocidad; 
    
   

    // Start is called before the first frame update
    void Start()
    {
       subir=true; 
       velocidad=5;
    }

    // Update is called once per frame
    void Update()
    {   

        if(subir==true){this.transform.position += Vector3.up * velocidad * Time.deltaTime;    if(this.transform.position.y>3){subir=false;}}   

        if(subir==false){this.transform.position -= Vector3.up * velocidad * Time.deltaTime;   if(this.transform.position.y<-3){subir=true;}}
   
    }


    void OnTriggerEnter2D (Collider2D col)
   {
     core_chancleta.core_chancleta_instance.zona_correcta=true;
     
   }
   
    void OnTriggerExit2D (Collider2D col)
   {
     core_chancleta.core_chancleta_instance.zona_correcta=false;
    
   }
    

}
