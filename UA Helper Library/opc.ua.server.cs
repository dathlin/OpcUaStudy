using Opc.Ua.Configuration;
using Opc.Ua.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opc.Ua.Hsl
{
    /// <summary>
    /// 一个OPC UA的服务器对象
    /// </summary>
    public class OpcUaServer
    {
        #region 构造方法

        /// <summary>
        /// 实例化一个 <see cref="OpcUaServer"/> 对象
        /// </summary>
        /// <param name="url"></param>
        /// <param name="standardServer">服务器的实例对象</param>
        public OpcUaServer(string url, StandardServer standardServer)
        {

            m_application = new ApplicationInstance();

            m_application.ApplicationType = ApplicationType.Server;

            // load the application configuration.
            //application.LoadApplicationConfiguration(false);

            m_application.ApplicationConfiguration = GetDefaultConfiguration(url);

            // check the application certificate.
            m_application.CheckApplicationInstanceCertificate(false, 0);





            m_server = standardServer ?? throw new ArgumentNullException("standardServer");

            // start the server.
            m_application.Start(m_server);

            m_configuration = m_application.ApplicationConfiguration;


        }



        //private int m_IsStarted = 0;

        ///// <summary>
        ///// 启动服务器
        ///// </summary>
        //public void ServerStart()
        //{
        //    if (System.Threading.Interlocked.CompareExchange(ref m_IsStarted, 1, 0) == 0)
        //    {
        //        m_application.Start(m_server);
        //    }
        //}


        
        private ApplicationConfiguration GetDefaultConfiguration(string url)
        {
            ApplicationConfiguration config = new ApplicationConfiguration();
            

            config.ApplicationName = "OpcUaServer";
            config.ApplicationType = ApplicationType.Server;


            config.SecurityConfiguration = new SecurityConfiguration()
            {
                ApplicationCertificate = new CertificateIdentifier()
                {
                    StoreType = "Directory",
                    StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\MachineDefault",
                    SubjectName = config.ApplicationName,
                },

                TrustedPeerCertificates = new CertificateTrustList()
                {
                    StoreType = "Directory",
                    StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Applications",
                },

                TrustedIssuerCertificates = new CertificateTrustList()
                {
                    StoreType = "Directory",
                    StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Certificate Authorities",
                },

                RejectedCertificateStore = new CertificateStoreIdentifier()
                {
                    StoreType = "Directory",
                    StorePath = @"% CommonApplicationData%\OPC Foundation\CertificateStores\RejectedCertificates"
                }
            };
            config.TransportConfigurations = new TransportConfigurationCollection();
            config.TransportQuotas = new TransportQuotas();


            config.ServerConfiguration = new ServerConfiguration()
            {
                // 配置登录的地址
                BaseAddresses = new string[]
                {
                     url
                },

                //SecurityPolicies=new ServerSecurityPolicyCollection(new List<ServerSecurityPolicy>() {
                //    new ServerSecurityPolicy()
                //    {
                //        SecurityMode=MessageSecurityMode.Sign,
                //        SecurityLevel=0,
                //    }
                //}),

                UserTokenPolicies = new UserTokenPolicyCollection(new List<UserTokenPolicy>()
                {
                    new UserTokenPolicy(UserTokenType.Anonymous),  // 支持匿名方式登录
                    new UserTokenPolicy(UserTokenType.UserName),   // 支持用户名加密码方式登录
                }),

                DiagnosticsEnabled = false,        // 是否启用诊断
                MaxSessionCount = 1000,            // 最大打开会话数
                MinSessionTimeout = 10000,         // 允许该会话在与客户端断开时（单位毫秒）仍然保持连接的最小时间
                MaxSessionTimeout = 60000,         // 允许该会话在与客户端断开时（单位毫秒）仍然保持连接的最大时间
                MaxBrowseContinuationPoints = 50,  // 用于Browse / BrowseNext操作的连续点的最大数量。
                MaxQueryContinuationPoints = 50,   // 用于Query / QueryNext操作的连续点的最大数量
                MaxHistoryContinuationPoints = 500,// 用于HistoryRead操作的最大连续点数。
                MaxRequestAge = 1000000,           // 传入请求的最大年龄（旧请求被拒绝）。
                MinPublishingInterval = 100,       // 服务器支持的最小发布间隔（以毫秒为单位）
                MaxPublishingInterval = 3600000,   // 服务器支持的最大发布间隔（以毫秒为单位）1小时
                PublishingResolution = 50,         // 支持的发布间隔（以毫秒为单位）的最小差异
                MaxSubscriptionLifetime = 3600000, // 订阅将在没有客户端发布的情况下保持打开多长时间 1小时
                MaxMessageQueueSize = 100,         // 每个订阅队列中保存的最大消息数
                MaxNotificationQueueSize = 100,    // 为每个被监视项目保存在队列中的最大证书数
                MaxNotificationsPerPublish = 1000, // 每次发布的最大通知数
                MinMetadataSamplingInterval = 1000,// 元数据的最小采样间隔
                AvailableSamplingRates = new SamplingRateGroupCollection(new List<SamplingRateGroup>()
                {
                    new SamplingRateGroup(5, 5, 20),
                    new SamplingRateGroup(100, 100, 4),
                    new SamplingRateGroup(500, 250, 2),
                    new SamplingRateGroup(1000, 500, 20),
                }),                                // 可用的采样率
                MaxRegistrationInterval = 30000,   // 两次注册尝试之间的最大时间（以毫秒为单位）
                //NodeManagerSaveFile = string.Empty,// 包含节点的文件的路径由核心节点管理器持久化 ??
                
            };

            config.TraceConfiguration = new TraceConfiguration()
            {
                OutputFilePath = @"Logs\opc.ua.server.log.txt",
                DeleteOnLoad = true,
                TraceMasks = 515
            };

            

            config.CertificateValidator = new CertificateValidator();
            config.CertificateValidator.Update(config);
            config.Extensions = new XmlElementCollection();

            return config;
        }

        #endregion

        private ApplicationInstance m_application;
        private StandardServer m_server;
        private ApplicationConfiguration m_configuration;
        
        /// <summary>
        /// 应用程序的实例
        /// </summary>
        public ApplicationInstance AppInstance
        {
            get { return m_application; }
        }
        /// <summary>
        /// 应用程序的配置
        /// </summary>
        public ApplicationConfiguration AppConfig
        {
            get { return m_application.ApplicationConfiguration; }
        }


        ///// <summary>
        ///// 格式依据服务器的标准来指定
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="url">格式</param>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public bool WriteNode<T>(string url, T value)
        //{
        //    WriteValue valueToWrite = new WriteValue()
        //    {
        //        NodeId = new NodeId(url),
        //        AttributeId = Attributes.Value
        //    };
        //    valueToWrite.Value.Value = value;
        //    valueToWrite.Value.StatusCode = StatusCodes.Good;
        //    valueToWrite.Value.ServerTimestamp = DateTime.MinValue;
        //    valueToWrite.Value.SourceTimestamp = DateTime.MinValue;

        //    WriteValueCollection valuesToWrite = new WriteValueCollection
        //    {
        //        valueToWrite
        //    };

        //    // 写入当前的值
        //    RequestHeader head = new RequestHeader();

        //    m_server.Write(
        //        head,
        //        valuesToWrite,
        //        out StatusCodeCollection results,
        //        out DiagnosticInfoCollection diagnosticInfos);

        //    ClientBase.ValidateResponse(results, valuesToWrite);
        //    ClientBase.ValidateDiagnosticInfos(diagnosticInfos, valuesToWrite);

        //    return !StatusCode.IsBad(results[0]);
        //}

        ///// <summary>
        ///// 格式依据服务器的标准来指定
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="url"></param>
        ///// <returns></returns>
        //public T ReadNode<T>(string url)
        //{

        //    ReadValueId nodeToRead = new ReadValueId()
        //    {
        //        NodeId = new NodeId(url),
        //        AttributeId = Attributes.Value
        //    };
        //    ReadValueIdCollection nodesToRead = new ReadValueIdCollection
        //    {
        //        nodeToRead
        //    };

        //    // 读取当前的值
        //    m_server.Read(
        //        null,
        //        0,
        //        TimestampsToReturn.Neither,
        //        nodesToRead,
        //        out DataValueCollection results,
        //        out DiagnosticInfoCollection diagnosticInfos);

        //    ClientBase.ValidateResponse(results, nodesToRead);
        //    ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

        //    return (T)results[0].Value;
        //}
    }


}
