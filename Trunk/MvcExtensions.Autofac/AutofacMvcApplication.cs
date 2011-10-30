﻿#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.Autofac
{
    using System;
    using System.Web;

    /// <summary>
    /// Defines a <see cref="HttpApplication"/> which is uses <seealso cref="AutofacBootstrapper"/>.
    /// </summary>
    public class AutofacMvcApplication : ExtendedMvcApplication
    {
        private static ILifetimeScopeProvider lifetimeScopeProvider;

        internal static void SetLifetimeScopeProvider(ILifetimeScopeProvider scopeProvider)
        {
            Invariant.IsNotNull(scopeProvider, "scopeProvider");

            lifetimeScopeProvider = scopeProvider;
        }

        /// <summary>
        /// Executes custom initialization code after all event handler modules have been added.
        /// </summary>
        public override void Init()
        {
            base.Init();
            EndRequest += OnEndRequest;
        }

        /// <summary>
        /// Creates the bootstrapper.
        /// </summary>
        /// <returns></returns>
        protected override IBootstrapper CreateBootstrapper()
        {
            return new AutofacBootstrapper(BuildManagerWrapper.Current, BootstrapperTasksRegistry.Current, PerRequestTasksRegistry.Current);
        }

        private static void OnEndRequest(object sender, EventArgs e)
        {
            if (lifetimeScopeProvider != null)
            {
                lifetimeScopeProvider.EndLifetimeScope();
            }
        }
    }
}