// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SharpCpp
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		SharpCpp.DragAndDropView DragAndDropArea { get; set; }

		[Outlet]
		AppKit.NSTextView GeneratedBody { get; set; }

		[Outlet]
		AppKit.NSTextView GeneratedHeader { get; set; }

		[Outlet]
		AppKit.NSTextView InputCode { get; set; }

		[Outlet]
		AppKit.NSTextField LabelAboveGeneratedBody { get; set; }

		[Outlet]
		AppKit.NSTextField LabelAboveGeneratedHeader { get; set; }

		[Outlet]
		AppKit.NSTextField LabelAboveInputCode { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (DragAndDropArea != null) {
				DragAndDropArea.Dispose ();
				DragAndDropArea = null;
			}

			if (GeneratedBody != null) {
				GeneratedBody.Dispose ();
				GeneratedBody = null;
			}

			if (GeneratedHeader != null) {
				GeneratedHeader.Dispose ();
				GeneratedHeader = null;
			}

			if (InputCode != null) {
				InputCode.Dispose ();
				InputCode = null;
			}

			if (LabelAboveGeneratedBody != null) {
				LabelAboveGeneratedBody.Dispose ();
				LabelAboveGeneratedBody = null;
			}

			if (LabelAboveGeneratedHeader != null) {
				LabelAboveGeneratedHeader.Dispose ();
				LabelAboveGeneratedHeader = null;
			}

			if (LabelAboveInputCode != null) {
				LabelAboveInputCode.Dispose ();
				LabelAboveInputCode = null;
			}
		}
	}
}
