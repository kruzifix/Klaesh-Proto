using System.Collections;
using System.Collections.Generic;
using Klaesh.Entity;
using UnityEditor;
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
            GUILayout.Label("Descriptors", EditorStyles.boldLabel);
            foreach (var desc in _manager.Descriptors)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);

                GUILayout.Label($"Name: {desc.Value.name}", EditorStyles.miniBoldLabel);

                GUILayout.Label($"Entity Id: {desc.Value.entityId}", EditorStyles.miniLabel);
                GUILayout.Label($"Entity Name: {desc.Value.entityName}", EditorStyles.miniLabel);
                GUILayout.Space(5f);

                GUILayout.Label($"Mesh Prefab: {desc.Value.meshPrefab.name}", EditorStyles.miniLabel);
                GUILayout.Label($"Mesh Offset: {desc.Value.meshOffset}", EditorStyles.miniLabel);
                GUILayout.Space(5f);

                GUILayout.Label($"Max Distance: {desc.Value.maxDistance}", EditorStyles.miniLabel);
                GUILayout.Label($"Jump Height: {desc.Value.jumpHeight}", EditorStyles.miniLabel);

                GUILayout.EndVertical();
            }

            GUILayout.Label("Entities", EditorStyles.boldLabel);
            foreach (var ent in _manager.Entities)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);

                GUILayout.Label($"Id: {ent.Id}", EditorStyles.miniBoldLabel);
                //GUILayout.Space(8f);

                GUILayout.Label($"Descriptor: {ent.Descriptor.name}", EditorStyles.miniLabel);
                GUILayout.Label($"Position: {ent.Position}", EditorStyles.miniLabel);

                GUILayout.EndVertical();
            }
        }

        public override void PrintStaticContent()
        {
        }
    }
}
