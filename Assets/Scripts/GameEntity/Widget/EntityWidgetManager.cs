using System;
using System.Collections.Generic;
using Klaesh.GameEntity.Message;
using Klaesh.UI;
using Klaesh.Utility;

namespace Klaesh.GameEntity.Widget
{
    public class EntityWidgetManager : ViewModelBehaviour
    {
        private List<EntityWidget> _widgets;

        public EntityWidget widgetPrefab;

        protected override void Init()
        {
            _widgets = new List<EntityWidget>();

            AddSubscription(_bus.Subscribe<EntityCreatedMessage>(OnEntityCreated));

            // create widgets for exisiting entiteis?
        }

        protected override void DeInit()
        {
            _widgets.Clear();
            transform.DestroyAllChildrenImmediate();
        }

        private void OnEntityCreated(EntityCreatedMessage msg)
        {
            CreateWidget(msg.Value);
        }

        private void CreateWidget(Entity target)
        {
            var go = Instantiate(widgetPrefab, transform);
            var widget = go.GetComponent<EntityWidget>();

            widget.SetTarget(target);

            _widgets.Add(widget);
        }
    }
}
