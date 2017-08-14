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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using Opc.Ua;
using Opc.Ua.Client;

namespace Opc.Ua.Client.Controls
{
    /// <summary>
    /// 一个用于连接到服务器的工具栏
    /// </summary>
    public partial class ConnectServerCtrl : UserControl
    {
        #region Constructors
        /// <summary>
        /// 初始化对象
        /// </summary>
        public ConnectServerCtrl()
        {
            InitializeComponent();
            m_CertificateValidation = new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);
        }
        #endregion

        #region 私有的字段
        private ApplicationConfiguration m_configuration;
        private Session m_session;
        private int m_reconnectPeriod = 10;
        private SessionReconnectHandler m_reconnectHandler;
        private CertificateValidationEventHandler m_CertificateValidation;
        private EventHandler m_ReconnectComplete;
        private EventHandler m_ReconnectStarting;
        private EventHandler m_KeepAliveComplete;
        private EventHandler m_ConnectComplete;
        private StatusStrip m_StatusStrip;
        private ToolStripItem m_ServerStatusLB;
        private ToolStripItem m_StatusUpateTimeLB;
        #endregion

        #region 公共的成员
        /// <summary>
        /// 一个用于显示会话状态信息的状态栏。
        /// </summary>
        public StatusStrip StatusStrip
        {
            get { return m_StatusStrip; }
            
            set 
            { 
                if (!ReferenceEquals(m_StatusStrip, value))
                {
                    m_StatusStrip = value;

                    if (value != null)
                    {
                        m_ServerStatusLB = new ToolStripStatusLabel();
                        m_StatusUpateTimeLB = new ToolStripStatusLabel();
                        m_StatusStrip.Items.Add(m_ServerStatusLB);
                        m_StatusStrip.Items.Add(m_StatusUpateTimeLB);
                    }
                }
            }
        }

        /// <summary>
        /// A control that contains the last time a keep alive was returned from the server.
        /// 一个包含最后一次从服务器返回的信息的控件
        /// </summary>
        public ToolStripItem ServerStatusControl { get { return m_ServerStatusLB; } set { m_ServerStatusLB = value; } }

        /// <summary>
        /// A control that contains the last time a keep alive was returned from the server.
        /// 一个包含最后一次从服务器更新时间信息的控件
        /// </summary>
        public ToolStripItem StatusUpateTimeControl { get { return m_StatusUpateTimeLB; } set { m_StatusUpateTimeLB = value; } }

        /// <summary>
        /// 要创建的会话的名称
        /// </summary>
        public string SessionName { get; set; }

        /// <summary>
        /// 获取或设置一个标志，指示连接时是否应该忽略域检查
        /// </summary>
        public bool DisableDomainCheck { get; set; }

        /// <summary>
        /// 获取或设置在控件中显示的服务器地址（URL）
        /// </summary>
        public string ServerUrl
        {
            get 
            {
                if (UrlCB.SelectedIndex >= 0)
                {
                    return (string)UrlCB.SelectedItem;
                }

                return UrlCB.Text; 
            }

            set
            {
                UrlCB.SelectedIndex = -1;
                UrlCB.Text = value;
            }
        }

        /// <summary>
        /// 在连接服务器的时候是否使用安全设置
        /// </summary>
        public bool UseSecurity
        {
            get { return UseSecurityCK.Checked; }
            set { UseSecurityCK.Checked = value; }
        }

        /// <summary>
        /// The locales to use when creating the session.
        /// 创建会话时使用的区域设置。
        /// </summary>
        public string[] PreferredLocales { get; set; }

        /// <summary>
        /// 创建会话时要使用的用户身份
        /// </summary>
        public IUserIdentity UserIdentity { get; set; }

        /// <summary>
        /// 客户端应用程序的配置
        /// </summary>
        public ApplicationConfiguration Configuration
        {
            get { return m_configuration; }
            
            set 
            {
                if (!ReferenceEquals(m_configuration, value))
                {
                    if (m_configuration != null)
                    {
                        m_configuration.CertificateValidator.CertificateValidation -= m_CertificateValidation;
                    }

                    m_configuration = value;

                    if (m_configuration != null)
                    {
                        m_configuration.CertificateValidator.CertificateValidation += m_CertificateValidation;
                    }
                }
            }
        }

        /// <summary>
        /// 当前活动的会话
        /// </summary>
        public Session Session
        {
            get { return m_session; }
        }

        /// <summary>
        /// 重新连接尝试之间的秒数（0表示重新连接被禁用）。
        /// </summary>
        [DefaultValue(10)]
        public int ReconnectPeriod
        {
            get { return m_reconnectPeriod; }
            set { m_reconnectPeriod = value; }
        }

        /// <summary>
        /// Raised when a good keep alive from the server arrives.
        /// 当一个来自服务器的正确的活动状态到达时触发
        /// </summary>
        public event EventHandler KeepAliveComplete
        {
            add { m_KeepAliveComplete += value; }
            remove { m_KeepAliveComplete -= value; }
        }
        
        /// <summary>
        /// 当重连服务器操作启动的时候触发
        /// </summary>
        public event EventHandler ReconnectStarting
        {
            add { m_ReconnectStarting += value; }
            remove { m_ReconnectStarting -= value; }
        }

        /// <summary>
        /// 当重连操作完成的时候触发
        /// </summary>
        public event EventHandler ReconnectComplete
        {
            add { m_ReconnectComplete += value; }
            remove { m_ReconnectComplete -= value; }
        }

        /// <summary>
        /// 当连接服务器完成时触发，无论是成功还是失败
        /// </summary>
        public event EventHandler ConnectComplete
        {
            add { m_ConnectComplete += value; }
            remove { m_ConnectComplete -= value; }
        }

        /// <summary>
        /// Sets the URLs shown in the control.
        /// 设置在控件中显示的服务器地址集合
        /// </summary>
        public void SetAvailableUrls(IList<string> urls)
        {
            UrlCB.Items.Clear();

            if (urls != null)
            {
                foreach (string url in urls)
                {
                    int index = url.LastIndexOf("/discovery", StringComparison.InvariantCultureIgnoreCase);

                    if (index != -1)
                    {
                        UrlCB.Items.Add(url.Substring(0, index));
                        continue;
                    }

                    UrlCB.Items.Add(url);
                }

                if (UrlCB.Items.Count > 0)
                {
                    UrlCB.SelectedIndex = 0;
                }
            }
        }
                
        /// <summary>
        /// 创建一个新的对话
        /// </summary>
        /// <returns>The new session object.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Session Connect()
        {
            // 先把当前的会话断开
            Disconnect();

            // 确定所选择的服务器地址
            string serverUrl = UrlCB.Text;

            if (UrlCB.SelectedIndex >= 0)
            {
                serverUrl = (string)UrlCB.SelectedItem;
            }

            if (m_configuration == null)
            {
                throw new ArgumentNullException("m_configuration");
            }

            // 根据当前的设置，选择一个最好的节点
            EndpointDescription endpointDescription = ClientUtils.SelectEndpoint(serverUrl, UseSecurityCK.Checked);

            EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(m_configuration);
            ConfiguredEndpoint endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

            m_session = Session.Create(
                m_configuration,
                endpoint,
                false,
                !DisableDomainCheck,
                (String.IsNullOrEmpty(SessionName))?m_configuration.ApplicationName:SessionName,
                60000,
                UserIdentity,
                PreferredLocales);

            // set up keep alive callback.
            m_session.KeepAlive += new KeepAliveEventHandler(Session_KeepAlive);

            // raise an event.
            DoConnectComplete(null);

            // return the new session.
            return m_session;
        }

        /// <summary>
        /// 创建一个新的会话
        /// </summary>
        /// <param name="serverUrl">The URL of a server endpoint.</param>
        /// <param name="useSecurity">Whether to use security.</param>
        /// <returns>The new session object.</returns>
        public Session Connect(string serverUrl, bool useSecurity)
        {
            UrlCB.Text = serverUrl;
            UseSecurityCK.Checked = useSecurity;
            return Connect();
        }

        /// <summary>
        /// 断开和服务器的连接
        /// </summary>
        public void Disconnect()
        {
            //更新状态控件的信息
            UpdateStatus(false, DateTime.UtcNow, "Disconnected");

            // stop any reconnect operation.
            // 停止任何的重连操作
            if (m_reconnectHandler != null)
            {
                m_reconnectHandler.Dispose();
                m_reconnectHandler = null;
            }

            // disconnect any existing session.
            // 断开任何的存在的会话
            if (m_session != null)
            {
                m_session.Close(10000);
                m_session = null;
            }

            // 引发一个事件
            DoConnectComplete(null);
        }

        /// <summary>
        /// Prompts the user to choose a server on another host.
        /// </summary>
        public void Discover(string hostName)
        {
            string endpointUrl = new DiscoverServerDlg().ShowDialog(m_configuration, hostName);

            if (endpointUrl != null)
            {
                ServerUrl = endpointUrl;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Raises the connect complete event on the main GUI thread.
        /// 在主界面的UI线程上引发一个连接完成事件
        /// </summary>
        private void DoConnectComplete(object state)
        {
            if (m_ConnectComplete != null)
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new System.Threading.WaitCallback(DoConnectComplete), state);
                    return;
                }

                m_ConnectComplete(this, null);
            }
        }

        /// <summary>
        /// 根据当前的配置寻找一个最适合的服务器节点
        /// </summary>
        private EndpointDescription SelectEndpoint()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // determine the URL that was selected.
                string discoveryUrl = UrlCB.Text;

                if (UrlCB.SelectedIndex >= 0)
                {
                    discoveryUrl = (string)UrlCB.SelectedItem;
                }

                // return the selected endpoint.
                return ClientUtils.SelectEndpoint(discoveryUrl, UseSecurityCK.Checked);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 事件处理
        /// <summary>
        /// 更新状态工具栏
        /// </summary>
        /// <param name="error">Whether the status represents an error.</param>
        /// <param name="time">The time associated with the status.</param>
        /// <param name="status">The status message.</param>
        /// <param name="args">Arguments used to format the status message.</param>
        private void UpdateStatus(bool error, DateTime time, string status, params object[] args)
        {
            if (m_ServerStatusLB != null)
            {
                m_ServerStatusLB.Text = String.Format(status, args);
                m_ServerStatusLB.ForeColor = (error) ? Color.Red : Color.Empty;
            }

            if (m_StatusUpateTimeLB != null)
            {
                m_StatusUpateTimeLB.Text = time.ToLocalTime().ToString("hh:mm:ss");
                m_StatusUpateTimeLB.ForeColor = (error) ? Color.Red : Color.Empty;
            }
        }

        /// <summary>
        /// Handles a keep alive event from a session.
        /// 处理会话中维持状态的事件
        /// </summary>
        private void Session_KeepAlive(Session session, KeepAliveEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new KeepAliveEventHandler(Session_KeepAlive), session, e);
                return;
            }

            try
            {
                // 检查会话是否已经被丢弃
                if (!ReferenceEquals(session, m_session))
                {
                    return;
                }

                // start reconnect sequence on communication error.
                // 当通信出错的时候进行重连
                if (ServiceResult.IsBad(e.Status))
                {
                    if (m_reconnectPeriod <= 0)
                    {
                        UpdateStatus(true, e.CurrentTime, "Communication Error ({0})", e.Status);
                        return;
                    }

                    UpdateStatus(true, e.CurrentTime, "Reconnecting in {0}s", m_reconnectPeriod);

                    if (m_reconnectHandler == null)
                    {
                        m_ReconnectStarting?.Invoke(this, e);

                        m_reconnectHandler = new SessionReconnectHandler();
                        m_reconnectHandler.BeginReconnect(m_session, m_reconnectPeriod * 1000, Server_ReconnectComplete);
                    }

                    return;
                }

                // update status.
                // 更新状态
                UpdateStatus(false, e.CurrentTime, "Connected [{0}]", session.Endpoint.EndpointUrl);

                // raise any additional notifications.
                // 触发保持成功状态的事件，相当于心跳机制确认
                m_KeepAliveComplete?.Invoke(this, e);
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        /// <summary>
        /// 处理连接服务器按钮的点击事件
        /// </summary>
        private void Server_ConnectMI_Click(object sender, EventArgs e)
        {
            try
            {
                Connect();
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        /// <summary>
        /// Handles a reconnect event complete from the reconnect handler.
        /// 处理重连服务器之后的完成事件
        /// </summary>
        private void Server_ReconnectComplete(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler(Server_ReconnectComplete), sender, e);
                return;
            }

            try
            {
                // ignore callbacks from discarded objects.
                // 如果事件已经被丢弃，就放弃处理
                if (!ReferenceEquals(sender, m_reconnectHandler))
                {
                    return;
                }

                m_session = m_reconnectHandler.Session;
                m_reconnectHandler.Dispose();
                m_reconnectHandler = null;

                // raise any additional notifications.
                // 触发链接完成的通知
                m_ReconnectComplete?.Invoke(this, e);
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        /// <summary>
        /// Handles a certificate validation error.
        /// 处理整数验证失败的情况
        /// </summary>
        private void CertificateValidator_CertificateValidation(CertificateValidator sender, CertificateValidationEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new CertificateValidationEventHandler(CertificateValidator_CertificateValidation), sender, e);
                return;
            }

            try
            {
                e.Accept = m_configuration.SecurityConfiguration.AutoAcceptUntrustedCertificates;

                if (!m_configuration.SecurityConfiguration.AutoAcceptUntrustedCertificates)
                {
                    DialogResult result = MessageBox.Show(
                        e.Certificate.Subject,
                        "Untrusted Certificate",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    e.Accept = (result == DialogResult.Yes);
                }
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }
        #endregion
    }
}
