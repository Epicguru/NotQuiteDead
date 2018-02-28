
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using UnityEngine;

public class UnityContractResolver : DefaultContractResolver
{
    public static readonly UnityContractResolver Instance = new UnityContractResolver();


    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);

        // Vector 2, 3, and 4
        if (property.DeclaringType == typeof(Vector2) || property.DeclaringType == typeof(Vector3) || property.DeclaringType == typeof(Vector4))
        {
            if(property.PropertyName == "normalized" || property.PropertyName == "magnitude" || property.PropertyName == "sqrMagnitude")
            {
                property.Ignored = true;
            }
        }

        // Colour
        if (property.DeclaringType == typeof(Color))
        {
            if (property.PropertyName == "linear" || property.PropertyName == "gamma" || property.PropertyName == "grayscale" || property.PropertyName == "maxColorComponent")
            {
                property.Ignored = true;
            }
        }
        return property;
    }
}