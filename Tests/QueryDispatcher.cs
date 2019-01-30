using Autofac;
using FluentValidation;
using FluentValidation.Results;
using FWTL.Core.CQRS;
using FWTL.Infrastructure.CQRS;
using FWTL.Infrastructure.Validation;
using Machine.Specifications;
using NSubstitute;
using Shouldly;
using System;

namespace Tests.QueryDispatcherTests
{
    public class Fixture
    {
        public IComponentContext Build()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<Query>().AsImplementedInterfaces();

            var fakeValidator = Substitute.For<AppAbstractValidation<Query>>();
            var fakeResult = Substitute.For<ValidationResult>();
            fakeResult.IsValid.Returns(false);

            fakeValidator.Validate(context: null).ReturnsForAnyArgs(fakeResult);

            builder.Register(b => fakeValidator);

            return builder.Build();
        }
    }

    public class Query : IQuery
    {
    }

    [Subject("CommandDispatcher")]
    public class QueryDispatcher_With_Failed_Validation
    {
        private static QueryDispatcher Subject;
        private static Exception Exception;

        private Establish context = () =>
        {
            var context = new Fixture().Build();
            Subject = new QueryDispatcher(context);
        };

        private Because of = async () =>
        {
            try
            {
                await Subject.DispatchAsync<Query,int>(new Query()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Exception = ex;
            }
        };

        private It should_throw_exception = () =>
        {
            Exception.GetType().ShouldBe(typeof(ValidationException));
        };
    }
}