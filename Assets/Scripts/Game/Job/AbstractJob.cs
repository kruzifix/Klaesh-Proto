using System;
using Klaesh.Core;
using Klaesh.Game.Data;
using Klaesh.GameEntity;
using Klaesh.GameEntity.Module;
using Klaesh.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Klaesh.Game.Job
{
    public delegate void JobCompleteEvent();

    [JsonConverter(typeof(JobCreationConverter))]
    public interface IJob
    {
        event JobCompleteEvent OnComplete;

        void StartJob();
    }

    public abstract class AbstractJob : IJob
    {
        [JsonProperty("type")]
        public string Type { get { return GetType().Name; } set { } }

        public event JobCompleteEvent OnComplete;

        public abstract void StartJob();

        protected void Completed()
        {
            OnComplete?.Invoke();
        }
    }

    /// <summary>
    /// EntityJob can be created through 2 different ways.
    /// by ctor(Entity) -> the client who makes the input does this
    /// OR: by deserializer on passive client with ctor()
    /// then the deserializer populates the properties
    /// because we can't serialize an entity refence
    /// we have to do it this way
    /// </summary>
    public abstract class EntityJob : AbstractJob
    {
        private Entity _cachedEntity;

        protected Entity Entity
        {
            get
            {
                if (_cachedEntity == null)
                    // this should only be called on passive clients
                    _cachedEntity = ServiceLocator.Instance.GetService<IGameManager>().ResolveEntityRef(EntityRef);
                return _cachedEntity;
            }
        }

        [JsonProperty("entity")]
        public SquadEntityRefData EntityRef { get; set; }

        protected EntityJob() { }

        protected EntityJob(Entity entity)
        {
            _cachedEntity = entity;
            EntityRef = entity.GetModule<SquadMember>().RefData;
        }
    }

    internal class JobCreationConverter : JsonCreationConverter<IJob>
    {
        protected override IJob Create(Type objectType, JObject jObject)
        {
            // OMG LOOK AT MY HACKING! IM HACKING SO MUCH! SO GOOD!
            Type type = Type.GetType("Klaesh.Game.Job." + jObject["type"].ToString());

            return Activator.CreateInstance(type) as IJob;
        }
    }
}
