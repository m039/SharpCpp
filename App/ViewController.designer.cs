// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace CSharpCpp
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSTextField LabelAboveGeneratedBody { get; set; }

		[Outlet]
		AppKit.NSTextField LabelAboveGeneratedHeader { get; set; }

		[Outlet]
		AppKit.NSTextField LabelAboveInputCode { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (LabelAboveInputCode != null) {
				LabelAboveInputCode.Dispose ();
				LabelAboveInputCode = null;
			}

			if (LabelAboveGeneratedHeader != null) {
				LabelAboveGeneratedHeader.Dispose ();
				LabelAboveGeneratedHeader = null;
			}

			if (LabelAboveGeneratedBody != null) {
				LabelAboveGeneratedBody.Dispose ();
				LabelAboveGeneratedBody = null;
			}
		}
	}
}
