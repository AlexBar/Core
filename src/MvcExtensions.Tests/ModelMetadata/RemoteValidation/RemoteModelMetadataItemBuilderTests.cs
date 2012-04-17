#region Copyright
// Copyright (c) 2009 - 2011, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>, hazzik <hazzik@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion
namespace MvcExtensions.Tests
{
    using System.Web.Mvc;
    using Xunit;
    using System.Linq;

    public class RemoteModelMetadataItemBuilderTests
    {
         private readonly ModelMetadataItem item;
         private readonly ModelMetadataItemBuilder<string> builder;

        public RemoteModelMetadataItemBuilderTests()
        {
            item = new ModelMetadataItem();
            builder = new ModelMetadataItemBuilder<string>(item);
        }

        [Fact]
        public void Should_be_able_to_set_remote()
        {
            builder.Remote(c => c.For<DummyController>(x => x.CheckUsername1));

            Assert.NotEmpty(item.Validations);
            Assert.IsType<RemoteValidationMetadata>(item.Validations.First());
        }

        [Fact]
        public void Should_be_able_to_set_remote_for_action()
        {
            builder.Remote(c => c.For<DummyController>(x => x.CheckUsername1));

            var metadata = (RemoteValidationMetadata)item.Validations.First();

            Assert.Equal("Dummy", metadata.Controller);
            Assert.Equal("CheckUsername1", metadata.Action);
            Assert.Equal(null, metadata.Area);
        }

        [Fact]
        public void Should_be_able_to_set_remote_for_action_with_area()
        {
            builder.Remote(c => c.For<DummyController>(x => x.CheckUsername1, "area"));

            var metadata = (RemoteValidationMetadata)item.Validations.First();

            Assert.Equal("Dummy", metadata.Controller);
            Assert.Equal("CheckUsername1", metadata.Action);
            Assert.Equal("area", metadata.Area);
        }

        [Fact]
        public void Should_be_able_to_set_remote_for_action_with_area_and_additional_fields()
        {
            builder.Remote(c => c.For<DummyController>(x => x.CheckUsername1, null, new [] { "Id" } ));

            var metadata = (RemoteValidationMetadata)item.Validations.First();

            Assert.Equal("Dummy", metadata.Controller);
            Assert.Equal("CheckUsername1", metadata.Action);
            Assert.Equal("Id", metadata.AdditionalFields);
        }

        [Fact]
        public void Should_be_able_to_set_remote_for_action_with_area_and_two_additional_fields()
        {
            builder.Remote(c => c.For<DummyController>(x => x.CheckUsername1, null, new[] { "Id1", "Id2" }));

            var metadata = (RemoteValidationMetadata)item.Validations.First();

            Assert.Equal("Id1,Id2", metadata.AdditionalFields);
        }

        [Fact]
        public void Should_be_able_to_set_remote_for_action_with_area_and_additional_fields_as_expression()
        {
            builder.Remote(c => c.With<TestModel>().For<DummyController>(x => x.CheckUsername2, m => m.Id));

            var metadata = (RemoteValidationMetadata)item.Validations.First();

            Assert.Equal("Dummy", metadata.Controller);
            Assert.Equal("CheckUsername2", metadata.Action);
            Assert.Equal("Id", metadata.AdditionalFields);
        }

        [Fact]
        public void Should_be_able_to_set_remote_for_action_with_area_and_two_additional_fields_as_expression()
        {
            builder.Remote(c => c.With<TestModel>().For<DummyController>(x => x.CheckUsername2, m => m.Id, m => m.Id2));

            var metadata = (RemoteValidationMetadata)item.Validations.First();

            Assert.Equal("Dummy", metadata.Controller);
            Assert.Equal("CheckUsername2", metadata.Action);
            Assert.Equal("Id,Id2", metadata.AdditionalFields);
        }


        // test classes

        public class TestModel
        {
            public int Id
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }

            public int Id2
            {
                get;
                set;
            }
        }

        /*public class Test : ModelMetadataConfiguration<TestModel>
        {
            public Test()
            {
                Configure(x => x.Name).Remote(r => r.For<DummyController>(c => c.CheckUsername1)).Required();
                Configure(x => x.Name).Remote(r => r.With<TestModel>().For<DummyController>(c => c.CheckUsername2, m => m.Id)).Required();
                Configure(x => x.Name).Remote(r => r.For("Dummy", "CheckUsername1")).Required();
                Configure(x => x.Name).Remote(r => r.For("routeName")).Required();
                Configure(x => x.Name).Remote(r =>
                {
                    //r.HttpMethod()
                    AbstractRemoteValidationConfigurator<string> core = r.For("asd");
                    return core;
                }).Required();
            }
        }*/

        /// <summary>
        /// DummyController for tests
        /// </summary>
        public class DummyController : Controller
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public JsonResult CheckUsername1(string name)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public JsonResult CheckUsername2(TestModel name)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
    }
}