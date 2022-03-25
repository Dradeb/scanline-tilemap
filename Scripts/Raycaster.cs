using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Raycaster : MonoBehaviour
{

    public GameObject parent; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
      
        if(Input.GetMouseButtonUp(0))
        {   
          
            Scan[] ss = GetComponentsInChildren<Scan>();
            foreach (Scan s in ss)
            {
                s.scan();
                
            }
        }
    }
}
