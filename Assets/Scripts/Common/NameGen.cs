using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class NameGen
{
    public static string[] FirstNames = new string[]
    {
        "James",
        "Curtis",
        "Zak",
        "Erik",
        "Alvarito",
        "Josh",
        "Pablo",
        "Mike",
        "Tom",
        "Brenda",
        "Marta",
        "Edna",
        "Sofia",
        "Mary",
        "Maria",
        "Bob",
        "Dugless",
        "Sir",
        "Mr Sir",
        "The Warden",
        "Carlos",
        "Elvis",
        "George",
        "Elise",
        "Carly"
    };

    public static string[] Gangs = new string[]
    {
        "Curry Cave",
        "Daywatch",
        "Nightswatch",
        "Cool Kids",
        "Nevermore",
        "Forgotten Rimworld",
        "Dwarven Fortress",
        "Dead Islands",
        "Savage Outback",
        "Wild West",
        "Somewhere",
        "Evil Wasteland",
        "Secret Society",
        "Unspoken Lands",
        "Rich Kidz",
        "Peasants Corner",
        "Pigworts School of Mismatch and Missery",
        "Tomorrowland Tribe"
    };

    public static string GenName()
    {
        return FirstNames[Random.Range(0, FirstNames.Length)] + " of The " + Gangs[Random.Range(0, Gangs.Length)];
    }
}
