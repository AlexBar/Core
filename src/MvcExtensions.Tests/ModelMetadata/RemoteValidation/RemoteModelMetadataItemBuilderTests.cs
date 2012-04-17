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
        public void Should_be_able_to_set_remote_with_http_method()
        {
            const string httpMethod = "POST";
            builder.Remote(c => c.HttpMethod(httpMethod).For<DummyController>(x => x.CheckUsername1));

            var metadata = (RemoteValidationMetadata)item.Validations.First();
            Assert.Equal(metadata.HttpMethod, httpMethod);
        }

        [Fact]
        public void Should_not_be_http_method_if_no_any_other_params_are_set()
        {
            const string httpMethod = "POST";
            builder.Remote(c => c.HttpMethod(httpMethod));

            Assert.Equal(0, item.Validations.Count);
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
            const string areaName = "area";
            builder.Remote(c => c.For<DummyController>(x => x.CheckUsername1, areaName));

            var metadata = (RemoteValidationMetadata)item.Validations.First();

            Assert.Equal("Dummy", metadata.Controller);
            Assert.Equal("CheckUsername1", metadata.Action);
            Assert.Equal(areaName, metadata.Area);
        }

        [Fact]
        public void Should_be_able_to_set_remote_for_controller_action_as_strings()
        {
            const string controller = "controller1";
            const string action = "action1";
            builder.Remote(c => c.For(controller, action));

            var metadata = (RemoteValidationMetadata)item.Validations.First();

            Assert.Equal(controller, metadata.Controller);
            Assert.Equal(action, metadata.Action);
            Assert.Equal(null, metadata.Area);
        }

        [Fact]
        public void Should_be_able_to_set_remote_for_controller_action_area_as_strings()
        {
            const string controller = "controller1";
            const string action = "action1";
            const string areaName = "area1";
            builder.Remote(c => c.For(controller, action, areaName));

            var metadata = (RemoteValidationMetadata)item.Validations.First();

            Assert.Equal(controller, metadata.Controller);
            Assert.Equal(action, metadata.Action);
            Assert.Equal(areaName, metadata.Area);
        }

        [Fact]
        public void Should_be_able_to_set_remote_for_controller_action_additional_fields_as_strings()
        {
            const string controller = "controller1";
            const string action = "action1";
            const string additionalFields = "Id";
            builder.Remote(c => c.For(controller, action, new[]{additionalFields}));

            var metadata = (RemoteValidationMetadata)item.Validations.First();

            Assert.Equal(controller, metadata.Controller);
            Assert.Equal(action, metadata.Action);
            Assert.Equal(null, metadata.Area);
            Assert.Equal(additionalFields, metadata.AdditionalFields);
        }

        [Fact]
        public void Should_be_able_to_set_remote_for_controller_action_with_two_additional_fields_as_strings()
        {
            const string controller = "controller1";
            const string action = "action1";
            const string additionalField1 = "Id";
            const string additionalField2 = "Id2";
            builder.Remote(c => c.For(controller, action, new[] { additionalField1, additionalField2 }));

            var metadata = (RemoteValidationMetadata)item.Validations.First();

            Assert.Equal(string.Format("{0},{1}", additionalField1, additionalField2), metadata.AdditionalFields);
        }

        [Fact]
        public void Should_be_able_to_set_remote_for_controller_action_area_additional_fields_as_strings()
        {
            const string controller = "controller1";
            const string action = "action1";
            const string additionalFields = "Id";
            const string areaName = "area1";
            builder.Remote(c => c.For(controller, action, areaName, new[] { additionalFields }));

            var metadata = (RemoteValidationMetadata)item.Validations.First();

            Assert.Equal(controller, metadata.Controller);
            Assert.Equal(action, metadata.Action);
            Assert.Equal(areaName, metadata.Area);
            Assert.Equal(additionalFields, metadata.AdditionalFields);
        }

        [Fact]
        public void Should_be_able_to_set_remote_for_action_with_area_and_additional_fields_as_expression()
        {
            builder.Remote(c => c.With<TestViewModel>()
                                    .For<DummyController>(x => x.CheckUsername2, m => m.Id));

            var metadata = (RemoteValidationMetadata)item.Validations.First();

            Assert.Equal("Dummy", metadata.Controller);
            Assert.Equal("CheckUsername2", metadata.Action);
            Assert.Equal("Id", metadata.AdditionalFields);
        }

        [Fact]
        public void Should_be_able_to_set_remote_for_action_with_area_and_two_additional_fields_as_expression()
        {
            builder.Remote(c => c.With<TestViewModel>().For<DummyController>(x => x.CheckUsername2, m => m.Id, m => m.Id2));

            var metadata = (RemoteValidationMetadata)item.Validations.First();

            Assert.Equal("Dummy", metadata.Controller);
            Assert.Equal("CheckUsername2", metadata.Action);
            Assert.Equal("Id,Id2", metadata.AdditionalFields);
        }


        // test classes

        public class TestViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Id2 { get; set; }
            public int Id3 { get; set; }
        }
        
        /// <summary>
        /// DummyController for tests
        /// </summary>
        public class DummyController : Controller
        {
            /// <summary>
            /// CheckUsername1 test method
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public JsonResult CheckUsername1(string name)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            /// <summary>
            /// CheckUsername2 test method
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public JsonResult CheckUsername2(TestViewModel name)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
    }
}