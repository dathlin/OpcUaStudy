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

            config.TraceConfiguration = new TraceConfiguration()
            {
                OutputFilePath = @"Logs\opc.ua.server.log.txt",
                DeleteOnLoad = true,
                TraceMasks = 515
            };
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
                BaseAddresses = new string[]
                {
                     url
                },
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
