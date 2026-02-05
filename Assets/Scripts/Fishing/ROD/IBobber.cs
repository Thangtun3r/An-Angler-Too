using UnityEngine;
using System;

public interface IBobber
{
    // Initialize and launch the bobber from an origin with initial velocity.
    // seed is used for deterministic behavior when desired.
    void Cast(Vector3 origin, Vector3 velocity, int seed, Transform attachPoint, bool deterministic = true);

    // Whether the bobber is currently floating on the surface.
    bool IsFloating { get; }

    // Reel the bobber back / clean-up.
    void ReelIn();
}
