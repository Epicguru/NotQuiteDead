
public class Blueprint
{
    public Item[] Products;
    public int[] Quantities;

    public Item[] Requirements;
    public int[] RequirementQuantities;

    public bool PlayerHasMaterials()
    {
        for (int i = 0; i < Requirements.Length; i++)
        {
            bool has = PlayerInventory.inv.Inventory.Contains(Requirements[i].Prefab, RequirementQuantities[i]);
            if (!has)
                return false;
        }
        return true;
    }
}
