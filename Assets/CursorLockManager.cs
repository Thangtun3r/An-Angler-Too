using System.Collections.Generic;
using UnityEngine;

public static class CursorLockManager
{
    private static readonly HashSet<string> reasons = new HashSet<string>();

    public static void RequestUnlock(string reason)
    {
        if (string.IsNullOrEmpty(reason)) reason = "Unknown";
        reasons.Add(reason);
        Apply();
    }

    public static void ReleaseUnlock(string reason)
    {
        if (string.IsNullOrEmpty(reason)) reason = "Unknown";
        reasons.Remove(reason);
        Apply();
    }

    public static bool IsUnlockedBySomeone => reasons.Count > 0;

    private static void Apply()
    {
        bool shouldUnlock = reasons.Count > 0;

        Cursor.lockState = shouldUnlock ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = shouldUnlock;
    }

    public static void ClearAll()
    {
        reasons.Clear();
        Apply();
    }
}