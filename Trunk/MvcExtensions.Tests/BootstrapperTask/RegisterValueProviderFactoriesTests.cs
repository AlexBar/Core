﻿#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    using Moq;
    using Xunit;

    public class RegisterValueProviderFactoriesTests
    {
        private readonly Mock<ContainerAdapter> adapter;

        public RegisterValueProviderFactoriesTests()
        {
            var buildManager = new Mock<IBuildManager>();
            buildManager.Setup(bm => bm.ConcreteTypes).Returns(new[] { typeof(DummyValueProviderFactory) });

            var factories = new List<ValueProviderFactory>();

            adapter = new Mock<ContainerAdapter>();
            adapter.Setup(a => a.GetService(typeof(IBuildManager))).Returns(buildManager.Object);
            adapter.Setup(a => a.RegisterType(null, It.IsAny<Type>(), It.IsAny<Type>(), LifetimeType.Singleton)).Callback((string k, Type t1, Type t2, LifetimeType lt) => factories.Add((ValueProviderFactory)Activator.CreateInstance(t2)));
            adapter.Setup(a => a.GetServices(typeof(ValueProviderFactory))).Returns(() => factories);
        }

        [Fact]
        public void Should_register_available_value_provider_factories()
        {
            adapter.Setup(a => a.RegisterType(null, typeof(ValueProviderFactory), typeof(DummyValueProviderFactory), LifetimeType.Transient)).Verifiable();

            new RegisterValueProviderFactories(adapter.Object).Execute();

            adapter.Verify();
        }

        [Fact]
        public void Should_not_register_value_provider_factory_when_factory_exists_in_ignored_list()
        {
            var registration = new RegisterValueProviderFactories(adapter.Object);

            registration.Ignore<DummyValueProviderFactory>();

            registration.Execute();

            adapter.Verify(a => a.RegisterType(null, typeof(ValueProviderFactory), typeof(DummyValueProviderFactory), LifetimeType.Transient), Times.Never());
        }

        private sealed class DummyValueProviderFactory : ValueProviderFactory
        {
            public override IValueProvider GetValueProvider(ControllerContext controllerContext)
            {
                return null;
            }
        }
    }
}