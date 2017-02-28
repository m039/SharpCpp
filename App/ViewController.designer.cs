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
		SharpCpp.EditTextView GeneratedFileContent { get; set; }

		[Outlet]
		SharpCpp.EditTextView InputCode { get; set; }

		[Outlet]
		AppKit.NSTextField LabelAboveGeneratedFileContent { get; set; }

		[Outlet]
		AppKit.NSTextField LabelAboveInputCode { get; set; }

		[Outlet]
		AppKit.NSTextField LabelAboveListOfGeneratedFiles { get; set; }

		[Outlet]
		AppKit.NSTableView ListOfGeneratedFiles { get; set; }

		[Action ("OnAction:")]
		partial void OnAction (AppKit.NSTableView sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (DragAndDropArea != null) {
				DragAndDropArea.Dispose ();
				DragAndDropArea = null;
			}

			if (GeneratedFileContent != null) {
				GeneratedFileContent.Dispose ();
				GeneratedFileContent = null;
			}

			if (InputCode != null) {
				InputCode.Dispose ();
				InputCode = null;
			}

			if (LabelAboveGeneratedFileContent != null) {
				LabelAboveGeneratedFileContent.Dispose ();
				LabelAboveGeneratedFileContent = null;
			}

			if (LabelAboveInputCode != null) {
				LabelAboveInputCode.Dispose ();
				LabelAboveInputCode = null;
			}

			if (LabelAboveListOfGeneratedFiles != null) {
				LabelAboveListOfGeneratedFiles.Dispose ();
				LabelAboveListOfGeneratedFiles = null;
			}

			if (ListOfGeneratedFiles != null) {
				ListOfGeneratedFiles.Dispose ();
				ListOfGeneratedFiles = null;
			}
		}
	}
}
