using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Win32;
using System.IO;
using System.ComponentModel;
using System.Reflection;
//using System.Windows.Controls.Primitives;

namespace vrClusterConfig
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string defaultDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        static readonly string configFileExtention = "CAVE config file (*.cfg)|*.cfg";
        static readonly string applicationFileExtention = "CAVE VR application (*.exe)|*.exe";
        public Config currentConfig;
        public AppRunner appRunner;
        //string currentFileName = null;
        readonly string versionInfo = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        string windowTitle = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            appRunner = new AppRunner();
            launcherTab.DataContext = appRunner;
            logTab.DataContext = appRunner;
            appLogTextBox.DataContext = AppLogger.instance;
            SetDefaultConfig();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetTitle();
        }


        //Save config file
        public void SaveConfig(object sender, RoutedEventArgs e)
        {
            Save(false);
        }

        public void SaveAsConfig(object sender, RoutedEventArgs e)
        {
            Save(true);
        }

        private void Save(bool isSaveAs)
        {
            if (currentConfig.Validate())
            {
                try
                {
                    string currentFileName = RegistrySaver.ReadStringFromRegistry(RegistrySaver.configName);
                    if (!isSaveAs && File.Exists(currentFileName))
                    {
                        File.WriteAllText(currentFileName, currentConfig.CreateConfig());
                    }
                    else
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.Filter = configFileExtention;
                        if (saveFileDialog.ShowDialog() == true)
                        {
                            currentFileName = saveFileDialog.FileName;
                            RegistrySaver.AddRegistryValue(RegistrySaver.configName, currentFileName);
                            File.WriteAllText(currentFileName, currentConfig.CreateConfig());
                        }
                    }
                    SetTitle();
                    AppLogger.Add("Config saved to " + currentFileName);
                }
                catch (Exception exception)
                {
                    InfoDialog errorDialog = new InfoDialog("Error! \nCan not save configuration file. \nSee exception message in Log");
                    errorDialog.Owner = this;
                    errorDialog.Width = 350;
                    errorDialog.Height = 200;
                    errorDialog.Show();
                    AppLogger.Add("ERROR! Can not save config to file. EXCEPTION: " + exception.Message);
                }
            }
            else
            {
                InfoDialog errorDialog = new InfoDialog("Error! \nCan not save configuration file. \nWrong config. See errors in Log");
                errorDialog.Owner = this;
                errorDialog.Width = 350;
                errorDialog.Height = 200;
                errorDialog.Show();
                AppLogger.Add("ERROR! Can not save config to file. Errors in configuration");
            }
        }

        private void SetDefaultConfig()
        {
            string configPath = RegistrySaver.ReadStringFromRegistry(RegistrySaver.configName);
            if (string.IsNullOrEmpty(configPath))
            {
                CreateConfig();
                
            }
            else
            {
                ConfigFileParser(configPath);
            }
        }

        //Open Config File
        public void OpenConfig(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = configFileExtention;
            if (openFileDialog.ShowDialog() == true)
            {
                ConfigFileParser(openFileDialog.FileName);
            }
        }

        //Config file parser
        private void ConfigFileParser(string filePath)
        {
            // refactoring needed
            CreateConfig();
            List<string> inputLines = new List<string>();
            List<string> sceneNodeLines = new List<string>();
            List<string> screenLines = new List<string>();
            List<string> viewportLines = new List<string>();
            List<string> clusterNodeLines = new List<string>();
            List<string> cameraLines = new List<string>();
            List<string> generalLines = new List<string>();
            List<string> stereoLines = new List<string>();
            List<string> debugLines = new List<string>();
            try
            {
                foreach (string line in File.ReadLines(filePath))
                {
                    if (line == string.Empty || line.First() == '#')
                    {
                        //Do nothing
                    }
                    else
                    {
                        if (line.Contains("[input]"))
                        {
                            inputLines.Add(line);
                        }
                        if (line.Contains("[scene_node]"))
                        {
                            sceneNodeLines.Add(line);
                        }
                        if (line.Contains("[screen]"))
                        {
                            screenLines.Add(line);
                        }
                        if (line.Contains("[viewport]"))
                        {
                            viewportLines.Add(line);
                        }
                        if (line.Contains("[cluster_node]"))
                        {
                            clusterNodeLines.Add(line);
                        }
                        if (line.Contains("[camera]"))
                        {
                            cameraLines.Add(line);
                        }
                        if (line.Contains("[general]"))
                        {
                            generalLines.Add(line);
                        }
                        if (line.Contains("[stereo]"))
                        {
                            stereoLines.Add(line);
                        }
                        if (line.Contains("[debug]"))
                        {
                            debugLines.Add(line);
                        }
                        if (line.Contains("[render]"))
                        {
                            //todo 
                        }
                        if (line.Contains("[custom]"))
                        {
                            //todo 
                        }
                    }
                }
                foreach (string line in viewportLines)
                {
                    currentConfig.ViewportParse(line);
                }
                foreach (string line in generalLines)
                {
                    currentConfig.GeneralParse(line);
                }
                foreach (string line in stereoLines)
                {
                    currentConfig.StereoParse(line);
                }
                foreach (string line in debugLines)
                {
                    currentConfig.DebugParse(line);
                }
                foreach (string line in inputLines)
                {
                    currentConfig.InputsParse(line);
                }
                foreach (string line in cameraLines)
                {
                    currentConfig.CameraParse(line);
                }
                foreach (string line in sceneNodeLines)
                {
                    currentConfig.SceneNodeParse(line);
                }
                foreach (string line in screenLines)
                {
                    currentConfig.ScreenParse(line);
                }
                foreach (string line in clusterNodeLines)
                {
                    currentConfig.ClusterNodeParse(line);
                }

                currentConfig.sceneNodesView = currentConfig.ConvertSceneNodeList(currentConfig.sceneNodes);
                currentConfig.name = Path.GetFileNameWithoutExtension(filePath);
                AppLogger.Add("Config " + currentConfig.name + " loaded");
                RegistrySaver.AddRegistryValue(RegistrySaver.configName, filePath);
                
                
                //Set first items in listboxes and treeview as default if existed
                currentConfig.SelectFirstItems();
                RefreshUiControls();
                try
                {
                    ((CollectionViewSource)this.Resources["cvsInputTrackers"]).View.Refresh();
                }
                catch (NullReferenceException)
                {

                }
                //sceneNodeTrackerCb.SelectedIndex = -1;
                SetTitle();
            }
            catch (FileNotFoundException e)
            {
                AppLogger.Add("ERROR! Config " + currentConfig.name + "not found!");
            }
        }

        //crutch for refreshing all listboxes and comboboxes after binding
        private void RefreshUiControls()
        {
            inputsListBox.Items.Refresh();
            screensListBox.Items.Refresh();
            viewportsListBox.Items.Refresh();
            nodesListBox.Items.Refresh();
            activeNodesListBox.Items.Refresh();
            parentWallsCb.Items.Refresh();
            screensCb.Items.Refresh();
            viewportsCb.Items.Refresh();
            cameraTrackerIDCb.Items.Refresh();
            sceneNodeTrackerCb.Items.Refresh();
            sceneNodesTreeView.Items.Refresh();
        }

        //Setting title of widow.
        private void SetTitle()
        {
            windowTitle = currentConfig.name + " - vrCluster runner & configurator ver. " + versionInfo;
            this.Title = windowTitle;
        }

        void CreateConfig()
        {
            RegistrySaver.RemoveAllRegistryValues(RegistrySaver.configName);
            currentConfig = new Config();
            this.DataContext = currentConfig;
            //crutch. for refactoring
            currentConfig.selectedSceneNodeView = null;
            AppLogger.Add("New config inited");
            SetTitle();
        }

        public static void ConfigModifyIndicator()
        {
            if (!Application.Current.MainWindow.Title.StartsWith("*"))
            {
                Application.Current.MainWindow.Title = "*" + Application.Current.MainWindow.Title;
            }
        }

        //New config file
        public void NewConfig(object sender, RoutedEventArgs e)
        {
            CreateConfig();
        }

        //Exit app
        private void Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            YesNoDialog dialogResult = new YesNoDialog("Do you really want to close?");
            dialogResult.Owner = this;
            if (!(bool)dialogResult.ShowDialog())
            {
                e.Cancel = true;
            }
            base.OnClosing(e);
        }


        private void addNodeButton_Click(object sender, RoutedEventArgs e)
        {
            currentConfig.clusterNodes.Add(new ClusterNode());
            currentConfig.selectedNode = currentConfig.clusterNodes.LastOrDefault();
            nodesListBox.Items.Refresh();
            nodeIdTb.Focus();
            AppLogger.Add("New cluster node added");
            AppLogger.Add("WARNING! Change default values");
        }

        private void deleteNodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentConfig.selectedNode != null)
            {
                var id = currentConfig.selectedNode.id;
                int selectedIndex = currentConfig.clusterNodes.IndexOf(currentConfig.selectedNode);
                currentConfig.clusterNodes.RemoveAt(selectedIndex);
                currentConfig.selectedNode = currentConfig.clusterNodes.FirstOrDefault();

                AppLogger.Add("Cluster node " + id + " deleted");
            }
            nodesListBox.Items.Refresh();
        }

        private void isMasterCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (ClusterNode node in currentConfig.clusterNodes)
            {
                if (node == currentConfig.selectedNode)
                {
                    node.isMaster = true;
                    AppLogger.Add("Cluster node " + currentConfig.selectedNode.id + " set as master node");
                }
                else
                {
                    node.isMaster = false;
                }
               
            }
        }

        private void addscreenbutton_click(object sender, RoutedEventArgs e)
        {
            currentConfig.screens.Add(new Screen());
            currentConfig.selectedScreen = currentConfig.screens.LastOrDefault();
            screensListBox.Items.Refresh();
            screensCb.Items.Refresh();
            screenIdTb.Focus();
            AppLogger.Add("New screen added");
            AppLogger.Add("WARNING! Change default values");
        }

        private void deleteScreenButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentConfig.selectedScreen != null)
            {
                var id = currentConfig.selectedScreen.id;
                int selectedIndex = currentConfig.screens.IndexOf(currentConfig.selectedScreen);
                currentConfig.screens.RemoveAt(selectedIndex);
                currentConfig.selectedScreen = currentConfig.screens.FirstOrDefault();

                AppLogger.Add("Screen " + id + " deleted");
            }
            screensListBox.Items.Refresh();
            screensCb.Items.Refresh();
        }

        private void addViewportButton_Click(object sender, RoutedEventArgs e)
        {
            currentConfig.viewports.Add(new Viewport());
            currentConfig.selectedViewport = currentConfig.viewports.LastOrDefault();
            viewportsListBox.Items.Refresh();
            viewportsCb.Items.Refresh();
            viewportIdTb.Focus();
            AppLogger.Add("New viewport added");
            AppLogger.Add("WARNING! Change default values");
        }

        private void deleteViewportButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentConfig.selectedViewport != null)
            {
                var id = currentConfig.selectedViewport.id;
                int selectedIndex = currentConfig.viewports.IndexOf(currentConfig.selectedViewport);

                currentConfig.viewports.RemoveAt(selectedIndex);
                currentConfig.selectedViewport = currentConfig.viewports.FirstOrDefault();

                AppLogger.Add("Viewport " + id + " deleted");
            }
            viewportsListBox.Items.Refresh();
            viewportsCb.Items.Refresh();
        }


        private void addInputBtn_Click(object sender, RoutedEventArgs e)
        {
            InputTypeDialog dialogChooser = new InputTypeDialog();
            dialogChooser.Owner = this;
            dialogChooser.Title = "Select new input type";
            dialogChooser.ShowDialog();
            if (dialogChooser.DialogResult.Value)
            {
                string type = dialogChooser.inputType;
                addInput(type);
                inputIdTb.Focus();
                AppLogger.Add("New " + type + " input added");
                AppLogger.Add("WARNING! Change default values");
            }
        }

        private void addInput(string type)
        {
            if (type != null)
            {
                if (type == "tracker")
                {
                    currentConfig.inputs.Add(new TrackerInput { id = "TrackerInputId", address = "TrackerInputName@127.0.0.1", locationX = "0", locationY = "0", locationZ = "0", rotationP = "0", rotationR = "0", rotationY = "0", front = "X", right = "Y", up = "-Z" });
                }
                else
                {
                    InputType currentType = (InputType)System.Enum.Parse(typeof(InputType), type);
                    currentConfig.inputs.Add(new BaseInput { id = "InputId", type = currentType, address = "InputName@127.0.0.1" });
                }
                currentConfig.selectedInput = currentConfig.inputs.LastOrDefault();
                RefreshUiControls();
                //inputsListBox.Items.Refresh();
                try
                {
                    ((CollectionViewSource)this.Resources["cvsInputTrackers"]).View.Refresh();
                }
                catch (NullReferenceException)
                {

                }
            }
        }

        private void delInputBtn_Click(object sender, RoutedEventArgs e)
        {
            if (currentConfig.selectedInput != null)
            {
                var id = currentConfig.selectedInput.id;
                int selectedIndex = currentConfig.inputs.IndexOf(currentConfig.selectedInput);
                currentConfig.inputs.RemoveAt(selectedIndex);
                AppLogger.Add(currentConfig.selectedInput.type + " input " + id + " deleted");
                currentConfig.selectedInput = currentConfig.inputs.FirstOrDefault();
                try
                {
                    ((CollectionViewSource)this.Resources["cvsInputTrackers"]).View.Refresh();
                }
                catch (NullReferenceException)
                {

                }
                RefreshUiControls();
            }
            //inputsListBox.Items.Refresh();
        }

        //Sets selected treeview item
        private void sceneNodesTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            currentConfig.selectedSceneNodeView = (SceneNodeView)sceneNodesTreeView.SelectedItem;


            if (currentConfig.selectedSceneNodeView != null)
            {
                if (currentConfig.inputs.Contains(currentConfig.selectedSceneNodeView.node.tracker))
                {
                    sceneNodeTrackerCb.SelectedItem = currentConfig.selectedSceneNodeView.node.tracker;
                }
                else
                {
                    sceneNodeTrackerCb.SelectedItem = null;
                }
            }
        }

        //Delete Scene Node
        private void deleteSceneNode_Click(object sender, RoutedEventArgs e)
        {
            if (sceneNodesTreeView.SelectedItem != null)
            {
                SceneNodeView item = (SceneNodeView)sceneNodesTreeView.SelectedItem;
                var id = item.node.id;
                currentConfig.DeleteSceneNode(item);
                currentConfig.selectedSceneNodeView = currentConfig.sceneNodesView.FirstOrDefault();
                AppLogger.Add("Scene Node " + id + " deleted");
            }
            sceneNodesTreeView.Items.Refresh();
            parentWallsCb.Items.Refresh();
        }

        //Add Scene node
        private void addSceneNode_Click(object sender, RoutedEventArgs e)
        {
            SceneNode newItem = new SceneNode();
            if (currentConfig.selectedSceneNodeView != null)
            {
                newItem.parent = currentConfig.selectedSceneNodeView.node;
            }
            SceneNodeView newViewItem = new SceneNodeView(newItem);
            newViewItem.isSelected = true;
            currentConfig.sceneNodes.Add(newItem);

            SceneNodeView parentNode = currentConfig.FindParentNode(newViewItem);
            if (parentNode == null)
            {
                currentConfig.sceneNodesView.Add(newViewItem);
            }
            else
            {
                parentNode.children.Add(newViewItem);
                parentNode.isExpanded = true;
            }
            AppLogger.Add("New scene node added");
            AppLogger.Add("WARNING! Change default values");
            sceneNodesTreeView.Items.Refresh();
            sceneNodeIdTb.Focus();
        }

        //Drag and drop in TreeView implementation
        Point _lastMouseDown;
        SceneNodeView draggedItem, _target;
        bool isNode = true;
        private void treeView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.OriginalSource is Grid)
                {
                    isNode = false;
                }
                else
                {
                    _lastMouseDown = e.GetPosition(sceneNodesTreeView);
                    isNode = true;
                }
            }
        }

        private void treeView_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (!(e.OriginalSource is System.Windows.Controls.Primitives.Thumb))
                    {
                        Point currentPosition = e.GetPosition(sceneNodesTreeView);

                        if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) ||
                            (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                        {
                            if (isNode)
                            {
                                draggedItem = (SceneNodeView)sceneNodesTreeView.SelectedItem;
                            }
                            if (draggedItem != null)
                            {
                                DragDropEffects finalDropEffect =
                    DragDrop.DoDragDrop(sceneNodesTreeView,
                        sceneNodesTreeView.SelectedValue,
                                    DragDropEffects.Move);
                                //Checking target is not null and item is
                                //dragging(moving)
                                if (finalDropEffect == DragDropEffects.Move)
                                {
                                    if (_target == null)
                                    {
                                        CopyItem(draggedItem, _target);
                                        draggedItem = null;
                                    }
                                    else
                                    {
                                        // A Move drop was accepted
                                        if (!draggedItem.node.Equals(_target.node))

                                        {
                                            CopyItem(draggedItem, _target);
                                            _target = null;
                                            draggedItem = null;
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void treeView_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                Point currentPosition = e.GetPosition(sceneNodesTreeView);

                if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) ||
                   (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                {
                    // Verify that this is a valid drop and then store the drop target
                    SceneNodeView item = e.OriginalSource as SceneNodeView;
                    if (CheckDropTarget(draggedItem, item))
                    {
                        e.Effects = DragDropEffects.Move;
                    }
                    else
                    {
                        e.Effects = DragDropEffects.None;
                    }
                }
                e.Handled = true;
            }
            catch
            {

            }
        }

        private void treeView_Drop(object sender, DragEventArgs e)
        {
            try
            {
                SceneNodeView TargetItem = null;
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                if (sender is TreeViewItem)
                {
                    // Verify that this is a valid drop and then store the drop target
                    ContentPresenter presenter = e.Source as ContentPresenter;
                    TargetItem = presenter.Content as SceneNodeView;
                }

                if (draggedItem != null)
                {
                    _target = TargetItem;
                    e.Effects = DragDropEffects.Move;
                }
            }
            catch
            {
            }
        }


        private void CopyItem(SceneNodeView _sourceItem, SceneNodeView _targetItem)
        {
            //Alert if node sets as child of it's own child node
            if (_targetItem != null && _sourceItem.FindNodeInChildren(_targetItem) != null)
            {
                InfoDialog alertDialog = new InfoDialog("Scene node can not be a child node of own children!");
                alertDialog.Owner = this;
                alertDialog.Show();
            }
            else
            {

                //Asking user wether he want to drop the dragged TreeViewItem here or not
                YesNoDialog dialogResult = new YesNoDialog("Would you like to drop " + _sourceItem.node.id + " into " + _targetItem.node.id + "");
                dialogResult.Owner = this;
                if ((bool)dialogResult.ShowDialog())
                {
                    try
                    {

                        //finding Parent TreeViewItem of dragged TreeViewItem 
                        SceneNodeView ParentItem = currentConfig.FindParentNode(_sourceItem);
                        if (ParentItem == null)
                        {
                            ((List<SceneNodeView>)sceneNodesTreeView.ItemsSource).Remove(_sourceItem);
                        }
                        else
                        {
                            ParentItem.children.Remove(_sourceItem);
                        }
                        //adding dragged TreeViewItem in target TreeViewItem
                        if (_targetItem == null)
                        {
                            ((List<SceneNodeView>)sceneNodesTreeView.ItemsSource).Add(_sourceItem);
                            _sourceItem.node.parent = null;
                        }
                        else
                        {
                            _targetItem.children.Add(_sourceItem);
                            _sourceItem.node.parent = _targetItem.node;
                        }
                        //Set SceneNode of _targetItem as parent node for _sourceItem Scene Node
                    }
                    catch
                    {
                    }
                    sceneNodesTreeView.Items.Refresh();
                }
            }
        }

        //Crutch for deselecting all nodes. refactoring needed!!!!!
        private void sceneNodesTreeView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Grid && currentConfig.selectedSceneNodeView != null)
            {
                currentConfig.selectedSceneNodeView.isSelected = false;
                currentConfig.selectedSceneNodeView = null;
                sceneNodesTreeView.ItemsSource = null;
                sceneNodesTreeView.ItemsSource = currentConfig.sceneNodesView;

            }
            isNode = true;
        }

        private bool CheckDropTarget(SceneNodeView _sourceItem, object _targetItem)
        {
            SceneNodeView target = (SceneNodeView)_targetItem;
            //Check whether the target item is meeting your condition
            bool _isEqual = false;
            if (!_sourceItem.node.Equals(target.node))
            {
                _isEqual = true;
            }
            return _isEqual;

        }


        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {

            if (e.Item is TrackerInput)
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }

        private void configsCb_DropDownOpened(object sender, EventArgs e)
        {
            configsCb.Items.Refresh();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            appRunner.GenerateLogLevelsString();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            appRunner.GenerateLogLevelsString();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            appRunner.GenerateLogLevelsString();
        }

        private void runBtn_Click(object sender, RoutedEventArgs e)
        {
            appRunner.RunCommand();
        }

        //private List<string> GetSelectedListItems(ListBox listBox)
        //{
        //    List<string> selectedlistNames = listBox.SelectedItems.Cast<string>().ToList();
        //    return selectedlistNames;
        //}

        private void statusBtn_Click(object sender, RoutedEventArgs e)
        {
            appRunner.StatusCommand();
        }

        private void killBtn_Click(object sender, RoutedEventArgs e)
        {
            appRunner.KillCommand();
        }

        private void copyLogBtn_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard(commandTextBox.Text);
        }

        private void CopyToClipboard(string text)
        {
            if (text != String.Empty)
            {
                Clipboard.SetText(text);
            }
        }

        private void logsClearBtn_Click(object sender, RoutedEventArgs e)
        {
            YesNoDialog dialogResult = new YesNoDialog("Do you really want to clean all logs?");
            dialogResult.Owner = this;
            if ((bool)dialogResult.ShowDialog())
            {
                appRunner.CleanLogs();
            }
        }

        private void logsFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            appRunner.CollectLogs();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            InfoDialog aboutDialog = new InfoDialog("Application for configuration and launching VR Cluster version "+ versionInfo + "\n \u00a9 Copiright Pixela Labs. All rights reserved.\n more info http://vrcluster.io/");
            aboutDialog.Owner = this;
            aboutDialog.Width = 350;
            aboutDialog.Height = 200;
            aboutDialog.Show();
        }

        private void addAppButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = applicationFileExtention;
            if (openFileDialog.ShowDialog() == true)
            {
                string appPath = openFileDialog.FileName;
                appRunner.AddApplication(appPath);
                applicationsListBox.Items.Refresh();
            }
        }

        private void addActiveNodeButton_Click(object sender, RoutedEventArgs e)
        {

            InputStringDialog dialogChooser = new InputStringDialog();
            dialogChooser.Owner = this;
            dialogChooser.Title = "Input node address";
            dialogChooser.ShowDialog();
            if (dialogChooser.DialogResult.Value)
            {
                string node = dialogChooser.inputString;
                if (!string.IsNullOrEmpty(node))
                {
                    if (appRunner.AddNode(node))
                    {
                        activeNodesListBox.Items.Refresh();
                    }
                    else
                    {
                        InfoDialog wrongNodeDialog = new InfoDialog("Node with this address already exists. Active node must have a unique address");
                        wrongNodeDialog.Owner = this;
                        wrongNodeDialog.Width = 350;
                        wrongNodeDialog.Height = 200;
                        wrongNodeDialog.Show();
                    }

                }
            }
        }

        private void deleteActiveNodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (activeNodesListBox.SelectedItems.Count > 0)
            {
                YesNoDialog dialogResult = new YesNoDialog("Do you really want to delene selected nodes?");
                dialogResult.Owner = this;
                if ((bool)dialogResult.ShowDialog())
                {
                    List<ActiveNode> selectedActiveNodes = activeNodesListBox.SelectedItems.OfType<ActiveNode>().ToList();
                    appRunner.DeleteNodes(selectedActiveNodes);
                    activeNodesListBox.Items.Refresh();
                }
            }
        }

        private void deleteAppButton_Click(object sender, RoutedEventArgs e)
        {
            if (applicationsListBox.SelectedItem != null)
            {
                YesNoDialog dialogResult = new YesNoDialog("Do you really want to delene selected application?");
                dialogResult.Owner = this;
                if ((bool)dialogResult.ShowDialog())
                {
                    appRunner.DeleteApplication();
                    applicationsListBox.Items.Refresh();
                }
            }
        }

        private void addConfigButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = configFileExtention;
            if (openFileDialog.ShowDialog() == true)
            {
                string configPath = openFileDialog.FileName;
                if (!appRunner.configs.Exists(x=> x==configPath))
                {
                    appRunner.AddConfig(configPath);
                    configsCb.Items.Refresh();
                }
            }
        }

        private void copyAppLogBtn_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard(appLogTextBox.Text);
        }

        private void cleanAppLogBtn_Click(object sender, RoutedEventArgs e)
        {
            AppLogger.CleanLog();
        }


        private void configsCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            appRunner.ChangeConfigSelection(appRunner.selectedConfig);
        }

        private void deleteConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (configsCb.SelectedItem != null)
            {
                YesNoDialog dialogResult = new YesNoDialog("Do you really want to delete selected Config file?");
                dialogResult.Owner = this;
                if ((bool)dialogResult.ShowDialog())
                {
                    appRunner.DeleteConfig();
                    
                    configsCb.Items.Refresh();
                }
            }
        }


    }
}