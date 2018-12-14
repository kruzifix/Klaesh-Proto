﻿using Klaesh.Core;
using Klaesh.Core.Message;
using Klaesh.Game.Job;
using Klaesh.GameEntity;
using Klaesh.GameEntity.Component;
using Klaesh.Hex;
using Klaesh.Network;

namespace Klaesh.Game.Input
{
    public class IdleInputState : AbstractInputState
    {
        private IGameManager _gm;
        private IHexMap _map;

        public IdleInputState(InputStateMachine context)
            : base(context)
        {
            _gm = ServiceLocator.Instance.GetService<IGameManager>();
            _map = ServiceLocator.Instance.GetService<IHexMap>();
        }

        public override void Enter()
        {
            // highlight active squad units
            _map.DeselectAllTiles();

            foreach (var unit in _gm.ActiveSquad.Members)
            {
                _map.GetTile(unit.GetComponent<HexMovementComp>().Position).SetColor(_gm.ActiveSquad.Config.Color);
            }
        }

        public override void ProcessInput(InputCode code, object data)
        {
            if (code == InputCode.RecruitUnit && data != null && data is string)
            {
                var originCoord = _gm.ActiveSquad.Config.Origin;
                var originTile = _map.GetTile(originCoord);
                var pickableTiles = _map.GetNeighbors(originCoord);
                pickableTiles.Add(originTile);

                var state = new HexPickState(Context, pickableTiles, (tile) =>
                    {
                        if (tile.HasEntityOnTop)
                            return;
                        // spawn unit at that tile
                        var job = new SpawnUnitJob
                        {
                            Position = tile.Position.OffsetCoord,
                            EntityId = data as string
                        };

                        var jm = ServiceLocator.Instance.GetService<IJobManager>();
                        jm.AddJob(job);
                        jm.ExecuteJobs();
                        ServiceLocator.Instance.GetService<INetworker>().SendData(EventCode.DoJob, job);

                        // callback
                        Context.SetState(this);
                    }, () =>
                    {
                        Context.SetState(this);
                    });
                Context.SetState(state);
                ServiceLocator.Instance.GetService<IMessageBus>().Publish(new FocusCameraMessage(this, originTile.GetTop()));
            }
        }

        public override void ProcessEntity(Entity entity)
        {
            if (_gm.IsPartOfActiveSquad(entity))
            {
                var moveComp = entity.GetComponent<HexMovementComp>();
                if (moveComp != null && moveComp.MovementLeft > 0)
                {
                    Context.SetState(new EntitySelectedInputState(Context, entity));
                    return;
                }
            }
        }

        public override void ProcessHexTile(HexTile tile)
        {
            if (tile.HasEntityOnTop)
            {
                ProcessEntity(tile.Entity);
                return;
            }
            ServiceLocator.Instance.GetService<IMessageBus>().Publish(new FocusCameraMessage(this, tile.GetTop()));
        }
    }
}
