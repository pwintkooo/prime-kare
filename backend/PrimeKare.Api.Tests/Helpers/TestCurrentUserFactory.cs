using Moq;
using PrimeKare.Api.Services;

namespace PrimeKare.Api.Tests.Helpers;

public static class TestCurrentUserFactory
{
    public static Mock<ICurrentUserService> CreateCustomer(
        int customerId,
        int userId = 1)
    {
        var mock = new Mock<ICurrentUserService>();

        mock.Setup(x => x.UserId)
            .Returns(userId);

        mock.Setup(x => x.CustomerId)
            .Returns(customerId);

        mock.Setup(x => x.IsCustomer)
            .Returns(true);

        return mock;
    }

    public static Mock<ICurrentUserService> CreateAdmin(
    int userId = 1)
    {
        var mock = new Mock<ICurrentUserService>();

        mock.Setup(x => x.UserId)
            .Returns(userId);

        mock.Setup(x => x.IsAdmin)
            .Returns(true);

        mock.Setup(x => x.IsCustomer)
            .Returns(false);

        return mock;
    }

    public static Mock<ICurrentUserService>
    CreateCustomerWithoutCustomerId(
        int userId = 1)
    {
        var mock = new Mock<ICurrentUserService>();

        mock.Setup(x => x.UserId)
            .Returns(userId);

        mock.Setup(x => x.CustomerId)
            .Returns((int?)null);

        mock.Setup(x => x.IsCustomer)
            .Returns(true);

        mock.Setup(x => x.IsAdmin)
            .Returns(false);

        return mock;
    }
}