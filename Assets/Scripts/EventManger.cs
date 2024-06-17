using System;

public static class EventManager
{
    public static Action<float> OnJumpRequested;

    public static void BroadcastJumpRequest(float jumpForce)
    {
        OnJumpRequested?.Invoke(jumpForce);
    }
}
