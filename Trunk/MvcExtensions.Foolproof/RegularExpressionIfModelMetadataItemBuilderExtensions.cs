#region Copyright
// Copyright (c) 2009 - 2011, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>, hazzik <hazzik@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions
{
    using System;
    using Foolproof;

    /// <summary>
    /// Adds validation for <see cref="RegularExpressionIfAttribute"/>
    /// </summary>
    public class RegularExpressionIfModelMetadataItemBuilderExtensions
    {
        /// <summary>
        /// Sets the range of value, this comes into action when is <code>Required</code> is <code>true</code>.
        /// </summary>
        /// <param name="self">The instance.</param>
        /// <param name="otherProperty">The other property.</param>
        /// <param name="expression"></param>
        /// <param name="operator"></param>
        /// <param name="dependentValue"></param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TItem, TItemBuilder> RegularExpressionIf<TItem, TItemBuilder>(ModelMetadataItemBuilder<TItem, TItemBuilder> self, string otherProperty, string expression, Operator @operator, object dependentValue)
            where TItem : ModelMetadataItem where TItemBuilder : ModelMetadataItemBuilder<TItem, TItemBuilder>
        {
            return RegularExpressionIf(self, otherProperty, expression, @operator, dependentValue, null, null, null);
        }

        /// <summary>
        /// Sets the range of value, this comes into action when is <code>Required</code> is <code>true</code>.
        /// </summary>
        /// <param name="dependentValue"></param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="self">The instance.</param>
        /// <param name="otherProperty">The other property.</param>
        /// <param name="expression"></param>
        /// <param name="operator"></param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TItem, TItemBuilder> RegularExpressionIf<TItem, TItemBuilder>(ModelMetadataItemBuilder<TItem, TItemBuilder> self, string otherProperty, string expression, Operator @operator, object dependentValue, string errorMessage)
            where TItem : ModelMetadataItem
            where TItemBuilder : ModelMetadataItemBuilder<TItem, TItemBuilder>
        {
            return RegularExpressionIf(self, otherProperty, expression, @operator, dependentValue, () => errorMessage);
        }

        /// <summary>
        /// Sets the range of value, this comes into action when is <code>Required</code> is <code>true</code>.
        /// </summary>
        /// <param name="dependentValue"></param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="self">The instance.</param>
        /// <param name="otherProperty">The other property.</param>
        /// <param name="expression"></param>
        /// <param name="operator"></param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TItem, TItemBuilder> RegularExpressionIf<TItem, TItemBuilder>(ModelMetadataItemBuilder<TItem, TItemBuilder> self, string otherProperty, string expression, Operator @operator, object dependentValue, Func<string> errorMessage)
            where TItem : ModelMetadataItem
            where TItemBuilder : ModelMetadataItemBuilder<TItem, TItemBuilder>
        {
            return RegularExpressionIf(self, otherProperty, expression, @operator, dependentValue, errorMessage, null, null);
        }

        /// <summary>
        /// Sets the range of value, this comes into action when is <code>Required</code> is <code>true</code>.
        /// </summary>
        /// <param name="self">The instance.</param>
        /// <param name="otherProperty">The other property.</param>
        /// <param name="dependentValue"></param>
        /// <param name="errorMessageResourceType">Type of the error message resource.</param>
        /// <param name="errorMessageResourceName">Name of the error message resource.</param>
        /// <param name="expression"></param>
        /// <param name="operator"></param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TItem, TItemBuilder> RegularExpressionIf<TItem, TItemBuilder>(ModelMetadataItemBuilder<TItem, TItemBuilder> self, string otherProperty, string expression, Operator @operator, object dependentValue, Type errorMessageResourceType, string errorMessageResourceName)
            where TItem : ModelMetadataItem
            where TItemBuilder : ModelMetadataItemBuilder<TItem, TItemBuilder>
        {
            return RegularExpressionIf(self, otherProperty, expression, @operator, dependentValue, null, errorMessageResourceType, errorMessageResourceName);
        }

        /// <summary>
        /// Sets the range of value, this comes into action when is <code>Required</code> is <code>true</code>.
        /// </summary>
        /// <param name="self">The instance.</param>
        /// <param name="otherProperty">The other property.</param>
        /// <param name="expression"></param>
        /// <param name="operator"></param>
        /// <param name="dependentValue"></param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="errorMessageResourceType">Type of the error message resource.</param>
        /// <param name="errorMessageResourceName">Name of the error message resource.</param>
        /// <returns></returns>
        protected virtual ModelMetadataItemBuilder<TItem, TItemBuilder> RegularExpressionIf<TItem, TItemBuilder>(ModelMetadataItemBuilder<TItem, TItemBuilder> self, string otherProperty, string expression, Operator @operator, object dependentValue, Func<string> errorMessage, Type errorMessageResourceType, string errorMessageResourceName)
            where TItem : ModelMetadataItem
            where TItemBuilder : ModelMetadataItemBuilder<TItem, TItemBuilder>
        {
            var validation = self.Item.GetValidationOrCreateNew<RegularExpressionIfAttributeMetadata>();

            validation.Expression = expression;
            validation.OtherProperty = otherProperty;
            validation.Operator = @operator;
            validation.DependentValue = dependentValue;
            validation.ErrorMessage = errorMessage;
            validation.ErrorMessageResourceType = errorMessageResourceType;
            validation.ErrorMessageResourceName = errorMessageResourceName;

            return self;
        }
    }
}