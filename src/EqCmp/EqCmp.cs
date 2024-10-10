using System;
using System.Collections.Generic;

namespace NickStrupat;

public sealed class EqCmp<T>(Func<T, T, Boolean> equals, Func<T, Int32> getHashCode) : IEqualityComparer<T> where T : notnull
{
	public Boolean Equals(T? x, T? y)
	{
		if (typeof(T).IsValueType)
			return equals(x!, y!);
		if (ReferenceEquals(x, y))
			return true;
		if (x is null || y is null)
			return false;
		return equals(x, y);
	}

	public Int32 GetHashCode(T obj) => getHashCode(obj);

	public static EqCmp<T> Create<TP>(Func<T, TP> prop) => new(
		(x, y) => Eq(prop, x, y),
		x => HashCode.Combine(prop(x))
	);

	public static EqCmp<T> Create<TP1, TP2>(Func<T, TP1> prop1, Func<T, TP2> prop2) => new(
		(x, y) => Eq(prop1, x, y) && Eq(prop2, x, y),
		x => HashCode.Combine(prop1(x), prop2(x))
	);
	
	public static EqCmp<T> Create<TP1, TP2, TP3>(Func<T, TP1> prop1, Func<T, TP2> prop2, Func<T, TP3> prop3) => new(
		(x, y) => Eq(prop1, x, y) && Eq(prop2, x, y) && Eq(prop3, x, y),
		x => HashCode.Combine(prop1(x), prop2(x), prop3(x))
	);
	
	public static EqCmp<T> Create<TP1, TP2, TP3, TP4>(Func<T, TP1> prop1, Func<T, TP2> prop2, Func<T, TP3> prop3, Func<T, TP4> prop4) => new(
		(x, y) => Eq(prop1, x, y) && Eq(prop2, x, y) && Eq(prop3, x, y) && Eq(prop4, x, y),
		x => HashCode.Combine(prop1(x), prop2(x), prop3(x), prop4(x))
	);
	
	private static Boolean Eq<TP>(Func<T, TP> prop, T x, T y) => (prop(x), prop(y)) switch
	{
		(null, null) => true,
		(null, _) or (_, null) => false,
		(IEquatable<TP> eq, var other) => eq.Equals(other),
		(var other, IEquatable<TP> eq) => eq.Equals(other),
		_ => x.Equals(y)
	};
}