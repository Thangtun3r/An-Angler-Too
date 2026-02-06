using UnityEngine;
using System;

public interface IFish
{
    bool IsBiting();

    void BobberLanded(Transform bobber);
    void BobberLeft();                 
    bool TryCatchFish(Transform handTransform);

    event Action OnFishBite;
    event Action OnFishGoAway;
}