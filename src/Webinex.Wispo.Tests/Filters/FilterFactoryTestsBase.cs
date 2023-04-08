using System.Linq;
using NUnit.Framework;
using Webinex.Wispo.DataAccess;
using Webinex.Wispo.Filters;

namespace Webinex.Wispo.Tests.Filters;

public class FilterFactoryTestsBase
{
    private FilterFactory _subject;
    protected Filter Filter;
    protected NotificationRow[] Values;
    protected NotificationRow[] Result;

    protected void Run()
    {
        Result = Values.AsQueryable().Where(_subject.Create(Filter)).ToArray();
    }

    [SetUp]
    public void SetUp()
    {
        _subject = new FilterFactory(new DefaultFieldMap());
        Filter = null;
        Values = null;
        Result = null;
    }
}