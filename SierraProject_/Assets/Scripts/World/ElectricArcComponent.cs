
public class ElectricArcComponent : IngredientComponent
{
    protected override void OnPowerOn()
    {
        base.OnPowerOn();
        SetUsed();
    }
}
