using System;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystemUIBridge : MonoBehaviour
{

    private  readonly DataLoader <ItemEntry>  inventoryLoader =  new()  ;
    private  readonly DataLoader<CraftableBase> craftablesLoader  = new (); 
    private readonly DataLoader<ICraftable> craftingQueueLoader =  new();

    
    [SerializeField] private List<CraftableBase>  craftables ; 
    private void Start(){   
        InitializeDataLoaders();
        craftablesLoader.UpdateData(craftables) ; 
    }
    private void InitializeDataLoaders (){
        // Bind callbacks to data casters
        Debug.Log(Inventory.Instance) ;
        Inventory.Instance.OnItemsUpdatedEvent +=  inventoryLoader.UpdateData ; 
        CraftingSystem.Instance.OnQueueUpdated +=  craftingQueueLoader.UpdateData ; 

    }

    [Serializable]
    private class DataLoader <T>{
        public List<T> Data { get => data ; set =>    data =  value; }
        private List<T> data ; 
    
        public void UpdateData(List<T> newData ){
            Data =  newData  ;
        }
    
    }

}

