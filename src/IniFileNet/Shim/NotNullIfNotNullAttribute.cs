﻿#if NETSTANDARD2_0
namespace System.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
	internal sealed class NotNullIfNotNullAttribute : Attribute
	{
		internal NotNullIfNotNullAttribute(string parameterName) { }
	}
}
#endif