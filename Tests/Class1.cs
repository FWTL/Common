using Machine.Specifications;
using Shouldly;
using System.Collections.Generic;

namespace FWTL.Common.Tests
{
    [Subject("Authentication")]
    public class PrimeService_IsPrimeShould
    {
        static List<int> Subject;

        Establish context = () => Subject = new List<int>() { 1, 2, 3 };

        Because of = () =>
        {
            Subject.Add(2);
        };

        It should_have_4_elemets = () =>
        {
            Subject.Count.ShouldBe(4);
        };
    }
}