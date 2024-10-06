
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine.UIElements;
using JetBrains.Annotations;

public class Inventory : MonoBehaviour
{

    [SerializeField] private  List<ItemEntry>  items  =  new (); 

    [SerializeField, Range( 0 , 100)] private int inventorySize  =  20;


    [SerializeField]  public Item addTestItem ;
    [SerializeField]  public Item  removeTestItem ; 



    public static Inventory Instance {get  ;  private set ;}   

    void Awake(){
        Instance = this  ;
        if (Instance.gameObject != gameObject) {
            Debug.Log($"Destroying inventory instance on {Instance.gameObject} , only one instance may exist.") ;
            Destroy(Instance) ; 
        }
        onItemChanged +=  LogItemChange;
    }

    public void AddItem (Item  item) {

        if (inventorySize  + 1 >=  items.Count ) {
            //items.Add() ; 
            bool softAddedSuccess = SoftAdd();
            if (!softAddedSuccess) {

                items.Add  ( new (
                    item , 1 
                )) ; 
            }
            onItemChanged(item , true);
        }   
        bool  SoftAdd(){

            ItemEntry preAssumedEntry =  QueryItem(item); 
            preAssumedEntry?.ChangeValue(true)  ;                 
            return  preAssumedEntry !=null ; 
        }
    }
    public void RemoveItem(Item  item){
        ItemEntry entry = QueryItem(item) ;    
        if (items.Contains(entry)){
            items.Remove(entry ); 
            onItemChanged(item , false) ;
        }   
        else {
            Debug.Log($"Can't remove {item.name} as it does not exist in inventory") ; 
        }
    }
    
    private ItemEntry QueryItem(Item searchItem)
    {   
        // Use FirstOrDefault to find the item
        return items.FirstOrDefault(item => item.Item == searchItem);
    }




    [Serializable]

    #region  Debug
    private delegate   void OnItemChanged (Item item , bool added );
    OnItemChanged onItemChanged ; 

    private void LogItemChange(Item  item  , bool added)
    {
        string  stateChange =   added  ? "added to"  : "removed from" ; 
        Debug.Log($"{item.name}  {stateChange }  inventory") ;  
    }


    public ItemEntry GetItemEntry(ItemEntry itemEntry){

        return  items.FirstOrDefault(entry => itemEntry.Item ==  entry.Item);
    }

    
    #endregion



}
    [Serializable]
    public class  ItemEntry {

        
        public Item Item  {get { return  item  ;  }  private set{}  } 
        public int Count {get {return count ;}  private set {} } 
        [SerializeField] private Item item ; 
        [SerializeField] private int count ;


        private int reservedAmount ; 
        public void ReserveVolume (int amount ){
        
            if (amount <=  count){
                count -=  amount; 
                reservedAmount += amount; 
            }
        }
        public void RestoreVolume( int restoreAmount){

            if (restoreAmount <=  reservedAmount){
                count +=  reservedAmount; 
                reservedAmount -= restoreAmount;
            }   
            else {

                Debug.Log("Attempt to restore item amount to more than max borrowed amount");
            }


        }

        /// <summary>
        /// Changes count value in inventory by 1 or -1 depending on  truthy-ness of passed
        /// boolean
        /// </summary>
        public void ChangeValue(bool increameant ){
            count +=  increameant ? 1 :  - 1; 
        }

        public ItemEntry (Item item__  , int count__) {
            item  =  item__  ; 
            count =  count__ ;
        }
    }