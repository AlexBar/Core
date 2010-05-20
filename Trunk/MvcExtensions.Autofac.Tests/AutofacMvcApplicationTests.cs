#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.Autofac.Tests
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Reflection;
    using System.Web;

    using Moq;
    using Xunit;

    using Microsoft.Practices.ServiceLocation;

    using ContainerBuilder = global::Autofac.ContainerBuilder;
    using ILifetimeScope = global::Autofac.ILifetimeScope;

    public class AutofacMvcApplicationTests
    {
        private static readonly FieldInfo privateBootStrapper = typeof(ExtendedMvcApplication).GetFields(BindingFlags.Static | BindingFlags.NonPublic).Single(f => f.Name == "bootstrapper");

        [Fact]
        public void Should_be_able_to_create_bootstrapper()
        {
            Assert.NotNull(new AutofacMvcApplicationTestDouble(null).PublicCreateBootstrapper());
        }

        [Fact]
        public void Should_set_adapter_when_application_starts()
        {
            var httpContext = new Mock<HttpContextBase>();
            httpContext.SetupGet(c => c.Items).Returns(new Hashtable());

            var httpApplication = SetupApplication(SetupAdapter(httpContext.Object).Object);

            httpApplication.Application_Start();

            Assert.NotNull(ServiceLocator.Current);

            SetBootstrapperToNull(httpApplication);
        }

        [Fact]
        public void Adapter_should_be_same_for_same_request()
        {
            var httpContext = new Mock<HttpContextBase>();
            httpContext.SetupGet(c => c.Items).Returns(new Hashtable());

            var httpApplication = SetupApplication(SetupAdapter(httpContext.Object).Object);

            httpApplication.Application_Start();

            Assert.Same(ServiceLocator.Current, ServiceLocator.Current);

            SetBootstrapperToNull(httpApplication);
        }

        [Fact]
        public void On_end_request_should_dispose_per_request_adapter()
        {
            var disposable = new Mock<IDisposable>();
            disposable.Setup(d => d.Dispose()).Verifiable();

            var httpContext = new Mock<HttpContextBase>();
            httpContext.SetupGet(c => c.Items).Returns(new Hashtable { { typeof(AutofacAdapter), disposable.Object } });

            var httpApplication = SetupApplication(SetupAdapter(httpContext.Object).Object);

            httpApplication.CompleteRequest(httpContext.Object);

            disposable.Verify();

            SetBootstrapperToNull(httpApplication);
        }

        private static AutofacMvcApplicationTestDouble SetupApplication(ContainerAdapter adapter)
        {
            var bootstrapper = new Mock<IBootstrapper>();

            bootstrapper.SetupGet(bs => bs.Adapter).Returns(adapter);

            return new AutofacMvcApplicationTestDouble(bootstrapper.Object);
        }

        private static Mock<AutofacAdapter> SetupAdapter(HttpContextBase httpContext)
        {
            var lifetimeScope = new Mock<ILifetimeScope>();
            lifetimeScope.Setup(ls => ls.BeginLifetimeScope(It.IsAny<string>())).Returns(new ContainerBuilder().Build());

            var adapter = new Mock<AutofacAdapter>(lifetimeScope.Object);
            adapter.Setup(a => a.GetInstance<HttpContextBase>()).Returns(httpContext);
            adapter.Setup(a => a.GetInstance<IBuildManager>()).Returns(new Mock<IBuildManager>().Object);

            return adapter;
        }

        private static void SetBootstrapperToNull(ExtendedMvcApplication httpApplication)
        {
            privateBootStrapper.SetValue(httpApplication, null);
        }

        public class AutofacMvcApplicationTestDouble : AutofacMvcApplication
        {
            private readonly IBootstrapper bootstrapper;

            public AutofacMvcApplicationTestDouble(IBootstrapper bootstrapper)
            {
                this.bootstrapper = bootstrapper;
            }

            public void CompleteRequest(HttpContextBase context)
            {
                OnEndRequest(context);
            }

            public IBootstrapper PublicCreateBootstrapper()
            {
                return base.CreateBootstrapper();
            }

            protected override IBootstrapper CreateBootstrapper()
            {
                return bootstrapper;
            }
        }
    }
}