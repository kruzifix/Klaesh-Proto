﻿using Klaesh.Entity;
using Klaesh.Hex;
using UnityEngine;

namespace Klaesh.Game
{
    public interface IInputState
    {
        void OnEnabled();
        void OnDisabled();

        IInputState OnPickHexTile(HexTile tile, RaycastHit hit);
        IInputState OnPickGameEntity(GameEntity entity, RaycastHit hit);
    }

    public abstract class BaseInputState : IInputState
    {
        public virtual void OnDisabled() { }

        public virtual void OnEnabled() { }

        public virtual IInputState OnPickGameEntity(GameEntity entity, RaycastHit hit)
        {
            return null;
        }

        public virtual IInputState OnPickHexTile(HexTile tile, RaycastHit hit)
        {
            return null;
        }
    }
}