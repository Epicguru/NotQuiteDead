using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Gun))]
public class GunAttachments : NetworkBehaviour
{
    public Transform MuzzleMount;
    public Transform SightMount;
    public Transform UnderBarrelMount;
    public bool AllowMag = true;
}