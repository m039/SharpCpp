using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace CSharpCpp
{
	public partial class EditTextView : AppKit.NSTextView
	{
		#region Constructors

		// Called when created from unmanaged code
		public EditTextView(IntPtr handle) : base(handle)
		{
			Initialize();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public EditTextView(NSCoder coder) : base(coder)
		{
			Initialize();
		}

		// Shared initialization code
		void Initialize()
		{
		}

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();

			UnregisterDraggedTypes();
		}

		#endregion

		public override string[] RegisteredDragTypes()
		{
			return null;
		}

		public override string[] AcceptableDragTypes()
		{
			return null;
		}

		public override NSDragOperation DraggingEntered(NSDraggingInfo sender)
		{
			return NSDragOperation.Copy;
		}
	}
}
