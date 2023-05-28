namespace Nebula.Objects.ObjectTypes;

public class Leaves : TypeWithImage
{
    private Color LeavesColor;

    public Leaves() : base(128, "Leaves", new SpriteLoader("Nebula.Resources.Leaves.png",200f))
    {
    }

    public override bool RequireMonoBehaviour => true;

    public override bool CanSeeInShadow(CustomObject? obj) { return false; }

    public override void Update(CustomObject obj, int command)
    {
        if(command == 1) obj.Renderer.color = Color.green;
        else obj.Renderer.color = LeavesColor;
    }

    public override void Initialize(CustomObject obj)
    {
        base.Initialize(obj);

        LeavesColor = Module.DynamicColors.MyColor.GetMainColor();
    }
}
