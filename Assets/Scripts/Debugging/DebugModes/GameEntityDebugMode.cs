using Klaesh.GameEntity;
using UnityEngine;

namespace Klaesh.Debugging
{
    public class GameEntityDebugMode : AbstractDebugMode
    {
        public override string Name => "Game Entities";

        public override string Description => "Overview of Entity descriptors (templates) and active Game entities";

        private IGameEntityManager _manager;

        public override void RegisterServices()
        {
            base.RegisterServices();

            _manager = _locator.GetService<IGameEntityManager>();
        }

        public override void PrintScrollContent()
        {
            GUILayout.Label("Descriptors", DStyles.boldLabel);
            foreach (var desc in _manager.Descriptors)
            {
                GUILayout.BeginVertical(DStyles.box);

                GUILayout.Label($"Name: {desc.Value.name}", DStyles.miniBoldLabel);

                //GUILayout.Label($"Entity Id: {desc.Value.entityId}", DStyles.miniLabel);
                //GUILayout.Label($"Entity Name: {desc.Value.entityName}", DStyles.miniLabel);
                //GUILayout.Space(5f);

                //GUILayout.Label($"Mesh Prefab: {desc.Value.meshPrefab.name}", DStyles.miniLabel);
                //GUILayout.Label($"Mesh Offset: {desc.Value.meshOffset}", DStyles.miniLabel);
                //GUILayout.Space(5f);

                //GUILayout.Label($"Max Distance: {desc.Value.maxDistance}", DStyles.miniLabel);
                //GUILayout.Label($"Jump Height: {desc.Value.jumpHeight}", DStyles.miniLabel);

                GUILayout.EndVertical();
            }

            GUILayout.Label("Entities", DStyles.boldLabel);
            foreach (var ent in _manager.Entities)
            {
                GUILayout.BeginVertical(DStyles.box);

                GUILayout.Label($"Id: {ent.Id}", DStyles.miniBoldLabel);
                GUILayout.Label($"Descriptor: {ent.Descriptor.name}", DStyles.miniLabel);

                // TODO: show all modules and properties
                //GUILayout.Label($"Position: {ent.Position}", DStyles.miniLabel);

                GUILayout.EndVertical();
            }
        }

        public override void PrintStaticContent()
        {
        }
    }
}
