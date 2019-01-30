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

namespace Tests.CommandDispatcherTests
{
    public class Fixture
    {
        public IComponentContext Build()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<Command>().AsImplementedInterfaces();

            var fakeValidator = Substitute.For<AppAbstractValidation<Command>>();
            var fakeResult = Substitute.For<ValidationResult>();
            fakeResult.IsValid.Returns(false);

            fakeValidator.Validate(context: null).ReturnsForAnyArgs(fakeResult);

            builder.Register(b => fakeValidator);

            return builder.Build();
        }
    }

    public class Command : ICommand
    {
    }

    [Subject("CommandDispatcher")]
    public class CommandDispatcher_With_Failed_Validation
    {
        private static CommandDispatcher Subject;
        private static Exception Exception;

        private Establish context = () =>
        {
            var context = new Fixture().Build();
            Subject = new CommandDispatcher(context);
        };

        private Because of = async () =>
        {
            try
            {
                await Subject.DispatchAsync(new Command()).ConfigureAwait(false);
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