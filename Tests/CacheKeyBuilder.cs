using Bogus;
using FWTL.Infrastructure.Cache;
using Machine.Specifications;
using Shouldly;
using System;
using System.Collections.Generic;

namespace Tests.CacheKeyBuilderTests
{
    public enum TestEnum
    {
        Test1 = 1,
        Test2 = 2
    }

    public class TestClass
    {
        public string TestString { get; set; }
        public int TestInt { get; set; }
        public long TestLong { get; set; }
        public float TestFloat { get; set; }
        public DateTime TestDate { get; set; }
        public TestEnum Enum { get; set; }
        public TestClass TestClassProperty { get; set; }
        public List<string> TestList { get; set; } = new List<string>();
    }

    public static class TestClassFixture
    {
        private static Faker<TestClass> _fake;

        static TestClassFixture()
        {
            _fake = new Faker<TestClass>()
            .RuleFor(x => x.Enum, f => f.PickRandom<TestEnum>())
            .RuleFor(x => x.TestDate, f => f.Date.Past(1))
            .RuleFor(x => x.TestFloat, f => f.Random.Float(0, 2))
            .RuleFor(x => x.TestInt, f => f.Random.Int(-100, 1000))
            .RuleFor(x => x.TestString, f => f.Lorem.Sentence(3))
            .RuleFor(x => x.TestLong, f => f.Random.Long(-111, 111111));
        }

        public static TestClass Build()
        {
            var obj = _fake.Generate();
            obj.TestClassProperty = _fake.Generate();
            return obj;
        }
    }

    [Subject("CacheKeyBuilder")]
    public class CacheKeyBuilder_For_Default_TestClass_Should
    {
        private static TestClass Subject;
        private static Exception Exception;

        private Establish context = () => Subject = new TestClass();

        private Because of = () => Exception = Catch.Exception(() => CacheKeyBuilder.Build<TestClass, TestClass>(Subject));

        private It should_throw_error_with_complex_classes = () =>
        {
            Exception.GetType().ShouldBe(typeof(NotImplementedException));
        };
    }

    [Subject("CacheKeyBuilder")]
    public class CacheKeyBuilder_For_TestClass
    {
        private static TestClass Subject;
        private static string TestResult;

        private Establish context = () => Subject = TestClassFixture.Build();

        private Because of = () =>
        {
            TestResult = CacheKeyBuilder.Build<TestClass, TestClass>(Subject,
                x => x.TestString,
                x => x.TestFloat,
                x => x.TestDate,
                x => x.TestClassProperty.Enum);
        };

        private It should_starts_with_key_name = () =>
        {
            TestResult.ShouldStartWith(nameof(TestClass));
        };

        private It should_be_equal_to = () =>
        {
            TestResult.ShouldBe($"TestClass.{Subject.TestString}.{Subject.TestFloat}.{Subject.TestDate.Ticks}.{Subject.TestClassProperty.Enum}");
        };
    }

    [Subject("CacheKeyBuilder")]
    public class CacheKeyBuilder_For_TestClass_With_Null_Complex_Property
    {
        private static TestClass Subject;
        private static string TestResult;

        private Establish context = () =>
        {
            Subject = TestClassFixture.Build();
            Subject.TestClassProperty = null;
        };

        private Because of = () =>
        {
            TestResult = CacheKeyBuilder.Build<TestClass, TestClass>(Subject,
                x => x.TestString,
                x => x.TestFloat,
                x => x.TestDate,
                x => x.TestClassProperty.Enum);
        };

        private It should_be_equal_to = () =>
        {
            TestResult.ShouldBe($"TestClass.{Subject.TestString}.{Subject.TestFloat}.{Subject.TestDate.Ticks}.null");
        };
    }

    [Subject("CacheKeyBuilder")]
    public class CacheKeyBuilder_For_TestClass_With_Null_Property
    {
        private static TestClass Subject;
        private static string TestResult;

        private Establish context = () =>
        {
            Subject = TestClassFixture.Build();
            Subject.TestString = null;
        };

        private Because of = () =>
        {
            TestResult = CacheKeyBuilder.Build<TestClass, TestClass>(Subject,
                x => x.TestString,
                x => x.TestFloat,
                x => x.TestDate,
                x => x.TestClassProperty.Enum);
        };

        private It should_be_equal_to = () =>
        {
            TestResult.ShouldBe($"TestClass.null.{Subject.TestFloat}.{Subject.TestDate.Ticks}.{Subject.TestClassProperty.Enum}");
        };
    }
}