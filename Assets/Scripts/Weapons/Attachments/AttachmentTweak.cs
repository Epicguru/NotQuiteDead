using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Attachment))]
public abstract class AttachmentTweak : NetworkBehaviour
{
    public abstract void Apply(Attachment a);
    public abstract void Remove(Attachment a);
    public abstract string GetEffects();
}
