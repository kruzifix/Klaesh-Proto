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

    private const float heightScale = 0.5f;

    private void Awake()
    {
        _renderer = model.GetComponent<MeshRenderer>();
    }

    public void Refresh()
    {
        var scale = model.transform.localScale;
        scale.z = height * heightScale;
        model.transform.localScale = scale;
    }

    public void SetColor(Color color)
    {
        _renderer.material.color = color;
    }

    public Vector3 GetTop()
    {
        return model.transform.position + new Vector3(0, height * heightScale, 0);
    }
}
