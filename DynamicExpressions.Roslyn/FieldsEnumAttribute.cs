using System;
using System.Diagnostics;
using CodeGeneration.Roslyn;

namespace DynamicExpressions.Roslyn
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
    [CodeGenerationAttribute(typeof(FieldsEnumGenerator))]
    [Conditional("CodeGeneration")]
    public class FieldsEnumAttribute : Attribute
    {
    }
}