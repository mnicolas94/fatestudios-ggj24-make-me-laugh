using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Bird : MonoBehaviour
{
    bool bird_is_dead;
    Rigidbody2D rb_bird; 
    Animator animator_bird;
    public int up_force;

    
    public  GameObject title_tmp,subtitle_tmp, game_over_tmp;
    public  TextMeshProUGUI score_tmp,max_score_tmp;

    int max_score,actual_score;  

    bool game_over;  

    public  GameObject tap_button;

   public AudioClip die_sound,hit_sound,point_sound,wing_sound;
   AudioSource my_audiosource;

 
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate =60;
        //game_over_tmp.SetActive(false);
        bird_is_dead=false;
        Debug.Log("El pájaro está vivo");
        rb_bird= GetComponent<Rigidbody2D>(); 
        animator_bird = GetComponent<Animator>();
        my_audiosource = GetComponent <AudioSource>();
        Time.timeScale=0;
        max_score=PlayerPrefs.GetInt("max_score",0); 
        max_score_tmp.text= "MAX SCORE: " + max_score;///
        score_tmp.text="";
        score_tmp.text="";
        actual_score=0;
 
    }

    // Update is called once per frame
    void Update()
    {
        if(this.transform.position.x<-5){
            Time.timeScale=0;
            game_over_tmp.SetActive(true);
            subtitle_tmp.SetActive(true);
            if(actual_score>max_score)   { max_score=actual_score;}
            max_score_tmp.text= "MAX SCORE: " + max_score;
            score_tmp.text="NEW SCORE: "+ actual_score;
            PlayerPrefs.SetInt("max_score",max_score); 
            tap_button.GetComponent<EventTrigger>().enabled = true;
            }
    }





    public void bird_jump()
    {
     if(bird_is_dead==false)
     {
     Time.timeScale=1;
     title_tmp.SetActive(false);   
     subtitle_tmp.SetActive(false);
     rb_bird.velocity=Vector2.zero;
     rb_bird.AddForce(new Vector2(0,up_force));
     animator_bird.SetTrigger("Flap");
     max_score_tmp.text= "";
     score_tmp.text="SCORE: "+ actual_score;
     my_audiosource.PlayOneShot(wing_sound);
     }

     else
      { 
        SceneManager.LoadScene("GameScene");
      }

      
    }

    
   private void OnCollisionEnter2D(Collision2D collision)
   {
     my_audiosource.PlayOneShot(hit_sound);
     bird_is_dead=true;
     rb_bird.SetRotation(-90);
     Debug.Log("El pájaro está muerto");
     animator_bird.SetTrigger("Die");
     tap_button.GetComponent<EventTrigger>().enabled = false;
   }

   
    void OnTriggerEnter2D (Collider2D col)
   {
     my_audiosource.PlayOneShot(point_sound);
     actual_score++;   
     score_tmp.text="SCORE: "+ actual_score;
   }

}
