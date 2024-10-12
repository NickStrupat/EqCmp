# EqCmp
A small C# library for constructing instances of `IEqualityComparer<T>` using a succinct API

[![EqCmp Nuget](https://img.shields.io/nuget/vpre/EqCmp.svg)](https://www.nuget.org/packages/EqCmp)

## Usage

```csharp
var comparer = EqCmp<Foo>.Create(x => (x.Bar, x.Baz));

var foo1 = new Foo { Bar = 1, Baz = "a" };
var foo2 = new Foo { Bar = 1, Baz = "a" };
var foo3 = new Foo { Bar = 2, Baz = "b" };

var set = new HashSet<Foo>(comparer);
set.Add(foo1);
set.Add(foo2);
set.Add(foo3);

Console.WriteLine(set.Count); // 2

class Foo
{
    public int Bar { get; set; }
    public string Baz { get; set; }
}
```