using Klaesh.Core;
using Klaesh.Core.Message;
using Klaesh.Hex;
using UnityEngine;

public class HexTilePicker : MonoBehaviour
{
    private IMessageBus _bus;
    private HexMap _map;

    private void Start()
    {
        _bus = ServiceLocator.Instance.GetService<IMessageBus>();
        _map = FindObjectOfType<HexMap>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            _map.DeselectAll();

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var root = hit.transform.parent;
                var tile = root.GetComponent<HexTile>();

                if (tile != null)
                {
                    _bus.Publish(new FocusCameraMessage(this, tile.GetTop()));

                    tile.SetColor(Color.red);

                    var colorStrings = new[] {
                        "#299AFF", "#5879BF", "#B6373F", "#E61600",
                    };

                    var colors = new Color[colorStrings.Length];
                    for (int i = 0; i < colors.Length; i++)
                        ColorUtility.TryParseHtmlString(colorStrings[i], out colors[i]);

                    int maxDist = 2;
                    foreach (var n in tile.Map.GetReachableTiles(tile.coord, maxDist, 1))
                    {
                        n.Item1.SetColor(colors[n.Item2 - 1]);
                    }
                }
            }
        }
    }
}
