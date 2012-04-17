#region Copyright
// Copyright (c) 2009 - 2012, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>, hazzik <hazzik@gmail.com>, AlexBar <abarbashin@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Mvc;

    /// <summary>
    /// RemoteValidationConfigurator class implements methods to configure remote validation
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class RemoteValidationConfigurator<TValue> : AbstractRemoteValidationConfigurator<TValue>
    {
        private readonly Func<string> errorMessage;
        private readonly string errorMessageResourceName;
        private readonly Type errorMessageResourceType;
        private string httpMethod;

        internal RemoteValidationConfigurator(ModelMetadataItemBuilder<TValue> modelMetadataItemBuilder,
                                    Func<string> errorMessage,
                                    string errorMessageResourceName,
                                    Type errorMessageResourceType)
        {
            this.errorMessage = errorMessage;
            this.errorMessageResourceName = errorMessageResourceName;
            this.errorMessageResourceType = errorMessageResourceType;
            Core.ModelMetadataItemBuilder = modelMetadataItemBuilder;
        }

        private IRemoteValidationConfigurator<TValue> Core
        {
            get { return this; }
        }

        /// <summary>
        /// HttpMethod. The Default one of GET
        /// </summary>
        /// <param name="method">Http method, e.g., Get, Post</param>
        /// <returns></returns>
        public RemoteValidationConfigurator<TValue> HttpMethod(string method)
        {
            httpMethod = method;
            return this;
        }


        /// <summary>
        /// Register Remote validator for the controller and specified action
        /// </summary>
        /// <param name="action">Action to call by validator</param>
        /// <typeparam name="TController">Target controller to find the action</typeparam>
        /// <returns><see cref="AbstractRemoteValidationConfigurator{TValue}"/></returns>
        public AbstractRemoteValidationConfigurator<TValue> For<TController>(Expression<Func<TController, Func<TValue, JsonResult>>> action)
            where TController : IController
        {
            CreateRemoteValidation(action, null, null, Enumerable.Empty<string>());
            return this;
        }

        /// <summary>
        /// Register Remote validator for the controller and specified action
        /// </summary>
        /// <param name="action">Action to call by validator</param>
        /// <param name="areaName">The name of area</param>
        /// <typeparam name="TController">Target controller to find the action</typeparam>
        /// <returns><see cref="AbstractRemoteValidationConfigurator{TValue}"/></returns>
        public AbstractRemoteValidationConfigurator<TValue> For<TController>(Expression<Func<TController, Func<TValue, JsonResult>>> action, string areaName)
            where TController : IController
        {
            CreateRemoteValidation(action, areaName, null, Enumerable.Empty<string>());
            return this;
        }


        /// <summary>
        /// Register Remote validator by the controller name and action name
        /// </summary>
        /// <param name="controller">The name of controller</param>
        /// <param name="action">The name of action</param>
        /// <returns><see cref="AbstractRemoteValidationConfigurator{TValue}"/></returns>
        public AbstractRemoteValidationConfigurator<TValue> For(string controller, string action)
        {
            return For(controller, action, (string)null);
        }

        /// <summary>
        /// Register Remote validator by the controller name and action name
        /// </summary>
        /// <param name="controller">The name of controller</param>
        /// <param name="action">The name of action</param>
        /// <param name="additionalFields">The additional fields</param>
        /// <returns><see cref="AbstractRemoteValidationConfigurator{TValue}"/></returns>
        public AbstractRemoteValidationConfigurator<TValue> For(string controller, string action, IEnumerable<string> additionalFields)
        {
            CreateRemoteValidation(controller, action, null, null, additionalFields);
            return this;
        }

        /// <summary>
        /// Register Remote validator by the controller name and action name
        /// </summary>
        /// <param name="controller">The name of controller</param>
        /// <param name="action">The name of action</param>
        /// <param name="areaName">The name of area</param>
        /// <returns><see cref="AbstractRemoteValidationConfigurator{TValue}"/></returns>
        public AbstractRemoteValidationConfigurator<TValue> For(string controller, string action, string areaName)
        {
            return For(controller, action, areaName, Enumerable.Empty<string>());
        }

        /// <summary>
        /// Register Remote validator by the controller name and action name
        /// </summary>
        /// <param name="controller">The name of controller</param>
        /// <param name="action">The name of action</param>
        /// <param name="areaName">The name of area</param>
        /// <param name="additionalFields">The additional fields</param>
        /// <returns><see cref="AbstractRemoteValidationConfigurator{TValue}"/></returns>
        public AbstractRemoteValidationConfigurator<TValue> For(string controller, string action, string areaName, IEnumerable<string> additionalFields)
        {
            CreateRemoteValidation(controller, action, areaName, null, additionalFields);
            return this;
        }

        /// <summary>
        /// Register Remote validator by the route name
        /// </summary>
        /// <param name="routeName">The name of the route</param>
        /// <returns><see cref="AbstractRemoteValidationConfigurator{TValue}"/></returns>
        public AbstractRemoteValidationConfigurator<TValue> For(string routeName)
        {
            return For(routeName, Enumerable.Empty<string>());
        }

        /// <summary>
        /// Register Remote validator by the route name
        /// </summary>
        /// <param name="routeName">The name of the route</param>
        /// <param name="additionalFields">The additional fields</param>
        /// <returns><see cref="AbstractRemoteValidationConfigurator{TValue}"/></returns>
        public AbstractRemoteValidationConfigurator<TValue> For(string routeName, IEnumerable<string> additionalFields)
        {
            CreateRemoteValidation(null, null, null, routeName, additionalFields);
            return this;
        }

        /// <summary>
        /// Specifies the type of model to retrive additional fields (usually needs to use the model you configure metadata for)
        /// </summary>
        /// <typeparam name="TModel">The Current view model</typeparam>
        /// <returns></returns>
        public ModelRemoteConfigurator<TModel> With<TModel>() where TModel : class 
        {
            return new ModelRemoteConfigurator<TModel>(this);
        }

        private void CreateRemoteValidation<TController, TModel, TParam>(Expression<Func<TController, Func<TParam, JsonResult>>> action,
                                                                   string areaName, string routeName,
                                                                   params Expression<Func<TModel, object>>[] additionalFields)
            where TController : IController
        {
            IEnumerable<string> fields = additionalFields.Select(f => new ExpressionUtil().GetFullPropertyName(f));
            CreateRemoteValidation(action, areaName, routeName, fields);
        }

        private void CreateRemoteValidation<TController, TParam>(Expression<Func<TController, Func<TParam, JsonResult>>> action,
                                                                   string areaName, string routeName,
                                                                   IEnumerable<string> additionalFields) where TController : IController
        {
            string controller = typeof(TController).Name.Replace("Controller", "");
            string name = new ExpressionUtil().GetMethod(action).Name;
            CreateRemoteValidation(controller, name, areaName, routeName, additionalFields);
        }

        private void CreateRemoteValidation(string controller, string action, string areaName, string routeName, IEnumerable<string> additionalFields)
        {
            ModelMetadataItemBuilder<TValue> self = Core.ModelMetadataItemBuilder;
            var validation = self.Item.GetValidationOrCreateNew<RemoteValidationMetadata>();

            validation.ErrorMessage = errorMessage;
            validation.Action = action;
            validation.Controller = controller;
            validation.RouteName = routeName;
            validation.Area = areaName;
            string fields = string.Join(",", additionalFields);
            validation.AdditionalFields = string.Join(",", fields);
            validation.HttpMethod = httpMethod;
            validation.ErrorMessageResourceType = errorMessageResourceType;
            validation.ErrorMessageResourceName = errorMessageResourceName;
        }
        
        #region ModelRemoteSettings<TModel>
        /// <summary>
        /// Incapsulates some additional method to register remote validation
        /// </summary>
        /// <typeparam name="TModel">The type of current model</typeparam>
        public class ModelRemoteConfigurator<TModel>
        {
            private readonly RemoteValidationConfigurator<TValue> value;

            internal ModelRemoteConfigurator(RemoteValidationConfigurator<TValue> value)
            {
                this.value = value;
            }

            /// <summary>
            /// Register Remote validator for the controller and specified action
            /// </summary>
            /// <param name="action">Action to call by validator</param>
            /// <param name="additionalFields"> The additional fields</param>
            /// <typeparam name="TController">Target controller to find the action</typeparam>
            /// <returns><see cref="AbstractRemoteValidationConfigurator{TValue}"/></returns>
            public AbstractRemoteValidationConfigurator<TValue> For<TController>(Expression<Func<TController, Func<TModel, JsonResult>>> action,
                                                                       params Expression<Func<TModel, object>>[] additionalFields)
                where TController : IController
            {
                value.CreateRemoteValidation(action, null, null, additionalFields);
                return value;
            }
        }
        #endregion
    }


}