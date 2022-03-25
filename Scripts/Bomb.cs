using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{

    public GameObject singleTileDestroyer;
    public GameObject singleTileGenerator;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        transform.localPosition = pos;
        if (Input.GetMouseButtonDown(0))
        {

            foreach (Transform a in singleTileGenerator.transform.GetComponentsInChildren<Transform>())
            {
                Scan s;
                SingleTile st;
                RaycastHit2D h = Physics2D.Raycast(a.transform.position, Vector2.zero);
                if (h.collider != null && (s = h.transform.GetComponent<Scan>()))
                {
                    if (a.gameObject.name == "0")
                    {
                        //Destroying tile at position and spawning a half-destructed one 
                        s.deleteAtAndSpawn(a.transform.position, true);
                    }
                    else if(a.gameObject.name == "1")
                    {
                        //Destroying half-destructed tiles at range once for all
                        s.deleteAtAndSpawn(a.transform.position);
                    
                    }
                }
            }

            singleTileDestroyer.SetActive(true);
            RaycastHit2D[] ht = Physics2D.CircleCastAll(pos, 3, Vector2.zero); 
            if (ht.Length > 0)
            {
                foreach(RaycastHit2D hs in ht)
                {
                    if(hs.transform.GetComponent<SingleTile>() && hs.transform.GetComponent<SingleTile>().destroyable)
                    {
                        hs.transform.gameObject.SetActive(false);
                    }
                }

            }
            

        }
        else if(Input.GetMouseButtonUp(0))
        {
            singleTileDestroyer.SetActive(false);
        }
    }
}
