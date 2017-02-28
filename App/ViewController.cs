using System;

using AppKit;
using Foundation;
using ObjCRuntime;
using System.Linq;

namespace SharpCpp
{
    public partial class ViewController : NSViewController
    {
        
        const string CellIdentifier = "FilenameIdentifier";

        class TVDataSource : NSTableViewDataSource
        {
            internal TFile[] files;
            internal int selectedPosition = -1;

            public override nint GetRowCount(NSTableView tableView)
            {
                return files != null? files.Length : 0;
            }

            internal void UpdateSelectedPosition(TFile file)
            {
                if (files != null) {
                    for (var i = 0; i < files.Length; i++) {
                        if (file == files[i]) {
                            selectedPosition = i;
                            return;
                        }
                    }
                }

                selectedPosition = -1;
            }
       }

        class TVDelegate : NSTableViewDelegate
        {

            TVDataSource _dataSource;

            public TVDelegate(TVDataSource dataSource)
            {
                _dataSource = dataSource;
            }

            public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
            {
                ListViewCell cell = (ListViewCell) tableView.MakeView(CellIdentifier, this);

                cell.SetFilename(
                    _dataSource.files[row].ToString(),
                    _dataSource.selectedPosition == row
                );

                return cell;
            }
        }

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        TVDataSource _tvDataSource;
        string _selectedFilename;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            InitTableView();

            LabelAboveInputCode.StringValue = "C# goes here";
            LabelAboveListOfGeneratedFiles.StringValue = "List of generated files";
            LabelAboveGeneratedFileContent.StringValue = "C++ is here";
            InputCode.TextDidChange += OnTextChanged;
            DragAndDropArea.DragAndDropHandler += OnDragAndDrop;

            InputCode.Value = TestData.InputCode;
            UpdateTextFields(TestData.InputCode);
        }

        void InitTableView()
        {
            // InitList

            _tvDataSource = new TVDataSource();
            var tvDelegate = new TVDelegate(_tvDataSource);

            var nib = new NSNib("ListViewCell", NSBundle.MainBundle);

            ListOfGeneratedFiles.RegisterNib(nib, CellIdentifier);
            ListOfGeneratedFiles.Delegate = tvDelegate;
            ListOfGeneratedFiles.DataSource = _tvDataSource;
            ListOfGeneratedFiles.ReloadData();
        }

        partial void OnAction(NSTableView sender)
        {
            var files = _tvDataSource.files;
            if (files != null && files.Length > sender.SelectedRow && sender.SelectedRow >= 0) {
                UpdateGeneratedFileViews(files[sender.SelectedRow]);

                // change selected row if needed
                ListOfGeneratedFiles.ReloadData();
            }
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

        #region Utils

        void UpdateTextFields(string inputCode)
        {
            try {
                var files = Transpiler.compileCSharpToCpp(inputCode);

                _tvDataSource.files = files;
                ListOfGeneratedFiles.ReloadData();

                try {
                    // with same name
                    UpdateGeneratedFileViews(files.Single((arg) => arg.ToString() == _selectedFilename));
                } catch (InvalidOperationException) {
                    // firs in the list
                    UpdateGeneratedFileViews(files[0]);
                }

            } catch (TException e) {
                Console.WriteLine(e);
            }
        }

        void UpdateGeneratedFileViews(TFile file)
        {
            _tvDataSource.UpdateSelectedPosition(file);

            if (file == null) {
                _selectedFilename = null;
                GeneratedFileContent.Value = null;
            } else {
                _selectedFilename = file.ToString();
                GeneratedFileContent.Value = file.Content;
            }
        }

        #endregion

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
