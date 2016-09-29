using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace NLinear
{
    public static class NumericConverter<T>
    {
        static Func<double, T> compiledTodoubleExpression;

        static Func<T, double> compiledFromdoubleExpression;

        static void CompileConvertTodoubleExpression()
        {
            ParameterExpression parameter1 = Expression.Parameter(typeof(double), "d");

            Expression convert = Expression.Convert(
                            parameter1,
                            typeof(T)
                        );

            compiledTodoubleExpression = Expression.Lambda<Func<double, T>>(convert, parameter1).Compile();
        }

        static void CompileConvertFromdoubleExpression()
        {
            ParameterExpression parameter1 = Expression.Parameter(typeof(T), "d");

            Expression convert = Expression.Convert(
                            parameter1,
                            typeof(double)
                        );

            compiledFromdoubleExpression = Expression.Lambda<Func<T, double>>(convert, parameter1).Compile();
        }

        static NumericConverter()
        {
            CompileConvertTodoubleExpression();

            CompileConvertFromdoubleExpression();
        }

        static public double ToFloat(T value)
        {
            return compiledFromdoubleExpression(value);
        }

        static public T Fromdouble(double d)
        {
            return compiledTodoubleExpression(d);
        }
    }
}
