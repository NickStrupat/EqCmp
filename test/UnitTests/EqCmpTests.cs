using FluentAssertions;
using NickStrupat;

namespace UnitTests;


public class EqCmpTests
{
    [TestCase("Test", "Test", true)]
    [TestCase("Test", "Other", false)]
    public void Create_WithOneMatchingProperty_ShouldReturnMatch(String name1, String name2, Boolean shouldMatch)
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
    public void Create_WithTwoMatchingProperties_ShouldReturnMatch(String name1, Int32 age1, String name2, Int32 age2, Boolean shouldMatch)
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
    public void Create_WithThreeMatchingProperties_ShouldReturnMatch(String fn1, String ln1, DateTime b1, String fn2, String ln2, DateTime b2, Boolean shouldMatch)
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
    public void Create_WithNull_ShouldReturnTrue()
    {
        // Arrange
        var eqCmp = EqCmp<Foo1>.Create(x => x.Name);

        // Act
        var result = eqCmp.Equals(null, null);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Create_WithNullAndNonNull_ShouldReturnFalse()
    {
        // Arrange
        var eqCmp = EqCmp<Foo1>.Create(x => x.Name);

        // Act
        var result = eqCmp.Equals(new("Test"), null);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Create_WithNonNullAndNull_ShouldReturnFalse()
    {
        // Arrange
        var eqCmp = EqCmp<Foo1>.Create(x => x.Name);

        // Act
        var result = eqCmp.Equals(null, new("Test"));

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Create_WithSameInstance_ShouldReturnTrue()
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
    public void Create_WithDifferentInstances_ShouldReturnFalse()
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
    public void Create_WithValueTypes_ShouldReturnTrue(Int32 a, Int32 b, Boolean shouldMatch)
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
    public void Create_WithNullableValueTypes_ShouldReturnTrue(Int32? a, Int32? b, Boolean shouldMatch)
    {
        // Arrange
        var eqCmp = EqCmp<Int32?>.Create(x => x);

        // Act
        var result = eqCmp.Equals(a, b);

        // Assert
        result.Should().Be(shouldMatch);
    }

    [Test]
    public void Create_WithIEquatable_ShouldReturnTrue()
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

    sealed record Foo1(String Name);
    sealed record Foo2(String Name, Int32 AgeInYears);
    sealed record Foo3(String FirstName, String LastName, DateOnly BirthDate);

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
