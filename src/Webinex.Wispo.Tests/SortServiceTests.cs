using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Webinex.Wispo.DataAccess;
using static Webinex.Wispo.NotificationField;

namespace Webinex.Wispo.Tests;

public class SortServiceTests
{
    private ISortService _subject;
    private NotificationRow[] _values;
    private SortRule[] _sort;
    private NotificationRow[] _result;

    [Test]
    public void WhenBySingleStringAsc_ShouldBeOk()
    {
        _sort = new[] { SortRule.Asc(BODY) };
        
        Run();

        _result.SequenceEqual(new[] { _values[1], _values[0], _values[2] }).Should().BeTrue();
    }

    [Test]
    public void WhenBySingleStringDesc_ShouldBeOk()
    {
        _sort = new[] { SortRule.Desc(BODY) };
        
        Run();

        _result.SequenceEqual(new[] { _values[2], _values[0], _values[1] }).Should().BeTrue();
    }

    [Test]
    public void WhenByMultipleStringsAsc_ShouldBeOk()
    {
        _sort = new[] { SortRule.Asc(SUBJECT), SortRule.Asc(BODY),  };
        
        Run();

        _result.SequenceEqual(new[] { _values[0], _values[2], _values[1] }).Should().BeTrue();
    }

    [Test]
    public void WhenByMultipleStringsMixedDir_ShouldBeOk()
    {
        _sort = new[] { SortRule.Desc(SUBJECT), SortRule.Asc(BODY),  };
        
        Run();

        _result.SequenceEqual(new[] { _values[1], _values[0], _values[2] }).Should().BeTrue();
    }

    [Test]
    public void WhenByMixedValuesAndMixedDir_ShouldBeOk()
    {
        _sort = new[] { SortRule.Asc(CREATED_AT), SortRule.Desc(BODY),  };
        
        Run();

        _result.SequenceEqual(new[] { _values[0], _values[1], _values[2] }).Should().BeTrue();
    }

    [Test]
    public void WhenByNullableValuesAndMixedDir_ShouldBeOk()
    {
        _values[0].Subject = null;
        
        _sort = new[] { SortRule.Asc(SUBJECT), SortRule.Desc(BODY),  };
        
        Run();

        _result.SequenceEqual(new[] { _values[0], _values[2], _values[1] }).Should().BeTrue();
    }

    private void Run()
    {
        _result = _subject.Apply(_values.AsQueryable(), _sort).ToArray();
    }

    [SetUp]
    public void SetUp()
    {
        _subject = new SortService(new DefaultFieldMap());
        _sort = null;
        _result = null;
        var now = DateTime.UtcNow;

        _values = new[]
        {
            new NotificationRow
            {
                Subject = "1",
                Body = "2",
                CreatedAt = now,
            },
            new NotificationRow
            {
                Subject = "2",
                Body = "1",
                CreatedAt = now,
            },
            new NotificationRow
            {
                Subject = "1",
                Body = "3",
                CreatedAt = DateTime.UtcNow.AddMinutes(1),
            },
        };
    }
}