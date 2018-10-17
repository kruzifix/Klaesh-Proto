using Klaesh.Core.Message;
using UnityEngine;

public class FocusCameraMessage : MessageBase
{
    public Vector3 Position { get; private set; }
    public bool InterpolatePosition { get;private set; }

    public FocusCameraMessage(object sender, Vector3 position, bool interpolatePosition = true)
        : base(sender)
    {
        Position = position;
        InterpolatePosition = interpolatePosition;
    }
}
