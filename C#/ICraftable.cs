
using System.Collections.Generic;

public interface  ICraftable  {

    public float CraftTime {  set  ;  get ; }

    public List<ItemEntry> NeededItems  {get  ; set ; }
}   