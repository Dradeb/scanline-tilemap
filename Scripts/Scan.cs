using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Scan : MonoBehaviour
{

    struct Point
    {
        public int x;
        public int y;
        public int state;

        public Point(int x, int y, int state)
        {
            this.x = x;
            this.y = y;
            this.state = state;
        }
    }

    bool isNotEmptyTile = false;
    bool isSolid = false; 

    public Tilemap tilemap;
    public TileBase tile, tile2;
    Point[,] tiles;


    public GameObject singleTile;
    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        tilemap.CompressBounds();
        tiles = new Point[tilemap.size.x, tilemap.size.y];
        tileMapToArray();
    }

    // Update is called once per frame
    void Update()
    {
    }

    
    //function that scanes the full tilemap and 
    public int scan()
    {
        tileMapToArray();
        Debug.Log("scanning " + tilemap.gameObject);
        isNotEmptyTile = false;
        for (int xm = tilemap.origin.x; xm < tilemap.origin.x + tilemap.size.x; xm++)
        {
            for (int ym = tilemap.origin.y; ym < tilemap.origin.y + tilemap.size.y; ym++)
            {
                if(tilemap.HasTile(new Vector3Int(xm,ym,0)))
                {
                    isNotEmptyTile = true;
                    return 0;
                   
                }
            }
        }
        if(!isNotEmptyTile)
        {
            this.gameObject.SetActive(false);   
        }
        return 0; 

    }
    public void delete()
    {
        Vector3Int v = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        tiles[v.x - tilemap.origin.x, v.y - tilemap.origin.y].state = 0;
        tilemap.SetTile(v, null);

    }
    
    public void deleteAt(Vector3 position)
    {
        Vector3Int v = tilemap.WorldToCell(position);
        tiles[v.x - tilemap.origin.x, v.y - tilemap.origin.y].state = 0;
        tilemap.SetTile(v, null);
    }
    public void deleteAtAndSpawn(Vector3 position, bool isTrigger = false)
    {
        Vector3Int v = tilemap.WorldToCell(position);
        tiles[v.x - tilemap.origin.x, v.y - tilemap.origin.y].state = 0;
        Sprite T =  tilemap.GetSprite(v);
        tilemap.SetTile(v, null);


        GameObject a = Instantiate(singleTile,position, Quaternion.identity, tilemap.transform.root);
        a.GetComponent<Rigidbody2D>().AddTorque(100f);
        if (isTrigger)
        {
            a.GetComponent<Collider2D>().isTrigger = isTrigger;
        }

        a.GetComponent<SpriteRenderer>().sprite = T;
        a.SetActive(true);

    }
    int Fill(Tilemap map, int x, int y)
    {

        int totalTiles = 0;
        //if tile is empty or tile already processed
        if (!map.HasTile(new Vector3Int(x, y, 0))) return 0;
        if (tiles[x - map.origin.x, y - map.origin.y].state == 2)
        {
            //tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            return 0;
        }

        int x1, y1;
        bool spanAbove, spanBellow;
        Stack<Point> stack = new Stack<Point>();
        stack.Push(new Point(x, y, 1));

        while (stack.Count > 0)
        {
            Point p = stack.Pop();
            x1 = p.x;
            y1 = p.y;


            spanAbove = spanBellow = false;
            while (x1 >= map.origin.x && tiles[x1 - map.origin.x, y1 - map.origin.y].state == 1)
            {

                x1--;
            }
            x1++;



            while (x1 < map.origin.x + map.size.x && tiles[x1 - map.origin.x, y1 - map.origin.y].state == 1)
            {
                tiles[x1 - map.origin.x, y1 - map.origin.y].state = 2;
                //tilemap.SetTile(new Vector3Int(x1, y1, 0), tile2);
                totalTiles++;

                if (spanAbove == false && y1 < map.origin.y + map.size.y - 1 && tiles[x1 - map.origin.x, (y1 + 1) - map.origin.y].state == 1 && tiles[x1 - map.origin.x, (y1 + 1) - map.origin.y].state != 0)
                {
                    stack.Push(new Point(x1, y1 + 1, 1));
                    spanAbove = true;

                }
                else if (spanAbove == true && y1 < map.origin.y + map.size.y - 1 && tiles[x1 - map.origin.x, (y1 + 1) - map.origin.y].state != 1)
                {
                    spanAbove = false;
                }

                if (spanBellow == false && y1 > map.origin.y && tiles[x1 - map.origin.x, (y1 - 1) - map.origin.y].state == 1 && tiles[x1 - map.origin.x, (y1 - 1) - map.origin.y].state != 0)
                {
                    stack.Push(new Point(x1, y1 - 1, 1));
                    spanBellow = true;

                }
                else if (spanBellow == true && y1 > map.origin.y && tiles[x1 - map.origin.x, (y1 - 1) - map.origin.y].state != 1)
                {
                    spanBellow = false;
                }


                x1++;

            }
        }

        int amount = 0;

        // loop through all of the tiles        
        BoundsInt bounds = tilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            Tile tile = tilemap.GetTile<Tile>(pos);
            if (tile != null)
            {
                    amount += 1;
            }
        }

        if(amount != totalTiles)
        {
            createNewTile(tilemap);
        }
        else
        {
            Debug.Log(gameObject.name + " is not solid ");

        }
        return totalTiles;
    }


    void createNewTile(Tilemap map)
    {


        int currentTiles = 0;

        GameObject a = Instantiate(tilemap.transform.gameObject, tilemap.transform.position, tilemap.transform.rotation, tilemap.transform.root);
        a.name = "Clone TileMap";
        Tilemap m = a.transform.GetComponent<Tilemap>();

        for (int xm = m.origin.x; xm < m.origin.x + m.size.x; xm++)
        {
            for (int ym = m.origin.y; ym < m.origin.y + m.size.y; ym++)
            {
                if (tiles[xm - m.origin.x, ym - map.origin.y].state != 2 && tiles[xm - m.origin.x, ym - map.origin.y].state != 0)
                {
                    m.SetTile(new Vector3Int(xm, ym, 0), null);
                    currentTiles++;

                }
                else
                {
                    tilemap.SetTile(new Vector3Int(xm, ym, 0), null);
                    
                }
            }
        }
        m.CompressBounds();
        tilemap.CompressBounds();

        Debug.Log(scan());
        Debug.Log(tilemap.gameObject.name + " :::: " + currentTiles);




    }
    public void checkIsSolid(Tilemap map)
    {
        Tilemap m = GetComponent<Tilemap>();
        

    }



    void tileMapToArray()
    {

        tiles = new Point[tilemap.size.x, tilemap.size.y];
        for (int x = tilemap.origin.x; x < tilemap.origin.x + tilemap.size.x; x++)
        {
            for (int y = tilemap.origin.y; y < tilemap.origin.y + tilemap.size.y; y++)
            {

                if (tilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    isNotEmptyTile = true;
                    //tilemap.SetTile(new Vector3Int(x, y, 0), tile2);
                    tiles[x - tilemap.origin.x, y - tilemap.origin.y].state = 1;

                }
                else
                {
                    tiles[x - tilemap.origin.x, y - tilemap.origin.y].state = 0;

                }

            }
        }

        if(!isNotEmptyTile)
        {
            this.gameObject.SetActive(false);
        }
        

    }
}


