#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;

    /// <summary>
    /// Defines a base class to fluently configure metadata.
    /// </summary>
    public class ModelMetadataItemBuilder<TValue> : IFluentSyntax
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelMetadataItemBuilder{TValue}"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public ModelMetadataItemBuilder(ModelMetadataItem item)
        {
            Invariant.IsNotNull(item, "item");

            Item = item;
        }

        /// <summary>
        /// Gets the internal item.
        /// </summary>
        /// <value>The item.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ModelMetadataItem Item
        {
            [EditorBrowsable(EditorBrowsableState.Never)]
            get;
            private set;
        }

        private ModelMetadataItemBuilder<TValue> This
        {
            [DebuggerStepThrough]
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Sets the Display name.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> DisplayName(string value)
        {
            return DisplayName(() => value);
        }

        /// <summary>
        /// Sets the Display name.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> DisplayName(Func<string> value)
        {
            Item.DisplayName = value;

            return This;
        }

        /// <summary>
        /// Sets the short display name.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> ShortDisplayName(string value)
        {
            return ShortDisplayName(() => value);
        }

        /// <summary>
        /// Sets the short display name.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> ShortDisplayName(Func<string> value)
        {
            Item.ShortDisplayName = value;

            return This;
        }

        /// <summary>
        /// Sets the Templates name.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> Template(string value)
        {
            Item.TemplateName = value;

            return This;
        }

        /// <summary>
        /// Sets the Description.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> Description(string value)
        {
            return Description(() => value);
        }

        /// <summary>
        /// Sets the Description.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> Description(Func<string> value)
        {
            Item.Description = value;

            return This;
        }

        /// <summary>
        /// Marks the value as read only.
        /// </summary>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> ReadOnly()
        {
            Item.IsReadOnly = true;

            return This;
        }

        /// <summary>
        /// Marks the value as writable, this is handy when the framework initializes the
        /// value as read only and you want to negate it.
        /// </summary>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> Writable()
        {
            Item.IsReadOnly = false;

            return This;
        }

        /// <summary>
        /// Sets the other property that the value must match.
        /// </summary>
        /// <param name="otherProperty">The other property</param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> Compare(string otherProperty)
        {
            return Compare(otherProperty, null, null, null);
        }

        /// <summary>
        /// Sets the other property that the value must match.
        /// </summary>
        /// <param name="otherProperty">The other property</param>
        /// <param name="errorMessage">The error message when the value is not specified.</param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> Compare(string otherProperty, string errorMessage)
        {
            return Compare(otherProperty, () => errorMessage);
        }

        /// <summary>
        /// Sets the other property that the value must match.
        /// </summary>
        /// <param name="otherProperty">The other property</param>
        /// <param name="errorMessage">The error message when the value is not specified.</param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> Compare(string otherProperty, Func<string> errorMessage)
        {
            return Compare(otherProperty, errorMessage, null, null);
        }

        /// <summary>
        /// Sets the other property that the value must match.
        /// </summary>
        /// <param name="errorMessageResourceType">Type of the error message resource.</param>
        /// <param name="errorMessageResourceName">Name of the error message resource.</param>
        /// <param name="otherProperty">The other property</param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> Compare(string otherProperty, Type errorMessageResourceType, string errorMessageResourceName)
        {
            return Compare(otherProperty, null, errorMessageResourceType, errorMessageResourceName);
        }

        /// <summary>
        /// Marks the value as required.
        /// </summary>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> Required()
        {
            return Required(null, null, null);
        }

        /// <summary>
        /// Marks the value as required.
        /// </summary>
        /// <param name="errorMessage">The error message when the value is not specified.</param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> Required(string errorMessage)
        {
            return Required(() => errorMessage);
        }

        /// <summary>
        /// Marks the value as required.
        /// </summary>
        /// <param name="errorMessage">The error message when the value is not specified.</param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> Required(Func<string> errorMessage)
        {
            return Required(errorMessage, null, null);
        }

        /// <summary>
        /// Marks the value as required.
        /// </summary>
        /// <param name="errorMessageResourceType">Type of the error message resource.</param>
        /// <param name="errorMessageResourceName">Name of the error message resource.</param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> Required(Type errorMessageResourceType, string errorMessageResourceName)
        {
            return Required(null, errorMessageResourceType, errorMessageResourceName);
        }

        /// <summary>
        /// Marks the value as optional, this is handy when the framework initializes the 
        /// value as required and you want to negate it.
        /// </summary>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> Optional()
        {
            Item.IsRequired = false;

            RequiredValidationMetadata requiredValidation = Item.GetValidation<RequiredValidationMetadata>();

            if (requiredValidation != null)
            {
                Item.Validations.Remove(requiredValidation);
            }

            return This;
        }

        /// <summary>
        /// Marks the value to render as hidden input element in edit mode.
        /// </summary>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> AsHidden()
        {
            Item.TemplateName = "HiddenInput";

            return This;
        }

        /// <summary>
        /// Marks the value to render as hidden input element in edit mode.
        /// </summary>
        /// <param name="hideSurroundingHtml">Indicates whether the value will appear in display mode</param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> AsHidden(bool hideSurroundingHtml)
        {
            Item.TemplateName = "HiddenInput";
            Item.HideSurroundingHtml = hideSurroundingHtml;

            return This;
        }

        /// <summary>
        /// Hides surrounding HTML.
        /// </summary>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> HideSurroundingHtml()
        {
            Item.HideSurroundingHtml = true;

            return This;
        }

        /// <summary>
        /// Shows surrounding HTML.
        /// </summary>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> ShowSurroundingHtml()
        {
            Item.HideSurroundingHtml = false;

            return This;
        }

        /// <summary>
        /// Disables request validation for property.
        /// </summary>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> AllowHtml()
        {
            Item.RequestValidationEnabled = false;

            return This;
        }

        /// <summary>
        /// Enebles request validation for property.
        /// </summary>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> DisallowHtml()
        {
            Item.RequestValidationEnabled = true;

            return This;
        }

        /// <summary>
        /// Shows the value in display mode.
        /// </summary>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> ShowForDisplay()
        {
            Item.ShowForDisplay = true;

            return This;
        }

        /// <summary>
        /// Hides the value in display mode.
        /// </summary>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> HideForDisplay()
        {
            Item.ShowForDisplay = false;

            return This;
        }

        /// <summary>
        /// Shows the value in edit mode.
        /// </summary>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> ShowForEdit()
        {
            Item.ShowForEdit = true;

            return This;
        }

        /// <summary>
        /// Hides the value in edit mode.
        /// </summary>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> HideForEdit()
        {
            Item.ShowForEdit = false;

            return This;
        }

        /// <summary>
        /// Shows the value in both display and edit mode.
        /// </summary>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> Show()
        {
            Item.ShowForDisplay = true;
            Item.ShowForEdit = true;

            return This;
        }

        /// <summary>
        /// Hides the value in both display and edit mode.
        /// </summary>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> Hide()
        {
            Item.ShowForDisplay = false;
            Item.ShowForEdit = false;

            return This;
        }

        /// <summary>
        /// Sets the display text when the value is null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> NullDisplayText(string value)
        {
            return NullDisplayText(() => value);
        }

        /// <summary>
        /// Sets the display text when the value is null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> NullDisplayText(Func<string> value)
        {
            Item.NullDisplayText = value;

            return This;
        }

        /// <summary>
        /// Sets the Watermark.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> Watermark(string value)
        {
            return Watermark(() => value);
        }

        /// <summary>
        /// Sets the Watermark.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> Watermark(Func<string> value)
        {
            Item.Watermark = value;

            return This;
        }

        /// <summary>
        /// Sets the order
        /// </summary>
        /// <param name="value">The order</param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> Order(int value)
        {
            Item.Order = value;
            return This;
        }

        /// <summary>
        /// Sets the format in display mode.
        /// </summary>
        /// <param name="format">The value.</param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> DisplayFormat(string format)
        {
            return DisplayFormat(() => format);
        }

        /// <summary>
        /// Sets the format in display mode.
        /// </summary>
        /// <param name="format">The value.</param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> DisplayFormat(Func<string> format)
        {
            Item.DisplayFormat = format;

            return This;
        }

        /// <summary>
        /// Sets the format in edit mode.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> EditFormat(string format)
        {
            return EditFormat(() => format);
        }

        /// <summary>
        /// Sets the format in edit mode.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> EditFormat(Func<string> format)
        {
            Item.EditFormat = format;

            return This;
        }

        /// <summary>
        /// Sets format for both display and edit mode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> Format(string value)
        {
            return Format(() => value);
        }

        /// <summary>
        /// Sets format for both display and edit mode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> Format(Func<string> value)
        {
            Item.DisplayFormat = Item.EditFormat = value;

            return This;
        }

        /// <summary>
        /// Indicates to apply format in edit mode.
        /// </summary>
        /// <returns></returns>
        public ModelMetadataItemBuilder<TValue> ApplyFormatInEditMode()
        {
            Item.ApplyFormatInEditMode = true;

            return This;
        }

        /// <summary>
        /// Marks the value as required.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="errorMessageResourceType">Type of the error message resource.</param>
        /// <param name="errorMessageResourceName">Name of the error message resource.</param>
        /// <returns></returns>
        protected virtual ModelMetadataItemBuilder<TValue> Required(Func<string> errorMessage, Type errorMessageResourceType, string errorMessageResourceName)
        {
            Item.IsRequired = true;

            var requiredValidation = Item.GetValidationOrCreateNew<RequiredValidationMetadata>();

            requiredValidation.ErrorMessage = errorMessage;
            requiredValidation.ErrorMessageResourceType = errorMessageResourceType;
            requiredValidation.ErrorMessageResourceName = errorMessageResourceName;

            return This;
        }

        /// <summary>
        /// Sets the other property that the value must match.
        /// </summary>
        /// <param name="otherProperty">The other property.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="errorMessageResourceType">Type of the error message resource.</param>
        /// <param name="errorMessageResourceName">Name of the error message resource.</param>
        /// <returns></returns>
        protected virtual ModelMetadataItemBuilder<TValue> Compare(string otherProperty, Func<string> errorMessage, Type errorMessageResourceType, string errorMessageResourceName)
        {
            var compareValidation = Item.GetValidationOrCreateNew<CompareValidationMetadata>();

            compareValidation.OtherProperty = otherProperty;
            compareValidation.ErrorMessage = errorMessage;
            compareValidation.ErrorMessageResourceType = errorMessageResourceType;
            compareValidation.ErrorMessageResourceName = errorMessageResourceName;

            return This;
        }
    }
}