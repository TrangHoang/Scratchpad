﻿using System.Web.Mvc;

namespace Reusables.Web.Mvc5
{
    /// <summary>
    /// Defines a common interface for action filter attribute handlers.
    /// If you are to create derived types from this interface, consider inheriting from the <see cref="ActionFilterBase{TFilterAttribute}"/> base class instead.
    /// </summary>
    /// <typeparam name="TFilterAttribute">The type of action filter attribute.</typeparam>
    public interface IActionFilter<TFilterAttribute> where TFilterAttribute : FilterAttribute
    {
        int Order { get; }

        /// <summary>
        /// Once this flag is set to <c>true</c>, all following executions of <see cref="IActionFilter"/> instances in the queue will be skipped.
        /// </summary>
        bool SkipNextFilters { get; }

        void OnActionExecuting(TFilterAttribute attribute, ActionExecutingContext filterContext);

        void OnActionExecuted(TFilterAttribute attribute, ActionExecutedContext filterContext);
    }
}
