using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2 pos;
    public Tile parents;
    public int type;
    public float F { get; set; }

    private void Update()
    {
        this.transform.GetComponentInChildren<TextMesh>().text = $"{this.pos.x},{this.pos.y}";
    }
}
