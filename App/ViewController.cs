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

            LabelAboveInputCode.StringValue = "C# goes here";
            InputCode.TextDidChange += OnTextChanged;
            DragAndDropArea.DragAndDropHandler += OnDragAndDrop;

            InputCode.Value = TestData.InputCode1;
            UpdateTextFields(TestData.InputCode1);
        }

        public void OnTextChanged(object sender, EventArgs args)
        {
            UpdateTextFields(InputCode.Value);
        }

        public void OnDragAndDrop(object sender, DragAndDropView.DragAndDropEventArgs args)
        {
            InputCode.Value = args.Text;
            UpdateTextFields(args.Text);
        }

        private void UpdateTextFields(string inputCode)
        {
            GeneratedHeader.Value = GeneratedBody.Value = "";

            try {
                var files = Transpiler.compileCSharpToCpp(inputCode);
                if (files.Length > 2) {
                    throw new Exception("Unsupported");
                }

                foreach (var file in files) {
                    switch (file.Type) {
                        case TFile.TFileType.SOURCE:
                            GeneratedBody.Value = file.Content;
                            LabelAboveGeneratedBody.StringValue = file.ToString();
                            break;
                        case TFile.TFileType.HEADER:
                            GeneratedHeader.Value = file.Content;
                            LabelAboveGeneratedHeader.StringValue = file.ToString();
                            break;
                    }
                }

            } catch (TException) {
                GeneratedHeader.Value = GeneratedBody.Value = "";
            }
        }

        public override NSObject RepresentedObject {
            get {
                return base.RepresentedObject;
            }
            set {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }
    }
}
