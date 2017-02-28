using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace SharpCpp
{
    /// <summary>
    /// This class only disables Drag and Drop functionlity to prevent clashes
    /// with DragAndDropView. But sometimes these hacks don't work, I don't know why.
    /// </summary>
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

        #endregion

        public override void MouseDown(NSEvent theEvent)
        {
            base.MouseDown(theEvent);

            Window.MakeFirstResponder(this);
        }

#if true
        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

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
#endif
    }
}
