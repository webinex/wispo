using System;
using System.Linq.Expressions;
using Webinex.Wispo.DataAccess;

namespace Webinex.Wispo.Filters
{
    public interface IFilterFactory
    {
        Expression<Func<NotificationRow, bool>> Create(Filter filter);
    }
}