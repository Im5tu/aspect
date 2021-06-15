using System;
using System.Collections.Generic;
using System.Linq;
using Aspect.Abstractions;

namespace Aspect.Policies.Tests.CompilerServices
{
    public class TestResource : IResource
    {
        public string Name { get; set; } = null!;
        public string CloudId { get; set; } = null!;
        public string Type { get; set; } = nameof(TestResource);
        public string Region { get; } = "eu-west-1";
        public IReadOnlyList<KeyValuePair<string, string>> Tags { get; set; } = new List<KeyValuePair<string, string>>
        {
            new ("TestKey", "TestValue")
        };


        public Int16 Int16 { get; set; } = Int16.MaxValue;
        public Int32 Int32 { get; set; } = Int32.MaxValue;
        public Int64 Int64 { get; set; } = Int64.MaxValue;
        public Decimal Decimal { get; set; } = Decimal.MaxValue;
        public NestedResource Nested { get; } = new();


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
                new SubResource { Values = new List<int>() { 4, 5, 6, 10} },
                new SubResource { Values = new List<int>() { 1, 2, 3, 10} },
                new SubResource { Values = new List<int>() { 10, 11, 12 } },
            };

            public class SubResource
            {
                public List<int> Values { get; init; } = new();
            }
        }

        public bool Equals(IResource? other)
            => false;

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TestResource) obj);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Name);
            hashCode.Add(CloudId);
            hashCode.Add(Type);
            hashCode.Add(Region);
            hashCode.Add(Tags);
            hashCode.Add(Int16);
            hashCode.Add(Int32);
            hashCode.Add(Int64);
            hashCode.Add(Decimal);
            hashCode.Add(Nested);
            hashCode.Add(Enumerable);
            hashCode.Add(List);
            hashCode.Add(Array);
            hashCode.Add(ListOfStrings);
            return hashCode.ToHashCode();
        }
    }
}
