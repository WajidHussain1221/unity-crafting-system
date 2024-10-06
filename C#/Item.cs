using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



[CreateAssetMenu(menuName = "Core/Inventory/Items" , fileName = "New Item")]
public class Item : ScriptableObject 

{   

    new public string  name; 
    public Sprite icon ; 

}
