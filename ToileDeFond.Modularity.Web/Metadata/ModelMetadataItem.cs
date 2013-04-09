using System;

namespace ToileDeFond.Modularity.Web.Metadata
{
    public class ModelMetadataItem
    {
        private readonly Func<object> _modelAccessor;
        private readonly Type _modelType;
        private readonly Type _containerType;
        private readonly string _propertyName;

        public ModelMetadataItem(Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
        {
            _modelAccessor = modelAccessor;
            _modelType = modelType;
            _containerType = containerType;
            _propertyName = propertyName;
            ShowForDisplay = true;
        }


        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the short name of the display.
        /// </summary>
        /// <value>The short name of the display.</value>
        public string ShortDisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the template.
        /// </summary>
        /// <value>The name of the template.</value>
        public string TemplateHint
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the whether associate model is read only.
        /// </summary>
        /// <value>The is read only.</value>
        public bool? IsReadOnly
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the whether associate model is required.
        /// </summary>
        /// <value>The is required.</value>
        public bool? IsRequired
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the hide surrounding HTML.
        /// </summary>
        /// <value>The hide surrounding HTML.</value>
        public bool? HideSurroundingHtml
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the request validation enabled.
        /// </summary>
        /// <value>The allow HTML.</value>
        public bool? RequestValidationEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show for display.
        /// </summary>
        /// <value><c>true</c> if [show for display]; otherwise, <c>false</c>.</value>
        public bool ShowForDisplay
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show for edit.
        /// </summary>
        /// <value>The show for edit.</value>
        public bool? ShowForEdit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the null display text.
        /// </summary>
        /// <value>The null display text.</value>
        public string NullDisplayText
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the watermark.
        /// </summary>
        /// <value>The watermark.</value>
        public string Watermark
        {
            get;
            set;
        }

        ///// <summary>
        ///// Gets or the validations metadata.
        ///// </summary>
        ///// <value>The validations.</value>
        //public IList<IModelValidationMetadata> Validations
        //{
        //    get;
        //    private set;
        //}

        ///// <summary>
        ///// Gets the additional settings.
        ///// </summary>
        ///// <value>The additional settings.</value>
        //public IList<IModelMetadataAdditionalSetting> AdditionalSettings
        //{
        //    get;
        //    private set;
        //}

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>The order</value>
        public int? Order
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the display format.
        /// </summary>
        /// <value>The display format.</value>
        public string DisplayFormatString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the edit format.
        /// </summary>
        /// <value>The edit format.</value>
        public string EditFormatString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to apply format in edit mode.
        /// </summary>
        /// <value>
        /// <c>true</c> if [apply format in edit mode]; otherwise, <c>false</c>.
        /// </value>
        public bool ApplyFormatInEditMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the value would be converted to null when the value is empty string.
        /// </summary>
        /// <value>
        /// <c>true</c> if [convert empty string to null]; otherwise, <c>false</c>.
        /// </value>
        public bool? ConvertEmptyStringToNull
        {
            get;
            set;
        }

        public Func<object> ModelAccessor
        {
            get { return _modelAccessor; }
        }

        public Type ModelType
        {
            get { return _modelType; }
        }

        public Type ContainerType
        {
            get { return _containerType; }
        }

        public string PropertyName
        {
            get { return _propertyName; }
        }


        //public Func<object> ModelAccessor { get; set; }
        //public Type ContainerType { get; set; }
        //public string PropertyName { get; set; }
        //public Type ModelType { get; set; }
    }
}
