using System;
using System.ComponentModel;
using System.Windows.Forms;

using Origam.Schema;
using Origam.Schema.EntityModel;
using Origam.Schema.LookupModel;
using Origam.Workbench.Services;
using System.Drawing;

namespace Origam.Gui.Win
{
	/// <summary>
	/// Summary description for AsOptionButton.
	/// </summary>
	public class AsRadioButton : RadioButton, IAsControl
	{
		private IPersistenceService _persistence = ServiceManager.Services.GetService(typeof(IPersistenceService)) as IPersistenceService;

		public AsRadioButton() : base()
		{
            this.CheckedChanged += new EventHandler(AsRadioButton_CheckedChanged);
            this.BackColor = Color.Transparent;
        }

		public bool ReadOnly
		{
			get
			{
				return ! this.Enabled;
			}
			set
			{
				this.Enabled = ! value;
			}
		}

		private Guid _lookupId;
		[Browsable(false)]
		public Guid LookupId
		{
			get
			{
				return _lookupId;
			}
			set
			{
				_lookupId = value;
			}
		}

		private Guid _dataConstantId;
		[Browsable(false)]
		public Guid DataConstantId
		{
			get
			{
				return _dataConstantId;
			}
			set
			{
				_dataConstantId = value;
			}
		}

		[TypeConverter(typeof(Origam.Schema.EntityModel.DataLookupConverter))]
		public AbstractDataLookup DataLookup
		{
			get
			{
				return (AbstractDataLookup)_persistence.SchemaProvider.RetrieveInstance(typeof(AbstractDataLookup), new ModelElementKey(this.LookupId));
			}
			set
			{
				this.LookupId = (value == null ? Guid.Empty : (Guid)value.PrimaryKey["Id"]);
			}
		}

		[TypeConverter(typeof(DataConstantConverter))]
		public DataConstant ValueConstant
		{
			get
			{
				return (DataConstant)_persistence.SchemaProvider.RetrieveInstance(typeof(DataConstant), new ModelElementKey(this.DataConstantId));
			}
			set
			{
				this.DataConstantId = (value == null ? Guid.Empty : (Guid)value.PrimaryKey["Id"]);
			}
		}

        bool _settingValue = false;
        object _value = null;
        
        /// <summary>
        /// Value set by the databinding. All radio buttons in the group (bound to the same field)
        /// have the same value. The radio button will only be checked if the value equals to the
        /// data constant specifed under ValueConstant.
        /// </summary>
        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_settingValue) return;

                _settingValue = true;
                try
                {
                    _value = value;
                    this.Checked = this.Value != null && this.Value.Equals(GetValue());
                    OnValueChanged();
                }
                finally
                {
                    _settingValue = false;
                }
            }
        }

        /// <summary>
        /// Returns value specified by the ValueConstant in the current radio button.
        /// </summary>
        /// <returns></returns>
        private object GetValue()
        {
            IParameterService pms = ServiceManager.Services.GetService(
                typeof(IParameterService)) as IParameterService;

            return pms.GetParameterValue(this.DataConstantId);
        }

        public event EventHandler ValueChanged;
        public void OnValueChanged()
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, EventArgs.Empty);
            }
        }

        void AsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if(this.Checked)
            {
                this.Value = GetValue();
            }
        }

        #region IAsControl Members
		public string DefaultBindableProperty
		{
			get
			{
				return "Value";
			}
		}
		#endregion
	}
}
