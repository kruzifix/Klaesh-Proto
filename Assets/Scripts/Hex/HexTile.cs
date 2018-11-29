using Klaesh.Entity;
using Klaesh.Hex;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    private const float heightScale = 0.5f;

    public GameObject model;

    [HideInInspector]
    public int height;
    [HideInInspector]
    public HexCubeCoord coord;

    private MeshRenderer _renderer;

    public GameEntity Entity { get; set; }
    public bool HasEntityOnTop { get { return Entity != null; } }

    private void Start()
    {
        _renderer = model.GetComponent<MeshRenderer>();
    }

    public void Refresh()
    {
        var scale = transform.localScale;
        scale.y = height * heightScale;
        transform.localScale = scale;
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
