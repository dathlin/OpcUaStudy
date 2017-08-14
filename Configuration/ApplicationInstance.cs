/* ========================================================================
 * Copyright (c) 2005-2017 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Foundation MIT License 1.00
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/MIT/1.00/
 * ======================================================================*/

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceProcess;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

using Opc.Ua;

namespace Opc.Ua.Configuration
{
    /// <summary>
    /// 一个用于安装，配置，运行opc Ua的封装类
    /// </summary>
    public class ApplicationInstance
    {
        #region 构造方法
        /// <summary>
        /// 实例化一个新的<see cref="ApplicationInstance"/>对象
        /// </summary>
        public ApplicationInstance()
        {

        }

        /// <summary>
        /// 使用指定的配置实例化一个新的<see cref="ApplicationInstance"/>对象
        /// </summary>
        /// <param name="applicationConfiguration">指定的opc ua应用程序配置</param>
        public ApplicationInstance(ApplicationConfiguration applicationConfiguration)
        {
            m_applicationConfiguration = applicationConfiguration;
        }
        #endregion

        #region 公开的属性
        /// <summary>
        /// 获取或设置应用程序的名称
        /// </summary>
        /// <value>The name of the application.</value>
        public string ApplicationName
        {
            get { return m_applicationName; }
            set { m_applicationName = value; }
        }

        /// <summary>
        /// 获取或设置应用程序的类型，主要分服务器，客户端，混合类型，查找服务器类型
        /// </summary>
        /// <value>The type of the application.</value>
        public ApplicationType ApplicationType
        {
            get { return m_applicationType; }
            set { m_applicationType = value; }
        }

        //Gets or sets the name of the config section containing the path to the application configuration file.
        /// <summary>
        /// 获取或设置一个包含应用程序配置文件路径的部分配置的名称
        /// </summary>
        /// <value>The name of the config section.</value>
        public string ConfigSectionName
        {
            get { return m_configSectionName; }
            set { m_configSectionName = value; }
        }

        //Gets or sets the type of configuration file.
        /// <summary>
        /// 获取或设置配置文件的类型
        /// </summary>
        /// <value>The type of configuration file.</value>
        public Type ConfigurationType
        {
            get { return m_configurationType; }
            set { m_configurationType = value; }
        }

        //Gets or sets the installation configuration.
        /// <summary>
        /// 获取或设置安装配置信息
        /// </summary>
        /// <value>The installation configuration.</value>
        public InstalledApplication InstallConfig
        {
            get { return m_installConfig; }
            set { m_installConfig = value; }
        }

        /// <summary>
        /// 获取基础服务器对象
        /// </summary>
        /// <value>The server.</value>
        public ServerBase Server
        {
            get { return m_server; }
        }

        //Gets the application configuration used when the Start() method was called.
        /// <summary>
        /// 获取或设置应用程序的配置对象，该对象在Start()方法调用时使用
        /// </summary>
        /// <value>The application configuration.</value>
        public ApplicationConfiguration ApplicationConfiguration
        {
            get { return m_applicationConfiguration; }
            set { m_applicationConfiguration = value; }
        }

        // Gets or sets a flag that indicates whether the application will be set up for management with the GDS agent.
        // If true the application will not be visible to the GDS local agent after installation.
        /// <summary>
        /// 获取或设置一个标志，指示是否将应用程序设置为使用GDS代理进行管理。
        /// </summary>
        /// <value>如果为true，安装后，应用程序将不会显示给GDS本地代理。</value>
        public bool NoGdsAgentAdmin { get; set; }
        #endregion

        #region 加载配置信息
        /// <summary>
        /// 从一个文件加载安装配置
        /// </summary>
        /// <param name="filePath">文件的路径</param>
        /// <exception cref="ArgumentNullException"></exception>
        public InstalledApplication LoadInstallConfigFromFile(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            Stream istrm = null;

            try
            {
                istrm = File.Open(filePath, FileMode.Open, FileAccess.Read);
            }
            catch (Exception e)
            {
                throw ServiceResultException.Create(StatusCodes.BadDecodingError, e, "Could not open file: {0}", filePath);
            }

            return LoadInstallConfigFromStream(istrm);
        }

        /// <summary>
        /// 从一个内嵌的资源文件中加载程序的安装配置
        /// </summary>
        /// <param name="resourcePath">资源名称</param>
        /// <param name="assembly">程序集</param>
        /// <exception cref="ArgumentNullException"></exception>
        public InstalledApplication LoadInstallConfigFromResource(string resourcePath, Assembly assembly)
        {
            if (resourcePath == null) throw new ArgumentNullException("resourcePath");

            if (assembly == null)
            {
                assembly = Assembly.GetCallingAssembly();
            }

            Stream istrm = assembly.GetManifestResourceStream(resourcePath);

            if (istrm == null)
            {
                throw ServiceResultException.Create(StatusCodes.BadDecodingError, "Could not find resource file: {0}", resourcePath);
            }

            return LoadInstallConfigFromStream(istrm);
        }

        /// <summary>
        /// 使用反序列化从一个数据流中加载程序的安装配置
        /// </summary>
        /// <param name="istrm">数据流</param>
        /// <exception cref="SerializationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public InstalledApplication LoadInstallConfigFromStream(Stream istrm)
        {
            try
            {
                using (XmlTextReader reader = new XmlTextReader(istrm))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(InstalledApplication));
                    return (InstalledApplication)serializer.ReadObject(reader, false);
                }
            }
            catch (Exception e)
            {
                throw ServiceResultException.Create(StatusCodes.BadDecodingError, e, "Could not parse install configuration.");
            }
        }

        /// <summary>
        /// 从配置文件中加载应用程序的安装配置
        /// </summary>
        /// <param name="configFile">一个可以为空的文件配置名称</param>
        public virtual void LoadInstallConfig(string configFile)
        {
            // 从文件中加载配置
            if (!String.IsNullOrEmpty(configFile))
            {
                InstallConfig = LoadInstallConfigFromFile(configFile);
            }

            // 从运行程序的所有资源中寻找InstallConfig.xml文件，如果存在就加载安装配置，如果不存在就退出
            else if (InstallConfig == null)
            {
                foreach (string resourcePath in Assembly.GetEntryAssembly().GetManifestResourceNames())
                {
                    if (resourcePath.EndsWith("InstallConfig.xml"))
                    {
                        InstallConfig = LoadInstallConfigFromResource(resourcePath, Assembly.GetEntryAssembly());
                        break;
                    }
                }

                if (InstallConfig == null)
                {
                    throw new ServiceResultException(StatusCodes.BadConfigurationError, "Could not load default installation config file.");
                }
            }

            // 如果安装配置中包含应用程序的名字，就覆盖到当前下面的应用程序名称
            if (String.IsNullOrEmpty(InstallConfig.ApplicationName))
            {
                InstallConfig.ApplicationName = ApplicationName;
            }
            else
            {
                ApplicationName = InstallConfig.ApplicationName;
            }
            
            // 更新安装配置中的固定字段，包括程序类型和可执行exe的文件名称
            InstallConfig.ApplicationType = (Opc.Ua.Security.ApplicationType)(int)ApplicationType;
            InstallConfig.ExecutableFile = Application.ExecutablePath;
            //如果安装配置中设置了追踪配置信息就将跟踪配置应用于当前应用程序
            if (InstallConfig.TraceConfiguration != null)
            {
                InstallConfig.TraceConfiguration.ApplySettings();
            }
        }
        #endregion
        
        #region 公开的方法
        /// <summary>
        /// 处理命令行
        /// </summary>
        /// <returns>
        /// 如果参数已经处理就返回True，否则返回False
        /// </returns>
        public bool ProcessCommandLine()
        {
            //删除了GDS想关功能，新的OPCF级别不在设置GDS
            // NP Jan-20-2012: removing GDS reference, per new OPCF decree of no GDS.
            NoGdsAgentAdmin = true;
            return ProcessCommandLine(false);
        }

        /// <summary>
        /// 处理命令行
        /// </summary>
        /// <param name="ignoreUnknownArguments">如果设置为<c>true</c>，将忽略未知参数</param>
        /// <returns>
        /// 如果参数已经处理就返回True，否则返回False
        /// </returns>
        public bool ProcessCommandLine(bool ignoreUnknownArguments)
        {
            // 生成一个追踪配置对象
            TraceConfiguration config = new TraceConfiguration();
            config.OutputFilePath = "%CommonApplicationData%\\OPC Foundation\\Logs\\Default.InstallLog.txt";
            config.DeleteOnLoad = false;
            config.TraceMasks = 1023;
            config.ApplySettings();

            string[] args =  Environment.GetCommandLineArgs();

            if (args.Length <= 1)
            {
                return false;
            }

            return ProcessCommandLine(ignoreUnknownArguments, args);
        }

        /// <summary>
        /// 处理命令行
        /// </summary>
        /// <returns>如果参数已经处理就返回True，否则返回False</returns>
        public bool ProcessCommandLine(bool ignoreUnknownArguments, params string[] args)
        {
            if (args.Length <= 1)
            {
                return false;
            }

            // 参数可以是独立的或名称值对由“:”分隔。
            Dictionary<string,string> argTable = new Dictionary<string, string>();

            for (int ii = 1; ii < args.Length; ii++)
            {
                string arg = args[ii];

                if (String.IsNullOrEmpty(arg))
                {
                    continue;
                }

                int index = args[ii].IndexOf(':');

                if (index != -1 && index > 0 && index < arg.Length-1)
                {
                    argTable[arg.Substring(0, index).ToLower()] = arg.Substring(index+1);
                }
                else
                {
                    argTable[arg.ToLower()] = String.Empty;
                }
            }

            // 验证参数信息
            string error = ValidateArguments(ignoreUnknownArguments, argTable);

            if (!String.IsNullOrEmpty(error))
            {
                throw ServiceResultException.Create(StatusCodes.BadInvalidArgument, error);
            }

            // 检查一个默认的开关，一般来说silent=False
            bool silent = !Environment.UserInteractive;

            if (argTable.ContainsKey("/silent"))
            {
                silent = true;
            }

            string configFile = null;

            try
            {
                // 从命令行获取配置文件的信息
                if (argTable.TryGetValue("/configfile", out configFile))
                {
                    configFile = Utils.GetAbsoluteFilePath(configFile, true, true, false);
                }

                // 加载安装配置信息
                LoadInstallConfig(configFile);
            }
            catch (Exception e)
            {
                StringBuilder buffer = new StringBuilder();
                buffer.Append("Could not load the install configuration. ");
                buffer.Append(configFile);

                if (!silent)
                {
                    throw ServiceResultException.Create(StatusCodes.BadInvalidArgument, e, buffer.ToString());
                }

                Utils.Trace(e, buffer.ToString());
                return true;
            }
                
            try
            {
                // 安装应用程序
                if (argTable.ContainsKey("/start"))
                {
                    if (ServiceInstaller.StartService(InstallConfig.ApplicationName))
                    {
                        Utils.Trace(Utils.TraceMasks.Information, "Service started '{0}'.", InstallConfig.ApplicationName);
                    }

                    return true;
                }

                // 安装应用程序
                if (argTable.ContainsKey("/install"))
                {
                    Install(silent, argTable);
                    return true;
                }

                // 卸载应用程序
                if (argTable.ContainsKey("/uninstall"))
                {
                    Uninstall(silent, argTable);
                    return true;
                }

                // 处理由子类定义的任何参数。
                return ProcessCommand(silent, argTable);
            }
            catch (Exception e)
            {
                StringBuilder buffer = new StringBuilder();
                buffer.Append("Could not process the command line arguments provided. ");

                if (args != null)
                {
                    for (int ii = 1; ii < args.Length; ii++)
                    {
                        buffer.AppendFormat("{0} ", args[ii]);
                    }
                }

                if (!silent)
                {
                    throw ServiceResultException.Create(StatusCodes.BadInvalidArgument, e, buffer.ToString());
                }

                Utils.Trace(e, buffer.ToString());
                return true;
            }
        }

        /// <summary>
        /// 将OPC UA服务作为一个Windows Service启动
        /// </summary>
        /// <param name="server">基础服务对象</param>
        public void StartAsService(ServerBase server)
        {
            m_server = server;
            ServiceBase.Run(new WindowsService(server, ConfigSectionName, ApplicationType, ConfigurationType));
        }

        /// <summary>
        /// 启动OPC UA服务
        /// </summary>
        /// <param name="server">基础服务对象</param>
        public void Start(ServerBase server)
        {
            m_server = server;

            if (m_applicationConfiguration == null)
            {
                LoadApplicationConfiguration(false);
            }

            if (m_applicationConfiguration.SecurityConfiguration != null && m_applicationConfiguration.SecurityConfiguration.AutoAcceptUntrustedCertificates)
            {
                m_applicationConfiguration.CertificateValidator.CertificateValidation += CertificateValidator_CertificateValidation;
            }
            
            server.Start(m_applicationConfiguration);
        }

        /// <summary>
        /// Stops the UA server.
        /// </summary>
        public void Stop()
        {
            m_server.Stop();
        }
        #endregion

        #region WindowsService 类
        /// <summary>
        /// 管理OPC UA服务和windows 服务管理器之前的交互
        /// </summary>
        protected class WindowsService : ServiceBase
        {
            #region 构造器
            /// <summary>
            ///实例化一个 <see cref="WindowsService"/> 对象
            /// </summary>
            /// <param name="server">基础服务对象</param>
            /// <param name="configSectionName">片段配置名称</param>
            /// <param name="applicationType">应用程序的类型</param>
            /// <param name="configurationType">程序的配置类型</param>
            public WindowsService(ServerBase server, string configSectionName, ApplicationType applicationType, Type configurationType)
            {
                m_server = server;
                m_configSectionName = configSectionName;
                m_applicationType = applicationType;
                m_configurationType = configurationType;
                EventLog.Source = "UA Application";
            }
            #endregion
            
            #region 重写的方法
            /// <summary>
            /// 在一个后台的线程里启动服务
            /// </summary>
            protected override void OnStart(string[] args)
            {
                Thread thread = new Thread(OnBackgroundStart);
                thread.Start(null);
            }

            /// <summary>
            /// 停止服务器以便让服务终结
            /// </summary>
            protected override void OnStop()
            {
                m_server.Stop();
            }
            #endregion
            
            #region 私有的方法
            /// <summary>
            /// 一个实际在后台运行服务的方法
            /// </summary>
            private void OnBackgroundStart(object state)
            {
                string filePath = null;
                ApplicationConfiguration configuration = null;

                try
                {
                    filePath = ApplicationConfiguration.GetFilePathFromAppConfig(m_configSectionName);
                    configuration = ApplicationInstance.LoadAppConfig(false, filePath, m_applicationType, m_configurationType, true);
                }
                catch (Exception e)
                {
                    ServiceResult error = ServiceResult.Create(e, StatusCodes.BadConfigurationError, "Could not load UA Service configuration file.\r\nPATH={0}", filePath);
                    this.EventLog.WriteEntry(error.ToLongString(), EventLogEntryType.Error);
                }

                try
                {
                    if (configuration.SecurityConfiguration != null && configuration.SecurityConfiguration.AutoAcceptUntrustedCertificates)                   
                    {
                        configuration.CertificateValidator.CertificateValidation += CertificateValidator_CertificateValidation;
                    }

                    m_server.Start(configuration);
                    // this.EventLog.WriteEntry("SERVICE STARTED! " + this.m_configSectionName, EventLogEntryType.Information);
                }
                catch (Exception e)
                {
                    ServiceResult error = ServiceResult.Create(e, StatusCodes.BadConfigurationError, "Could not start UA Service.");
                    this.EventLog.WriteEntry(error.ToLongString(), EventLogEntryType.Error);
                    Utils.Trace((int)Utils.TraceMasks.Error, error.ToLongString());
                }
            }

            #endregion

            #region Private Fields
            private ServerBase m_server;
            private string m_configSectionName;
            private ApplicationType m_applicationType;
            private Type m_configurationType;
            #endregion
        }
        #endregion

        #region 参数描述类
        /// <summary>
        /// 存储参数的描述
        /// </summary>
        protected class ArgumentDescription
        {
            /// <summary>
            /// 参数的名称
            /// </summary>
            public string Name;

            /// <summary>
            /// 参数的描述
            /// </summary>
            public string Description;

            /// <summary>
            /// 指示参数是否需要一个值
            /// </summary>
            public bool ValueRequired;

            /// <summary>
            /// 指示参数是否允许一个值
            /// </summary>
            public bool ValueAllowed;

            /// <summary>
            /// 实例化一个 <see cref="ArgumentDescription"/> 对象.
            /// </summary>
            /// <param name="name">参数名称</param>
            /// <param name="valueRequired">如果设置为 <c>true</c> ，必须需要一个值</param>
            /// <param name="valueAllowed">如果设置为 <c>true</c> ，允许需要一个值</param>
            /// <param name="description">参数的描述</param>
            public ArgumentDescription(
                 string name,
                 bool valueRequired,
                 bool valueAllowed,
                 string description)
            {
                Name = name;
                ValueRequired = valueRequired;
                ValueAllowed = valueAllowed;
                Description = description;
            }
        }

        private static ArgumentDescription[] s_SupportedArguments = new ArgumentDescription[]
        {            
            new ArgumentDescription("/start", false, false, "Starts the application as a service (/start [/silent] [/configFile:<filepath>])."),
            new ArgumentDescription("/install", false, false, "Installs the application (/install [/silent] [/configFile:<filepath>])."),
            new ArgumentDescription("/uninstall", false, false, "Uninstalls the application (/uninstall [/silent] [/configFile:<filepath>])."),
            new ArgumentDescription("/silent", false, false, "Performs operations without prompting user to confirm or displaying errors."),
            new ArgumentDescription("/configFile", true, true, "Specifies the installation configuration file."),
        };
        #endregion

        #region 受保护的方法
        /// <summary>
        /// 获取支持的参数的描述
        /// </summary>
        protected virtual ArgumentDescription[] GetArgumentDescriptions()
        {
            return s_SupportedArguments;
        }

        /// <summary>
        /// 获取帮助文本
        /// </summary>
        protected virtual string GetHelpString(ArgumentDescription[] commands)
        {
            StringBuilder text = new StringBuilder();
            text.Append("These are the supported arguments:\r\n");

            for (int ii = 0; ii < commands.Length; ii++)
            {
                ArgumentDescription command = commands[ii];

                text.Append("\r\n");

                if (command.ValueRequired)
                {
                    text.AppendFormat("{0}:<value> {1}", command.Name, command.Description);
                }
                else if (command.ValueAllowed)
                {
                    text.AppendFormat("{0}[:<value>] {1}", command.Name, command.Description);
                }
                else
                {
                    text.AppendFormat("{0} {1}", command.Name, command.Description);
                }
            }

            text.Append("\r\n");
            return text.ToString();
        }

        /// <summary>
        /// 验证参数
        /// </summary>
        protected virtual string ValidateArguments(bool ignoreUnknownArguments, Dictionary<string, string> args)
        {
            ArgumentDescription[] commands = GetArgumentDescriptions();

            // 检查是否要求帮助
            if (args.ContainsKey("/?"))
            {
                return GetHelpString(commands);
            }

            // 验证参数
            StringBuilder error = new StringBuilder();

            foreach (KeyValuePair<string,string> arg in args)
            {
                ArgumentDescription command = null;

                for (int ii = 0; ii < commands.Length; ii++)
                {
                    if (String.Compare(commands[ii].Name, arg.Key, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        command = commands[ii];
                        break;
                    }
                }

                if (command == null)
                {
                    if (!ignoreUnknownArguments)
                    {
                        if (error.Length > 0)
                        {
                            error.Append("\r\n");
                        }

                        error.AppendFormat("Unrecognized argument: {0}", arg.Key);
                    }

                    continue;
                }

                if (command.ValueRequired && String.IsNullOrEmpty(arg.Value))
                {
                    if (error.Length > 0)
                    {
                        error.Append("\r\n");
                    }

                    error.AppendFormat("{0} requires a value to be specified (syntax {0}:<value>).", arg.Key);
                    continue;
                }

                if (!command.ValueAllowed && !String.IsNullOrEmpty(arg.Value))
                {
                    if (error.Length > 0)
                    {
                        error.Append("\r\n");
                    }

                    error.AppendFormat("{0} does not allow a value to be specified.", arg.Key);
                    continue;
                }
            }

            // return any error text.
            return error.ToString();
        }

        /// <summary>
        /// 根据安装配置中的信息来更新应用程序配置的一些值
        /// </summary>
        /// <param name="configuration">等待更新的应用程序配置</param>
        protected virtual void UpdateAppConfigWithInstallConfig(ApplicationConfiguration configuration)
        {
            // 覆盖应用程序的名称
            if (InstallConfig.ApplicationName != null)
            {
                //如果证书不为空
                if (configuration.SecurityConfiguration != null && configuration.SecurityConfiguration.ApplicationCertificate != null)
                {
                    if (configuration.SecurityConfiguration.ApplicationCertificate.SubjectName == configuration.ApplicationName)
                    {
                        configuration.SecurityConfiguration.ApplicationCertificate.SubjectName = InstallConfig.ApplicationName;
                    }
                }

                configuration.ApplicationName = InstallConfig.ApplicationName;
            }

            if (InstallConfig.ApplicationUri != null)
            {
                configuration.ApplicationUri = InstallConfig.ApplicationUri;
            }

            // 如果地址中包含了localhost(忽略大小写)，就用本机名称来替代
            if (configuration.ApplicationUri != null)
            {
                int index = configuration.ApplicationUri.IndexOf("localhost", StringComparison.OrdinalIgnoreCase);

                if (index != -1)
                {
                    StringBuilder buffer = new StringBuilder();
                    buffer.Append(configuration.ApplicationUri.Substring(0, index));
                    buffer.Append(System.Net.Dns.GetHostName());
                    buffer.Append(configuration.ApplicationUri.Substring(index+"localhost".Length));
                    configuration.ApplicationUri = buffer.ToString();
                }
            }

            ServerBaseConfiguration serverConfiguration = null ;

            if (configuration.ServerConfiguration != null)
            {
                serverConfiguration = configuration.ServerConfiguration;
            }
            else if (configuration.DiscoveryServerConfiguration != null)
            {
                serverConfiguration = configuration.DiscoveryServerConfiguration;
            }

            //根据 安装配置 中的信息进行重新设置地址
            if (serverConfiguration != null)
            {
                if (InstallConfig.BaseAddresses != null && InstallConfig.BaseAddresses.Count > 0)
                {
                    Dictionary<string, string> addresses = new Dictionary<string, string>();
                    serverConfiguration.BaseAddresses.Clear();

                    for (int ii = 0; ii < InstallConfig.BaseAddresses.Count; ii++)
                    {
                        Uri url = Utils.ParseUri(InstallConfig.BaseAddresses[ii]);

                        if (url != null)
                        {
                            if (!addresses.ContainsKey(url.Scheme))
                            {
                                serverConfiguration.BaseAddresses.Add(url.ToString());
                                addresses.Add(url.Scheme, String.Empty);
                            }
                            else
                            {
                                serverConfiguration.AlternateBaseAddresses.Add(url.ToString());
                            }
                        }
                    }
                }

                if (InstallConfig.SecurityProfiles != null && InstallConfig.SecurityProfiles.Count > 0)
                {
                    ServerSecurityPolicyCollection securityPolicies = new ServerSecurityPolicyCollection();

                    for (int ii = 0; ii < InstallConfig.SecurityProfiles.Count; ii++)
                    {
                        for (int jj = 0; jj < serverConfiguration.SecurityPolicies.Count; jj++)
                        {
                            if (serverConfiguration.SecurityPolicies[jj].SecurityPolicyUri == InstallConfig.SecurityProfiles[ii].ProfileUri)
                            {
                                securityPolicies.Add(serverConfiguration.SecurityPolicies[jj]);
                            }
                        }
                    }

                    serverConfiguration.SecurityPolicies = securityPolicies;
                }
            }

            if (InstallConfig.ApplicationCertificate != null)
            {
                configuration.SecurityConfiguration.ApplicationCertificate.StoreType = InstallConfig.ApplicationCertificate.StoreType;
                configuration.SecurityConfiguration.ApplicationCertificate.StorePath = InstallConfig.ApplicationCertificate.StorePath;

                if (String.IsNullOrEmpty(InstallConfig.ApplicationCertificate.SubjectName))
                {
                    configuration.SecurityConfiguration.ApplicationCertificate.SubjectName = InstallConfig.ApplicationCertificate.SubjectName;
                }
            }

            if (InstallConfig.RejectedCertificatesStore != null)
            {
                configuration.SecurityConfiguration.RejectedCertificateStore = Opc.Ua.Security.SecuredApplication.FromCertificateStoreIdentifier(InstallConfig.RejectedCertificatesStore);
            }
            
            if (InstallConfig.IssuerCertificateStore != null)
            {
                configuration.SecurityConfiguration.TrustedIssuerCertificates.StoreType = InstallConfig.IssuerCertificateStore.StoreType;
                configuration.SecurityConfiguration.TrustedIssuerCertificates.StorePath = InstallConfig.IssuerCertificateStore.StorePath;
                configuration.SecurityConfiguration.TrustedIssuerCertificates.ValidationOptions = (CertificateValidationOptions)(int)InstallConfig.IssuerCertificateStore.ValidationOptions;
            }
            
            if (InstallConfig.TrustedCertificateStore != null)
            {
                configuration.SecurityConfiguration.TrustedPeerCertificates.StoreType = InstallConfig.TrustedCertificateStore.StoreType;
                configuration.SecurityConfiguration.TrustedPeerCertificates.StorePath = InstallConfig.TrustedCertificateStore.StorePath;
                configuration.SecurityConfiguration.TrustedPeerCertificates.ValidationOptions = (CertificateValidationOptions)(int)InstallConfig.TrustedCertificateStore.ValidationOptions;
            }

            configuration.CertificateValidator.Update(configuration);
        }

        /// <summary>
        /// 安装服务
        /// </summary>
        /// <param name="silent">如果设置为 <c>true</c> 不会显示任何的对话框</param>
        /// <param name="args">提供给命令行的额外参数</param>
        protected virtual void Install(bool silent, Dictionary<string, string> args)
        {
            //写入追踪日志
            Utils.Trace(Utils.TraceMasks.Information, "Installing application.");

            // 检查配置信息
            string filePath = Utils.GetAbsoluteFilePath(InstallConfig.ConfigurationFile, true, false, false);

            if (filePath == null)
            {
                Utils.Trace("WARNING: Could not load config file specified in the installation configuration: {0}", InstallConfig.ConfigurationFile);
                filePath = ApplicationConfiguration.GetFilePathFromAppConfig(ConfigSectionName);
                InstallConfig.ConfigurationFile = filePath;
            }

            ApplicationConfiguration configuration = LoadAppConfig(silent, filePath, 
                Security.SecuredApplication.FromApplicationType(InstallConfig.ApplicationType), ConfigurationType, false);

            if (configuration == null)
            {
                return;
            }

            // 更新配置
            UpdateAppConfigWithInstallConfig(configuration);
            ApplicationConfiguration = configuration;
            
            // 检查证书
            X509Certificate2 certificate = configuration.SecurityConfiguration.ApplicationCertificate.Find(true);

            if (certificate != null)
            {
                if (!silent)
                {
                    if (!CheckApplicationInstanceCertificate(configuration, certificate, silent, InstallConfig.MinimumKeySize))
                    {
                        certificate = null;
                    }
                }
            }

            // 创建一个新的证书
            if (certificate == null)
            {
                certificate = CreateApplicationInstanceCertificate(configuration, InstallConfig.MinimumKeySize, InstallConfig.LifeTimeInMonths);
            }

            // 确保证书是被信任的
            AddToTrustedStore(configuration, certificate);
            
            // 如果是服务器运行，将证书添加到发现服务的信任列表中
            if (configuration.ApplicationType == ApplicationType.Server || configuration.ApplicationType == ApplicationType.ClientAndServer)
            {
                try
                {
                    AddToDiscoveryServerTrustList(certificate, null, null, configuration.SecurityConfiguration.TrustedPeerCertificates);
                }
                catch (Exception e)
                {
                    Utils.Trace(e, "Could not add certificate to LDS trust list.");
                }
            }

            // 设置防火墙
            if (InstallConfig.ConfigureFirewall)
            {
                ConfigureFirewall(configuration, silent, false);
            }

            // 设置HTTP访问
            ConfigureHttpAccess(configuration, false);
 
            // 设置可执行文件的访问，包括文件和私钥
            ConfigureFileAccess(configuration);

            // 更新配置文件
            ConfigUtils.UpdateConfigurationLocation(InstallConfig.ExecutableFile, InstallConfig.ConfigurationFile);

            try
            {
                // ensure the RawData does not get serialized.
                certificate = configuration.SecurityConfiguration.ApplicationCertificate.Certificate;

                configuration.SecurityConfiguration.ApplicationCertificate.Certificate = null;
                configuration.SecurityConfiguration.ApplicationCertificate.SubjectName = certificate.Subject;
                configuration.SecurityConfiguration.ApplicationCertificate.Thumbprint  = certificate.Thumbprint;

                configuration.SaveToFile(configuration.SourceFilePath);
                
                // restore the configuration.
                configuration.SecurityConfiguration.ApplicationCertificate.Certificate = certificate;
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Could not save configuration file. FilePath={0}", configuration.SourceFilePath);
            }

            if (!NoGdsAgentAdmin)
            {
                try
                {
                    // install the GDS agent configuration file
                    string agentPath = Utils.GetAbsoluteDirectoryPath("%CommonApplicationData%\\OPC Foundation\\GDS\\Applications", false, false, true);

                    if (agentPath != null)
                    {
                        Opc.Ua.Security.SecuredApplication export = new Opc.Ua.Security.SecurityConfigurationManager().ReadConfiguration(configuration.SourceFilePath);
                        export.ExecutableFile = InstallConfig.ExecutableFile;

                        DataContractSerializer serializer = new DataContractSerializer(typeof(Opc.Ua.Security.SecuredApplication));

                        using (FileStream ostrm = File.Open(agentPath + "\\" + configuration.ApplicationName + ".xml", FileMode.Create))
                        {
                            serializer.WriteObject(ostrm, export);
                            Utils.Trace(Utils.TraceMasks.Information, "Created GDS agent configuration file.");
                        }
                    }
                }
                catch (Exception e)
                {
                    Utils.Trace(Utils.TraceMasks.Error, "Could not create GDS agent configuration file: {0}", e.Message);
                }
            }

            // 安装服务
            if (InstallConfig.InstallAsService)
            {
                Utils.Trace(Utils.TraceMasks.Information, "Installing service '{0}'.", InstallConfig.ApplicationName);

                OnBeforeInstallService();

                bool start = true;

                bool result = Opc.Ua.Configuration.ServiceInstaller.InstallService(
                    Application.ExecutablePath,
                    InstallConfig.ApplicationName,
                    configuration.ApplicationName,
                    InstallConfig.ServiceDescription,
                    InstallConfig.ServiceStartMode,
                    InstallConfig.ServiceUserName,
                    InstallConfig.ServicePassword,
                    ref start);

                if (!result)
                {
                    throw ServiceResultException.Create(StatusCodes.BadConfigurationError, "Could not install service.");
                }

                Utils.Trace(Utils.TraceMasks.Information, "Service '{0}' installed as {1}.", InstallConfig.ApplicationName, InstallConfig.ServiceStartMode);
            }
        }

        /// <summary>
        /// 在服务安装之前立即调用
        /// </summary>
        protected virtual void OnBeforeInstallService()
        {
            // can be overridden in child class.
        }

        /// <summary>
        /// 卸载服务的方法
        /// </summary>
        /// <param name="silent">如果设置为 <c>true</c> 不会显示所有的对话框</param>
        /// <param name="args">由命令行提供的额外参数</param>
        protected virtual void Uninstall(bool silent, Dictionary<string, string> args)
        {
            // 检查配置
            string filePath = Utils.GetAbsoluteFilePath(InstallConfig.ConfigurationFile, true, false, false);

            if (filePath == null)
            {
                Utils.Trace("WARNING: Could not load config file specified in the installation configuration: {0}", InstallConfig.ConfigurationFile);
                filePath = ApplicationConfiguration.GetFilePathFromAppConfig(ConfigSectionName);
                InstallConfig.ConfigurationFile = filePath;
            }

            ApplicationConfiguration configuration = LoadAppConfig(silent, filePath, Opc.Ua.Security.SecuredApplication.FromApplicationType(InstallConfig.ApplicationType), ConfigurationType, false);
            ApplicationConfiguration = configuration;

            if (configuration != null)
            {
                // configure the firewall.
                ConfigureFirewall(configuration, false, true);

                // configure HTTP access.
                ConfigureHttpAccess(configuration, true);

                // delete certificate.
                if (InstallConfig.DeleteCertificatesOnUninstall)
                {
                    DeleteApplicationInstanceCertificate(configuration);
                }            
            }

            if (InstallConfig.InstallAsService)
            {
                if (!Opc.Ua.Configuration.ServiceInstaller.UnInstallService(InstallConfig.ApplicationName))
                {
                    Utils.Trace("Service could not be uninstalled.");
                }
            }

            if (!NoGdsAgentAdmin)
            {
                try
                {
                    string agentPath = Utils.GetAbsoluteDirectoryPath("%CommonApplicationData%\\OPC Foundation\\GDS\\Applications", false, false, false);

                    if (agentPath != null)
                    {
                        File.Delete(agentPath + "\\" + configuration.ApplicationName + ".xml");
                    }
                }
                catch (Exception e)
                {
                    Utils.Trace(Utils.TraceMasks.Error, "Could not create GDS agent configuration file: {0}", e.Message);
                }
            }
        }

        /// <summary>
        /// 处理命令行
        /// </summary>
        /// <param name="silent">如果设置为 <c>true</c> 不会显示任何的对话框</param>
        /// <param name="args">由命令行提供的额外参数</param>
        /// <returns>如果参数已被处理，返回True</returns>
        protected virtual bool ProcessCommand(bool silent, Dictionary<string, string> args)
        {
            return false;
        }
        #endregion

        #region 静态方法
        /// <summary>
        /// 载入一个配置
        /// </summary>
        public static ApplicationConfiguration LoadAppConfig(
            bool silent, 
            string filePath,
            ApplicationType applicationType,
            Type configurationType, 
            bool applyTraceSettings)
        {
            Utils.Trace(Utils.TraceMasks.Information, "Loading application configuration file. {0}", filePath);

            try
            {
                // load the configuration file.
                ApplicationConfiguration configuration = ApplicationConfiguration.Load(
                    new System.IO.FileInfo(filePath), 
                    applicationType,
                    configurationType,
                    applyTraceSettings);

                if (configuration == null)
                {
                    return null;
                }

                return configuration;
            }
            catch (Exception e)
            {
                // warn user.
                if (!silent)
                {
                    ExceptionDlg.Show("Load Application Configuration", e);
                }

                Utils.Trace(e, "Could not load configuration file. {0}", filePath);
                return null;
            }
        }

        /// <summary>
        /// 加载应用程序配置
        /// </summary>
        public ApplicationConfiguration LoadApplicationConfiguration(string filePath, bool silent)
        {
            ApplicationConfiguration configuration = LoadAppConfig(silent, filePath, ApplicationType, ConfigurationType, true);
            m_applicationConfiguration = configuration ?? throw ServiceResultException.Create(StatusCodes.BadConfigurationError, "Could not load configuration file.");

            return configuration;
        }

        /// <summary>
        /// 加载应用程序配置
        /// </summary>
        public ApplicationConfiguration LoadApplicationConfiguration(bool silent)
        {
            string filePath = ApplicationConfiguration.GetFilePathFromAppConfig(ConfigSectionName);
            ApplicationConfiguration configuration = LoadAppConfig(silent, filePath, ApplicationType, ConfigurationType, true);

            m_applicationConfiguration = configuration ?? throw ServiceResultException.Create(StatusCodes.BadConfigurationError, "Could not load configuration file.");

            return configuration;
        }

        /// <summary>
        /// 检查一个有效的应用程序实例证书
        /// </summary>
        /// <param name="silent">if set to <c>true</c> no dialogs will be displayed.</param>
        /// <param name="minimumKeySize">Minimum size of the key.</param>
        public void CheckApplicationInstanceCertificate(
            bool silent,
            ushort minimumKeySize)
        {
            Utils.Trace(Utils.TraceMasks.Information, "Checking application instance certificate.");

            ApplicationConfiguration configuration = null;

            if (m_applicationConfiguration == null)
            {
                LoadApplicationConfiguration(silent);
            }

            configuration = m_applicationConfiguration;
            bool createNewCertificate = true;

            // find the existing certificate.
            CertificateIdentifier id = configuration.SecurityConfiguration.ApplicationCertificate;

            if (id == null)
            {
                throw ServiceResultException.Create(StatusCodes.BadConfigurationError, "Configuration file does not specify a certificate.");
            }

            X509Certificate2 certificate = id.Find(true);

            // check that it is ok.
            if (certificate != null)
            {
                createNewCertificate = !CheckApplicationInstanceCertificate(configuration, certificate, silent, minimumKeySize);
            }
            else
            {
                // check for missing private key.
                certificate = id.Find(false);

                if (certificate != null)
                {
                    throw ServiceResultException.Create(StatusCodes.BadConfigurationError, "Cannot access certificate private key. Subject={0}", certificate.Subject);
                }

                // check for missing thumbprint.
                if (!String.IsNullOrEmpty(id.Thumbprint))
                {
                    if (!String.IsNullOrEmpty(id.SubjectName))
                    {
                        CertificateIdentifier id2 = new CertificateIdentifier();
                        id2.StoreType = id.StoreType;
                        id2.StorePath = id.StorePath;
                        id2.SubjectName = id.SubjectName;

                        certificate = id2.Find(true);
                    }

                    if (certificate != null)
                    {
                        string message = Utils.Format(
                            "Thumbprint was explicitly specified in the configuration." +
                            "\r\nAnother certificate with the same subject name was found." +
                            "\r\nUse it instead?\r\n" +
                            "\r\nRequested: {0}" +
                            "\r\nFound: {1}",
                            id.SubjectName,
                            certificate.Subject);

                        throw ServiceResultException.Create(StatusCodes.BadConfigurationError, message);
                    }
                    else
                    {
                        string message = Utils.Format("Thumbprint was explicitly specified in the configuration. Cannot generate a new certificate.");
                        throw ServiceResultException.Create(StatusCodes.BadConfigurationError, message);
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(id.SubjectName))
                    {
                        string message = Utils.Format("Both SubjectName and Thumbprint are not specified in the configuration. Cannot generate a new certificate.");
                        throw ServiceResultException.Create(StatusCodes.BadConfigurationError, message);
                    }
                }
            }
            
            // create a new certificate.
            if (createNewCertificate)
            {
                certificate = CreateApplicationInstanceCertificate(configuration, minimumKeySize, 600);
            }

            // ensure it is trusted.
            else
            {
                AddToTrustedStore(configuration, certificate);
            }

            // add to discovery server.
            if (configuration.ApplicationType == ApplicationType.Server || configuration.ApplicationType == ApplicationType.ClientAndServer)
            {
                try
                {
                    AddToDiscoveryServerTrustList(certificate, null, null, configuration.SecurityConfiguration.TrustedPeerCertificates);
                }
                catch (Exception e)
                {
                    Utils.Trace(e, "Could not add certificate to LDS trust list.");
                }
            }
        }
        #endregion

        #region HTTPS 支持
        /// <summary>
        /// 对HTTPS证书使用UA验证逻辑
        /// </summary>
        /// <param name="validator">The validator.</param>
        public static void SetUaValidationForHttps(CertificateValidator validator)
        {
            m_validator = validator;
            System.Net.ServicePointManager.ServerCertificateValidationCallback = HttpsCertificateValidation;
        }

        /// <summary>
        /// 远程验证证书
        /// </summary>
        private static bool HttpsCertificateValidation(
            object sender,
            X509Certificate cert,
            X509Chain chain,
            System.Net.Security.SslPolicyErrors error)
        {
            try
            {
                m_validator.Validate(new X509Certificate2(cert.GetRawCertData()));
                return true;
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Could not verify SSL certificate: {0}", cert.Subject);
                return false;
            }
        }

        private static CertificateValidator m_validator;
        #endregion

        #region 私有方法
        /// <summary>
        /// 处理证书验证错误
        /// </summary>
        private static void CertificateValidator_CertificateValidation(CertificateValidator validator, CertificateValidationEventArgs e)
        {
            try
            {
                if (e.Error != null && e.Error.Code == StatusCodes.BadCertificateUntrusted)
                {
                    e.Accept = true;
                    Utils.Trace((int)Utils.TraceMasks.Security, "Automatically accepted certificate: {0}", e.Certificate.Subject);
                }
            }
            catch (Exception exception)
            {
                Utils.Trace(exception, "Error accepting certificate.");
            }
        }

        /// <summary>
        /// 创建应用程序实例证书（如果尚不存在）。
        /// </summary>
        private static bool CheckApplicationInstanceCertificate(
            ApplicationConfiguration configuration,
            X509Certificate2 certificate,
            bool silent,
            ushort minimumKeySize)
        {
            if (certificate == null)
            {
                return false;
            }

            Utils.Trace(Utils.TraceMasks.Information, "Checking application instance certificate. {0}", certificate.Subject);

            // validate certificate.
            configuration.CertificateValidator.Validate(certificate);

            // check key size.
            if (minimumKeySize > certificate.PublicKey.Key.KeySize)
            {
                bool valid = false;

                string message = Utils.Format(
                    "The key size ({0}) in the certificate is less than the minimum provided ({1}). Update certificate?",
                    certificate.PublicKey.Key.KeySize,
                    minimumKeySize);

                if (!silent)
                {
                    if (MessageBox.Show(message, configuration.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    {
                        valid = true;
                    }
                }

                Utils.Trace(message);

                if (!valid)
                {
                    return false;
                }
            }

            // check domains.
            if (configuration.ApplicationType != ApplicationType.Client)
            {
                if (!CheckDomainsInCertificate(configuration, certificate, silent))
                {
                    return false;
                }
            }

            // update uri.
            string applicationUri = Utils.GetApplicationUriFromCertficate(certificate);

            if (String.IsNullOrEmpty(applicationUri))
            {
                bool valid = false;

                string message = "The Application URI is not specified in the certificate. Update certificate?";

                if (!silent)
                {
                    if (MessageBox.Show(message, configuration.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    {
                        valid = true;
                    }
                }

                Utils.Trace(message);

                if (!valid)
                {
                    return false;
                }
            }
            
            // update configuration.
            configuration.ApplicationUri = applicationUri;
            configuration.SecurityConfiguration.ApplicationCertificate.Certificate = certificate;

            return true;
        }

        /// <summary>
        /// 检查服务器地址中的域与证书中的域匹配
        /// </summary>
        private static bool CheckDomainsInCertificate(
            ApplicationConfiguration configuration,
            X509Certificate2 certificate,
            bool silent)
        {
            Utils.Trace(Utils.TraceMasks.Information, "Checking domains in certificate. {0}", certificate.Subject);

            bool valid = true;
            IList<string> serverDomainNames = configuration.GetServerDomainNames();
            IList<string> certificateDomainNames = Utils.GetDomainsFromCertficate(certificate);

            // 获取计算机的名称
            string computerName = System.Net.Dns.GetHostName();

            // get DNS aliases and IP addresses.
            System.Net.IPHostEntry entry = System.Net.Dns.GetHostEntry(computerName);

            for (int ii = 0; ii < serverDomainNames.Count; ii++)
            {
                if (Utils.FindStringIgnoreCase(certificateDomainNames, serverDomainNames[ii]))
                {
                    continue;
                }

                if (String.Compare(serverDomainNames[ii], "localhost", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (Utils.FindStringIgnoreCase(certificateDomainNames, computerName))
                    {
                        continue;
                    }

                    // check for aliases.
                    bool found = false;

                    for (int jj = 0; jj < entry.Aliases.Length; jj++)
                    {
                        if (Utils.FindStringIgnoreCase(certificateDomainNames, entry.Aliases[jj]))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found)
                    {
                        continue;
                    }

                    // check for ip addresses.
                    for (int jj = 0; jj < entry.AddressList.Length; jj++)
                    {
                        if (Utils.FindStringIgnoreCase(certificateDomainNames, entry.AddressList[jj].ToString()))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found)
                    {
                        continue;
                    }
                }

                string message = Utils.Format(
                    "The server is configured to use domain '{0}' which does not appear in the certificate. Update certificate?",
                    serverDomainNames[ii]);

                valid = false;

                if (!silent)
                {
                    if (MessageBox.Show(message, configuration.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    {
                        valid = true;
                        continue;
                    }
                }

                Utils.Trace(message);
                break;
            }

            return valid;
        }

        /// <summary>
        /// 设置防火墙
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="silent">if set to <c>true</c> if no dialogs should be displayed.</param>
        /// <param name="remove">if set to <c>true</c> if removing permissions.</param>
        private static void ConfigureFirewall(ApplicationConfiguration configuration, bool silent, bool remove)
        {
            Utils.Trace(Utils.TraceMasks.Information, "Configuring firewall.");

            // check for ports to open/close.
            StringCollection baseAddresses = new StringCollection();

            if (configuration.ServerConfiguration != null)
            {
                baseAddresses = configuration.ServerConfiguration.BaseAddresses;
            }

            if (configuration.DiscoveryServerConfiguration != null)
            {
                baseAddresses = configuration.DiscoveryServerConfiguration.BaseAddresses;
            }

            // remove access.
            if (remove)
            {
                try
                {
                    ConfigUtils.RemoveFirewallAccess(Application.ExecutablePath, baseAddresses);
                }
                catch (Exception e)
                {
                    Utils.Trace(e, "Unexpected error while removing firewall access.");
                }

                return;
            }

            // enable access.
            try
            {
                // check if firewall needs configuration.
                if (!ConfigUtils.CheckFirewallAccess(Application.ExecutablePath, baseAddresses))
                {
                    bool configure = true;

                    if (!silent)
                    {
                        string message = "The firewall has not been configured to allow external access to the server. Configure firewall?";

                        if (MessageBox.Show(message, configuration.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        {
                            configure = false;
                        }
                    }

                    if (configure)
                    {
                        ConfigUtils.SetFirewallAccess(configuration.ApplicationName, Application.ExecutablePath, baseAddresses);
                    }
                }
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Unexpected error while checking or changing the firewall configuration.");
            }
        }

        /// <summary>
        /// 创建应用程序实例证书
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="keySize">Size of the key.</param>
        /// <param name="lifetimeInMonths">The lifetime in months.</param>
        /// <returns>The new certificate</returns>
        private static X509Certificate2 CreateApplicationInstanceCertificate(
            ApplicationConfiguration configuration, 
            ushort keySize, 
            ushort lifetimeInMonths)
        {
            Utils.Trace(Utils.TraceMasks.Information, "Creating application instance certificate. KeySize={0}, Lifetime={1}", keySize, lifetimeInMonths);

            // delete existing any existing certificate.
            DeleteApplicationInstanceCertificate(configuration);

            CertificateIdentifier id = configuration.SecurityConfiguration.ApplicationCertificate;

            // get the domains from the configuration file.
            IList<string> serverDomainNames = configuration.GetServerDomainNames();

            if (serverDomainNames.Count == 0)
            {
                serverDomainNames.Add(System.Net.Dns.GetHostName());
            }

            // ensure the certificate store directory exists.
            if (id.StoreType == CertificateStoreType.Directory)
            {
                Utils.GetAbsoluteDirectoryPath(id.StorePath, true, true, true);
            }

            X509Certificate2 certificate = Opc.Ua.CertificateFactory.CreateCertificate(
                id.StoreType,
                id.StorePath,
                configuration.ApplicationUri,
                configuration.ApplicationName,
                null,
                serverDomainNames,
                keySize,
                lifetimeInMonths);

            id.Certificate = certificate;
            AddToTrustedStore(configuration, certificate);

            /*
            if (id.StoreType == CertificateStoreType.Directory)
            {
                DirectoryCertificateStore store = new DirectoryCertificateStore();
                store.Open(id.StorePath);

                List<ApplicationAccessRule> rules = new List<ApplicationAccessRule>();

                ApplicationAccessRule rule = new ApplicationAccessRule();
                rule.RuleType = AccessControlType.Allow;
                rule.Right = ApplicationAccessRight.Run;
                rule.IdentityName = WellKnownSids.NetworkService;
                rules.Add(rule);

                rule = new ApplicationAccessRule();
                rule.RuleType = AccessControlType.Allow;
                rule.Right = ApplicationAccessRight.Run;
                rule.IdentityName = WellKnownSids.LocalService;
                rules.Add(rule);

                rule = new ApplicationAccessRule();
                rule.RuleType = AccessControlType.Allow;
                rule.Right = ApplicationAccessRight.Run;
                rule.IdentityName = WellKnownSids.LocalSystem;
                rules.Add(rule);

                store.SetAccessRules(certificate.Thumbprint, rules, false);
            }
            */

            configuration.CertificateValidator.Update(configuration.SecurityConfiguration);

            Utils.Trace(Utils.TraceMasks.Information, "Certificate created. Thumbprint={0}", certificate.Thumbprint);

            // reload the certificate from disk.
            return configuration.SecurityConfiguration.ApplicationCertificate.LoadPrivateKey(null);
        }

        /// <summary>
        /// 删除一个已经存在的应用程序实例证书。
        /// </summary>
        /// <param name="configuration">The configuration instance that stores the configurable information for a UA application.</param>
        private static void DeleteApplicationInstanceCertificate(ApplicationConfiguration configuration)
        {
            Utils.Trace(Utils.TraceMasks.Information, "Deleting application instance certificate.");

            // create a default certificate id none specified.
            CertificateIdentifier id = configuration.SecurityConfiguration.ApplicationCertificate;

            if (id == null)
            {
                return;
            }

            // delete private key.
            X509Certificate2 certificate = id.Find();

            // delete trusted peer certificate.
            if (configuration.SecurityConfiguration != null && configuration.SecurityConfiguration.TrustedPeerCertificates != null)
            {
                string thumbprint = id.Thumbprint;

                if (certificate != null)
                {
                    thumbprint = certificate.Thumbprint;
                }

                using (ICertificateStore store = configuration.SecurityConfiguration.TrustedPeerCertificates.OpenStore())
                {
                    store.Delete(thumbprint);
                }
            }

            // delete private key.
            if (certificate != null)
            {
                using (ICertificateStore store = id.OpenStore())
                {
                    store.Delete(certificate.Thumbprint);
                }
            }
        }

        /// <summary>
        /// 将应用程序证书添加到发现服务器信任列表
        /// </summary>
        public static void AddToDiscoveryServerTrustList(
            X509Certificate2 certificate,
            string oldThumbprint,
            IList<X509Certificate2> issuers,
            CertificateStoreIdentifier trustedCertificateStore)
        {
            Utils.Trace(Utils.TraceMasks.Information, "Adding certificate to discovery server trust list.");

            try
            {
                string configurationPath = Utils.GetAbsoluteFilePath(@"%CommonApplicationData%\OPC Foundation\Config\Opc.Ua.DiscoveryServer.Config.xml", true, false, false);

                if (configurationPath == null)
                {
                    throw new ServiceResultException("Could not find the discovery server configuration file. Please confirm that it is installed.");
                }

                Opc.Ua.Security.SecuredApplication ldsConfiguration = new Opc.Ua.Security.SecurityConfigurationManager().ReadConfiguration(configurationPath);
                CertificateStoreIdentifier csid = Opc.Ua.Security.SecuredApplication.FromCertificateStoreIdentifier(ldsConfiguration.TrustedCertificateStore);
                AddApplicationCertificateToStore(csid, certificate, oldThumbprint);

                if (issuers != null && ldsConfiguration.IssuerCertificateStore != null)
                {
                    csid = Opc.Ua.Security.SecuredApplication.FromCertificateStoreIdentifier(ldsConfiguration.IssuerCertificateStore);
                    AddIssuerCertificatesToStore(csid, issuers);
                }

                CertificateIdentifier cid = Opc.Ua.Security.SecuredApplication.FromCertificateIdentifier(ldsConfiguration.ApplicationCertificate);
                X509Certificate2 ldsCertificate = cid.Find(false);

                // add LDS certificate to application trust list.
                if (ldsCertificate != null && trustedCertificateStore != null)
                {
                    AddApplicationCertificateToStore(csid, ldsCertificate, null);
                }
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Could not add certificate to discovery server trust list.");
            }
        }

        /// <summary>
        /// 将应用证书添加到存储
        /// </summary>
        private static void AddApplicationCertificateToStore(
            CertificateStoreIdentifier csid,
            X509Certificate2 certificate,
            string oldThumbprint)
        {
            ICertificateStore store = csid.OpenStore();

            try
            {
                // 删除旧的证书
                if (oldThumbprint != null)
                {
                    store.Delete(oldThumbprint);
                }

                // delete certificates with the same application uri.
                if (store.FindByThumbprint(certificate.Thumbprint) == null)
                {
                    string applicationUri = Utils.GetApplicationUriFromCertficate(certificate);

                    // delete any existing certificates.
                    foreach (X509Certificate2 target in store.Enumerate())
                    {
                        if (Utils.CompareDistinguishedName(target.Subject, certificate.Subject))
                        {
                            if (Utils.GetApplicationUriFromCertficate(target) == applicationUri)
                            {
                                store.Delete(target.Thumbprint);
                            }
                        }
                    }

                    // add new certificate.
                    store.Add(new X509Certificate2(certificate.RawData));
                }
            }
            finally
            {
                store.Close();
            }
        }

        /// <summary>
        /// 将应用证书添加到存储
        /// </summary>
        private static void AddIssuerCertificatesToStore(CertificateStoreIdentifier csid, IList<X509Certificate2> issuers)
        {
            ICertificateStore store = csid.OpenStore();

            try
            {
                foreach (X509Certificate2 issuer in issuers)
                {
                    if (store.FindByThumbprint(issuer.Thumbprint) == null)
                    {
                        store.Add(issuer);
                    }
                }
            }
            finally
            {
                store.Close();
            }
        }

        /// <summary>
        /// 将证书添加到可信证书存储
        /// </summary>
        /// <param name="configuration">The application's configuration which specifies the location of the TrustedStore.</param>
        /// <param name="certificate">The certificate to register.</param>
        private static void AddToTrustedStore(ApplicationConfiguration configuration, X509Certificate2 certificate)
        {
            string storePath = null;

            if (configuration != null && configuration.SecurityConfiguration != null && configuration.SecurityConfiguration.TrustedPeerCertificates != null)
            {
                storePath = configuration.SecurityConfiguration.TrustedPeerCertificates.StorePath;
            }

            if (String.IsNullOrEmpty(storePath))
            {
                Utils.Trace(Utils.TraceMasks.Information, "WARNING: Trusted peer store not specified.");
                return;
            }

            try
            {
                ICertificateStore store = configuration.SecurityConfiguration.TrustedPeerCertificates.OpenStore();

                if (store == null)
                {
                    Utils.Trace("Could not open trusted peer store. StorePath={0}", storePath);
                    return;
                }

                try
                {
                    // check if it already exists.
                    X509Certificate2 certificate2 = store.FindByThumbprint(certificate.Thumbprint);

                    if (certificate2 != null)
                    {
                        return;
                    } 
                    
                    Utils.Trace(Utils.TraceMasks.Information, "Adding certificate to trusted peer store. StorePath={0}", storePath);

                    List<string> subjectName = Utils.ParseDistinguishedName(certificate.Subject);

                    // check for old certificate.
                    X509Certificate2Collection certificates = store.Enumerate();

                    for (int ii = 0; ii < certificates.Count; ii++)
                    {
                        if (Utils.CompareDistinguishedName(certificates[ii], subjectName))
                        {
                            if (certificates[ii].Thumbprint == certificate.Thumbprint)
                            {
                                return;
                            }

                            store.Delete(certificates[ii].Thumbprint);
                            break;
                        }
                    }

                    // add new certificate.
                    X509Certificate2 publicKey = new X509Certificate2(certificate.GetRawCertData());
                    store.Add(publicKey);
                }
                finally
                {
                    store.Close();
                }
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Could not add certificate to trusted peer store. StorePath={0}", storePath);
            }
        }

        /// <summary>
        /// 设置 HTTP 访问.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="remove">if set to <c>true</c> then the HTTP access should be removed.</param>
        private void ConfigureHttpAccess(ApplicationConfiguration configuration, bool remove)
        {
            Utils.Trace(Utils.TraceMasks.Information, "Configuring HTTP access.");

            // check for HTTP endpoints which need configuring.
            StringCollection baseAddresses = new StringCollection();

            if (configuration.DiscoveryServerConfiguration != null)
            {
                baseAddresses = configuration.DiscoveryServerConfiguration.BaseAddresses;
            }

            if (configuration.ServerConfiguration != null)
            {
                baseAddresses = configuration.ServerConfiguration.BaseAddresses;
            }

            // configure WCF http access.
            for (int ii = 0; ii < baseAddresses.Count; ii++)
            {
                string url = GetHttpUrlForAccessRule(baseAddresses[ii]);

                if (url != null)
                {
                    SetHttpAccessRules(url, remove);
                }
            }
        }

        /// <summary>
        /// 获取用于HTTP访问规则的HTTP URL。
        /// </summary>
        public static string GetHttpUrlForAccessRule(string baseAddress)
        {
            Uri url = Utils.ParseUri(baseAddress);

            if (url == null)
            {
                return null;
            }
            
            UriBuilder builder = new UriBuilder(url);

            switch (url.Scheme)
            {
                case Utils.UriSchemeHttps:
                {
                    builder.Path = String.Empty;
                    builder.Query = String.Empty;
                    break;
                }

                case Utils.UriSchemeNoSecurityHttp:
                {
                    builder.Scheme = Utils.UriSchemeHttp;
                    builder.Path = String.Empty;
                    builder.Query = String.Empty;
                    break;
                }

                case Utils.UriSchemeHttp:
                {
                    break;
                }

                default:
                {
                    return null;
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// 获取用于应用程序的访问规则
        /// </summary>
        private List<ApplicationAccessRule> GetAccessRules()
        {
            List<ApplicationAccessRule> rules = new List<ApplicationAccessRule>();

            // check for rules specified in the installer configuration.
            bool hasAdmin = false;

            if (InstallConfig.AccessRules != null)
            {
                for (int ii = 0; ii < InstallConfig.AccessRules.Count; ii++)
                {
                    ApplicationAccessRule rule = InstallConfig.AccessRules[ii];

                    if (rule.Right == ApplicationAccessRight.Configure && rule.RuleType == AccessControlType.Allow)
                    {
                        hasAdmin = true;
                        break;
                    }
                }

                rules = InstallConfig.AccessRules;
            }

            // provide some default rules.
            if (rules.Count == 0)
            {
                // give user run access.
                ApplicationAccessRule rule = new ApplicationAccessRule();
                rule.RuleType = AccessControlType.Allow;
                rule.Right = ApplicationAccessRight.Run;
                rule.IdentityName = WellKnownSids.Users;
                rules.Add(rule);

                // ensure service can access.
                if (InstallConfig.InstallAsService)
                {
                    rule = new ApplicationAccessRule();
                    rule.RuleType = AccessControlType.Allow;
                    rule.Right = ApplicationAccessRight.Run;
                    rule.IdentityName = WellKnownSids.NetworkService;
                    rules.Add(rule);

                    rule = new ApplicationAccessRule();
                    rule.RuleType = AccessControlType.Allow;
                    rule.Right = ApplicationAccessRight.Run;
                    rule.IdentityName = WellKnownSids.LocalService;
                    rules.Add(rule);
                }               
            }

            // ensure someone can change the configuration later.
            if (!hasAdmin)
            {
                ApplicationAccessRule rule = new ApplicationAccessRule();
                rule.RuleType = AccessControlType.Allow;
                rule.Right = ApplicationAccessRight.Configure;
                rule.IdentityName = WellKnownSids.Administrators;
                rules.Add(rule);
            }

            return rules;
        }

        /// <summary>
        /// 设置URL的HTTP访问规则
        /// </summary>
        private void SetHttpAccessRules(string url, bool remove)
        {
            try
            {
                List<ApplicationAccessRule> rules = new List<ApplicationAccessRule>();

                if (!remove)
                {
                    rules = GetAccessRules();
                }

                HttpAccessRule.SetAccessRules(new Uri(url), rules, false);
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Unexpected configuring the HTTP access rules.");
            }
        }

        /// <summary>
        /// 配置对可执行文件，配置文件和私钥的访问
        /// </summary>
        private void ConfigureFileAccess(ApplicationConfiguration configuration)
        {
            Utils.Trace(Utils.TraceMasks.Information, "Configuring file access.");

            List<ApplicationAccessRule> rules = GetAccessRules();

            // apply access rules to the excutable file.
            try
            {
                if (InstallConfig.SetExecutableFilePermissions)
                {
                    ApplicationAccessRule.SetAccessRules(InstallConfig.ExecutableFile, rules, true);
                }
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Could not set executable file permissions.");
            }

            // apply access rules to the configuration file.
            try
            {
                if (InstallConfig.SetConfigurationFilePermisions)
                {
                    ApplicationAccessRule.SetAccessRules(configuration.SourceFilePath, rules, true);
                }
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Could not set configuration file permissions.");
            }

            // apply access rules to the private key file.
            try
            {
                X509Certificate2 certificate = configuration.SecurityConfiguration.ApplicationCertificate.Find(true);

                if (certificate != null)
                {
                    ICertificateStore store = configuration.SecurityConfiguration.ApplicationCertificate.OpenStore();
                    store.SetAccessRules(certificate.Thumbprint, rules, true);
                }
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Could not set private key file permissions.");
            }
        }
        #endregion
        
        #region 私有字段
        private string m_applicationName;
        private ApplicationType m_applicationType;
        private string m_configSectionName;
        private Type m_configurationType;
        private InstalledApplication m_installConfig;
        private ServerBase m_server;
        private ApplicationConfiguration m_applicationConfiguration;
        #endregion
    }
}
