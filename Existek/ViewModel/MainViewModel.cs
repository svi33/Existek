using Existek.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Existek.ViewModel
{
    class MainViewModel : DependencyObject
    {
        private DirectoryInfoWrapper selectedFolder = null;
        private bool controling = false;

        private RelayCommand addGroup;
        public RelayCommand AddGroup
        {
            get
            {
                return addGroup ??
                  (addGroup = new RelayCommand(obj =>
                  {
                      if (selectedFolder != null)
                      {
                          if (selectedFolder.Data is ControlInGroup) { return; }
                          DirectoryInfoWrapper _treeNodes = new DirectoryInfoWrapper(new GroupInTree());
                          _treeNodes.PropertyChanged += new PropertyChangedEventHandler(Start_PropertyChanged);
                          _treeNodes.Parent = selectedFolder;
                          _treeNodes.Type=_treeNodes.Data.getRole();
                          selectedFolder.Children.Add(_treeNodes);

                      }
                      else 
                      {
                          DirectoryInfoWrapper Start = new DirectoryInfoWrapper(new GroupInTree());
                          Start.PropertyChanged += new PropertyChangedEventHandler(Start_PropertyChanged);
                          CreateChildren(Start);
                          Roots.Add(Start);
                      }
                  }));
            }
        }

        private RelayCommand addControl;
        public RelayCommand AddControl
        {
            get
            {
                return addControl ??
                  (addControl = new RelayCommand(obj =>
                  {
                      Mouse.OverrideCursor = Cursors.Cross;
                      controling = true;
                  }));
            }
        }

        private RelayCommand escCommand;
        public RelayCommand EscCommand
        {
            get
            {
                return escCommand ??
                  (escCommand = new RelayCommand(obj =>
                  {
                      Mouse.OverrideCursor = null;
                      controling = false;
                  }));
            }
        }

        private RelayCommand mouseUpCommand;
        public RelayCommand MouseUpCommand
        {
            get
            {

                return mouseUpCommand ??
                  (mouseUpCommand = new RelayCommand(obj =>
                  {
                      if (controling)
                      {

                          if (selectedFolder != null)
                          {
                              GroupInTree Parent;
                              DirectoryInfoWrapper ParentFolder;
                              if (selectedFolder.Data is GroupInTree)
                              {
                                  Parent=(GroupInTree)selectedFolder.Data;
                                  ParentFolder = selectedFolder;
                                  
                              }
                              else
                              {
                                  Parent=(GroupInTree)selectedFolder.Parent.Data;
                                  ParentFolder = selectedFolder.Parent;
                              }
                              DirectoryInfoWrapper _treeNodes = new DirectoryInfoWrapper(new ControlInGroup(Parent,selectedFolder.Data));
                              _treeNodes.Type = _treeNodes.Data.getRole();
                              _treeNodes.PropertyChanged += new PropertyChangedEventHandler(Start_PropertyChanged);
                              _treeNodes.Parent = ParentFolder;
                              ParentFolder.Children.Add(_treeNodes);
                          }
                          Mouse.OverrideCursor = null;
                          controling = false;
                      }
                  }));
            }
        }

        private RelayCommand highlight;
        public RelayCommand Highlight
        {
            get
            {
                return highlight ??
                  (highlight = new RelayCommand(obj =>
                  {
                      if (selectedFolder != null)
                      {
                          selectedFolder.IsSelected = false;
                          selectedFolder = null;
                      }

                  }));
            }
        }

        private RelayCommand removeItem;
        public RelayCommand RemoveItem
        {
            get
            {
                return removeItem ??
                  (removeItem = new RelayCommand(obj =>
                  {
                      if (selectedFolder != null)
                      {

                          if (selectedFolder.Parent != null)
                          {
                              DirectoryInfoWrapper Parent = selectedFolder.Parent;
                              Parent.Children.Remove(selectedFolder);

                          }
                          else
                          {
                              Roots.Remove(selectedFolder);
                          }
                          selectedFolder = null;
                      }
                  }));
            }
        }

        public ObservableCollection<DirectoryInfoWrapper> Roots
        {
            get { return (ObservableCollection<DirectoryInfoWrapper>)GetValue(RootsProperty); }
            set { SetValue(RootsProperty, value); }
        }

        public static readonly DependencyProperty RootsProperty =
            DependencyProperty.Register("Roots", typeof(ObservableCollection<DirectoryInfoWrapper>), typeof(MainViewModel), new UIPropertyMetadata(null));

        public IEnumerable<INode> Files
        {
            get { return (ObservableCollection<INode>)GetValue(FilesProperty); }
            set { SetValue(FilesProperty, value); }
        }
        public static readonly DependencyProperty FilesProperty =
         DependencyProperty.Register("Files", typeof(IEnumerable<INode>), typeof(MainViewModel), new UIPropertyMetadata(null));

        public MainViewModel()
        {
            Roots = new ObservableCollection<DirectoryInfoWrapper>();
        }

        void Start_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            DirectoryInfoWrapper wrapper = sender as DirectoryInfoWrapper;
            if (wrapper != null)
            {
                selectedFolder = wrapper;
                if (e.PropertyName == "IsSelected")
                {
                    Files = wrapper.Data.children;
                    
                }
                if (e.PropertyName == "IsExpanded")
                {
                    
                    foreach (var item in wrapper.Children)
                    {
                        
                        CreateChildren(item);

                    }
                }
            }
        }

        private void CreateChildren(DirectoryInfoWrapper p_wrapper)
        {
            if (p_wrapper.Children == null)
            {
                p_wrapper.Children = new ObservableCollection<DirectoryInfoWrapper>();
                try
                {
                    foreach (var directory in p_wrapper.Data.children)
                    {
                        DirectoryInfoWrapper childWrapper = new DirectoryInfoWrapper(directory)
                        {
                            Data = directory
                        };
                        childWrapper.PropertyChanged += Start_PropertyChanged;
                        p_wrapper.Children.Add(
                            childWrapper
                            );
                    }
                }
                catch
                {
                }
            }
        }
    }
}
