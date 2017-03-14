using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;

namespace vrClusterConfig
{
    public class AppRunner : INotifyPropertyChanged
    {
        // net
        public const int nodeListenerPort = 9777;

        // conf files
        public const string confFileApp = "appList.conf";
        public const string confFileNodes = "nodeList.conf";
        public const string confFileConfig = "confList.conf";
        public const string confFileCameras = "camList.conf";
        public const string confFileLogCategories = "logCategoriesList.conf";

        //Log file name
        public const string logFileName = "logFilename.log";

        public static string logLevels = "";

        private const string logFilenamePrefix = "LogUVR_";

        private const string temporaryZipDir = @"\__dirForZIP\";

        // cluster commands
        private const string cCmdStart = "start ";  // [space] is required here
        private const string cCmdKill = "kill ";
        private const string cCmdStatus = "status";

        // run application params\keys
        private const string uvrParamStatic = " -uvr_cluster -fullscreen -nosplash";

        private const string uvrParamConfig = " uvr_cfg=";     // note:  no need [-] before it
        private const string uvrParamLogFilename = " log=";         // note:  no need [-] before it
        private const string uvrParamCameraDefault = " uvr_camera=";  // note:  no need [-] before it

        // switches
        private const string uvrParamOpenGL3 = " -opengl3";
        private const string uvrParamOpenGL4 = " -opengl4";
        private const string uvrParamStereo = " -quad_buffer_stereo";
        private const string uvrParamNoSound = " -nosound";
        private const string uvrParamFixedSeed = " -fixedseed";
        private const string uvrParamNoWrite = " -nowrite";

        private const string uvrParamForceLogFlush = " -forcelogflush";
        private const string uvrParamNoTextureStreaming = " -notexturestreaming";
        private const string uvrParamUseAllAvailableCores = " -useallavailablecores";

        //OpenGL parameters dictionary
        private Dictionary<string, string> _openGlParams = new Dictionary<string, string>
        {
            {"OpenGL3", " -opengl3" },
            {"OpenGL4", " -opengl4" }
        };
        public Dictionary<string, string> openGlParams
        {
            get { return _openGlParams; }
            set { Set(ref _openGlParams, value, "openGlParams"); }
        }

        //Selected OpenGL parameter
        private KeyValuePair<string, string> _selectedOpenGlParam;
        public KeyValuePair<string, string> selectedOpenGlParam
        {
            get { return _selectedOpenGlParam; }
            set
            {
                Set(ref _selectedOpenGlParam, value, "selectedOpenGlParam");
                GenerateCmdStartApp();
            }
        }

        //Properties for binding switches
        private bool _isStereo = true;
        public bool isStereo
        {
            get { return _isStereo;  }
            set
            {
                Set(ref _isStereo, value, "isStereo");
                GenerateCmdStartApp();
            }
        }

        private bool _isUseAllCores;
        public bool isUseAllCores
        {
            get { return _isUseAllCores; }
            set
            {
                Set(ref _isUseAllCores, value, "isUseAllCores");
                GenerateCmdStartApp();
            }
        }

        private bool _isNoSound = true;
        public bool isNoSound
        {
            get { return _isNoSound; }
            set
            {
                Set(ref _isNoSound, value, "isNoSound");
                GenerateCmdStartApp();
            }
        }

        private bool _isFixedSeed;
        public bool isFixedSeed
        {
            get { return _isFixedSeed; }
            set
            {
                Set(ref _isFixedSeed, value, "isFixedSeed");
                GenerateCmdStartApp();
            }
        }

        private bool _isNotextureStreaming;
        public bool isNotextureStreaming
        {
            get { return _isNotextureStreaming; }
            set
            {
                Set(ref _isNotextureStreaming, value, "isNotextureStreaming");
                GenerateCmdStartApp();
            }
        }

        private bool _isLogEnabled;
        public bool isLogEnabled
        {
            get { return _isLogEnabled; }
            set
            {
                Set(ref _isLogEnabled, value, "isLogEnabled");
                GenerateCmdStartApp();
            }
        }

        private bool _isForceLogFlush;
        public bool isForceLogFlush
        {
            get { return _isForceLogFlush; }
            set
            {
                Set(ref _isForceLogFlush, value, "isForceLogFlush");
                GenerateCmdStartApp();
            }
        }

        private bool _isLogZip;
        public bool isLogZip
        {
            get { return _isLogZip; }
            set
            { Set(ref _isLogZip, value, "isLogZip"); }
        }

        private bool _isLogRemove;
        public bool isLogRemove
        {
            get { return _isLogRemove; }
            set { Set(ref _isLogRemove, value, "isLogRemove"); }
        }

        private bool _isDebugRun;
        public bool isDebugRun
        {
            get { return _isDebugRun; }
            set
            {
                Set(ref _isDebugRun, value, "isDebugRun");
                GenerateCmdStartApp();
            }
        }

        //Active nodes list
        private List<string> _activeNodes;
        public List<string> activeNodes
        {
            get { return _activeNodes; }
            set { Set(ref _activeNodes, value, "activeNodes"); }
        }

        //Applications list
        private List<string> _applications;
        public List<string> applications
        {
            get { return _applications; }
            set { Set(ref _applications, value, "applications"); }
        }

        //Configs list
        private List<string> _configs;
        public List<string> configs
        {
            get { return _configs; }
            set { Set(ref _configs, value, "configs"); }
        }

        //Cameras list
        private List<string> _cameras;
        public List<string> cameras
        {
            get { return _cameras; }
            set { Set(ref _cameras, value, "cameras"); }
        }

        //Log categories list
        private List<LogCategory> _logCategories;
        public List<LogCategory> logCategories
        {
            get { return _logCategories; }
            set { Set(ref _logCategories, value, "logCategories"); }
        }

        //Log file name
        private string _logFile;
        public string logFile
        {
            get { return _logFile; }
            set
            {
                Set(ref _logFile, value, "logFile");
                GenerateCmdStartApp();
            }
        }

        //Additional command line params
        private string _additionalParams;
        public string additionalParams
        {
            get { return _additionalParams; }
            set
            {
                Set(ref _additionalParams, value, "additionalParams");
                GenerateCmdStartApp();
            }
        }

        //Command line string
        private string _commandLine;
        public string commandLine
        {
            get { return _commandLine; }
            set { Set(ref _commandLine, value, "commandLine"); }
        }

        //Selected Application
        private string _selectedApplication;
        public string selectedApplication
        {
            get { return _selectedApplication; }
            set
            {
                Set(ref _selectedApplication, value, "selectedApplication");
                GenerateCmdStartApp();
            }
        }

        //Selected Config
        private string _selectedConfig;
        public string selectedConfig
        {
            get { return _selectedConfig; }
            set
            {
                Set(ref _selectedConfig, value, "selectedConfig");
                GenerateCmdStartApp();
            }
        }

        //Selected Camera
        private string _selectedCamera;
        public string selectedCamera
        {
            get { return _selectedCamera; }
            set
            {
                Set(ref _selectedCamera, value, "selectedCamera");
                GenerateCmdStartApp();
            }
        }

        public enum ClusterCommandType
        {
            Run,
            Kill,
            Status,

            COUNT
        }

        public AppRunner()
        {
            InitConfigLists();
            InitLogCategories();
            logFile = logFileName;
        }

        //Implementation of INotifyPropertyChanged method for TwoWay binding
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnNotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        //Set property with OnNotifyPropertyChanged call
        protected void Set<T>(ref T field, T newValue, string propertyName)
        {
            field = newValue;
            OnNotifyPropertyChanged(propertyName);
        }

        private List<string> ReadConfigFile(string confFilePath)
        {
            List<string> infoList = new List<string>();

            try
            {
                string[] lines = System.IO.File.ReadAllLines(confFilePath);
                infoList = lines.ToList();

                LogInfo("Read file [" + confFilePath + "] finished.");
            }
            catch (Exception exception)
            {
                LogInfo("Can't read file [" + confFilePath + "]. EXCEPTION: " + exception.Message);
            }

            return infoList;
        }

        //Reloading all config lists
        private void InitConfigLists()
        {
            applications = ReadConfigFile(confFileApp);
            activeNodes = ReadConfigFile(confFileNodes);
            cameras = ReadConfigFile(confFileCameras);
            configs = ReadConfigFile(confFileConfig);
            selectedOpenGlParam = openGlParams.SingleOrDefault( x => x.Key == "OpenGL3" );

        }

        private void InitLogCategories()
        {
            List<string> logCats = ReadConfigFile(confFileLogCategories);
            if (logCategories == null)
            {
                logCategories = new List<LogCategory>();
            }
            else
            {
                logCategories.Clear();
            }
            foreach (string logCat in logCats)
            {
                logCategories.Add(new LogCategory(logCat));
            }
        }

        public void ReloadList(List<string> list)
        {
            string listName = string.Empty;
            if (list == applications)
            {
                listName = confFileApp;
            }
            if (list == activeNodes)
            {
                listName = confFileNodes;
            }
            if (list == cameras)
            {
                listName = confFileCameras;
            }
            if (list == configs)
            {
                listName = confFileConfig;
            }
            List<string> loadedList = ReadConfigFile(listName);
            if (!list.SequenceEqual(loadedList))
            {
                list.Clear();
                list.AddRange(loadedList);
            }
        }

        //Generating command line for the App
        private void GenerateCmdStartApp()
        {
            string appPath = selectedApplication;
            string confString = uvrParamConfig + selectedConfig;

            // switches
            string swOpengl = selectedOpenGlParam.Value;
            string swStereo = (isStereo) ? uvrParamStereo : "";
            string swNoSound = (isNoSound) ? uvrParamNoSound : "";
            string swFixedSeed = (isFixedSeed) ? uvrParamFixedSeed : "";

            string swNoTextureStreaming = (isNotextureStreaming) ? uvrParamNoTextureStreaming : "";
            string swUseAllAvailableCores = (isUseAllCores) ? uvrParamUseAllAvailableCores : "";

            // logging params
            string swNoWrite = (isLogEnabled) ? "" : uvrParamNoWrite;

            string swForceLogFlush = "";
            string paramLogFilename = "";

            if (isLogEnabled)
            {
                swForceLogFlush = (isForceLogFlush) ? uvrParamForceLogFlush : "";
                paramLogFilename = uvrParamLogFilename + logFile;

            }


            // camera by default
            string paramDefaultCamera = "";
            if (selectedCamera != null)
            {
                paramDefaultCamera = uvrParamCameraDefault + selectedCamera;
            }

            // additional params
            //string additionalParameters = " " + additionalParams;

            // cmd
            string cmd = appPath + swOpengl + uvrParamStatic + confString + swStereo + swNoSound + swFixedSeed
                                 + swNoTextureStreaming + swUseAllAvailableCores + swForceLogFlush + swNoWrite
                                 + paramLogFilename + paramDefaultCamera + " " + additionalParams + logLevels;

            if (isDebugRun)
            {
                // this is for debug only! Run application WITHOUT any params
                cmd = appPath;
            }

            // set value
            commandLine = cmd;

            //return cmd;
        }

        public void GenerateLogLevelsString()
        {
            string resString = String.Empty;

            resString = " -LogCmds=\"";
            foreach (LogCategory item in logCategories)
            {
                if (item.isChecked)
                {
                    resString += item.id + " " + item.value.ToString() + ", ";
                }
            }

            resString += "\"";
            logLevels = resString;
            GenerateCmdStartApp();
            
        }

        public void RunCommand(List<string> runningList)
        {
            ClusterCommand(ClusterCommandType.Run, runningList);
        }

        public void KillCommand(List<string> runningList)
        {
            ClusterCommand(ClusterCommandType.Kill, runningList);
        }

        public void StatusCommand(List<string> runningList)
        {
            ClusterCommand(ClusterCommandType.Status, runningList);
        }

        private void ClusterCommand(ClusterCommandType ccType, List<string> nodes)
        {
            // get all nodes address
            //List<string> nodes = GetNodes();

            if (nodes.Count == 0)
            {
                return;
            }

            // gen.command for cluster nodes
            string clusterCmd = "";

            switch (ccType)
            {
                case ClusterCommandType.Run:
                    clusterCmd = cCmdStart + commandLine;
                    break;

                case ClusterCommandType.Kill:
                    clusterCmd = cCmdKill + selectedApplication;
                    break;

                case ClusterCommandType.Status:
                    clusterCmd = cCmdStatus;
                    break;
            }

            // send cmd for each node
            LogInfo("Command for client is :  " + clusterCmd);

            foreach (string node in nodes)
            {
                SendDaemonCommand(node, clusterCmd);
            }
        }

        private void SendDaemonCommand(string nodeAddress, string cmd)
        {
            TcpClient nodeClient = new TcpClient();

            try
            {
                nodeClient.Connect(nodeAddress, nodeListenerPort);
                NetworkStream networkStream = nodeClient.GetStream();
                StreamWriter clientStreamWriter = new StreamWriter(networkStream);

                if (networkStream.CanWrite)
                {
                    clientStreamWriter.Write(cmd);
                    clientStreamWriter.Flush();
                }
                else
                {
                    LogInfo("Can't write to client on node [" + nodeAddress + "]");
                }

                nodeClient.Close();
            }
            catch (Exception exception)
            {
                LogInfo("Can't connect to node " + nodeAddress + ". EXCEPTION: " + exception.Message);
            }
        }

        public void CleanLogs(List<string> nodesList)
        {
            CleanLogFolders(nodesList);
        }

        private void CleanLogFolders(List<string> nodes)
        {
            foreach (string node in nodes)
            {
                string dirPath = GetLogFolder(node);
                RemoveAllRecursively(dirPath);
                LogInfo("Removed all files in : " + dirPath);
            }
        }

        private string GetLogFolder(string node)
        {
            //get path
            string appPath = selectedApplication;
            // remove drive-name, like      [C:]
            appPath = appPath.Substring(2, appPath.Length - 2);
            // remove filename and extension
            string logPath = Path.GetDirectoryName(appPath);

            // replace slash to back-slash
            logPath.Replace("/", "\\");

            string projectName = Path.GetFileNameWithoutExtension(appPath);
            string fullpath = @"\\" + node + logPath + @"\" + projectName + @"\Saved\Logs\";

            return fullpath;
        }


        private void RemoveAllRecursively(string rootDir)
        {
            try
            {
                // remove all files
                var allFilesToDelete = Directory.EnumerateFiles(rootDir, "*.*", SearchOption.AllDirectories);
                foreach (var file in allFilesToDelete)
                {
                    File.Delete(file);
                }

                // remove all directories
                DirectoryInfo diRoot = new DirectoryInfo(rootDir);
                foreach (DirectoryInfo subDir in diRoot.GetDirectories())
                {
                    subDir.Delete(true);
                }
            }
            catch (Exception exception)
            {
                LogInfo("RemoveAllRecursively. EXCEPTION: " + exception.Message);
            }
        }

        public void CollectLogs(List<string> nodes)
        {
            CollectLogFiles(nodes);
        }

        private void CollectLogFiles(List<string> nodes)
        {
            //List<string> nodes = GetNodes();

            FolderBrowserDialog fbDialog = new FolderBrowserDialog();
            //fbDialog.Description = "Select a folder for save log files from nodes :";
            if (fbDialog.ShowDialog() != DialogResult.OK || nodes.Count == 0)
            {
                return;
            }



            // clean all files except *.zip, *.rar


            // list of new files
            List<string> fileList = new List<string>();

            // copy + rename
            foreach (string node in nodes)
            {
                string logFilename = logFile;

                string logFilenameSep = (logFilename == string.Empty) ? "" : ("_");

                string srcLogPath = GetLogFolder(node) + logFilename;
                string dstLogPath = fbDialog.SelectedPath + @"\" + logFilenamePrefix + node + logFilenameSep + logFilename;
                string logMsg = "[" + srcLogPath + "] to [" + dstLogPath + "]";

                // add to list
                fileList.Add(dstLogPath);

                try
                {
                    File.Copy(srcLogPath, dstLogPath);
                    LogInfo("Copied file from " + logMsg);
                }
                catch (Exception exception)
                {
                    LogInfo("Can't copy file from " + logMsg + ". EXCEPTION: " + exception.Message);
                }
            }

            // create archive
            if (!isLogZip)
            {
                return;
            }

            string currentTime = DateTime.Now.ToString("HHmmss");
            string currentDate = DateTime.Now.ToString("yyyyMMdd");

            string zipFilename = fbDialog.SelectedPath + @"\" + logFilenamePrefix + currentDate + "_" + currentTime + ".zip";

            CreateZipLogs(zipFilename, fileList);

            // clean *.log-files
            if (isLogRemove)
            {
                RemoveListOfFiles(fileList);
            }
        }

        private void CreateZipLogs(string zipFilename, List<string> files)
        {
            if (files.Count == 0)
            {
                return;
            }

            string currentDir = Path.GetDirectoryName(zipFilename);

            string dirForZip = currentDir + temporaryZipDir;

            // create tmp-dir
            Directory.CreateDirectory(dirForZip);

            // copy to temporary folder for zip
            foreach (string file in files)
            {
                File.Copy(file, dirForZip + Path.GetFileName(file));
            }

            try
            {
                // pack it
                ZipFile.CreateFromDirectory(dirForZip, zipFilename, CompressionLevel.Optimal, false);

                // remove tmp dir and all files after ZIP
                RemoveAllRecursively(dirForZip);
                Directory.Delete(dirForZip);
            }
            catch (Exception exception)
            {
                LogInfo("CreateZipLogs. EXCEPTION: " + exception.Message);
            }
        }

        private void RemoveListOfFiles(List<string> fList)
        {
            foreach (string file in fList)
            {
                File.Delete(file);
            }
        }


        private void RewriteListToFile(string filePath, List<string> list)
        {
            try
            {
                File.WriteAllLines(filePath, list);
            }
            catch(Exception exception)
            {
                LogInfo("Can't renew file [" + confFileNodes + "]. EXCEPTION: " + exception.Message);
            }
        }

        public void AddApplication(string appPath)
        {
            applications.Add(appPath);
            AddStringToFile(confFileApp, appPath);
        }

        public void DeleteApplication()
        {
            applications.Remove(selectedApplication);
            RewriteListToFile(confFileApp, applications);
            selectedApplication = null;
        }


        public void AddNode(string node)
        {
            activeNodes.Add(node);
            AddStringToFile(confFileNodes, node);
        }

        public void DeleteNodes(List<string> nodes)
        {
            activeNodes = activeNodes.Except(nodes).ToList();
            RewriteListToFile(confFileNodes, activeNodes);
        }

        public void AddStringToFile(string filePath, string newString)
        {
            try
            {
                File.AppendAllText(filePath, Environment.NewLine + newString);
            }
            catch (Exception exception)
            {
                LogInfo("Can't write to file [" + confFileNodes + "]. EXCEPTION: " + exception.Message);
            }
        }


        private void LogInfo(string info)
        {
            //todo
            if (info == "")
            {
                return;
            }

        }
    }
}
