namespace Nebula.Module;

public static class GameHistory{
    public enum TimeType{
        Dead,
        Revive
    }

    public static List<Vector3> localPositonHistoty;
    public static float DeadTime;
    public static float ReviveTime;
    public static bool isWinding;
    
    public static void RecordTime(TimeType t,float data){
        if(t == TimeType.Dead) DeadTime = data;
        else if(t == TimeType.Revive) ReviveTime = data;
    }

    public static void RecordPosition(Vector3 pos){
        if(!isWinding) localPositonHistoty.Add(pos);
    }

    public static void CleanUpHistory(){
        DeadTime = float.MaxValue;
        ReviveTime = float.MaxValue;
        localPositonHistoty.Clear();
        isWinding = false;
    }

    public static Vector3 getLastPosition(){
        try{
            Vector3 pos = localPositonHistoty.Last();
            localPositonHistoty.Remove(pos);
            return pos;
        }catch{ return new Vector3(0f,0f,0f); }
    }
}