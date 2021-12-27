﻿using Models;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using ViewModels.Commands;

namespace ViewModels {
    public class ApplicationViewModel : INotifyPropertyChanged {
        private SystemFile _rootDirectory;
        private string _selectedFolderPath;
        private bool _inProcess = false;

        public string SelectedFolderPath {
            get {
                return _selectedFolderPath;
            } 
            set {
                _selectedFolderPath = value;
                OnPropertyChanged("SelectedFolderPath");
            } 
        }

        public SystemFile RootDirectory {
            get {
                return _rootDirectory;
            }
            set {
                _rootDirectory = value;
                OnPropertyChanged("RootDirectory");
            }
        }

        #region Commands
        private ICommand _selectFolderCommand;
        public ICommand SelectFolderCommand {
            get {
                return _selectFolderCommand ??= new RelayCommand(x => {
                        using var folderDialog = new FolderBrowserDialog();
                        var result = folderDialog.ShowDialog();
                        if (result == DialogResult.OK) {
                            SelectedFolderPath = folderDialog.SelectedPath;
                        }
                    });
            }
        }

        private IAsyncCommand _analyzeFolderCommand;
        public IAsyncCommand AnalyzeFolderCommand {
            get {
                return _analyzeFolderCommand ??= new BaseAsyncCommand(GetFilesCommandAsync, CanExecute);
            }
        }

        private async Task GetFilesCommandAsync() {
            _inProcess = true;
            var info = new DirectoryInfo(SelectedFolderPath);
            var root = await Task.Run(() => SystemFile.GetSystemFileAsync(info));
            RootDirectory = root;
            _inProcess = false;
        }
        private bool CanExecute(object parameter) {
            return !string.IsNullOrEmpty(_selectedFolderPath) && !_inProcess;
        }
        #endregion


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
