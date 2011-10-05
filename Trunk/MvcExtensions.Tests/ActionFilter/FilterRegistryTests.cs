#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Moq;
    using Xunit;

    public class FilterRegistryTests
    {
        private readonly Mock<ContainerAdapter> adapter;
        private readonly FilterRegistryTestDouble registry;

        public FilterRegistryTests()
        {
            adapter = new Mock<ContainerAdapter>();

            adapter.Setup(a => a.GetService(typeof(DummyFilter1))).Returns(new DummyFilter1());
            adapter.Setup(a => a.GetService(typeof(DummyFilter2))).Returns(new DummyFilter2());
            adapter.Setup(a => a.GetService(typeof(DummyFilter3))).Returns(new DummyFilter3());
            adapter.Setup(a => a.GetService(typeof(DummyFilter4))).Returns(new DummyFilter4());

            registry = new FilterRegistryTestDouble(adapter.Object);
        }

        [Fact]
        public void Should_be_able_to_register_filter_for_multiple_controllers()
        {
            registry.Register<DummyFilter1>(CreateTypeCatalog());

            Assert.Equal(2, registry.PublicItems.Count());
            Assert.Equal(1, registry.PublicItems[0].GetFilters().Count());
            Assert.Equal(1, registry.PublicItems[1].GetFilters().Count());
        }

        [Fact]
        public void Should_be_able_to_register_and_configure_filter_for_multiple_controllers()
        {
            registry.Register<DummyFilter2>(
                                                CreateTypeCatalog(),
                                                filter =>
                                                          {
                                                              filter.IntegerProperty = 10;
                                                              filter.StringProperty = "foo";
                                                          });

            Assert.Equal(2, registry.PublicItems.Count());

            var item1 = registry.PublicItems[0];
            var item2 = registry.PublicItems[1];

            Assert.Equal(1, item1.GetFilters().Count());

            var filter1 = Assert.IsType<DummyFilter2>(GetInstance(item1, 0));
            Assert.Equal(10, filter1.IntegerProperty);
            Assert.Equal("foo", filter1.StringProperty);

            Assert.Equal(1, item2.GetFilters().Count());
            var filter2 = Assert.IsType<DummyFilter2>(GetInstance(item2, 0));
            Assert.Equal(10, filter2.IntegerProperty);
            Assert.Equal("foo", filter2.StringProperty);
        }

        [Fact]
        public void Should_be_able_to_register_two_filters_for_multiple_controllers()
        {
            registry.Register<DummyFilter1, DummyFilter2>(CreateTypeCatalog());

            Assert.Equal(2, registry.PublicItems.Count());
            Assert.Equal(2, registry.PublicItems[0].GetFilters().Count());
            Assert.Equal(2, registry.PublicItems[1].GetFilters().Count());
        }

        [Fact]
        public void Should_be_able_to_register_three_filters_for_multiple_controllers()
        {
            registry.Register<DummyFilter1, DummyFilter2, DummyFilter3>(CreateTypeCatalog());

            Assert.Equal(2, registry.PublicItems.Count());
            Assert.Equal(3, registry.PublicItems[0].GetFilters().Count());
            Assert.Equal(3, registry.PublicItems[1].GetFilters().Count());
        }

        [Fact]
        public void Should_be_able_to_register_four_filters_for_multiple_controllers()
        {
            registry.Register<DummyFilter1, DummyFilter2, DummyFilter3, DummyFilter4>(CreateTypeCatalog());

            Assert.Equal(2, registry.PublicItems.Count());
            Assert.Equal(4, registry.PublicItems[0].GetFilters().Count());
            Assert.Equal(4, registry.PublicItems[1].GetFilters().Count());
        }

        [Fact]
        public void Should_throw_exception_when_invalid_controller_type_is_passed_to_register_filter_for_multiple_controllers()
        {
            var catalog = CreateTypeCatalog();
            catalog.IncludeFilters.Add(type => type == typeof(FilterRegistryTests));

            Assert.Throws<ArgumentException>(() => registry.Register<DummyFilter1>(catalog));
        }

        [Fact]
        public void Should_be_able_to_register_filter_for_controller()
        {
            registry.Register<Dummy1Controller, DummyFilter1>();

            Assert.Equal(1, registry.PublicItems[0].GetFilters().Count());
        }

        [Fact]
        public void Should_be_able_to_register_and_configure_filter_for_controller()
        {
            registry.Register<Dummy1Controller, DummyFilter2>(f =>
                                                                  {
                                                                      f.IntegerProperty = 10;
                                                                      f.StringProperty = "foo";
                                                                  });

            var item = (FilterRegistryControllerItem<Dummy1Controller>)registry.PublicItems[0];

            Assert.Equal(1, item.GetFilters().Count());
            var filter = Assert.IsType<DummyFilter2>(GetInstance(item, 0));
            Assert.Equal(10, filter.IntegerProperty);
            Assert.Equal("foo", filter.StringProperty);
        }

        [Fact]
        public void Should_be_able_to_register_two_filters_for_controller()
        {
            registry.Register<Dummy1Controller, DummyFilter1, DummyFilter2>();

            Assert.Equal(2, registry.PublicItems[0].GetFilters().Count());
        }

        [Fact]
        public void Should_be_able_to_register_three_filters_for_controller()
        {
            registry.Register<Dummy1Controller, DummyFilter1, DummyFilter2, DummyFilter3>();

            Assert.Equal(3, registry.PublicItems[0].GetFilters().Count());
        }

        [Fact]
        public void Should_be_able_to_register_four_filters_for_controller()
        {
            registry.Register<Dummy1Controller, DummyFilter1, DummyFilter2, DummyFilter3, DummyFilter4>();

            Assert.Equal(4, registry.PublicItems[0].GetFilters().Count());
        }

        [Fact]
        public void Should_be_able_to_register_filter_for_action()
        {
            registry.Register<Dummy1Controller, DummyFilter1>(c => c.Index());

            Assert.Equal(1, registry.PublicItems[0].GetFilters().Count());
        }

        [Fact]
        public void Should_be_able_to_register_and_configure_filter_for_action()
        {
            registry.Register<Dummy1Controller, DummyFilter3>(
                                                                c => c.Index(),
                                                                filter =>
                                                                {
                                                                    filter.LongProperty = 10;
                                                                    filter.DecimalProperty = 100;
                                                                });

            var item = (FilterRegistryActionItem<Dummy1Controller>)registry.PublicItems[0];

            Assert.Equal(1, item.GetFilters().Count());
            var filter3 = Assert.IsType<DummyFilter3>(GetInstance(item, 0));
            Assert.Equal(10, filter3.LongProperty);
            Assert.Equal(100, filter3.DecimalProperty);
        }

        [Fact]
        public void Should_be_able_to_register_two_filters_for_action()
        {
            registry.Register<Dummy1Controller, DummyFilter1, DummyFilter2>(c => c.Index());

            Assert.Equal(2, registry.PublicItems[0].GetFilters().Count());
        }

        [Fact]
        public void Should_be_able_to_register_three_filters_for_action()
        {
            registry.Register<Dummy1Controller, DummyFilter1, DummyFilter2, DummyFilter3>(c => c.Index());

            Assert.Equal(3, registry.PublicItems[0].GetFilters().Count());
        }

        [Fact]
        public void Should_be_able_to_register_four_filters_for_action()
        {
            registry.Register<Dummy1Controller, DummyFilter1, DummyFilter2, DummyFilter3, DummyFilter4>(c => c.Index());

            Assert.Equal(4, registry.PublicItems[0].GetFilters().Count());
        }

        [Fact]
        public void Matching_should_return_matched_filters()
        {
            var controllerContext = new ControllerContext
                                        {
                                            Controller = new Dummy1Controller()
                                        };

            var controllerDescriptor = new Mock<ControllerDescriptor>();
            controllerDescriptor.SetupGet(cd => cd.ControllerName).Returns("Dummy1");

            var actionDescriptor = new Mock<ActionDescriptor>();
            actionDescriptor.SetupGet(ad => ad.ControllerDescriptor).Returns(controllerDescriptor.Object);
            actionDescriptor.SetupGet(ad => ad.ActionName).Returns("Index");

            registry.Register<Dummy1Controller, DummyFilter1, DummyFilter4>();
            registry.Register<Dummy1Controller, DummyFilter2, DummyFilter3>(c => c.Index());

            var filters = registry.Matching(controllerContext, actionDescriptor.Object);

            Assert.IsType<DummyFilter1>(filters.AuthorizationFilters[0]);
            Assert.IsType<DummyFilter2>(filters.ActionFilters[0]);
            Assert.IsType<DummyFilter3>(filters.ResultFilters[0]);
            Assert.IsType<DummyFilter4>(filters.ExceptionFilters[0]);
        }

        private static TypeCatalog CreateTypeCatalog()
        {
            var catalog = new TypeCatalog();

            catalog.Assemblies.Add(typeof(FilterRegistryTests).Assembly);
            catalog.IncludeFilters.Add(type => ((typeof(Dummy1Controller) == type) || (typeof(Dummy2Controller) == type)));

            return catalog;
        }

        private static object GetInstance(FilterRegistryItem item, int index)
        {
            return item.GetFilters().ElementAt(index).Instance;
        }

        private sealed class FilterRegistryTestDouble : FilterRegistry
        {
            public FilterRegistryTestDouble(ContainerAdapter container) : base(container)
            {
            }

            public IList<FilterRegistryItem> PublicItems
            {
                get
                {
                    return Items;
                }
            }
        }

        private sealed class DummyFilter1 : FilterAttribute, IAuthorizationFilter
        {
            public void OnAuthorization(AuthorizationContext filterContext)
            {
            }
        }

        private sealed class DummyFilter2 : FilterAttribute, IActionFilter
        {
            public int IntegerProperty
            {
                get;
                set;
            }

            public string StringProperty
            {
                get;
                set;
            }

            public void OnActionExecuting(ActionExecutingContext filterContext)
            {
            }

            public void OnActionExecuted(ActionExecutedContext filterContext)
            {
            }
        }

        private sealed class DummyFilter3 : FilterAttribute, IResultFilter
        {
            public long LongProperty
            {
                get;
                set;
            }

            public decimal DecimalProperty
            {
                get;
                set;
            }

            public void OnResultExecuting(ResultExecutingContext filterContext)
            {
            }

            public void OnResultExecuted(ResultExecutedContext filterContext)
            {
            }
        }

        private sealed class DummyFilter4 : FilterAttribute, IExceptionFilter
        {
            public void OnException(ExceptionContext filterContext)
            {
            }
        }
    }
}