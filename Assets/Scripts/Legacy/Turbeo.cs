using UnityEngine;

namespace Legacy
{
    public class Turbeo : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        void OnTriggerEnter2D (Collider2D col)
        {
            core_chancleta.core_chancleta_instance.zona_incorrecta=true;
        }
    }
}