
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment")]
public class CraftableBase : Item , ICraftable
{
    // To-Do  implement base

    [Header("Crafting")]
    [SerializeField] private float craftTime  = 3.4f;
    public float CraftTime { get => craftTime; set =>   craftTime =  value; }
    public List<ItemEntry> NeededItems { get => neededItems; set { neededItems  =  value; }  }

    public List<ItemEntry>  neededItems ;

}
