using UnityEngine;

namespace Klaesh.Game.Cards
{
    public interface ICardData
    {
        string Name { get; }
        string Description { get; }
        string Type { get; }
        Sprite Art { get; }
    }
}
