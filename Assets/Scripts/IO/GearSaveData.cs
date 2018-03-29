
public class GearSaveData
{
    public string Slot;
    public string Prefab;
    public ItemData Data;

    public GearSaveData()
    {

    }

    public GearSaveData(BodyGear gearSlot)
    {
        Slot = gearSlot.Name;
        GearItem item = gearSlot.GetGearItem();
        if(item != null)
        {
            Prefab = item.Item.Prefab;
            item.Item.RequestDataUpdate();
            Data = item.Item.Data;
        }
        else
        {
            Prefab = null;
            Data = null;
        }
    }
}