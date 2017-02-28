using System;

using AppKit;
using Foundation;
using ObjCRuntime;
using System.Linq;
using System.IO;

namespace SharpCpp
{
    public partial class ViewController : NSViewController
    {

        const string CellIdentifier = "FilenameIdentifier";

        class TVDataSource : NSTableViewDataSource
        {
            internal GeneratedFile[] files;
            internal int selectedPosition = -1;

            internal int FilesCount()
            {
                return files != null? files.Length : 0;
            }

            public override nint GetRowCount(NSTableView tableView)
            {
                return FilesCount();
            }

            internal void UpdateSelectedPosition(GeneratedFile file)
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
                ListViewCell cell = (ListViewCell)tableView.MakeView(CellIdentifier, this);

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

                if (_tvDataSource.FilesCount() > 0) {
                    try {
                        // with same name
                        UpdateGeneratedFileViews(files.Single((arg) => arg.ToString() == Preferences.LastSelectedFilename));
                    } catch (InvalidOperationException) {
                        // first in the list
                        UpdateGeneratedFileViews(files[0]);
                    }
                }

            } catch (TException e) {
                Console.WriteLine(e);

                _tvDataSource.files = null;
                ListOfGeneratedFiles.ReloadData();
                UpdateGeneratedFileViews(null);
            }
        }

        void UpdateGeneratedFileViews(GeneratedFile file)
        {
            _tvDataSource.UpdateSelectedPosition(file);

            if (file == null) {
                Preferences.LastSelectedFilename = null;
                GeneratedFileContent.Value = "";
            } else {
                Preferences.LastSelectedFilename = file.ToString();
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

        #region Save Menu Actions

        const int SaveMenuTag = 1;

        const int SaveAsMenuTag = 2;

        [Action("validateMenuItem:")]
        public bool ValidateMenuItem(NSMenuItem item)
        {
            switch (item.Tag) {
                case SaveMenuTag:
                    return _tvDataSource.FilesCount() > 0;
                case SaveAsMenuTag:
                    return _tvDataSource.FilesCount() > 0;
            }

            return true;
        }

        [Export("saveDocument:")]
        void SaveDocument(NSObject sender)
        {
            var lastDirectory = Preferences.LastPickedSaveDirectory;
            if (lastDirectory == null || !Directory.Exists(lastDirectory)) {
                Preferences.LastPickedSaveDirectory = null;
                SaveDocumentAs(sender);
            } else {
                PerformSave(lastDirectory);
            }
        }

        [Export("saveDocumentAs:")]
        void SaveDocumentAs(NSObject sender)
        {
            var dialog = NSOpenPanel.OpenPanel;
            dialog.CanChooseFiles = false;
            dialog.CanChooseDirectories = true;

            if (dialog.RunModal() == 1 && dialog.Urls.Length > 0) {
                var pickedDirectoryPath = dialog.Urls[0].Path;
                Preferences.LastPickedSaveDirectory = pickedDirectoryPath;
                PerformSave(pickedDirectoryPath);
            }
        }

        /// <summary>
        /// Save all generated files to the directory, without any warnings.
        /// </summary>
        void PerformSave(string directoryPath)
        {
            foreach (var file in _tvDataSource.files) {
                var filePath = Path.Combine(directoryPath, file.ToString());
                File.WriteAllText(filePath, file.Content);
            }
        }

        #endregion
    }
}
