using UnityEngine;

public class BlueprintTest : MonoBehaviour
{
    [TextArea(20, 20)]
    public string In;

    public void Awake()
    {
        BlueprintLoader.GetBlueprint(In);
    }

    public void Update()
    {
        if (Input.anyKeyDown)
        {
            Blueprint b = BlueprintLoader.GetBlueprint(In);
            Debug.Log("You get:");
            for (int i = 0; i < b.Products.Length; i++)
            {
                Debug.Log("   -" + b.Products[i].Name + " x" + b.Quantities[i]);
            }
            Debug.Log("Using:");
            for (int i = 0; i < b.Requirements.Length; i++)
            {
                Debug.Log("   -" + b.Requirements[i].Name + " x" + b.RequirementQuantities[i]);
            }
        }
    }
}