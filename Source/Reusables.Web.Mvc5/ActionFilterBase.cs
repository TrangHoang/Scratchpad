﻿using System.Web.Mvc;

namespace Reusables.Web.Mvc5
{
    /// <summary>
    /// Provides a basic, non-generic implementation for <see cref="IActionFilter{TFilterAttribute}"/> interface.
    /// You are encouraged to inherit from this class, and override the generic versions.
    /// </summary>
    /// <typeparam name="TFilterAttribute"></typeparam>
    public abstract class ActionFilterBase<TFilterAttribute> : IActionFilter<TFilterAttribute> where TFilterAttribute : FilterAttribute
    {
        public int Order { get; set; }

        public bool SkipNextFilters { get; set; }

        public void OnActionExecuting(object attribute, ActionExecutingContext filterContext)
        {
            OnActionExecuting((TFilterAttribute) attribute, filterContext);
        }

        public void OnActionExecuted(object attribute, ActionExecutedContext filterContext)
        {
            OnActionExecuted((TFilterAttribute) attribute, filterContext);
        }

        public abstract void OnActionExecuting(TFilterAttribute attribute, ActionExecutingContext filterContext);

        public abstract void OnActionExecuted(TFilterAttribute attribute, ActionExecutedContext filterContext);
    }
}
