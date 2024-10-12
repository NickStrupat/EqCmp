using System;
using System.Collections.Generic;

namespace NickStrupat;

public static class EqCmp<T>
{
	public static IEqualityComparer<T> Create<TP>(Func<T, TP> prop) => EqualityComparer<T>.Create(
		(x, y) =>
		{
			if (!typeof(T).IsValueType)
			{
				if (ReferenceEquals(x, y))
					return true;
				if (x is null || y is null)
					return false;
			}
			var p1 = prop(x!);
			var p2 = prop(y!);
			if (!typeof(TP).IsValueType)
			{
				if (ReferenceEquals(p1, p2))
					return true;
				if (p1 is null || p2 is null)
					return false;
			}
			if ((p1, p2) is (IEquatable<TP> eq1, var other1))
				return eq1.Equals(other1);
			if ((p1, p2) is (var other2, IEquatable<TP> eq2))
				return eq2.Equals(other2);
			return x!.Equals(y);
		},
		x => prop(x)?.GetHashCode() ?? 0
	);
}