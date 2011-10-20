#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.Autofac
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using ContainerBuilder = global::Autofac.ContainerBuilder;
    using ILifetimeScope = global::Autofac.ILifetimeScope;
    using RegisterExtensions = global::Autofac.RegistrationExtensions;
    using ResolutionExtensions = global::Autofac.ResolutionExtensions;

    /// <summary>
    /// Defines an adapter class which is backed by Autofac <seealso cref="Container">Container</seealso>.
    /// </summary>
    public class AutofacAdapter : ContainerAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacAdapter"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public AutofacAdapter(ILifetimeScope container)
        {
            Invariant.IsNotNull(container, "container");

            Container = container;
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        public ILifetimeScope Container
        {
            get;
            private set;
        }

        /// <summary>
        /// Registers the service and its implementation with the lifetime behavior.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="lifetime">The lifetime of the service.</param>
        /// <returns></returns>
        public override IServiceRegistrar RegisterType(string key, Type serviceType, Type implementationType, LifetimeType lifetime)
        {
            Invariant.IsNotNull(serviceType, "serviceType");
            Invariant.IsNotNull(implementationType, "implementationType");

            var builder = new ContainerBuilder();

            var registration = RegisterExtensions.RegisterType(builder, implementationType).As(serviceType);

            if (!string.IsNullOrEmpty(key))
            {
                registration = registration.Named(key, serviceType);
            }

            switch (lifetime)
            {
                case LifetimeType.PerRequest:
                    registration.InstancePerLifetimeScope();
                    break;
                case LifetimeType.Singleton:
                    registration.SingleInstance();
                    break;
                default:
                    registration.InstancePerDependency();
                    break;
            }

            builder.Update(Container.ComponentRegistry);

            return this;
        }

        /// <summary>
        /// Registers the instance as singleton.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public override IServiceRegistrar RegisterInstance(string key, Type serviceType, object instance)
        {
            Invariant.IsNotNull(serviceType, "serviceType");
            Invariant.IsNotNull(instance, "instance");

            var builder = new ContainerBuilder();

            if (string.IsNullOrEmpty(key))
            {
                RegisterExtensions.RegisterInstance(builder, instance).As(serviceType).ExternallyOwned();
            }
            else
            {
                RegisterExtensions.RegisterInstance(builder, instance).Named(key, serviceType).As(serviceType).ExternallyOwned();
            }

            builder.Update(Container.ComponentRegistry);

            return this;
        }

        /// <summary>
        /// Injects the matching dependences.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public override void Inject(object instance)
        {
            if (instance != null)
            {
                ResolutionExtensions.InjectUnsetProperties(Container, instance);
            }
        }

        /// <summary>
        /// Gets the matching instance for the given type and key.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected override object DoGetService(Type serviceType, string key)
        {
            return key != null ? ResolutionExtensions.ResolveNamed(Container, key, serviceType) : ResolutionExtensions.Resolve(Container, serviceType);
        }

        /// <summary>
        /// Gets all the instances for the given type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        protected override IEnumerable<object> DoGetServices(Type serviceType)
        {
            Type type = typeof(IEnumerable<>).MakeGenericType(serviceType);
            
            object instances = ResolutionExtensions.Resolve(Container, type);

            return ((IEnumerable)instances).Cast<object>();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        protected override void DisposeCore()
        {
            Container.Dispose();
        }
    }
}