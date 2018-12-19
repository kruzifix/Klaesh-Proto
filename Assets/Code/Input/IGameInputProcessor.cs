using UnityEngine;

namespace Klaesh.Input
{
    public interface IGameInputProcessor
    {
        void OnEnter(GameObject go);
        void OnExit(GameObject go);

        void OnDown(GameObject go);
        void OnUp(GameObject go);

        void OnClick(GameObject go);
    }
}
