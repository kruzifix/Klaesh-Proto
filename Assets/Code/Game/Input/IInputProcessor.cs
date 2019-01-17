namespace Klaesh.Game.Input
{
    public enum InputCode
    {
        RecruitUnit,
        AttackMode,
        CardInput
    }

    public interface IInputProcessor
    {
        void ProcessInput(InputCode code, object data);
    }
}
