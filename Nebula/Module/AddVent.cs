namespace Nebula.Module;

public class VentManager
{
    public Vent vent;
    public static List<VentManager> AllVents = new();
    public static ShipStatus CachedShipStatus;
    //public bool isSpawn = true;

    public VentManager(Vector3 p){
        /*if(AllVents.Count != 0 && p == AllVents[AllVents.Count - 1].vent.transform.position ){
            isSpawn = false;
            return;
        }*/
        ShipStatus CachedShipStatus = ShipStatus.Instance;
        var referenceVent = UnityEngine.Object.FindObjectOfType<Vent>();
        vent = UnityEngine.Object.Instantiate<Vent>(referenceVent);
        vent.transform.position = p;
        vent.Left = null;
        vent.Right = null;
        vent.Center = null;
        Vent tmp = CachedShipStatus.AllVents[0];
        vent.EnterVentAnim = tmp.EnterVentAnim;
        vent.ExitVentAnim = tmp.ExitVentAnim;
        vent.Id = CachedShipStatus.AllVents.Select(x => x.Id).Max() + 1;
        var allVentsList = CachedShipStatus.AllVents.ToList();
        allVentsList.Add(vent);
        CachedShipStatus.AllVents = allVentsList.ToArray();
        vent.gameObject.SetActive(true);
        vent.name = "AdditionalVent_" + vent.Id;
        AllVents.Add(this);
        Game.GameData.data.VentMap.Add(vent.gameObject.name, new Game.VentData(vent));
    }

    public static void newVent(Vector3 p){
        VentManager vents = new VentManager(p);
        //if(!vents.isSpawn) return; //什么叫高级屎山代码啊(战术后仰)
        if(AllVents.Count <= 1) return;
        vents.vent.Left = AllVents[AllVents.Count - 2].vent;
        AllVents[AllVents.Count - 2].vent.Right = vents.vent;
    }

    public static void CleanUp(){
        AllVents = new();
    }
}