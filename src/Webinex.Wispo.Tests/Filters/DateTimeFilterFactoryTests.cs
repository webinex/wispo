using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Webinex.Wispo.DataAccess;
using static Webinex.Wispo.NotificationField;

namespace Webinex.Wispo.Tests.Filters;

public class DateTimeFilterFactoryTests : FilterFactoryTestsBase
{
    private readonly DateTime _now = DateTime.UtcNow;

    [Test]
    public void WhenEq_ShouldBeOk()
    {
        Filter = Filter.Eq(CREATED_AT, _now);
        Values = New(_now.AddMilliseconds(-1), _now, _now.AddMilliseconds(1));

        Run();

        Result.Should().BeEquivalentTo(new[] { Values[1] });
    }

    [Test]
    public void WhenNotEq_ShouldBeOk()
    {
        Filter = Filter.NotEq(CREATED_AT, _now);
        Values = New(_now.AddMilliseconds(-1), _now, _now.AddMilliseconds(1));

        Run();

        Result.Should().BeEquivalentTo(new[] { Values[0], Values[2] });
    }

    [Test]
    public void WhenIn_ShouldBeOk()
    {
        Filter = Filter.In(CREATED_AT, new object[] { _now, _now.AddHours(1) });
        Values = New(_now, _now.AddHours(1), _now.AddHours(1), _now.AddHours(2));

        Run();

        Result.Should().BeEquivalentTo(new[] { Values[0], Values[1], Values[2] });
    }

    [Test]
    public void WhenNotIn_ShouldBeOk()
    {
        Filter = Filter.NotIn(CREATED_AT, new object[] { _now, _now.AddHours(1) });
        Values = New(_now, _now.AddHours(1), _now.AddHours(2));

        Run();

        Result.Should().BeEquivalentTo(new[] { Values[2] });
    }

    [Test]
    public void WhenGt_ShouldBeOk()
    {
        Filter = Filter.Gt(CREATED_AT, DateTime.UtcNow);
        Values = New(_now.AddMinutes(1), _now.AddMinutes(-1));

        Run();

        Result.Should().BeEquivalentTo(new[] { Values[0] });
    }

    [Test]
    public void WhenGte_ShouldBeOk()
    {
        Filter = Filter.Gte(CREATED_AT, _now);
        Values = New(_now, _now.AddMinutes(1), _now.AddMinutes(-1));

        Run();

        Result.Should().BeEquivalentTo(new[] { Values[0], Values[1] });
    }

    [Test]
    public void WhenLt_ShouldBeOk()
    {
        Filter = Filter.Lt(CREATED_AT, DateTime.UtcNow);
        Values = New(_now.AddMinutes(1), _now.AddMinutes(-1));

        Run();

        Result.Should().BeEquivalentTo(new[] { Values[1] });
    }

    [Test]
    public void WhenLte_ShouldBeOk()
    {
        Filter = Filter.Lte(CREATED_AT, _now);
        Values = New(_now, _now.AddMinutes(1), _now.AddMinutes(-1));

        Run();

        Result.Should().BeEquivalentTo(new[] { Values[0], Values[2] });
    }

    private NotificationRow[] New(params DateTime[] createdAts)
    {
        return createdAts.Select(x => new NotificationRow { CreatedAt = x }).ToArray();
    }
}