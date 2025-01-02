using FluentAssertions;
using NickStrupat;

namespace UnitTests;


public class EqCmpTests
{
    [Test]
    public void Create_WhenPassedNull_ShouldThrowArgumentNullException()
    {
        // Act
        var action = () => EqCmp<Foo1>.Create<Object>(null!);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }
    
    [TestCase("Test", "Test", true)]
    [TestCase("Test", "Other", false)]
    public void Equals_WhenCreateWithOneMatchingProperty_ShouldReturnMatch(String name1, String name2, Boolean shouldMatch)
    {
        // Arrange
        var eqCmp = EqCmp<Foo1>.Create(x => x.Name);

        // Act
        var result = eqCmp.Equals(
            new(name1),
            new(name2)
        );

        // Assert
        result.Should().Be(shouldMatch);
    }

    [TestCase("Test", 1, "Test", 1, true)]
    [TestCase("Test", 1, "Other", 1, false)]
    [TestCase("Test", 1, "Test", 2, false)]
    [TestCase("Test", 1, "Other", 2, false)]
    public void Equals_WhenCreateWithTwoMatchingProperties_ShouldReturnMatch(String name1, Int32 age1, String name2, Int32 age2, Boolean shouldMatch)
    {
        // Arrange
        var eqCmp = EqCmp<Foo2>.Create(x => (x.Name, x.AgeInYears));

        // Act
        var result = eqCmp.Equals(
            new(name1, age1),
            new(name2, age2)
        );

        // Assert
        result.Should().Be(shouldMatch);
    }

    [TestCase("Test", "Test", "1970-5-21", "Test", "Test", "1970-5-21", true)]
    [TestCase("Test", "Test", "1970-5-21", "Other", "Test", "1970-5-22", false)]
    [TestCase("Test", "Test", "1970-5-21", "Test", "Other", "1970-5-22", false)]
    [TestCase("Test", "Test", "1970-5-21", "Test", "Test", "1970-5-22", false)]
    public void Equals_WhenCreateWithThreeMatchingProperties_ShouldReturnMatch(String fn1, String ln1, DateTime b1, String fn2, String ln2, DateTime b2, Boolean shouldMatch)
    {
        // Arrange
        var eqCmp = EqCmp<Foo3>.Create(x => (x.FirstName, x.LastName, x.BirthDate));

        // Act
        var result = eqCmp.Equals(
            new(fn1, ln1, DateOnly.FromDateTime(b1)),
            new(fn2, ln2, DateOnly.FromDateTime(b2))
        );

        // Assert
        result.Should().Be(shouldMatch);
    }

    [Test]
    public void Equals_WhenCreateWithNull_ShouldReturnTrue()
    {
        // Arrange
        var eqCmp = EqCmp<Foo1>.Create(x => x.Name);

        // Act
        var result = eqCmp.Equals(null, null);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Equals_WhenCreateWithNullAndNonNull_ShouldReturnFalse()
    {
        // Arrange
        var eqCmp = EqCmp<Foo1>.Create(x => x.Name);

        // Act
        var result = eqCmp.Equals(new("Test"), null);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Equals_WhenCreateWithNonNullAndNull_ShouldReturnFalse()
    {
        // Arrange
        var eqCmp = EqCmp<Foo1>.Create(x => x.Name);

        // Act
        var result = eqCmp.Equals(null, new("Test"));

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Equals_WhenCreateWithSameInstance_ShouldReturnTrue()
    {
        // Arrange
        var eqCmp = EqCmp<Object>.Create(x => x);
        var o = new Object();

        // Act
        var result = eqCmp.Equals(o, o);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Equals_WhenCreateWithDifferentInstances_ShouldReturnFalse()
    {
        // Arrange
        var eqCmp = EqCmp<Object>.Create(x => x);
        var o1 = new Object();
        var o2 = new Object();

        // Act
        var result = eqCmp.Equals(o1, o2);

        // Assert
        result.Should().BeFalse();
    }
    
    [TestCase(1, 1, true)]
    [TestCase(1, 2, false)]
    public void Equals_WhenCreateWithValueTypes_ShouldReturnTrue(Int32 a, Int32 b, Boolean shouldMatch)
    {
        // Arrange
        var eqCmp = EqCmp<Int32?>.Create(x => x);

        // Act
        var result = eqCmp.Equals(a, b);

        // Assert
        result.Should().Be(shouldMatch);
    }

    [TestCase(1, 1, true)]
    [TestCase(1, 2, false)]
    [TestCase(null, null, true)]
    [TestCase(1, null, false)]
    [TestCase(null, 1, false)]
    public void Equals_WhenCreateWithNullableValueTypes_ShouldReturnTrue(Int32? a, Int32? b, Boolean shouldMatch)
    {
        // Arrange
        var eqCmp = EqCmp<Int32?>.Create(x => x);

        // Act
        var result = eqCmp.Equals(a, b);

        // Assert
        result.Should().Be(shouldMatch);
    }

    [Test]
    public void Equals_WhenCreateWithIEquatable_ShouldReturnTrue()
    {
        // Arrange
        var eqCmp = EqCmp<FooEquatable>.Create(x => x.Birth);

        var fooEquatable = new FooEquatable(new BirthdayEquatable(new DateTime(1970, 5, 21, 1, 1, 1)));
        var fooEquatable2 = new FooEquatable(new BirthdayEquatable(new DateTime(1970, 5, 21, 2, 2, 2)));

        // Act
        var result = eqCmp.Equals(fooEquatable, fooEquatable2);

        // Assert
        result.Should().BeTrue();
        fooEquatable.Birth.HasEquatableEqualsBeenCalled.Should().BeTrue();
    }
    
    [Test]
    public void GetHashCode_WhenPassedWithNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var eqCmp = EqCmp<Foo1>.Create(x => x.Name);

        // Act
        var action = () => eqCmp.GetHashCode(null!);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
    public void GetHashCode_WhenEqCmpOfValueType_ShouldReturnZero()
    {
        // Arrange
        var eqCmp = EqCmp<Foo4>.Create(x => x.Name);

        // Act
        var result = eqCmp.GetHashCode(default);

        // Assert
        result.Should().Be(0);
    }

    sealed record Foo1(String Name);
    sealed record Foo2(String Name, Int32 AgeInYears);
    sealed record Foo3(String FirstName, String LastName, DateOnly BirthDate);

    record struct Foo4(String Name);

    sealed class BirthdayEquatable(DateTime birth) : IEquatable<BirthdayEquatable>
    {
        public Boolean HasEquatableEqualsBeenCalled { get; private set; }
        public DateTime Birth { get; } = birth;
        public Boolean Equals(BirthdayEquatable? other)
        {
            HasEquatableEqualsBeenCalled = true;
            return other is not null && Birth.Date == other.Birth.Date;
        }

        // these shouldn't be called because we're using IEquatable
        public override Int32 GetHashCode() => throw new InvalidOperationException(); //HashCode.Combine(Birth.Date);
        public override Boolean Equals(Object? obj) => throw new InvalidOperationException(); //obj is BirthdayEquatable other && Equals(other);
    }

    sealed record FooEquatable(BirthdayEquatable Birth);
}
