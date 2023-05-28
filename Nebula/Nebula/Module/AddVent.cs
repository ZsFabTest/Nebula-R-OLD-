namespace Nebula.Module;

/*public class BombEvent : Events.LocalEvent{
    private PlayerControl target;
    public BombEvent(PlayerControl target) : base(0.1f) { this.target = target; }
    public override void OnTerminal()
    {
        Helpers.checkMuderAttemptAndKill(target,target,Game.PlayerData.PlayerStatus.Dead,false,false);
        //Debug.Log("Bomb finished.");
    }
}
*/

public class VentManager
{
    public Vent vent;
    public static List<VentManager> AllVents = new();
    public static List<Vent> bombVents = new();
    public static ShipStatus CachedShipStatus;
    public static Dictionary<string,Game.VentData> OriginData = new();
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

    public static void setBomb(Vent vent){
        bombVents.Add(vent);
    }

    public static void checkBomb(Vent vent,PlayerControl target){
        for(int i = 0;i < bombVents.Count;i++){
            Vent v = bombVents[i];
            if(v.name == vent.name){
                //Events.LocalEvent.Activate(new BombEvent(PlayerControl.LocalPlayer));
                Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer,target,Game.PlayerData.PlayerStatus.Killed,false,false);
                bombVents.Remove(v);
            }
        }
    }

    public static void blockUp(Vent vent){
        OriginData.Add(vent.name,Game.GameData.data.GetVentData(vent.name));
        vent.GetVentData().Sealed = true;
        vent.GetVentData().PreSealed = true;
    }

    public static bool setOrigin(Vent vent){
        try{
            Game.VentData origin = OriginData[vent.name];
            Game.GameData.data.VentMap[vent.name] = origin;
            return true;
        }catch{
            return false;
        }
    }

    public static void moveVent(Vent vent,Vector3 pos){
        vent.transform.position = pos;
    }

    public static void CleanUp(){
        AllVents = new();
        bombVents = new();
        OriginData = new();
    }
}