using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;




public class core_chancleta : MonoBehaviour
{
    public static core_chancleta core_chancleta_instance;
    
    public GameObject player;

    GameObject player_instance;
    public Transform posicion_inicial;


    Rigidbody2D rb_player; 
    public Animator animator_player;

     public Animator guajiro_animator;
     public Animator serrucho_animator;



    public int force;
    //public  GameObject title_tmp,subtitle_tmp, game_over_tmp;
    public  TextMeshProUGUI score_tmp,max_score_tmp,info_tmp;
    
    int max_score,actual_score;  
    bool game_over;  
    public  GameObject der_btn,izq_btn;
    public AudioClip die_sound,hit_sound,point_sound,wing_sound;

    public AudioClip cristales_rotos,sierra_sound;
    public AudioClip sierra_sound_01,sierra_sound_02,sierra_sound_03,sierra_sound_04,sierra_sound_05,sierra_sound_06;
    

    AudioSource my_audiosource;


    int nivel_actual;
    public GameObject fondo;
    public List<Sprite> listado_fondos; 
    public bool zona_correcta;
    public bool zona_incorrecta;
    
    public List <GameObject> niveles;


    public GameObject boton_derecho,boton_izquierdo; 
    public Sprite boton_up,boton_press;

    
    public bool usuario_win;



     private void Awake() {
      if(core_chancleta_instance==null)
      {
        core_chancleta_instance=this;
      }
      else{Destroy(gameObject);}

      }



    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate =60;
        my_audiosource = GetComponent <AudioSource>();
      
        //game_over_tmp.SetActive(false);
       
        //max_score=PlayerPrefs.GetInt("max_score",0); 
        //max_score_tmp.text= "MAX SCORE: " + max_score;///
        //score_tmp.text="";
        actual_score=0;
        score_tmp.text="Score: " + actual_score;
        player_instance = Instantiate(player,posicion_inicial);
        cambiar_nivel();
        StartCoroutine(pasar_tiempo());

            }



    // Update is called once per frame
    void Update()
    {
        switch(nivel_actual)
        {
           case 1: 
            //Time.timeScale=0;
            //game_over_tmp.SetActive(true);
            //subtitle_tmp.SetActive(true);
            //if(actual_score>max_score)   { max_score=actual_score;}
            //max_score_tmp.text= "MAX SCORE: " + max_score;
            //score_tmp.text="NEW SCORE: "+ actual_score;
            //PlayerPrefs.SetInt("max_score",max_score); 
            //tap_button.GetComponent<EventTrigger>().enabled = true;
           break;


           case 4: 
           
           
           break;
        }
    }



    public void derecha_btn_down()
    {
boton_derecho.GetComponent<Image>().sprite=boton_press;
     switch(nivel_actual)
    
    {
    
    case 1: boton_derecho_01(); break;
    case 2: boton_derecho_02(); break;
    case 3: boton_derecho_03(); break;
    case 4: boton_derecha_04(); break;   
    
    }

    }




      public void izquierda_btn_down()
    {
     boton_izquierdo.GetComponent<Image>().sprite=boton_press;
     switch(nivel_actual)
        {
        case 1: boton_izquierda_01(); break;
        case 2: boton_izquierda_02(); break;
        case 3: boton_izquierda_03(); break;
        case 4: boton_izquierda_04(); break;   
        }

    } 




    public void izquierda_btn_up(){ boton_izquierdo.GetComponent<Image>().sprite=boton_up;}
    public void derecha_btn_up(){ boton_derecho.GetComponent<Image>().sprite=boton_up;}


 

    IEnumerator pasar_tiempo()
    {
       yield return new WaitForSeconds(5);
       cambiar_nivel();
       
    }

    
       IEnumerator mostrar_info(string texto)
    {
       info_tmp.text=texto;
       yield return new WaitForSeconds(2);
       info_tmp.text="";
    }


     public  void cambiar_nivel()
    {   

        int nivel_aleatorio= Random.Range(1,5);
        
        do{ nivel_aleatorio= Random.Range(1,5);}
        while (nivel_aleatorio==nivel_actual);

        nivel_actual=nivel_aleatorio;
        //nivel_actual=4;
        modificar_juego(nivel_actual);
        cambiar_fondo(nivel_actual);
        actual_score++;
        score_tmp.text="Score: " + actual_score;
        StartCoroutine(pasar_tiempo());
        boton_derecho.GetComponent<EventTrigger>().enabled=true;
        boton_izquierdo.GetComponent<EventTrigger>().enabled=true;
    }


    
    IEnumerator terminar_nivel()
    {
        boton_derecho.GetComponent<EventTrigger>().enabled=false;
        boton_izquierdo.GetComponent<EventTrigger>().enabled=false;
             
        if(usuario_win) 
        { 
         switch(nivel_actual)
         {
           case 1: break; 
           case 2:
           StopCoroutine(pasar_tiempo());
           StartCoroutine(mostrar_info("Bien Hecho"));
           guajiro_animator.SetTrigger("guajiro_win");
           cambiar_nivel(); 
            break; 
           case 3: break; 
           case 4: serrucho_animator.SetTrigger("serruchar_win"); break; 
         }
     
        }
        
        else
         {
          
          switch(nivel_actual){
          case 1: break; 
          case 2: 
          my_audiosource.PlayOneShot(cristales_rotos);  
          StopCoroutine(pasar_tiempo());
          StartCoroutine(mostrar_info("Has perdido"));
          guajiro_animator.SetTrigger("guajiro_fail");
         cambiar_nivel();  
          break; 
          case 3: break; 
          case 4: serrucho_animator.SetTrigger("serruchar_fail"); break; 
          } 
         }



        yield return new WaitForSeconds(1.5f);
    }





    public void cambiar_fondo(int level)
    {
        fondo.GetComponent<SpriteRenderer>().sprite =  listado_fondos[level-1];  
    }


    
    public void modificar_juego(int level)
    {

      for (int i=0;i<niveles.Count;i++)
          {       
             if(i==level-1) {niveles[i].SetActive(true);}
             else {
                niveles[i].SetActive(false);
              }
          }

        
        Destroy(player_instance);
        player_instance= Instantiate(player,posicion_inicial);

        switch(level)
        {
          case 1: //flappy
          zona_correcta=false;
          zona_incorrecta=false;
          rb_player=player_instance.AddComponent<Rigidbody2D>();  
                    
          break;
          
          case 2: //guajiro
          zona_correcta=false;
          zona_incorrecta=false;
          player_instance.transform.position= new Vector3 (7,0,0);
          player_instance.AddComponent<Presicion>();
          rb_player=player_instance.AddComponent<Rigidbody2D>();  
          rb_player.gravityScale=0;
          break;
          
          case 3: //turbeo
          zona_correcta=false;
          zona_incorrecta=false;        
          rb_player=player_instance.AddComponent<Rigidbody2D>();    
          player_instance.AddComponent<Turbeo>();
          break;

          case 4: //serrucho
          usuario_win=false;
          zona_correcta=false;
          zona_incorrecta=false;  
          rb_player=player_instance.AddComponent<Rigidbody2D>();
          player_instance.AddComponent<Turbeo>();    
          break;

        }      
    }
     





//nivel 1 turbeo
void boton_derecho_01()
{
     rb_player.velocity=Vector2.zero;
     rb_player.AddForce(new Vector2(0,200));
     //animator_player.SetTrigger("Flap");
     my_audiosource.PlayOneShot(wing_sound);
   
}

void boton_izquierda_01()
{

}


void boton_derecho_02()
{
  if(zona_correcta ==true)
     {
      usuario_win=true;      
     }

     else
      { 
       usuario_win=false; 
      }
}

void boton_izquierda_02()
{

}


void boton_derecho_03()
{
  if(zona_incorrecta ==false)
     {
      
      rb_player.AddForce ( Vector2.up *2.4f, ForceMode2D.Impulse); 
      usuario_win=true;
     }

     else
      { 
        usuario_win=false;
      }
}



void boton_izquierda_03()
{

}




void boton_derecha_04()
{
  serrucho_animator.SetTrigger("serruchar_derecha");
  boton_derecho.GetComponent<EventTrigger>().enabled=false;
  boton_izquierdo.GetComponent<EventTrigger>().enabled=true;
  my_audiosource.PlayOneShot(sierra_sound_01);
  rb_player.AddForce ( Vector2.up *2.4f, ForceMode2D.Impulse); 
}

void boton_izquierda_04()
{
 serrucho_animator.SetTrigger("serruchar_izquierda");
 boton_derecho.GetComponent<EventTrigger>().enabled=true;
 boton_izquierdo.GetComponent<EventTrigger>().enabled=false;
 my_audiosource.PlayOneShot(sierra_sound_02);
 rb_player.AddForce ( Vector2.up *2.4f, ForceMode2D.Impulse); 
  
}






}





//nivel 2 potenciometro