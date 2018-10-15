using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

namespace Origam.Gui.Win
{
	/// <summary>
	/// Summary description for ImageBox.
	/// </summary>
	public class ImageBox : PictureBox, IAsCaptionControl, IAsControl
	{
		private ImageBoxSourceType _sourceType = ImageBoxSourceType.Blob;

		public ImageBox() : base()
		{
		}

		[Browsable(true)]
		public new int TabIndex
		{
			get
			{
				return base.TabIndex;
			}
			set
			{
				base.TabIndex = value;
			}
		}

		[Browsable(true)]
		public ImageBoxSourceType SourceType
		{
			get
			{
				return _sourceType;
			}
			set
			{
				_sourceType = value;
			}
		}


		private byte[] _imageData;
		public object ImageData
		{
			get
			{
				return _imageData;
			}
			set
			{
				if(value == DBNull.Value)
				{
					_imageData = null;
					this.Image = null;
				}
				else
				{
					Stream stream = null;
					try
					{
						switch(this.SourceType)
						{
							case ImageBoxSourceType.Blob:
								if(!(value is byte[]))
								{
									throw new Exception(ResourceUtils.GetString("ErrorImageData"));
								}

								_imageData = (byte[])value;
								stream = new MemoryStream(_imageData);
								break;

							case ImageBoxSourceType.Url:
								if(!(value is string))
								{
									throw new Exception(ResourceUtils.GetString("ErrorImageData"));
								}
								
                                string svalue = (string) value;
								string path = svalue;
								if (svalue.StartsWith("http"))
								{
									stream = HttpTools.SendRequest(
										path, "GET", null, null, null, null, null, null, true, null) as Stream;
								}
								{
									path = Path.Combine(Application.StartupPath, @"images\");
									path = Path.Combine(path, svalue);


									try
									{
										stream = new FileStream(path, FileMode.Open);
									}
									catch
									{
									}
									break;
								}
						}

						if(stream == null)
						{
							_imageData = null;
							this.Image = null;
						}
						else
						{
							System.Drawing.Bitmap bm = new System.Drawing.Bitmap(System.Drawing.Image.FromStream(stream));
							bm.MakeTransparent(System.Drawing.Color.Transparent);
							this.Image = bm;
						}
					}
					finally
					{
						if(stream != null) stream.Close();
					}
				}

				OnImageDataChanged(EventArgs.Empty);
			}
		}

		#region Events
		public event System.EventHandler imageDataChanged;
		protected virtual void OnImageDataChanged(EventArgs e)
		{
			if (this.imageDataChanged != null)
			{
				this.imageDataChanged(this, e);
			}
		}
		#endregion

		#region IAsControl Members

		public string DefaultBindableProperty
		{
			get
			{
				return "ImageData";
			}
		}

		#endregion

		#region IAsCaptionControl Members

		int _gridColumnWidth;
		[Category("(ORIGAM)")]
		[DefaultValue(100)]
		[Description(CaptionDoc.GridColumnWidthDescription)]
		public int GridColumnWidth
		{
			get
			{
				return _gridColumnWidth;
			}
			set
			{
				_gridColumnWidth = value;
			}
		}


		string _gridColumnCaption = "";
		[Category("(ORIGAM)")]
		public string GridColumnCaption
		{
			get
			{
				return _gridColumnCaption;
			}
			set
			{
				_gridColumnCaption = value;
			}
		}


		string _caption = "";
		public string Caption
		{
			get
			{
				return _caption;
			}
			set
			{
				_caption = value;
			}
		}

		CaptionPosition _captionPosition = CaptionPosition.Left;
		public CaptionPosition CaptionPosition
		{
			get
			{
				return _captionPosition;
			}
			set
			{
				_captionPosition = value;
			}
		}
		private int _captionLength = 100;
		[Category("(ORIGAM)")]
		public int CaptionLength
		{
			get
			{
				return _captionLength;
			}
			set
			{
				_captionLength = value;
			}
		}

		private bool _hideOnForm = false;
		public bool HideOnForm
		{
			get
			{
				return _hideOnForm;
			}
			set
			{
				_hideOnForm = value;

				if (value && !this.DesignMode)
				{
					this.Hide();
				}
			}
		}
		#endregion
	}
}
