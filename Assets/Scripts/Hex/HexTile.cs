using Klaesh.Hex;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    public GameObject model;

    [HideInInspector]
    public int height;
    [HideInInspector]
    public HexCubeCoord coord;

    public HexMap Map { get; set; }

    private MeshRenderer _renderer;

    private void Awake()
    {
        _renderer = model.GetComponent<MeshRenderer>();
    }

    public void Refresh()
    {
        var scale = model.transform.localScale;
        scale.z = height * 0.5f;
        model.transform.localScale = scale;
    }

    public void SetColor(Color color)
    {
        _renderer.material.color = color;
    }
}
