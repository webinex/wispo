using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Webinex.Wispo.DataAccess;
using static Webinex.Wispo.NotificationField;

namespace Webinex.Wispo.Tests.Filters;

public class StringFilterFactoryTests : FilterFactoryTestsBase
{
    [Test]
    public void WhenEq_ShouldBeOk()
    {
        Filter = Filter.Eq(SUBJECT, "123");
        Values = New("123", "321");

        Run();

        Result.Should().BeEquivalentTo(new[] { Values[0] });
    }

    [Test]
    public void WhenNotEq_ShouldBeOk()
    {
        Filter = Filter.NotEq(SUBJECT, "321");
        Values = New("123", "321");

        Run();

        Result.Should().BeEquivalentTo(new[] { Values[0] });
    }

    [Test]
    public void WhenIn_ShouldBeOk()
    {
        Filter = Filter.In(SUBJECT, new[] { "123", "321" });
        Values = New("123", "321", "111");

        Run();

        Result.Should().BeEquivalentTo(new[] { Values[0], Values[1] });
    }

    [Test]
    public void WhenNotIn_ShouldBeOk()
    {
        Filter = Filter.NotIn(SUBJECT, new[] { "123", "321" });
        Values = New("123", "321", "111");

        Run();

        Result.Should().BeEquivalentTo(new[] { Values[2] });
    }

    [Test]
    public void WhenContains_ShouldBeOk()
    {
        Filter = Filter.Contains(SUBJECT, "11");
        Values = New("111", "112", "123");

        Run();

        Result.Should().BeEquivalentTo(new[] { Values[0], Values[1] });
    }

    [Test]
    public void WhenNotContains_ShouldBeOk()
    {
        Filter = Filter.NotContains(SUBJECT, "11");
        Values = New("111", "112", "123");

        Run();

        Result.Should().BeEquivalentTo(new[] { Values[2] });
    }

    private NotificationRow[] New(params string[] subjects)
    {
        return subjects.Select(x => new NotificationRow { Subject = x }).ToArray();
    }
}