namespace Klaesh.Game.Input
{
    public enum InputCode
    {
        //RecruitUnit,
        AttackMode,
        Card
    }

    public interface IInputProcessor
    {
        void ProcessInput(InputCode code, object data);
    }
}
