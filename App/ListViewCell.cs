using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace SharpCpp
{
    public partial class ListViewCell : AppKit.NSTableCellView
    {
        #region Constructors

        // Called when created from unmanaged code
        public ListViewCell(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public ListViewCell(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        #endregion

        public void SetFilename(string filename, bool selected)
        {
            // Very simplified version how to select text
            var size = NSFont.SystemFontSize;

            if (selected) {
                Label.Font = NSFont.BoldSystemFontOfSize(size);
            } else {
                Label.Font = NSFont.SystemFontOfSize(size);
            }

            Label.StringValue = filename;
        }
    }
}
