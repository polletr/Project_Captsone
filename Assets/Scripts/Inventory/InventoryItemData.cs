using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "InventoryItems"), Serializable]
public class InventoryItemData : ScriptableObject
{
    public string Name;

    public string Description;

    public int SlotSize;

    public int StackSize;

    public bool QuickAccess;

    public Sprite Image;
}
