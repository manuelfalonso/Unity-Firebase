using UnityEngine;

public static class FrameRateLimiter
{
    [RuntimeInitializeOnLoadMethod]
    public static void LimitFrameRate()
    {
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
    }
}
