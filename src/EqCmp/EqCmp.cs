using System;
using System.Collections.Generic;

namespace NickStrupat;

public static class EqCmp
{
	public static Boolean DefaultEquals<T>(T x, T y) => EqualityComparer<T>.Default.Equals(x, y);
}

public static class EqCmp<T>
{
	public static IEqualityComparer<T> Create<TP>(Func<T, TP> equalitySelector)
	{
        ArgumentNullException.ThrowIfNull(equalitySelector);
        return new Impl<TP>(equalitySelector);
	}

	private sealed class Impl<TP>(Func<T, TP> selector) : IEqualityComparer<T>
	{
		Boolean IEqualityComparer<T>.Equals(T? x, T? y) => typeof(T).IsValueType switch
		{
			false when ReferenceEquals(x, y) => true,
			false when x is null || y is null => false,
			_ => EqualityComparer<TP>.Default.Equals(selector(x!), selector(y!))
		};

		Int32 IEqualityComparer<T>.GetHashCode(T obj)
		{
			if (typeof(TP).IsValueType)
				return selector(obj)!.GetHashCode();
			ArgumentNullException.ThrowIfNull(obj);
			return selector(obj) is { } value ? value.GetHashCode() : 0;
		}
	}
}

// public sealed class EqCmp<T> : IEqualityComparer<T>
// {
// 	private readonly Func<T?, T?, Boolean> eq;
// 	private readonly Func<T, Int32> ghc;
//
// 	private EqCmp(Func<T?, T?, Boolean> eq, Func<T, Int32> ghc) => (this.eq, this.ghc) = (eq, ghc);
//
// 	public Boolean Equals(T? x, T? y) => eq(x, y);
// 	public Int32 GetHashCode([DisallowNull] T obj) => ghc(obj);
//
// 	public override Boolean Equals(Object? obj) => obj is EqCmp<T> other && eq == other.eq && ghc == other.ghc;
// 	public override Int32 GetHashCode() => HashCode.Combine(eq, ghc);
//
// 	public static EqCmp<T> Create<TP>(Func<T, TP> equalitySelector)
// 	{
// 		ArgumentNullException.ThrowIfNull(equalitySelector);
// 		return new(EqualsWrapper, GetHashCodeWrapper);
//
// 		Boolean EqualsWrapper(T? x, T? y) => typeof(T).IsValueType switch
// 		{
// 			false when ReferenceEquals(x, y) => true,
// 			false when x is null || y is null => false,
// 			_ => EqualityComparer<TP>.Default.Equals(equalitySelector(x!), equalitySelector(y!))
// 		};
//
// 		Int32 GetHashCodeWrapper(T obj)
// 		{
// 			if (typeof(TP).IsValueType) return equalitySelector(obj)!.GetHashCode();
// 			ArgumentNullException.ThrowIfNull(obj);
// 			return equalitySelector(obj) is { } value ? value.GetHashCode() : 0;
// 		}
// 	}
// }