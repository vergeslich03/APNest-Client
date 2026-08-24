using System;
using HarmonyLib;
using Il2Cpp;

namespace APNestClient;

// this is needed, because I spawn a new instance of the CBTimer, which means, if the game spawns one, there are two
// instances of the Timer now. So if that happens I destroy my version. and run everything through the game's.
[HarmonyPatch(typeof(Il2CppSleepyNodes.State_StartTimer), "OnEnter")]
public class CBTimerDuplicationHandler
{
    private static CounterBatteryTimer _cbTimer = null;
    
    public static bool Prefix(Il2CppSleepyNodes.State_StartTimer __instance)
    {
        try
        {
            if (CounterBatteryTimer.Instance != null)
            {
                _cbTimer = CounterBatteryTimer.Instance;
            }
        }
        catch (NullReferenceException)
        {
            _cbTimer = null;
        }
       
        
        return true;
    }

    public static void Postfix(Il2CppSleepyNodes.State_StartTimer __instance)
    {
        if (_cbTimer != CounterBatteryTimer.Instance)
        {
            UnityEngine.Object.Destroy(_cbTimer.gameObject);
        }
    }
}