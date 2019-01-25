namespace Klaesh.GameEntity.Component
{
    public delegate void MovementChangedEvent();

    public class HexMovementComp : HexPosComp, INewTurnHandler
    {
        private int _movementLeft;

        public int MovementLeft
        {
            get { return _movementLeft; }
            set { _movementLeft = value; MovementChanged?.Invoke(); }
        }

        public int maxDistance;
        public int jumpHeight;

        public event MovementChangedEvent MovementChanged;

        public void OnNewTurn()
        {
            MovementLeft = maxDistance;
        }
    }
}
