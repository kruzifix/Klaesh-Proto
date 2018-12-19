using System.Collections.Generic;
using System.Linq;
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
            AddSubscription(_bus.Subscribe<EntityKilledMessage>(OnEntityKilled));

            // create widgets for exisiting entities!!??
        }

        protected override void DeInit()
        {
            _widgets.Clear();
            transform.DestroyAllChildrenImmediate();
        }

        private void OnEntityCreated(EntityCreatedMessage msg)
        {
            if (msg.Value.createWidget)
                CreateWidget(msg.Value);
        }

        private void CreateWidget(Entity target)
        {
            var go = Instantiate(widgetPrefab, transform);
            var widget = go.GetComponent<EntityWidget>();

            widget.SetTarget(target);

            _widgets.Add(widget);
        }

        private void OnEntityKilled(EntityKilledMessage msg)
        {
            var ent = msg.Value;
            // didn't create a widget in the first place -> so we don't have to remove one
            if (!ent.createWidget)
                return;

            var widschets = _widgets.Where(w => w.Target == ent).ToList();
            foreach (var w in widschets)
            {
                _widgets.Remove(w);
                Destroy(w.gameObject);
            }
        }
    }
}
