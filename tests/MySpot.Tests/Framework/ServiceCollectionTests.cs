using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace MySpot.Tests.Framework;

public class ServiceCollectionTests
{
    [Fact]
    public void test()
    {
        var services = new ServiceCollection();
        services.AddTransient<IMessanger, Messanger>();
        services.AddTransient<IMessanger, Messanger2>();
        var serviceProvider = services.BuildServiceProvider();

        var messangers = serviceProvider.GetServices<IMessanger>();

        messangers.ShouldNotBeNull();
    }

    private interface IMessanger
    {
        void Send();
    }

    private class Messanger : IMessanger
    {
        private readonly Guid _id = Guid.NewGuid();

        public void Send() => Console.WriteLine($"Sending a message... [{_id}]");
    }

    private class Messanger2 : IMessanger
    {
        private readonly Guid _id = Guid.NewGuid();

        public void Send() => Console.WriteLine($"Sending a message2... [{_id}]");
    }
}
