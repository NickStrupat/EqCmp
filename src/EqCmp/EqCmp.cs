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
			return EqualityComparer<TP>.Default.Equals(p1, p2);
		},
		x => prop(x)?.GetHashCode() ?? 0
	);
}