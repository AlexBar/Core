#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// The default <seealso cref="IActionInvoker"/> which supports the fluent filter registration and dependency injection.
    /// </summary>
    public class ExtendedControllerActionInvoker : ControllerActionInvoker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedControllerActionInvoker"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public ExtendedControllerActionInvoker(ContainerAdapter container)
        {
            Invariant.IsNotNull(container, "container");

            Container = container;
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        protected ContainerAdapter Container
        {
            get;
            private set;
        }

        /// <summary>
        /// Retrieves information about the action filters.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <returns>Information about the action filters.</returns>
        protected override FilterInfo GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            Invariant.IsNotNull(controllerContext, "controllerContext");
            Invariant.IsNotNull(actionDescriptor, "actionDescriptor");

            FilterInfo decoratedFilters = base.GetFilters(controllerContext, actionDescriptor);

            Inject(decoratedFilters);

            FilterInfo registeredFilters = Container.GetInstance<IFilterRegistry>().Matching(controllerContext, actionDescriptor);

            FilterInfo mergedFilters = new FilterInfo();

            MergeOrdered(mergedFilters, decoratedFilters, registeredFilters);
            MergeUnordered(mergedFilters, decoratedFilters, registeredFilters);

            return mergedFilters;
        }

        private static void MergeOrdered(FilterInfo mergedFilters, FilterInfo decoratedFilters, FilterInfo registeredFilters)
        {
            Merge(decoratedFilters.AuthorizationFilters, registeredFilters.AuthorizationFilters, IsFilterAttriute)
                .Cast<FilterAttribute>()
                .OrderBy(filter => filter.Order)
                .Cast<IAuthorizationFilter>()
                .Each(filter => mergedFilters.AuthorizationFilters.Add(filter));

            Merge(decoratedFilters.ActionFilters, registeredFilters.ActionFilters, IsFilterAttriute)
                .Cast<FilterAttribute>()
                .OrderBy(filter => filter.Order)
                .Cast<IActionFilter>()
                .Each(filter => mergedFilters.ActionFilters.Add(filter));

            Merge(decoratedFilters.ResultFilters, registeredFilters.ResultFilters, IsFilterAttriute)
                .Cast<FilterAttribute>()
                .OrderBy(filter => filter.Order)
                .Cast<IResultFilter>()
                .Each(filter => mergedFilters.ResultFilters.Add(filter));

            Merge(decoratedFilters.ExceptionFilters, registeredFilters.ExceptionFilters, IsFilterAttriute)
                .Cast<FilterAttribute>()
                .OrderBy(filter => filter.Order)
                .Cast<IExceptionFilter>()
                .Each(filter => mergedFilters.ExceptionFilters.Add(filter));
        }

        private static void MergeUnordered(FilterInfo mergedFilters, FilterInfo decoratedFilters, FilterInfo registeredFilters)
        {
            Merge(decoratedFilters.AuthorizationFilters, registeredFilters.AuthorizationFilters, IsNonFilterAttriute)
                .Reverse()
                .Each(filter => mergedFilters.AuthorizationFilters.Insert(0, filter));

            Merge(decoratedFilters.ActionFilters, registeredFilters.ActionFilters, IsNonFilterAttriute)
                .Reverse()
                .Each(filter => mergedFilters.ActionFilters.Insert(0, filter));

            Merge(decoratedFilters.ResultFilters, registeredFilters.ResultFilters, IsNonFilterAttriute)
                .Reverse()
                .Each(filter => mergedFilters.ResultFilters.Insert(0, filter));

            Merge(decoratedFilters.ExceptionFilters, registeredFilters.ExceptionFilters, IsNonFilterAttriute)
                .Reverse()
                .Each(filter => mergedFilters.ExceptionFilters.Insert(0, filter));
        }

        private static IEnumerable<T> Merge<T>(IEnumerable<T> source1, IEnumerable<T> source2, Func<T, bool> filter)
        {
            return source1.Where(filter).Concat(source2.Where(filter));
        }

        private static bool IsFilterAttriute<TFilter>(TFilter filter)
        {
            return KnownTypes.FilterAttributeType.IsAssignableFrom(filter.GetType());
        }

        private static bool IsNonFilterAttriute<TFilter>(TFilter filter)
        {
            return !KnownTypes.FilterAttributeType.IsAssignableFrom(filter.GetType());
        }

        private void Inject(FilterInfo decoratedFilters)
        {
            ICollection<object> injectedFilters = new List<object>();

            Inject(injectedFilters, decoratedFilters.AuthorizationFilters);
            Inject(injectedFilters, decoratedFilters.ActionFilters);
            Inject(injectedFilters, decoratedFilters.ResultFilters);
            Inject(injectedFilters, decoratedFilters.ExceptionFilters);
        }

        private void Inject<TFilter>(ICollection<object> injectedFilters, IEnumerable<TFilter> filters) where TFilter : class
        {
            foreach (TFilter filter in filters.Where(filter => IsFilterAttriute(filter) && !injectedFilters.Contains(filter)))
            {
                Container.Inject(filter);
                injectedFilters.Add(filter);
            }
        }
    }
}