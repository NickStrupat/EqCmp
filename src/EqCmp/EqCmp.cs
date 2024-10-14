using System;
using System.Collections.Generic;

namespace NickStrupat;

public static class EqCmp
{
	public static Boolean DefaultEquals<T>(T x, T y) => EqualityComparer<T>.Default.Equals(x, y);
}

public static class EqCmp<T>
{
	public static IEqualityComparer<T> Create<TP>(Func<T, TP> prop) => new Impl<TP>(prop);
	
	private sealed class Impl<TP>(Func<T, TP> prop) : IEqualityComparer<T>
	{
		public Boolean Equals(T? x, T? y)
		{
			if (!typeof(T).IsValueType)
			{
				if (ReferenceEquals(x, y))
					return true;
				if (x is null || y is null)
					return false;
			}

			return EqualityComparer<TP>.Default.Equals(prop(x!), prop(y!));
		}
		
		public Int32 GetHashCode(T obj)
		{
			var value = prop(obj);
			if (!typeof(TP).IsValueType)
			{
				if (value == null)
					return 0;
			}
			return value!.GetHashCode();
		}
	}
}