using System;
using System.Collections.Generic;
using System.Linq;
using Aspect.Abstractions;

namespace Aspect.Policies.Tests.CompilerServices
{
    public class TestResource : IResource
    {
        public string Name { get; set; } = null!;
        public string Type { get; set; } = nameof(TestResource);
        public string Region { get; } = "eu-west-1";
        public IReadOnlyList<KeyValuePair<string, string>> Tags { get; set; } = null!;


        public Int16 Int16 { get; set; } = Int16.MaxValue;
        public Int32 Int32 { get; set; } = Int32.MaxValue;
        public Int64 Int64 { get; set; } = Int64.MaxValue;
        public Decimal Decimal { get; set; } = Decimal.MaxValue;
        public NestedResource Nested { get; } = new NestedResource();


        public IEnumerable<int> Enumerable { get; set; } = System.Linq.Enumerable.Empty<int>();
        public List<int> List  { get; set; }= System.Linq.Enumerable.Empty<int>().ToList();
        public int[] Array  { get; set; }= System.Linq.Enumerable.Empty<int>().ToArray();

        public List<string> ListOfStrings { get; set; } = new()
        {
            "Hello World",
            "Hello Stu",
            "Hello Ash"
        };

        public class NestedResource
        {
            public string Name { get; } = "Nested";
            public List<SubResource> List { get; } = new()
            {
                new SubResource { Values = new() { 4, 5, 6, 10} },
                new SubResource { Values = new () { 1, 2, 3, 10} },
                new SubResource { Values = new () { 10, 11, 12 } },
            };

            public class SubResource
            {
                public List<int> Values { get; init; } = new();
            }
        }
    }
}
