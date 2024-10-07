
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;
using System.Net.Http.Headers;
using Unity.Collections.LowLevel.Unsafe;

public class Inventory : MonoBehaviour
{
    #region   Singleton  
    public static Inventory Instance { get; private set; }
    void Awake()
    {
        Instance = this;
        if (Instance.gameObject != gameObject)
        {
            Debug.Log($"Destroying inventory instance on {Instance.gameObject} , only one instance may exist.");
            Destroy(Instance);
        }
        OnItemsUpdatedEvent +=  TestEvent ;
    }

    #endregion

    [SerializeField] private List<ItemEntry> items = new();

    [SerializeField, Range(0, 100)] private int inventorySize = 20;

    #region   Public Events

    //public delegate List<ItemEntry> OnItemsUpdated ( ) ;  
    public event  Action<List<ItemEntry>>  OnItemsUpdatedEvent ; 

    #endregion  

    private void TestEvent(List<ItemEntry> passedData){
        print(passedData) ; 
    }
    void Start(){TestAdd();}
    public Item testItem ; 
    [ContextMenu("Test Add")]
    public void TestAdd(){

        AddItem(testItem)  ;
    }

    public void AddItem(Item item)
    {   

        if (inventorySize + 1 >= items.Count)
        {
            //items.Add() ; 
            bool softAddedSuccess = SoftAdd();
            if (!softAddedSuccess)
            {
                ItemEntry  freshEntry  =  new (
                    item , 1
                ) ; 

                items.Add(freshEntry);
            }

        }
        bool SoftAdd()
        {

            ItemEntry preAssumedEntry = QueryItem(item);
            preAssumedEntry?.AddStack(1);
            return preAssumedEntry != null;

        }

        OnItemsUpdatedEvent?.Invoke(items);

    }   

  


    public void RemoveItem(Item item)
    {
        ItemEntry entry = QueryItem(item);
        if (items.Contains(entry))
        {
            items.Remove(entry);
            //onItemChanged(item, false);
        }
        else
        {
            Debug.Log($"Can't remove {item.name} as it does not exist in inventory");
        }
        OnItemsUpdatedEvent.Invoke(items);
    }

    private ItemEntry QueryItem(Item searchItem)
    {
        // Use FirstOrDefault to find the item
        return items.FirstOrDefault(item => item.Item == searchItem);
    }





    public ItemEntry GetItemEntry(ItemEntry itemEntry)
    {
        return items.FirstOrDefault(entry => itemEntry.Item == entry.Item);
    }






}
[Serializable]
public class ItemEntry
{


    public Item Item { get { return item; } private set { } }
    public int Count { get { return count; } private set { } }
    [SerializeField] private Item item;
    [SerializeField] private int count;


    private int reservedAmount;
    public void ReserveVolume(int amount)
    {

        if (amount <= count)
        {
            count -= amount;
            reservedAmount += amount;
        }
    }
    public void RestoreVolume(int restoreAmount)
    {

        if (restoreAmount <= reservedAmount)
        {
            count += reservedAmount;
            reservedAmount -= restoreAmount;
        }
        else
        {

            Debug.Log("Attempt to restore item amount to more than max borrowed amount");
        }
    }

    /// <summary>
    /// Changes count value in inventory by 1 or -1 depending on  truthy-ness of passed
    /// boolean
    /// </summary>
    public void AddStack(int stack__)
    {
        count +=  stack__ ;
    }

    public ItemEntry(Item item__, int count__)
    {
        item = item__;
        count = count__;
    }
}