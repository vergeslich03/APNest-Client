#if BEPINEX
using System;
using UnityEngine;

namespace APNestClient;

// BasePlugin has no per-frame callback the way MelonMod.OnUpdate does, so ModCore.Update
// is driven from a MonoBehaviour attached to a persistent object instead.
public class TickBehaviour : MonoBehaviour
{
    // Il2CppInterop constructs injected types from the Il2Cpp side through this pointer
    public TickBehaviour(IntPtr ptr) : base(ptr)
    {
    }

    internal static ModCore Core;

    private void Update() => Core?.Update();
}
#endif
