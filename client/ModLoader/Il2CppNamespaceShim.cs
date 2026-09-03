// Melon and BepInEx configure Il2CppInterop's namespace prefixing differently.
//
// MelonLoader prefixes every generated namespace, so game types land in Il2Cpp.*,
// Il2CppTMPro.*, Il2CppSleepyNodes.* and Il2CppLocalisation.*. BepInEx keeps
// Il2CppInterop's defaults, whose NamespacesAndAssembliesToNotPrefix is
// {Assembly-CSharp, Unity} — so the same types land in the *global* namespace and
// in TMPro.*/SleepyNodes.*/Localisation.*
//
// Il2CppSystem.*, Il2Cppmscorlib and Il2CppInterop.Runtime are prefixed identically
// by both loaders

#if BEPINEX
global using TMPro;
global using SleepyNodes;
global using Localisation;
#else
global using Il2Cpp;
global using Il2CppTMPro;
global using Il2CppSleepyNodes;
global using Il2CppLocalisation;
#endif
