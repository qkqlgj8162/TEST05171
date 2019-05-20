using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;

public class TestAstar : MonoBehaviour
{
    public int[,] arrMap;
    public int idx;
    public Button btn;
    public Button btn2;

    private int tempx;
    private int tempy;
    private Color defalutColor;
    private Vector2 targetPos;
    private Vector2 thisPos;
    private float angle;
    private int count;
    private int count2;
    private Tile currentParentsNode;
    private Tile endTile;
    
    List<Tile> tileList;
    List<Tile> openList;
    List<Tile> closeList;
       
    private void Awake()
    {
        this.btn2.onClick.AddListener(() =>
        {
            foreach(var node in this.openList) //현재 오픈리스트 확인.
            {
                print($"{node.pos.x},{node.pos.y}");
            }
        });

        this.btn.onClick.AddListener(() =>
        {           
            this.NextStage();
        });
    }

    public void NextStage()
    {
        
        var sortList = this.openList.OrderBy(x => x.F);
        List<Tile> tempList = new List<Tile>();
        foreach (var node in sortList)
        {
            tempList.Add(node);
        }
        this.openList = null;
        this.openList = tempList;

        var firstValue = this.openList[0];
        print($"{this.openList[0].pos.x},{this.openList[0].pos.y}");
        this.currentParentsNode.type = 5;
        firstValue.type = 1;
        this.closeList.Add(firstValue);
        this.openList.RemoveAt(0);

        this.currentParentsNode = firstValue;
        this.TileColorReset();
        this.currentParentsNode.GetComponent<SpriteRenderer>().color = Color.green;
        for (int i=0;i<this.openList.Count;i++)
        {
            if (this.openList[i] == this.currentParentsNode)
            {                
                this.closeList.Add(this.openList[i]);
                this.openList.RemoveAt(i);
                break;
            }
        }
        List<Tile> tempTileList = new List<Tile>();
        this.CheckNode();
    }


    private void Start()
    {
        this.arrMap = new int[7, 5];
        this.tileList = new List<Tile>();
        this.openList = new List<Tile>();
        this.closeList = new List<Tile>();

        this.CreateMap();
        this.CreateTile();
        this.CheckNode();
    }

    public void CheckNode()
    {
        foreach (var tile in this.tileList)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = 1; j >= -1; j--)
                {
                    if (tile.pos == this.currentParentsNode.pos - new Vector2(j, i))
                    {
                        if (this.currentParentsNode.pos == tile.pos || tile.type == 3 || tile.type == 1)
                        {
                            this.closeList.Add(tile);
                        }
                        else
                        {
                            if(tile.type!=1 && tile.type != 5)
                            this.openList.Add(tile);
                        }
                        tile.parents = this.currentParentsNode;
                    }
                }
            }
        }

        foreach (var tile in this.openList)
        {
            var gDis = Vector2.Distance(this.currentParentsNode.pos, tile.pos);
            GameObject.Find($"{tile.transform.gameObject.name}/PathInfo/TextMeshG").transform.GetComponent<TextMesh>().text = gDis.ToString("N1");

            var hDis = Vector2.Distance(this.endTile.pos, tile.pos);
            GameObject.Find($"{tile.transform.gameObject.name}/PathInfo/TextMeshH").transform.GetComponent<TextMesh>().text = hDis.ToString("N1");

            tile.F = gDis + hDis;
            GameObject.Find($"{tile.transform.gameObject.name}/PathInfo/TextMeshF").transform.GetComponent<TextMesh>().text = (gDis + hDis).ToString("N1");
            tile.transform.GetComponent<SpriteRenderer>().color = Color.gray;

            var dirGO = Instantiate(Resources.Load<GameObject>("dir"));
            dirGO.transform.SetParent(tile.transform, false);
            this.targetPos = this.currentParentsNode.transform.position;
            this.thisPos = dirGO.transform.position;
            targetPos.x = targetPos.x - thisPos.x;
            targetPos.y = targetPos.y - thisPos.y;
            angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
            dirGO.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));
        }
    }
    
    public void TileColorReset()
    {
        foreach (var node in this.tileList)
        {
            if (node.type == 0 || node.type ==5)
            {
                node.transform.GetComponent<SpriteRenderer>().color = this.defalutColor;                
            }
            if(node.type ==5)
            {
                node.transform.GetComponent<SpriteRenderer>().color = Color.yellow;
            }
        }
        foreach(var node in this.openList)
        {
            node.GetComponentInChildren<TextMesh>().text = " ";
        }
        var a =GameObject.FindGameObjectsWithTag("dir");

        foreach(var obj in a)
        {
            Destroy(obj);
        }        
    }    

    public void CreateTile()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                var tileId = this.arrMap[j, i];
                var tileGo = Instantiate(Resources.Load<GameObject>("Tile" + tileId));
                var pos = Camera.main.ScreenToWorldPoint(new Vector2((tempx * 100) + 512 - 300, (tempy * -100) + 768 - 200));
                tileGo.AddComponent<Tile>();
                tileGo.GetComponent<Tile>().pos = new Vector2(j, i);
                if (tileId == 1)
                {
                    this.currentParentsNode = tileGo.transform.GetComponent<Tile>();
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
            foreach (var node in this.tileList)
            {
                if (node.type == 0)
                {
                    this.defalutColor = node.GetComponent<SpriteRenderer>().color;
                    break;
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
