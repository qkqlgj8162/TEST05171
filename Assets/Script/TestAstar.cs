using System.Collections.Generic;
using UnityEngine;

public class TestAstar : MonoBehaviour
{
    public int[,] arrMap;
    public int idx;

    private int tempx;
    private int tempy;

    private Tile startTile;
    private Tile endTile;
    List<Tile> tileList;

    List<Tile> openList;

    List<Tile> closeList;

    void Start()
    {
        this.arrMap = new int[7, 5];
        this.tileList = new List<Tile>();
        this.openList = new List<Tile>();
        this.closeList = new List<Tile>();

        this.CreateMap();
        this.CreateTile();
        
        foreach(var tile in this.tileList)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = 1; j >= -1; j--)
                {
                    if(tile.pos==this.startTile.pos- new Vector2(j,i))
                    {
                        if (this.startTile.pos == tile.pos || tile.type == 3)
                        {
                            this.closeList.Add(tile);
                        }
                        else
                        {
                            this.openList.Add(tile);
                        }
                    }
                }
            }
        }

        foreach(var tile in this.openList)
        {
            var gDis = Vector2.Distance(this.startTile.pos, tile.pos);
            var gCsting = (int)(gDis * 10);            
            GameObject.Find($"{tile.transform.gameObject.name}/PathInfo/TextMeshG").transform.GetComponent<TextMesh>().text = gCsting.ToString();

            var hDis = Vector2.Distance(this.endTile.pos, tile.pos);
            var hCsting = (int)(hDis * 10);
            GameObject.Find($"{tile.transform.gameObject.name}/PathInfo/TextMeshH").transform.GetComponent<TextMesh>().text = hCsting.ToString();
                        
            GameObject.Find($"{tile.transform.gameObject.name}/PathInfo/TextMeshF").transform.GetComponent<TextMesh>().text = (gCsting+hCsting).ToString();
            tile.transform.GetComponent<SpriteRenderer>().color = Color.gray;

            var dirGO = Instantiate(Resources.Load<GameObject>("dir"));
            dirGO.transform.SetParent(tile.transform,false);
            //var screen  = Camera.main.WorldToScreenPoint(this.startTile.transform.position);
            //print(this.startTile.transform.position);
            //print(screen);            







            this.targetPos = this.startTile.transform.position;            
            this.thisPos = dirGO.transform.position;
            targetPos.x = targetPos.x - thisPos.x;
            targetPos.y = targetPos.y - thisPos.y;
            angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
            dirGO.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle+90));        







            //Vector3 targetPos = this.startTile.transform.position;
            //Vector3 targetPosFlattened = new Vector3(targetPos.x, targetPos.y, 0);
            //dirGO.transform.LookAt(targetPosFlattened);
        }
    }
    private Vector2 targetPos;
    private Vector2 thisPos;
    private float angle;



    private int count;

    public void CreateTile()
    {
        
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                var tileId = this.arrMap[j, i];                                
                var tileGo = Instantiate(Resources.Load<GameObject>("Tile" + tileId));
                var pos = Camera.main.ScreenToWorldPoint(new Vector2((tempx * 100) + 512 - 300, (tempy * -100) + 768-200));
                tileGo.AddComponent<Tile>();
                tileGo.GetComponent<Tile>().pos = new Vector2(j, i);
                if (tileId ==1)
                {
                    this.startTile = tileGo.transform.GetComponent<Tile>();
                }
                if (tileId == 2)
                {
                    this.endTile = tileGo.transform.GetComponent<Tile>();
                }
                pos.z = 0;
                tileGo.transform.position = pos;
                tileGo.name = $"Tile{this.count++}";
                tileGo.transform.GetComponent<Tile>().type = tileId;
                this.tileList.Add(tileGo.transform.GetComponent<Tile>());
                this.tempx++;
                if (this.tempx == 7)
                {
                    this.tempy++;
                    this.tempx = 0;
                }
            }
        }
    }

    public void CreateMap()
    {
        var textAsset = Resources.Load<TextAsset>("STAGE01");
        var arrStr = textAsset.text.Split(new char[] { ',', '\n' });

        for (int j = 0; j < 5; j++)
        {
            for (int i = 0; i < 7; i++)
            {
                var a = int.Parse(arrStr[idx]);
                arrMap[i, j] = a;
                idx++;
            }
        }
    }


}
