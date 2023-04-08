using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Webinex.Wispo.DataAccess;
using static Webinex.Wispo.NotificationField;

namespace Webinex.Wispo.Tests.Filters;

public class LogicalFilterFactoryTests : FilterFactoryTestsBase
{
    [Test]
    public void WhenOr_ShouldBeOk()
    {
        Filter = Filter.Or(
            Filter.Eq(SUBJECT, "123"),
            Filter.Eq(SUBJECT, "321"));

        Values = New("123", "321", "222");

        Run();

        Result.Should().BeEquivalentTo(new[] { Values[0], Values[1] });
    }

    [Test]
    public void WhenAnd_ShouldBeOk()
    {
        Filter = Filter.And(
            Filter.Contains(SUBJECT, "123"),
            Filter.Contains(SUBJECT, "321"));

        Values = New("123", "321", "12322321", "333");

        Run();

        Result.Should().BeEquivalentTo(new[] { Values[2] });
    }

    private NotificationRow[] New(params string[] subjects)
    {
        return subjects.Select(x => new NotificationRow { Subject = x }).ToArray();
    }
}