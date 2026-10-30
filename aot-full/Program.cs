using System;
using osu.Framework.Bindables;
using osu.Framework.Allocation;

var source = new Bindable<int>(5);
var follower = new Bindable<int>();
follower.BindTo(source);
source.Value = 42;
Console.WriteLine($"bindable follower = {follower.Value}");

var deps = new DependencyContainer();
deps.CacheAs("hello from AOT");
Console.WriteLine("DI container created");
