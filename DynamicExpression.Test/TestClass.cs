using System;
using System.Linq;
using System.Linq.Expressions;
using DynamicExpression.Example;

namespace DynamicExpression.Test
{
    public class TestClass
    {
        void Test()
        {
            var xField = SampleClassField.X;

            var foos = new[] {new SampleClass()}.AsQueryable()
                .OrderBy(SampleClassExpressionHelper.FieldExpressions[xField]);
        }
    }
}
