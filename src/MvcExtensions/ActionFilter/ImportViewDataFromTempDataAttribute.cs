﻿#region Copyright
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
    /// Defines an attribute which is used to copy  the previous viewdata from the tempdata into current viewdata.
    /// <remarks>This  filter does  not execute for child action.</remarks>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class ImportViewDataFromTempDataAttribute : ViewDataTempDataTransferAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportViewDataFromTempDataAttribute"/> class.
        /// </summary>
        public ImportViewDataFromTempDataAttribute()
        {
            ReplaceExisting = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to replace the current viewdata with previous viewdata when same viewdata key exists.
        /// </summary>
        /// <value><c>true</c> if [replace existing]; otherwise, <c>false</c>.</value>
        public bool ReplaceExisting
        {
            get;
            set;
        }

        /// <summary>
        /// Called before an action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Invariant.IsNotNull(filterContext, "filterContext");

            if (filterContext.IsChildAction)
            {
                return;
            }

            ViewDataDictionary importingViewData = filterContext.Controller.TempData[Key] as ViewDataDictionary;

            if (importingViewData != null)
            {
                ViewDataDictionary currentViewData = filterContext.Controller.ViewData;

                foreach (KeyValuePair<string, object> pair in importingViewData.Where(pair => ReplaceExisting || !currentViewData.ContainsKey(pair.Key)))
                {
                    currentViewData[pair.Key] = pair.Value;
                }

                if (ReplaceExisting || (currentViewData.Model == null))
                {
                    currentViewData.Model = importingViewData.Model;
                }

                ModelStateDictionary currentModelState = filterContext.Controller.ViewData.ModelState;

                foreach (KeyValuePair<string, ModelState> pair in importingViewData.ModelState.Where(pair => ReplaceExisting || !currentModelState.ContainsKey(pair.Key)))
                {
                    currentModelState[pair.Key] = pair.Value;
                }

                filterContext.Controller.TempData.Remove(Key);
            }
        }
    }
}