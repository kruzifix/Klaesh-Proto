using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Klaesh.Core;
using Klaesh.Core.Message;
using Klaesh.Hex;
using Klaesh.Utility;
using Newtonsoft.Json;
using UnityEngine;

namespace Klaesh.Game.Job
{
    public struct TerrainChange
    {
        [JsonProperty("c")]
        public HexOffsetCoord coord;
        [JsonProperty("a")]
        public int amount;
    }

    public class TerraformJob : AbstractJob
    {
        private ICoroutineStarter _starter;

        [JsonProperty("origin")]
        public HexCubeCoord Origin { get; set; }

        [JsonProperty("changes")]
        public List<TerrainChange> Changes { get; set; }

        [JsonProperty("card_id")]
        public int CardId { get; set; }

        public override void StartJob()
        {
            _starter = ServiceLocator.Instance.GetService<ICoroutineStarter>();

            _starter.StartCoroutine(DoTerraform());
        }

        private IEnumerator DoTerraform()
        {
            var bus = ServiceLocator.Instance.GetService<IMessageBus>();
            var map = ServiceLocator.Instance.GetService<IHexMap>();

            var gm = ServiceLocator.Instance.GetService<IGameManager>();

            gm.UseCard(CardId);

            bus.Publish(new FocusCameraMessage(this, map.GetTile(Origin).GetTop()));

            var tiles = Changes
                .Select(c => Tuple.Create(map.GetTile(Origin + c.coord.CubeCoord), c.amount))
                .Where(t => t.Item1 != null)
                .ToList();
            foreach (var t in tiles)
            {
                t.Item1.SetColor(t.Item2 > 0 ? Colors.TerraformRaise : Colors.TerraformLower);
                yield return new WaitForSeconds(0.1f);
            }

            // TODO: play animation here?
            foreach (var t in tiles)
            {
                var tile = t.Item1;
                tile.Height = Mathf.Max(tile.Height + t.Item2, 3);
                tile.SetColor(Color.white);

                // TODO: spawn particles!
                yield return new WaitForSeconds(0.1f);
            }
            yield return null;

            Completed();
        }
    }
}
