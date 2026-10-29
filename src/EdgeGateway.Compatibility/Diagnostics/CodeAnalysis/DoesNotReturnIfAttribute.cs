#if NETSTANDARD2_0

namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
internal sealed class DoesNotReturnIfAttribute(bool parameterValue) : Attribute
{
    public bool ParameterValue { get; } = parameterValue;
}

#endif