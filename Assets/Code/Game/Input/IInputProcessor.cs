namespace Klaesh.Game.Input
{
    public enum InputCode
    {
        RecruitUnit
    }

    public interface IInputProcessor
    {
        void ProcessInput(InputCode code, object data);
    }
}
