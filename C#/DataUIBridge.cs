using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class DataUIBridge : MonoBehaviour
{

    private  readonly DataLoader <ItemEntry>  inventoryLoader       =  new();
    private  readonly DataLoader<CraftableBase> craftablesLoader    =  new(); 
    private readonly DataLoader<ICraftable> craftingQueueLoader     =  new();
    
    [SerializeField] private List<CraftableBase>  craftables ; 


    

    public DataLoader<ItemEntry> InventoryData  () =>  inventoryLoader  ;  
    public DataLoader<CraftableBase> CraftablesData () =>  craftablesLoader ; 
    public DataLoader<ICraftable> CraftingQueueData () =>  craftingQueueLoader ; 


    private void Start(){      
        BindDynamicDataLoaders(); 

        // Manually read  craftables once, because of  thier  static data nature , in this context. 
        craftablesLoader.UpdateData(craftables) ; 
    }
    private void BindDynamicDataLoaders (){
        // Bind dynamic data  callbacks to data casters 
        Inventory.Instance.OnItemsUpdatedEvent +=  inventoryLoader.UpdateData ; 
        CraftingSystem.Instance.OnQueueUpdated +=  craftingQueueLoader.UpdateData ;     
    }   

}
   
    public class DataLoader <T>{
        public List<T> Data { get => data ; set =>    data =  value; }
        private List<T> data ; 
        public event Action <DataLoader<T>>  OnChangeRecieved ; 

        public void UpdateData(List<T> newData ){
            Data =  newData  ;
            OnChangeRecieved(this) ; 
        }

    }
