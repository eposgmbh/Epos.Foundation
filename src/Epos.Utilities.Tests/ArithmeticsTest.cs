using System;

using NUnit.Framework;

namespace Epos.Utilities;

[TestFixture]
public class ArithmeticsTest
{
    private const double DoubleTolerance = 0.0000000001;
    private const float FloatTolerance = 0.000001f;

    [Test]
    public void InvalidCreation() {
        Assert.Throws<InvalidOperationException>(() => Arithmetics.CreateAddOperation<ArithmeticsTest>());
    }

    [Test]
    public void DecimalArithmetics() {
        var theAdd = Arithmetics.CreateAddOperation<decimal>();
        var theSubtract = Arithmetics.CreateSubtractOperation<decimal>();
        var theMultiply = Arithmetics.CreateMultiplyOperation<decimal>();
        var theDivide = Arithmetics.CreateDivideOperation<decimal>();
        var theNegate = Arithmetics.CreateNegateOperation<decimal>();
        var theRound = Arithmetics.CreateRoundOperation<decimal>();

        Assert.That(Arithmetics.GetZeroValue<decimal>(), Is.EqualTo(0.0m));
        Assert.That(Arithmetics.GetOneValue<decimal>(), Is.EqualTo(1.0m));
        Assert.That(Arithmetics.GetMinValue<decimal>(), Is.EqualTo(decimal.MinValue));
        Assert.That(Arithmetics.GetMaxValue<decimal>(), Is.EqualTo(decimal.MaxValue));
        Assert.That(theAdd(0.3m, 0.8m), Is.EqualTo(1.1m));
        Assert.That(theAdd(9.378m, 1.765m), Is.EqualTo(11.143m));
        Assert.That(theSubtract(11.143m, 9.378m), Is.EqualTo(1.765m));
        Assert.That(theSubtract(1.1m, 0.8m), Is.EqualTo(0.3m));
        Assert.That(theMultiply(5.0m, 0.25m), Is.EqualTo(1.25m));
        Assert.That(theMultiply(3.33m, 3.0m), Is.EqualTo(9.99m));
        Assert.That(theDivide(9.99m, 3.0m), Is.EqualTo(3.33m));
        Assert.That(theDivide(1.25m, 0.25m), Is.EqualTo(5.0m));
        Assert.That(theNegate(3.456m), Is.EqualTo(-3.456m));
        Assert.That(theNegate(-0.08m), Is.EqualTo(0.08m));
        Assert.That(theRound(2.3456m, 2, MidpointRounding.AwayFromZero), Is.EqualTo(2.35m));

        decimal? theNullableDecimal = 9.876m;
        var theNullableRound = Arithmetics.CreateRoundOperation<decimal?>();
        Assert.That(theNullableRound(theNullableDecimal, 2, MidpointRounding.AwayFromZero), Is.EqualTo(9.88m));
        Assert.That(theNullableRound(null, 2, MidpointRounding.AwayFromZero), Is.EqualTo(null));
    }

    [Test]
    public void DoubleArithmetics() {
        var theAdd = Arithmetics.CreateAddOperation<double>();
        var theSubtract = Arithmetics.CreateSubtractOperation<double>();
        var theMultiply = Arithmetics.CreateMultiplyOperation<double>();
        var theDivide = Arithmetics.CreateDivideOperation<double>();
        var theNegate = Arithmetics.CreateNegateOperation<double>();
        var theRound = Arithmetics.CreateRoundOperation<double>();

        Assert.That(Arithmetics.GetZeroValue<double>(), Is.EqualTo(0.0));
        Assert.That(Arithmetics.GetOneValue<double>(), Is.EqualTo(1.0));
        Assert.That(theAdd(0.3, 0.8), Is.EqualTo(1.1).Within(DoubleTolerance));
        Assert.That(theAdd(9.378, 1.765), Is.EqualTo(11.143).Within(DoubleTolerance));
        Assert.That(theSubtract(11.143, 9.378), Is.EqualTo(1.765).Within(DoubleTolerance));
        Assert.That(theSubtract(1.1, 0.8), Is.EqualTo(0.3).Within(DoubleTolerance));
        Assert.That(theMultiply(5.0, 0.25), Is.EqualTo(1.25).Within(DoubleTolerance));
        Assert.That(theMultiply(3.33, 3.0), Is.EqualTo(9.99).Within(DoubleTolerance));
        Assert.That(theDivide(9.99, 3.0), Is.EqualTo(3.33).Within(DoubleTolerance));
        Assert.That(theDivide(1.25, 0.25), Is.EqualTo(5.0).Within(DoubleTolerance));
        Assert.That(theNegate(3.456), Is.EqualTo(-3.456).Within(DoubleTolerance));
        Assert.That(theNegate(-0.08), Is.EqualTo(0.08).Within(DoubleTolerance));
        Assert.That(theRound(2.6789, 3, MidpointRounding.AwayFromZero), Is.EqualTo(2.679));

        double? theNullableDouble = 1.234;
        var theNullableRound = Arithmetics.CreateRoundOperation<double?>();
        Assert.That(theNullableRound(theNullableDouble, 2, MidpointRounding.AwayFromZero), Is.EqualTo(1.23));
        Assert.That(theNullableRound(null, 2, MidpointRounding.AwayFromZero), Is.EqualTo(null));
    }

    [Test]
    public void FloatArithmetics() {
        var theAdd = Arithmetics.CreateAddOperation<float>();
        var theSubtract = Arithmetics.CreateSubtractOperation<float>();
        var theMultiply = Arithmetics.CreateMultiplyOperation<float>();
        var theDivide = Arithmetics.CreateDivideOperation<float>();
        var theNegate = Arithmetics.CreateNegateOperation<float>();

        Assert.That(Arithmetics.GetZeroValue<float>(), Is.EqualTo(0.0f));
        Assert.That(Arithmetics.GetOneValue<float>(), Is.EqualTo(1.0f));
        Assert.That(theAdd(0.3f, 0.8f), Is.EqualTo(1.1f).Within(FloatTolerance));
        Assert.That(theAdd(9.378f, 1.765f), Is.EqualTo(11.143f).Within(FloatTolerance));
        Assert.That(theSubtract(11.143f, 9.378f), Is.EqualTo(1.765f).Within(FloatTolerance));
        Assert.That(theSubtract(1.1f, 0.8f), Is.EqualTo(0.3f).Within(FloatTolerance));
        Assert.That(theMultiply(5.0f, 0.25f), Is.EqualTo(1.25f).Within(FloatTolerance));
        Assert.That(theMultiply(3.33f, 3.0f), Is.EqualTo(9.99f).Within(FloatTolerance));
        Assert.That(theDivide(9.99f, 3.0f), Is.EqualTo(3.33f).Within(FloatTolerance));
        Assert.That(theDivide(1.25f, 0.25f), Is.EqualTo(5.0f).Within(FloatTolerance));
        Assert.That(theNegate(3.456f), Is.EqualTo(-3.456f).Within(FloatTolerance));
        Assert.That(theNegate(-0.08f), Is.EqualTo(0.08f).Within(FloatTolerance));
    }

    [Test]
    public void IntegerArithmetics() {
        var theAdd = Arithmetics.CreateAddOperation<int>();
        var theSubtract = Arithmetics.CreateSubtractOperation<int>();
        var theMultiply = Arithmetics.CreateMultiplyOperation<int>();
        var theDivide = Arithmetics.CreateDivideOperation<int>();
        var theNegate = Arithmetics.CreateNegateOperation<int>();

        Assert.That(Arithmetics.GetZeroValue<int>(), Is.EqualTo(0));
        Assert.That(Arithmetics.GetOneValue<int>(), Is.EqualTo(1));
        Assert.That(theAdd(3, 8), Is.EqualTo(11));
        Assert.That(theAdd(9378, 1765), Is.EqualTo(11143));
        Assert.That(theSubtract(11143, 9378), Is.EqualTo(1765));
        Assert.That(theSubtract(11, 8), Is.EqualTo(3));
        Assert.That(theMultiply(50, 25), Is.EqualTo(1250));
        Assert.That(theMultiply(333, 3), Is.EqualTo(999));
        Assert.That(theDivide(999, 3), Is.EqualTo(333));
        Assert.That(theDivide(125, 25), Is.EqualTo(5));
        Assert.That(theNegate(3456), Is.EqualTo(-3456));
        Assert.That(theNegate(-8), Is.EqualTo(8));
    }

    [Test]
    public void LongArithmetics() {
        var theAdd = Arithmetics.CreateAddOperation<long>();
        var theSubtract = Arithmetics.CreateSubtractOperation<long>();
        var theMultiply = Arithmetics.CreateMultiplyOperation<long>();
        var theDivide = Arithmetics.CreateDivideOperation<long>();
        var theNegate = Arithmetics.CreateNegateOperation<long>();

        Assert.That(Arithmetics.GetZeroValue<long>(), Is.EqualTo(0L));
        Assert.That(Arithmetics.GetOneValue<long>(), Is.EqualTo(1L));
        Assert.That(theAdd(3L, 8L), Is.EqualTo(11L));
        Assert.That(theAdd(9378L, 1765L), Is.EqualTo(11143L));
        Assert.That(theSubtract(11143L, 9378L), Is.EqualTo(1765L));
        Assert.That(theSubtract(11L, 8L), Is.EqualTo(3L));
        Assert.That(theMultiply(50L, 25L), Is.EqualTo(1250L));
        Assert.That(theMultiply(333L, 3L), Is.EqualTo(999L));
        Assert.That(theDivide(999L, 3L), Is.EqualTo(333L));
        Assert.That(theDivide(125L, 25L), Is.EqualTo(5L));
        Assert.That(theNegate(3456L), Is.EqualTo(-3456L));
        Assert.That(theNegate(-8L), Is.EqualTo(8L));
    }

    [Test]
    public void DateTimeArithmetics() {
        var theAddTimeSpan = Arithmetics.CreateAddOperation<DateTime, TimeSpan, DateTime>();
        var theSubtractDateTime = Arithmetics.CreateSubtractOperation<DateTime, DateTime, TimeSpan>();
        var theSubtractTimeSpan = Arithmetics.CreateSubtractOperation<DateTime, TimeSpan, DateTime>();
        Assert.Throws<InvalidOperationException>(() => Arithmetics.CreateMultiplyOperation<DateTime>());
        Assert.Throws<InvalidOperationException>(() => Arithmetics.CreateDivideOperation<DateTime>());
        Assert.Throws<InvalidOperationException>(() => Arithmetics.CreateNegateOperation<DateTime>());
        var theNegateTimeSpan = Arithmetics.CreateNegateOperation<TimeSpan>();

        Assert.That(Arithmetics.GetZeroValue<TimeSpan>(), Is.EqualTo(TimeSpan.Zero));
        Assert.That(Arithmetics.GetZeroValue<DateTime>(), Is.EqualTo(new DateTime()));

        Assert.Throws<InvalidOperationException>(() => Arithmetics.GetOneValue<DateTime>());
        Assert.Throws<InvalidOperationException>(() => Arithmetics.GetOneValue<TimeSpan>());

        Assert.That(theAddTimeSpan(new DateTime(2000, 1, 1), new TimeSpan(3, 0, 0, 0)), Is.EqualTo(new DateTime(2000, 1, 4)));
        Assert.That(theAddTimeSpan(new DateTime(2000, 3, 17), new TimeSpan(2, 2, 2)), Is.EqualTo(new DateTime(2000, 3, 17, 2, 2, 2)));
        Assert.That(theSubtractDateTime(new DateTime(2000, 12, 12), new DateTime(2000, 12, 1)), Is.EqualTo(new TimeSpan(11, 0, 0, 0)));
        Assert.That(theSubtractDateTime(new DateTime(2000, 3, 1), new DateTime(2000, 2, 28)), Is.EqualTo(new TimeSpan(2, 0, 0, 0)));
        Assert.That(theSubtractTimeSpan(new DateTime(2000, 12, 12), new TimeSpan(11, 0, 0, 0)), Is.EqualTo(new DateTime(2000, 12, 1)));
        Assert.That(theSubtractTimeSpan(new DateTime(2000, 3, 1), new TimeSpan(2, 0, 0, 0)), Is.EqualTo(new DateTime(2000, 2, 28)));

        Assert.That(theNegateTimeSpan(new TimeSpan(3, 3, 3, 3)), Is.EqualTo(new TimeSpan(-3, -3, -3, -3)));
        Assert.That(theNegateTimeSpan(new TimeSpan(-4, -4, -4, -4)), Is.EqualTo(new TimeSpan(4, 4, 4, 4)));
    }

    [Test]
    public void Exceptions() {
        Assert.Throws<InvalidOperationException>(() => Arithmetics.GetMinValue<string>());
        Assert.Throws<InvalidOperationException>(() => Arithmetics.GetMaxValue<string>());
    }
}
