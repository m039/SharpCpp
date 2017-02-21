using System;

using AppKit;
using Foundation;

namespace CSharpCpp
{
	public partial class ViewController : NSViewController
	{
		public ViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// Do any additional setup after loading the view.

			LabelAboveInputCode.StringValue = "Huyak";
			InputCode.TextDidChange += OnTextChanged;
			DragAndDropArea.DragAndDropHandler += OnDragAndDrop;
		}

		public void OnTextChanged(object sender, EventArgs args)
		{
			GeneratedHeader.Value = InputCode.Value;
			GeneratedBody.Value = InputCode.Value;
		}

		public void OnDragAndDrop(object sender, DragAndDropView.DragAndDropEventArgs args)
		{
			InputCode.Value = GeneratedHeader.Value = GeneratedBody.Value = args.Text;
		}

		public override NSObject RepresentedObject
		{
			get
			{
				return base.RepresentedObject;
			}
			set
			{
				base.RepresentedObject = value;
				// Update the view, if already loaded.
			}
		}
	}
}
