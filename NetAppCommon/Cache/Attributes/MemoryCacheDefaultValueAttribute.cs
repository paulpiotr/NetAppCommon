#region using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using log4net;

#endregion

namespace NetAppCommon.Cache.Attributes;

/// <summary>
///     Rozszerzenia programowania zorientowanego na aspekty - Atrybut wartości domyślnej pamięci podręcznej
///     Aspect Oriented Programming Extensions Memory Cache Default Value Attribute
/// </summary>
public static class MemoryCacheDefaultValueAttribute
{
    /// <summary>
    ///     Dictionary to hold type initialization methods' cache
    /// </summary>
    private static readonly Dictionary<Type, Action<object>> TypesInitializers = new();

    #region private readonly log4net.ILog log4net

    /// <summary>
    ///     Instancja do klasy Log4netLogger
    ///     Instance to Log4netLogger class
    /// </summary>
    private static readonly ILog _log4Net =
        Log4NetLogger.Log4NetLogger.GetLog4NetInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

    #endregion

    /// <summary>
    ///     Implements precompiled setters with embedded constant values from DefaultValueAttributes
    /// </summary>
    public static void ApplyDefaultValues(this object @this)
    {
        try
        {
            // Attempt to get it from cache
            if (!TypesInitializers.TryGetValue(@this.GetType(), out Action<object> setter))
            {
                // If no initializers are added do nothing
                setter = o => { };
                // Iterate through each property
                ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "this");
                foreach (PropertyInfo prop in @this.GetType()
                             .GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (prop.CanWrite &&
                        prop.GetCustomAttributes(typeof(DefaultValueAttribute), false) is DefaultValueAttribute[]
                            attr &&
                        attr.Length > 0)
                    {
                        Expression expressionConvert =
                            Expression.Convert(Expression.Constant(attr[0].Value), prop.PropertyType);
                        Expression expressionCall =
                            Expression.Call(Expression.TypeAs(parameterExpression, @this.GetType()),
                                prop.GetSetMethod(), expressionConvert);
                        var expressionLambda =
                            Expression.Lambda<Action<object>>(expressionCall, parameterExpression);
                        // Add this action to multicast delegate
                        setter += expressionLambda.Compile();
                    }
                }

                if (true)
                {
                    try
                    {
                        // Save in the type cache
                        TypesInitializers.Add(@this.GetType(), setter);
                        setter(@this);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    /// <summary>
    ///     Implements cache of ResetValue delegates
    /// </summary>
    public static void ResetDefaultValues(this object @this)
    {
        try
        {
            // Attempt to get it from cache
            if (!TypesInitializers.TryGetValue(@this.GetType(), out Action<object> setter))
            {
                // Init delegate with empty body,
                // If no initializers are added do nothing
                setter = o => { };
                // Go through each property and compile Reset delegates
                foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(@this))
                {
                    // Add only these which values can be reset
                    if (prop.CanResetValue(@this))
                    {
                        setter += prop.ResetValue;
                    }
                }

                // Save in the type cache
                TypesInitializers.Add(@this.GetType(), setter);
            }

            // Initialize member properties
            setter(@this);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
