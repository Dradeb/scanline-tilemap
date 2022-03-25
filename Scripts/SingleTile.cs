using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTile : MonoBehaviour
{

    float lifeTime = 0;
    public bool destroyable = false; 
    // Start is called before the first frame update
    void Start()
    {
        destroyable = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!destroyable)
        {
            lifeTime += Time.deltaTime;
            if (lifeTime > 0.4)
                destroyable = true;
        }


    }
}
