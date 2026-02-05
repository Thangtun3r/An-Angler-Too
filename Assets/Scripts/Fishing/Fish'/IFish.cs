using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFish
{
    bool IsBiting();
    
    void BobberLanded(Transform bobber);
    bool TryCatchFish(Transform bobber);
}

