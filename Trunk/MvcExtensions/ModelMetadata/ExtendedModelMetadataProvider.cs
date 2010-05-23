#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// Defines a metadata provider which supports fluent registration.
    /// </summary>
    public class ExtendedModelMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        private readonly IModelMetadataRegistry registry;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedModelMetadataProvider"/> class.
        /// </summary>
        /// <param name="registry">The registry.</param>
        public ExtendedModelMetadataProvider(IModelMetadataRegistry registry)
        {
            Invariant.IsNotNull(registry, "registry");

            this.registry = registry;
        }

        /// <summary>
        /// Gets a <see cref="T:System.Web.Mvc.ModelMetadata"/> object for each property of a model.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="containerType">The type of the container.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Mvc.ModelMetadata"/> object for each property of a model.
        /// </returns>
        public override IEnumerable<ModelMetadata> GetMetadataForProperties(object container, Type containerType)
        {
            Invariant.IsNotNull(containerType, "containerType");

            IDictionary<string, ModelMetadataItem> metadataItems = registry.GetModelPropertiesMetadata(containerType);

            if ((metadataItems == null) || (metadataItems.Count == 0))
            {
                return base.GetMetadataForProperties(container, containerType);
            }

            IList<ModelMetadata> list = new List<ModelMetadata>();

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(containerType))
            {
                ModelMetadataItem metadata;
                metadataItems.TryGetValue(descriptor.Name, out metadata);

                PropertyDescriptor tempDescriptor = descriptor;

                ModelMetadata modelMetadata = CreatePropertyMetadata(containerType, tempDescriptor.Name, tempDescriptor.PropertyType, metadata, () => tempDescriptor.GetValue(container));

                list.Add(modelMetadata);
            }

            return list;
        }

        /// <summary>
        /// Gets metadata for the specified property.
        /// </summary>
        /// <param name="modelAccessor">The model accessor.</param>
        /// <param name="containerType">The type of the container.</param>
        /// <param name="propertyName">The property to get the metadata model for.</param>
        /// <returns>
        /// The metadata model for the specified property.
        /// </returns>
        public override ModelMetadata GetMetadataForProperty(Func<object> modelAccessor, Type containerType, string propertyName)
        {
            Invariant.IsNotNull(containerType, "containerType");
            Invariant.IsNotNull(propertyName, "propertyName");

            ModelMetadataItem metadataItem = registry.GetModelPropertyMetadata(containerType, propertyName);

            if (metadataItem != null)
            {
                return base.GetMetadataForProperty(modelAccessor, containerType, propertyName);
            }

            PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(containerType)
                                                                  .Cast<PropertyDescriptor>()
                                                                  .FirstOrDefault(property => property.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

            if (propertyDescriptor == null)
            {
                throw new ArgumentException(string.Format(Culture.Current, ExceptionMessages.ThePropertyNameOfTypeCouldNotBeFound, containerType.FullName, propertyName));
            }

            return CreatePropertyMetadata(containerType, propertyName, propertyDescriptor.PropertyType, metadataItem, modelAccessor);
        }

        /// <summary>
        /// Gets metadata for the specified model accessor and model type.
        /// </summary>
        /// <param name="modelAccessor">The model accessor.</param>
        /// <param name="modelType">They type of the model.</param>
        /// <returns>The metadata.</returns>
        public override ModelMetadata GetMetadataForType(Func<object> modelAccessor, Type modelType)
        {
            ModelMetadataItem metadataItem = registry.GetModelMetadata(modelType);

            if (metadataItem == null)
            {
                return base.GetMetadataForType(modelAccessor, modelType);
            }

            return CreateModelMetadata(modelType, modelAccessor, metadataItem);
        }

        private ModelMetadata CreatePropertyMetadata(Type containerType, string propertyName, Type propertyType, ModelMetadataItem propertyMetadata, Func<object> modelAccessor)
        {
            ModelMetadata modelMetadata = new ExtendedModelMetadata(this, containerType, modelAccessor, propertyType, propertyName, propertyMetadata);

            if (propertyMetadata != null)
            {
                Copy(propertyMetadata, modelMetadata);
            }

            return modelMetadata;
        }

        private ModelMetadata CreateModelMetadata(Type modelType, Func<object> modelAccessor, ModelMetadataItem metadataItem)
        {
            ModelMetadata modelMetadata = new ExtendedModelMetadata(this, null, modelAccessor, modelType, null, metadataItem);

            if (metadataItem != null)
            {
                Copy(metadataItem, modelMetadata);
            }

            return modelMetadata;
        }

        private static void Copy(ModelMetadataItem metadataItem, ModelMetadata metadata)
        {
            metadata.ShowForDisplay = metadataItem.ShowForDisplay;
            metadata.DisplayName = !string.IsNullOrEmpty(metadataItem.DisplayName) ? metadataItem.DisplayName : metadata.DisplayName;
            metadata.ShortDisplayName = !string.IsNullOrEmpty(metadataItem.ShortDisplayName) ? metadataItem.ShortDisplayName : metadata.ShortDisplayName;
            metadata.TemplateHint = !string.IsNullOrEmpty(metadataItem.TemplateName) ? metadataItem.TemplateName : metadata.TemplateHint;
            metadata.Description = !string.IsNullOrEmpty(metadataItem.Description) ? metadataItem.Description : metadata.Description;
            metadata.NullDisplayText = !string.IsNullOrEmpty(metadataItem.NullDisplayText) ? metadataItem.NullDisplayText : metadata.NullDisplayText;
            metadata.Watermark = !string.IsNullOrEmpty(metadataItem.Watermark) ? metadataItem.Watermark : metadata.Watermark;

            if (metadataItem.HideSurroundingHtml.HasValue)
            {
                metadata.HideSurroundingHtml = metadataItem.HideSurroundingHtml.Value;
            }

            if (metadataItem.IsReadOnly.HasValue)
            {
                metadata.IsReadOnly = metadataItem.IsReadOnly.Value;
            }

            if (metadataItem.IsRequired.HasValue)
            {
                metadata.IsRequired = metadataItem.IsRequired.Value;
            }

            if (metadataItem.ShowForEdit.HasValue)
            {
                metadata.ShowForEdit = metadataItem.ShowForEdit.Value;
            }
            else
            {
                metadata.ShowForEdit = !metadata.IsReadOnly;
            }

            IModelMetadataFormattableItem formattableItem = metadataItem as IModelMetadataFormattableItem;

            if (formattableItem != null)
            {
                metadata.DisplayFormatString = formattableItem.DisplayFormat;

                if (formattableItem.ApplyFormatInEditMode && metadata.ShowForEdit)
                {
                    metadata.EditFormatString = formattableItem.EditFormat;
                }
            }

            StringMetadataItem stringMetadataItem = metadataItem as StringMetadataItem;

            if (stringMetadataItem != null)
            {
                metadata.ConvertEmptyStringToNull = stringMetadataItem.ConvertEmptyStringToNull;
            }
        }
    }
}