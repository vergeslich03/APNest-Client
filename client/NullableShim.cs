// MelonLoader.dll, Il2CppInterop.Runtime.dll, and UnityEngine.CoreModule.dll each embed
// their own private copy of these attributes (normal for netstandard2.0-era builds).
// With <Nullable>disable</Nullable> the compiler won't embed its own, and picking between
// those three referenced copies to read nullable metadata off .NET 6 BCL APIs (e.g.
// Task.ContinueWith) fails with CS0656. Defining them here ourselves gives the compiler
// one unambiguous, fully accessible copy to use instead.
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Module, AllowMultiple = false, Inherited = false)]
    internal sealed class NullableContextAttribute : Attribute
    {
        public readonly byte Flag;
        public NullableContextAttribute(byte flag) => Flag = flag;
    }

    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.GenericParameter |
        AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue,
        AllowMultiple = false, Inherited = false)]
    internal sealed class NullableAttribute : Attribute
    {
        public readonly byte[] NullableFlags;
        public NullableAttribute(byte flag) => NullableFlags = new byte[] { flag };
        public NullableAttribute(byte[] flags) => NullableFlags = flags;
    }
}
