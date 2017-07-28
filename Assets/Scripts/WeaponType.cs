using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum WeaponType
{
    RIFLE,
    HANDGUN,
    SHOTGUN_MAG
}

public class WeaponTypeUtils
{
    /// <summary>
    /// Gets the string representation of each weapon type, with no capitalization.
    /// </summary>
    /// <param name="type">The weapon type.</param>
    /// <returns>The string representation of the type of weapon. Not a description, just the type. Never null.</returns>
    public static string GetTypeName(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.HANDGUN:
                return "handgun";
            case WeaponType.RIFLE:
                return "rifle";
            case WeaponType.SHOTGUN_MAG:
                return "shotgun";
            default:
                return "UNKNOWN";
        }
    }

    /// <summary>
    /// Gets a brief description of the general type of weapon. Includes capitalization.
    /// </summary>
    /// <param name="type">The type of weapon.</param>
    /// <returns>The brief description, including capitalization and punctuation. Never null.</returns>
    public static string GetTypeDescription(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.HANDGUN:
                return "Weapons that have a short range, low caliber ammunition but are genrally very accurate.";
            case WeaponType.RIFLE:
                return "Rapid firing, armour penetrating weapons that have medium range and accuracy.";
            case WeaponType.SHOTGUN_MAG:
                return "Pellet firing weapons that specialize in crowd control and stopping power. This type of shotgun is fed by a magazine.";
            default:
                return "UNKNOWN";
        }
    }
}