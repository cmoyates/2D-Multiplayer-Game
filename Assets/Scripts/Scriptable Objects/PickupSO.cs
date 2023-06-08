using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickupSO : ScriptableObject
{
    //public Sprite sprite;
    public abstract void Activate(Vector3 pos);
}
