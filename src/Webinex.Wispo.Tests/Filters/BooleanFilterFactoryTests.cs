using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Webinex.Wispo.DataAccess;
using static Webinex.Wispo.NotificationField;

namespace Webinex.Wispo.Tests.Filters;

public class BooleanFilterFactoryTests : FilterFactoryTestsBase
{
    [Test]
    public void WhenEq_ShouldBeOk()
    {
        Filter = Filter.Eq(IS_READ, true);
        Values = New(true, true, false);
        
        Run();

        Result.Should().BeEquivalentTo(new[] { Values[0], Values[1] });
    }

    [Test]
    public void WhenNotEq_ShouldBeOk()
    {
        Filter = Filter.NotEq(IS_READ, true);
        Values = New(true, true, false);
        
        Run();

        Result.Should().BeEquivalentTo(new[] { Values[2] });
    }

    [Test]
    public void WhenIn_ShouldBeOk()
    {
        Filter = Filter.In(IS_READ, new object[] { true });
        Values = New(true, true, false);
        
        Run();

        Result.Should().BeEquivalentTo(new[] { Values[0], Values[1] });
    }

    [Test]
    public void WhenNotIn_ShouldBeOk()
    {
        Filter = Filter.NotIn(IS_READ, new object[] { true });
        Values = New(true, true, false);
        
        Run();

        Result.Should().BeEquivalentTo(new[] { Values[2] });
    }

    private NotificationRow[] New(params bool[] isRead)
    {
        return isRead.Select(v => new NotificationRow { IsRead = v }).ToArray();
    }
}