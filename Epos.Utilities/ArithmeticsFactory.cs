using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Epos.Utilities
{
    public static class ArithmeticsFactory
    {
        private static readonly ConcurrentDictionary<Type, Delegate> RoundOperations = new ConcurrentDictionary<Type, Delegate>();
        private static readonly ConcurrentDictionary<Tuple<Type, Type, Type>, Delegate> AddOperations = 
            new ConcurrentDictionary<Tuple<Type, Type, Type>, Delegate>();
        private static readonly ConcurrentDictionary<Tuple<Type, Type, Type>, Delegate> SubtractOperations = 
            new ConcurrentDictionary<Tuple<Type, Type, Type>, Delegate>();
        private static readonly ConcurrentDictionary<Tuple<Type, Type, Type>, Delegate> MultiplyOperations = 
            new ConcurrentDictionary<Tuple<Type, Type, Type>, Delegate>();
        private static readonly ConcurrentDictionary<Tuple<Type, Type, Type>, Delegate> DivideOperations = 
            new ConcurrentDictionary<Tuple<Type, Type, Type>, Delegate>();
        private static readonly ConcurrentDictionary<Tuple<Type, Type>, Delegate> NegateOperations = 
            new ConcurrentDictionary<Tuple<Type, Type>, Delegate>();
        private static readonly ConcurrentDictionary<Type, object> Ones = new ConcurrentDictionary<Type, object>();
        private static readonly ConcurrentDictionary<Type, object> MinValues = new ConcurrentDictionary<Type, object>();
        private static readonly ConcurrentDictionary<Type, object> MaxValues = new ConcurrentDictionary<Type, object>();

        public static T CreateZero<T>() {
            return default(T);
        }

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

        public static T CreateOne<T>() {
            Type theKey = typeof(T);

            if (!Ones.TryGetValue(theKey, out object theOne)) {
                try {
                    Func<T> theGetOneFunc = Expression.Lambda<Func<T>>(
                        Expression.MakeUnary(ExpressionType.Convert, Expression.Constant(1), theKey)
                    ).Compile();
                    theOne = theGetOneFunc();
                } catch (InvalidOperationException) {
                    throw new InvalidOperationException(
                        $"Cannot create 1-value for the type '{theKey.Dump()}'."
                        );
                }
                Ones[theKey] = theOne;
            }

            return (T) theOne;
        }

        public static T CreateMinValue<T>() {
            Type theKey = typeof(T);

            if (!MinValues.TryGetValue(theKey, out object theMinValue)) {
                FieldInfo theFieldInfo = theKey.GetField("MinValue", BindingFlags.Static | BindingFlags.Public);
                if (theFieldInfo != null) {
                    theMinValue = theFieldInfo.GetValue(null);
                    MinValues[theKey] = theMinValue;
                } else {
                    throw new InvalidOperationException(
                        $"Cannot create MinValue for the type '{theKey.Dump()}'."
                        );
                }
            }

            return (T) theMinValue;
        }

        public static T CreateMaxValue<T>() {
            Type theKey = typeof(T);

            if (!MaxValues.TryGetValue(theKey, out object theMaxValue)) {
                FieldInfo theFieldInfo = theKey.GetField("MaxValue", BindingFlags.Static | BindingFlags.Public);
                if (theFieldInfo != null) {
                    theMaxValue = theFieldInfo.GetValue(null);
                    MaxValues[theKey] = theMaxValue;
                } else {
                    throw new InvalidOperationException(
                        $"Cannot create MaxValue for the type '{theKey.Dump()}'."
                        );
                }
            }

            return (T) theMaxValue;
        }

        public static UnaryOperation<T> CreateNegateOperation<T>() {
            var theKey = Tuple.Create(typeof(T), typeof(T));

            if (!NegateOperations.TryGetValue(theKey, out Delegate theDelegate)) {
                ParameterExpression theArg = Expression.Parameter(typeof(T));
                theDelegate = Expression.Lambda<UnaryOperation<T>>(
                    Expression.MakeUnary(ExpressionType.Negate, theArg, null), theArg
                ).Compile();
                NegateOperations[theKey] = theDelegate;
            }

            return theDelegate as UnaryOperation<T>;
        }

        public static BinaryOperation<T, T, T> CreateAddOperation<T>() {
            return CreateAddOperation<T, T, T>();
        }

        public static BinaryOperation<T1, T2, TResult> CreateAddOperation<T1, T2, TResult>() {
            return GetBinaryOperation<T1, T2, TResult>(ExpressionType.Add, AddOperations);
        }

        public static BinaryOperation<T, T, T> CreateSubtractOperation<T>() {
            return CreateSubtractOperation<T, T, T>();
        }

        public static BinaryOperation<T1, T2, TResult> CreateSubtractOperation<T1, T2, TResult>() {
            return GetBinaryOperation<T1, T2, TResult>(ExpressionType.Subtract, SubtractOperations);
        }

        public static BinaryOperation<T, T, T> CreateMultiplyOperation<T>() {
            return CreateMultiplyOperation<T, T, T>();
        }

        public static BinaryOperation<T1, T2, TResult> CreateMultiplyOperation<T1, T2, TResult>() {
            return GetBinaryOperation<T1, T2, TResult>(ExpressionType.Multiply, MultiplyOperations);
        }

        public static BinaryOperation<T, T, T> CreateDivideOperation<T>() {
            return CreateDivideOperation<T, T, T>();
        }

        public static BinaryOperation<T1, T2, TResult> CreateDivideOperation<T1, T2, TResult>() {
            return GetBinaryOperation<T1, T2, TResult>(ExpressionType.Divide, DivideOperations);
        }

        private static BinaryOperation<T1, T2, TResult> GetBinaryOperation<T1, T2, TResult>(
            ExpressionType expressionType, ConcurrentDictionary<Tuple<Type, Type, Type>, Delegate> dictionary
        ) {
            var theKey = Tuple.Create(typeof(T1), typeof(T2), typeof(TResult));

            if (!dictionary.TryGetValue(theKey, out Delegate theDelegate)) {
                theDelegate = CreateDelegate<T1, T2, TResult>(expressionType);
                dictionary[theKey] = theDelegate;
            }

            return theDelegate as BinaryOperation<T1, T2, TResult>;
        }

        private static Delegate CreateDelegate<T1, T2, TResult>(ExpressionType expressionType) {
            ParameterExpression theArg1 = Expression.Parameter(typeof(T1));
            ParameterExpression theArg2 = Expression.Parameter(typeof(T2));
            return Expression.Lambda<BinaryOperation<T1, T2, TResult>>(
                Expression.MakeBinary(expressionType, theArg1, theArg2), theArg1, theArg2
            ).Compile();
        }
    }

    public delegate T UnaryOperation<T>(T arg);

    public delegate TResult BinaryOperation<in T1, in T2, out TResult>(T1 arg1, T2 arg2);

    public delegate T RoundOperation<T>(T value, int roundingPrecision, MidpointRounding mode);
}