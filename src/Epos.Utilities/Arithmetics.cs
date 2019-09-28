using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Epos.Utilities
{
    /// <summary>Encapsulates a unary operation like negating on type
    /// <typeparamref name="T" />.</summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="arg">Operand</param>
    /// <returns>Return value</returns>
    public delegate T UnaryOperation<T>(T arg);

    /// <summary>Encapsulates a binary operation like adding on the types
    /// <typeparamref name="T1" />, <typeparamref name="T2" /> and
    /// <typeparamref name="TResult" />.</summary>
    /// <typeparam name="T1">Type of left operand</typeparam>
    /// <typeparam name="T2">Type of right operand</typeparam>
    /// <typeparam name="TResult">Result type</typeparam>
    /// <param name="arg1">Left operand</param>
    /// <param name="arg2">Right operand</param>
    /// <returns>Return value</returns>
    public delegate TResult BinaryOperation<in T1, in T2, out TResult>(T1 arg1, T2 arg2);

    /// <summary>Encapsulates a rounding operation for the type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="value">Value to round</param>
    /// <param name="roundingPrecision">Rounding precision</param>
    /// <param name="mode">Rounding mode</param>
    /// <returns>Rounded value</returns>
    public delegate T RoundOperation<T>(T value, int roundingPrecision, MidpointRounding mode);

    /// <summary>Provides an abstract factory for arithmetic operations like add, subtract,
    /// multiply, divide and negate.</summary>
    public static class Arithmetics
    {
        private static readonly ConcurrentDictionary<Type, object> Ones = new ConcurrentDictionary<Type, object>();
        private static readonly ConcurrentDictionary<Type, object> MinValues = new ConcurrentDictionary<Type, object>();
        private static readonly ConcurrentDictionary<Type, object> MaxValues = new ConcurrentDictionary<Type, object>();
        private static readonly ConcurrentDictionary<Type, Delegate> RoundOperations = new ConcurrentDictionary<Type, Delegate>();

        private static readonly ConcurrentDictionary<(Type T1, Type T2, Type TResult), Delegate> AddOperations =
            new ConcurrentDictionary<(Type T1, Type T2, Type TResult), Delegate>();
        private static readonly ConcurrentDictionary<(Type T1, Type T2, Type TResult), Delegate> SubtractOperations =
            new ConcurrentDictionary<(Type T1, Type T2, Type TResult), Delegate>();
        private static readonly ConcurrentDictionary<(Type T1, Type T2, Type TResult), Delegate> MultiplyOperations =
            new ConcurrentDictionary<(Type T1, Type T2, Type TResult), Delegate>();
        private static readonly ConcurrentDictionary<(Type T1, Type T2, Type TResult), Delegate> DivideOperations =
            new ConcurrentDictionary<(Type T1, Type T2, Type TResult), Delegate>();
        private static readonly ConcurrentDictionary<Type, Delegate> NegateOperations =
            new ConcurrentDictionary<Type, Delegate>();

        /// <summary>Gets the zero value for the type <typeparamref name="T" />.</summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Zero value</returns>
        public static T GetZeroValue<T>() => default!;

        /// <summary>Gets the one value for the type <typeparamref name="T" />.</summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>One value</returns>
        public static T GetOneValue<T>() {
            Type theKey = typeof(T);

            if (!Ones.TryGetValue(theKey, out object theOne)) {
                try {
                    Func<T> theGetOneFunc = Expression.Lambda<Func<T>>(
                        Expression.MakeUnary(ExpressionType.Convert, Expression.Constant(1), theKey)
                    ).Compile();
                    theOne = theGetOneFunc()!;
                } catch (InvalidOperationException) {
                    throw new InvalidOperationException($"Cannot create 1-value for the type '{theKey.Dump()}'.");
                }
                Ones[theKey] = theOne;
            }

            return (T) theOne;
        }

        /// <summary>Gets the minimum value for the type <typeparamref name="T" />.</summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Minimum value</returns>
        /// <remarks>This method returns the value of the static <b>MinValue</b> property of
        /// the type <typeparamref name="T" />.</remarks>
        public static T GetMinValue<T>() {
            Type theKey = typeof(T);

            if (!MinValues.TryGetValue(theKey, out object theMinValue)) {
                FieldInfo theFieldInfo = theKey.GetField("MinValue", BindingFlags.Static | BindingFlags.Public);
                if (theFieldInfo != null) {
                    theMinValue = theFieldInfo.GetValue(null);
                    MinValues[theKey] = theMinValue;
                } else {
                    throw new InvalidOperationException($"Cannot create MinValue for the type '{theKey.Dump()}'.");
                }
            }

            return (T) theMinValue;
        }

        /// <summary>Gets the maximum value for the type <typeparamref name="T" />.</summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Maximum value</returns>
        /// <remarks>This method returns the value of the static <b>MaxValue</b> property of
        /// the type <typeparamref name="T" />.</remarks>
        public static T GetMaxValue<T>() {
            Type theKey = typeof(T);

            if (!MaxValues.TryGetValue(theKey, out object theMaxValue)) {
                FieldInfo theFieldInfo = theKey.GetField("MaxValue", BindingFlags.Static | BindingFlags.Public);
                if (theFieldInfo != null) {
                    theMaxValue = theFieldInfo.GetValue(null);
                    MaxValues[theKey] = theMaxValue;
                } else {
                    throw new InvalidOperationException($"Cannot create MaxValue for the type '{theKey.Dump()}'.");
                }
            }

            return (T) theMaxValue;
        }

        /// <summary>Creates the <see cref="RoundOperation{T}" /> for for the type
        /// <typeparamref name="T" />.</summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns><see cref="RoundOperation{T}" /></returns>
        /// <remarks>Works only with the following types: <see cref="double" />, <see cref="decimal" /></remarks>
        public static RoundOperation<T> CreateRoundOperation<T>() {
            Type theType = typeof(T);

            if (!RoundOperations.TryGetValue(theType, out Delegate theDelegate)) {
                bool isNullableType = theType.IsGenericType && theType.GetGenericTypeDefinition() == typeof(Nullable<>);
                Type theTypeToRound = isNullableType ? theType.GetGenericArguments().Single() : theType;

                ParameterExpression theTypeToRoundArgument = Expression.Parameter(theTypeToRound);
                ParameterExpression theInt32Argument = Expression.Parameter(typeof(int));
                ParameterExpression theMidpointRoundingArgument = Expression.Parameter(typeof(MidpointRounding));

                MethodInfo theRoundMethodInfo = typeof(Math).GetMethod(
                    "Round", new[] { theTypeToRound, typeof(int), typeof(MidpointRounding) }
                );
                MethodCallExpression theCallRoundExpression = Expression.Call(
                    theRoundMethodInfo, theTypeToRoundArgument, theInt32Argument, theMidpointRoundingArgument
                );

                if (!isNullableType) {
                    theDelegate = Expression.Lambda<RoundOperation<T>>(
                        theCallRoundExpression, theTypeToRoundArgument, theInt32Argument, theMidpointRoundingArgument
                    ).Compile();
                } else {
                    ParameterExpression theTypeArgument = Expression.Parameter(typeof(T));

                    MethodInfo theGetHasValueMethodInfo = theType.GetMethod("get_HasValue");
                    MethodCallExpression theCallGetHasValueExpression = 
                        Expression.Call(theTypeArgument, theGetHasValueMethodInfo);

                    MethodInfo theGetValueMethodInfo = theType.GetMethod("get_Value");
                    MethodCallExpression theCallGetValueExpression = 
                        Expression.Call(theTypeArgument, theGetValueMethodInfo);

                    theCallRoundExpression = Expression.Call(
                        theRoundMethodInfo, theCallGetValueExpression, theInt32Argument, theMidpointRoundingArgument
                    );

                    ConstructorInfo theConstructorInfo = theType.GetConstructor(new[] { theTypeToRound });
                    NewExpression theNewNullableExpression = 
                        Expression.New(theConstructorInfo, theCallRoundExpression);

                    theDelegate = Expression.Lambda<RoundOperation<T>>(
                        Expression.Condition(theCallGetHasValueExpression, theNewNullableExpression, theTypeArgument),
                        theTypeArgument, theInt32Argument, theMidpointRoundingArgument
                    ).Compile();
                }

                RoundOperations[theType] = theDelegate;
            }

            return (RoundOperation<T>) theDelegate;
        }

        /// <summary>Creates the negate operation for for the type
        /// <typeparamref name="T" />.</summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Negate operation</returns>
        public static UnaryOperation<T> CreateNegateOperation<T>() {
            Type theKey = typeof(T);

            if (!NegateOperations.TryGetValue(theKey, out Delegate theDelegate)) {
                ParameterExpression theArg = Expression.Parameter(typeof(T));
                theDelegate = Expression.Lambda<UnaryOperation<T>>(
                    Expression.MakeUnary(ExpressionType.Negate, theArg, null), theArg
                ).Compile();
                NegateOperations[theKey] = theDelegate;
            }

            return (UnaryOperation<T>) theDelegate;
        }

        /// <summary>Creates the add operation for for the type
        /// <typeparamref name="T" />.</summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Add operation</returns>
        public static BinaryOperation<T, T, T> CreateAddOperation<T>() => CreateAddOperation<T, T, T>();

        /// <summary>Creates the add operation for the types
        /// <typeparamref name="T1" /> and <typeparamref name="T2" />.</summary>
        /// <typeparam name="T1">Type for left operand</typeparam>
        /// <typeparam name="T2">Type for right operand</typeparam>
        /// <typeparam name="TResult">Return type</typeparam>
        /// <returns>Add operation that returns a <typeparamref name="TResult" /> instance</returns>
        public static BinaryOperation<T1, T2, TResult> CreateAddOperation<T1, T2, TResult>() =>
            GetBinaryOperation<T1, T2, TResult>(ExpressionType.Add, AddOperations);

        /// <summary>Creates the subtract operation for for the type
        /// <typeparamref name="T" />.</summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Subtract operation</returns>
        public static BinaryOperation<T, T, T> CreateSubtractOperation<T>() => CreateSubtractOperation<T, T, T>();

        /// <summary>Creates the subtract operation for the types
        /// <typeparamref name="T1" /> and <typeparamref name="T2" />.</summary>
        /// <typeparam name="T1">Type for left operand</typeparam>
        /// <typeparam name="T2">Type for right operand</typeparam>
        /// <typeparam name="TResult">Return type</typeparam>
        /// <returns>Subtract operation that returns a <typeparamref name="TResult" /> instance</returns>
        public static BinaryOperation<T1, T2, TResult> CreateSubtractOperation<T1, T2, TResult>() =>
            GetBinaryOperation<T1, T2, TResult>(ExpressionType.Subtract, SubtractOperations);

        /// <summary>Creates the multiply operation for for the type
        /// <typeparamref name="T" />.</summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Multiply operation</returns>
        public static BinaryOperation<T, T, T> CreateMultiplyOperation<T>() =>
            GetBinaryOperation<T, T, T>(ExpressionType.Multiply, MultiplyOperations);

        /// <summary>Creates the divide operation for for the type
        /// <typeparamref name="T" />.</summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Divide operation</returns>
        public static BinaryOperation<T, T, T> CreateDivideOperation<T>() =>
            GetBinaryOperation<T, T, T>(ExpressionType.Divide, DivideOperations);

        #region Helper methods

        private static BinaryOperation<T1, T2, TResult> GetBinaryOperation<T1, T2, TResult>(
            ExpressionType expressionType,
            IDictionary<(Type T1, Type T2, Type TResult), Delegate> dictionary
        ) {
            (Type T1, Type T2, Type TResult) theKey = (T1: typeof(T1), T2: typeof(T2), TResult: typeof(TResult));

            if (!dictionary.TryGetValue(theKey, out Delegate theDelegate)) {
                ParameterExpression theArg1 = Expression.Parameter(typeof(T1));
                ParameterExpression theArg2 = Expression.Parameter(typeof(T2));

                theDelegate = Expression.Lambda<BinaryOperation<T1, T2, TResult>>(
                    Expression.MakeBinary(expressionType, theArg1, theArg2), theArg1, theArg2
                ).Compile();

                dictionary[theKey] = theDelegate;
            }

            return (BinaryOperation<T1, T2, TResult>) theDelegate;
        }

        #endregion
    }
}
