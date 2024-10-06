
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
/// <summary>
/// Factory pattern or sum shi , but finna prepare the recipe (or however tf you spell it) thing first
/// </summary>
public class CraftingSystem : MonoBehaviour
{   

    #region  Debugging 
    public Item[]  items ; 

    [ContextMenu("Test Add to Queue")]

    public void AddToQueue(){

        foreach (Item  item  in items){
            if (item is ICraftable craftable)
                AddToQueue(craftable);
        }

    }

    #endregion  

    /*  
      Steps to craft an object :  
        - What object  [ Done ]
        - Requirements to craft it , the needed materials  [ Done ]
        - Quintity of needed materials is validated [ Done ]
        - Reserve required items into a container  [ Done ]
        - Begin crafting and sleep [Done]
        - End crafting and return the crafted object as <type> (Item) [Done]
        - Refund items if queue is cancelled [ Done ]
    */  

    private readonly Queue<ICraftable>  craftQueue =  new() ;
    public ICraftable currentCraftable  ; 
   

    [SerializeField , Range ( 0 , 20)]  private int queueQoutta ; 
 
    public  ICraftable  CraftTaskTarget {  get { return  currentCraftable ; }      private  set {
        value = currentCraftable ; 
    } }    

    void Start(){

    }
    void Update(){
        if(Input.GetKeyDown(KeyCode.C) ){
            if (!threadRunning)
            Craft();
            else 
            Debug.Log("There is already a crafting queue in process");
        }  
        if(Input.GetKeyDown(KeyCode.X) ){
            if(craftQueue.Count >  0  && !threadRunning)
            CancelQueue(); 
            //Debug.Log("There is no  crafting queue in process");
        }  
      
    }

    # region System Main

    bool  threadRunning = false ;
    private async void TickCore (Queue<ICraftable> craftables){
        threadRunning =  true ; 
        foreach (ICraftable  craftable in  craftables)
        {   
            Debug.Log((craftable as Item).name) ;
            Item craftResult =  await  CrafItem(craftable) ; 
            Inventory.Instance.AddItem(craftResult) ;
          
        }
        threadRunning =  false ;
    }

    public async Task<Item>  CrafItem(ICraftable  craftable){
        Debug.Log($"Crafting {(craftable as Item).name } ...") ;     
        await Task.Delay(SecondsToMs( craftable.CraftTime) );
        return  craftable  as Item  ;  
       
    }
 
    #endregion
    # region Queue Management       

    
  
    public void AddToQueue (ICraftable  craftable){


        // Get a list of needed items
        ItemEntry[]  neededItems  =  craftable.NeededItems.ToArray() ; 

        bool  readyToBeQueued =  false ; 
        HashSet<int> resourceFootPrint  = new(); 
        foreach( ItemEntry item  in neededItems){

            ItemEntry itemEntry =  Inventory.Instance.GetItemEntry(item) ;
            if(itemEntry !=  null  &&   itemEntry.Count <= item.Count ){
                readyToBeQueued =  false ;
                Debug.Log($"Insufficent {itemEntry.Item.name} to craft {(craftable  as Item).name } \n  {itemEntry.Count} / {item.Count}") ;
                resourceFootPrint.Add(0) ;
            }
            else if (itemEntry ==  null) {Debug.Log(item.Item.name + " not found in inventory"); }
            else  resourceFootPrint.Add(1) ; 

        }   
        readyToBeQueued =  resourceFootPrint.Count == 1 &&  resourceFootPrint.Contains(1) ;
        if(craftQueue.Count  < queueQoutta   && readyToBeQueued ) {
            Debug.Log( "Reserving resources for "+ (craftable as Item ).name);

            // Reserve resources 
            foreach( ItemEntry item  in neededItems){
            
                Inventory.Instance.GetItemEntry(item).ReserveVolume(item.Count);   
                KeyValuePair<ItemEntry, int> backupKey =  new(item ,  item.Count); 
                 ItemReservationMap.Instance.AddFundbackup(backupKey);
            }
 
            craftQueue.Enqueue(craftable) ;
        }

    }

    void Craft(){
        ItemReservationMap.Instance.PurgeAll();
        TickCore(craftQueue);
    
    }
    void CancelQueue(){
        ItemReservationMap.Instance.RefundAll();
        craftQueue.Clear() ;
    }

    #endregion
    #region  Util Functions

    private int SecondsToMs(float  timeInSeconds) {
        // May the overlords have mercy on me in end times 
        HashSet<char> filter = new() { 'f' , '.'} ; 
        string numericValues =  timeInSeconds.ToString().ReplaceMultiple(filter , ' ').Replace(" " , "") ; 
        int timeInMs =   int.Parse(numericValues)  * 1000 ;
        return timeInMs ; 
    }

    #endregion


    class ItemReservationMap {

        private ItemReservationMap(){

        }
        private static ItemReservationMap instance ;
        public static ItemReservationMap Instance  {get {
            if (instance != null)
                return instance;
                else {
                    instance = new ItemReservationMap();
                    return instance ;
                }
            } 
            private set {}} 

        private readonly Dictionary<ItemEntry , int> reservation =  new() ;


        public void AddFundbackup(KeyValuePair<ItemEntry,  int> keyValuePair){
            reservation.Add(keyValuePair.Key , keyValuePair.Value) ; 
        }

        public void PurgeAll(){
            reservation.Clear();
        }

        public void RefundAll(){

            foreach (KeyValuePair<ItemEntry ,  int> keyValuePair  in reservation) {
                Inventory.Instance.GetItemEntry(keyValuePair.Key).RestoreVolume(keyValuePair.Value) ; 
            }
            PurgeAll();
        }
        public void Refund(ItemEntry entry){

            foreach (KeyValuePair<ItemEntry ,  int> keyValuePair  in reservation) {
                if(keyValuePair.Key ==  entry)
                    Inventory.Instance.GetItemEntry(keyValuePair.Key).RestoreVolume(keyValuePair.Value) ; 
                    break ; 
            }
        }
    }   

}


