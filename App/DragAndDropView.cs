using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using MobileCoreServices;
using ObjCRuntime;

namespace CSharpCpp
{
	public partial class DragAndDropView : AppKit.NSView
	{
		public event DragAndDropEventHandler DragAndDropHandler;

		#region Constructors

		// Called when created from unmanaged code
		public DragAndDropView(IntPtr handle) : base(handle)
		{
			Initialize();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public DragAndDropView(NSCoder coder) : base(coder)
		{
			Initialize();
		}

		// Shared initialization code
		void Initialize()
		{
			RegisterForDraggedTypes(new string[] {
				NSPasteboard.NSUrlType
			});
		}

		#endregion

		public override NSDragOperation DraggingEntered(NSDraggingInfo sender)
		{
			if (IsCSharpFilePathUrl(GetFilePathUrl(sender)))
			{
				return NSDragOperation.Copy;
			}

			return NSDragOperation.None;
		}

		public override void DraggingEnded(NSDraggingInfo sender)
		{
			if (DragAndDropHandler != null)
			{
				var filePathUrl = GetFilePathUrl(sender);
				if (IsCSharpFilePathUrl(filePathUrl))
				{
					var args = new DragAndDropEventArgs
					{
						Text = System.IO.File.ReadAllText(filePathUrl.Path)
					};

					DragAndDropHandler(this, args);
				}
			}
		}

		public class DragAndDropEventArgs : EventArgs
		{
			public string Text { get; internal set; }
		}

		public delegate void DragAndDropEventHandler(object sender, DragAndDropEventArgs e);

		#region Utils

		/// <summary>
		/// Returns an absolute path to a dropped file or null.
		/// </summary>
		private static NSUrl GetFilePathUrl(NSDraggingInfo sender)
		{
			var pasteboard = sender.DraggingPasteboard;
			var classes = new[] { new Class(typeof(NSUrl)) };
			if (pasteboard.CanReadObjectForClasses(classes, null))
			{
				var urls = pasteboard.ReadObjectsForClasses(classes, null) as NSObject[];
				if (urls != null && urls.Length == 1)
				{
					return (NSUrl) urls[0];
				}
			}

			return null;
		}

		private static bool IsCSharpFilePathUrl(NSUrl url)
		{
			return url?.PathComponents
					   .Last()
					   .EndsWith(".cs", StringComparison.CurrentCulture) == true;
		}

		#endregion
	}
}
