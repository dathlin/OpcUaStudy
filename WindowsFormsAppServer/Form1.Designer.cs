namespace WindowsFormsAppServer
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.serverDiagnosticsCtrl1 = new Opc.Ua.Server.Controls.ServerDiagnosticsCtrl();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.writeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.button15 = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.textBox_Rz = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.textBox_Ry = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textBox_Rx = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textBox_Tz = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBox_Ty = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBox_Tx = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBox_J6 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox_J5 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox_J4 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_J3 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_J2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_J1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupIncrementalMove = new System.Windows.Forms.GroupBox();
            this.numStep = new System.Windows.Forms.NumericUpDown();
            this.lblstepIncrement = new System.Windows.Forms.Label();
            this.rad_Move_Joints = new System.Windows.Forms.RadioButton();
            this.rad_Move_wrt_Tool = new System.Windows.Forms.RadioButton();
            this.rad_Move_wrt_Reference = new System.Windows.Forms.RadioButton();
            this.btnTXneg = new System.Windows.Forms.Button();
            this.btnTXpos = new System.Windows.Forms.Button();
            this.btnTYneg = new System.Windows.Forms.Button();
            this.btnTYpos = new System.Windows.Forms.Button();
            this.btnRZpos = new System.Windows.Forms.Button();
            this.btnTZneg = new System.Windows.Forms.Button();
            this.btnRZneg = new System.Windows.Forms.Button();
            this.btnTZpos = new System.Windows.Forms.Button();
            this.btnRYpos = new System.Windows.Forms.Button();
            this.btnRXneg = new System.Windows.Forms.Button();
            this.btnRYneg = new System.Windows.Forms.Button();
            this.btnRXpos = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.groupIncrementalMove.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStep)).BeginInit();
            this.SuspendLayout();
            // 
            // serverDiagnosticsCtrl1
            // 
            this.serverDiagnosticsCtrl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.serverDiagnosticsCtrl1.Location = new System.Drawing.Point(0, 289);
            this.serverDiagnosticsCtrl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.serverDiagnosticsCtrl1.Name = "serverDiagnosticsCtrl1";
            this.serverDiagnosticsCtrl1.Size = new System.Drawing.Size(984, 362);
            this.serverDiagnosticsCtrl1.TabIndex = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(984, 27);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.writeToolStripMenuItem});
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(41, 21);
            this.testToolStripMenuItem.Text = "test";
            // 
            // writeToolStripMenuItem
            // 
            this.writeToolStripMenuItem.Name = "writeToolStripMenuItem";
            this.writeToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
            this.writeToolStripMenuItem.Text = "write";
            this.writeToolStripMenuItem.Click += new System.EventHandler(this.writeToolStripMenuItem_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(12, 31);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(75, 21);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "使能测试";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // button15
            // 
            this.button15.Location = new System.Drawing.Point(817, 236);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(53, 28);
            this.button15.TabIndex = 60;
            this.button15.Text = "paste";
            this.button15.UseVisualStyleBackColor = true;
            this.button15.Click += new System.EventHandler(this.button15_Click);
            // 
            // button14
            // 
            this.button14.Location = new System.Drawing.Point(655, 236);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(53, 28);
            this.button14.TabIndex = 59;
            this.button14.Text = "paste";
            this.button14.UseVisualStyleBackColor = true;
            this.button14.Click += new System.EventHandler(this.button14_Click);
            // 
            // textBox_Rz
            // 
            this.textBox_Rz.Location = new System.Drawing.Point(804, 207);
            this.textBox_Rz.Name = "textBox_Rz";
            this.textBox_Rz.Size = new System.Drawing.Size(88, 23);
            this.textBox_Rz.TabIndex = 58;
            this.textBox_Rz.Text = "0";
            this.textBox_Rz.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(756, 210);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(34, 17);
            this.label15.TabIndex = 57;
            this.label15.Text = "Rz：";
            // 
            // textBox_Ry
            // 
            this.textBox_Ry.Location = new System.Drawing.Point(804, 177);
            this.textBox_Ry.Name = "textBox_Ry";
            this.textBox_Ry.Size = new System.Drawing.Size(88, 23);
            this.textBox_Ry.TabIndex = 56;
            this.textBox_Ry.Text = "0";
            this.textBox_Ry.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(756, 180);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(34, 17);
            this.label14.TabIndex = 55;
            this.label14.Text = "Ry：";
            // 
            // textBox_Rx
            // 
            this.textBox_Rx.Location = new System.Drawing.Point(804, 149);
            this.textBox_Rx.Name = "textBox_Rx";
            this.textBox_Rx.Size = new System.Drawing.Size(88, 23);
            this.textBox_Rx.TabIndex = 54;
            this.textBox_Rx.Text = "0";
            this.textBox_Rx.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(756, 152);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(34, 17);
            this.label13.TabIndex = 53;
            this.label13.Text = "Rx：";
            // 
            // textBox_Tz
            // 
            this.textBox_Tz.Location = new System.Drawing.Point(804, 120);
            this.textBox_Tz.Name = "textBox_Tz";
            this.textBox_Tz.Size = new System.Drawing.Size(88, 23);
            this.textBox_Tz.TabIndex = 52;
            this.textBox_Tz.Text = "0";
            this.textBox_Tz.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(756, 123);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(33, 17);
            this.label12.TabIndex = 51;
            this.label12.Text = "Tz：";
            // 
            // textBox_Ty
            // 
            this.textBox_Ty.Location = new System.Drawing.Point(804, 90);
            this.textBox_Ty.Name = "textBox_Ty";
            this.textBox_Ty.Size = new System.Drawing.Size(88, 23);
            this.textBox_Ty.TabIndex = 50;
            this.textBox_Ty.Text = "0";
            this.textBox_Ty.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(756, 93);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(33, 17);
            this.label11.TabIndex = 49;
            this.label11.Text = "Ty：";
            // 
            // textBox_Tx
            // 
            this.textBox_Tx.Location = new System.Drawing.Point(804, 59);
            this.textBox_Tx.Name = "textBox_Tx";
            this.textBox_Tx.Size = new System.Drawing.Size(88, 23);
            this.textBox_Tx.TabIndex = 48;
            this.textBox_Tx.Text = "0";
            this.textBox_Tx.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(756, 62);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(33, 17);
            this.label10.TabIndex = 47;
            this.label10.Text = "Tx：";
            // 
            // textBox_J6
            // 
            this.textBox_J6.Location = new System.Drawing.Point(644, 207);
            this.textBox_J6.Name = "textBox_J6";
            this.textBox_J6.Size = new System.Drawing.Size(88, 23);
            this.textBox_J6.TabIndex = 46;
            this.textBox_J6.Text = "0";
            this.textBox_J6.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(596, 210);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(32, 17);
            this.label9.TabIndex = 45;
            this.label9.Text = "J6：";
            // 
            // textBox_J5
            // 
            this.textBox_J5.Location = new System.Drawing.Point(644, 177);
            this.textBox_J5.Name = "textBox_J5";
            this.textBox_J5.Size = new System.Drawing.Size(88, 23);
            this.textBox_J5.TabIndex = 44;
            this.textBox_J5.Text = "0";
            this.textBox_J5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(596, 180);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 17);
            this.label8.TabIndex = 43;
            this.label8.Text = "J5：";
            // 
            // textBox_J4
            // 
            this.textBox_J4.Location = new System.Drawing.Point(644, 149);
            this.textBox_J4.Name = "textBox_J4";
            this.textBox_J4.Size = new System.Drawing.Size(88, 23);
            this.textBox_J4.TabIndex = 42;
            this.textBox_J4.Text = "0";
            this.textBox_J4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(596, 152);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 17);
            this.label7.TabIndex = 41;
            this.label7.Text = "J4：";
            // 
            // textBox_J3
            // 
            this.textBox_J3.Location = new System.Drawing.Point(644, 120);
            this.textBox_J3.Name = "textBox_J3";
            this.textBox_J3.Size = new System.Drawing.Size(88, 23);
            this.textBox_J3.TabIndex = 40;
            this.textBox_J3.Text = "0";
            this.textBox_J3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(596, 123);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 17);
            this.label6.TabIndex = 39;
            this.label6.Text = "J3：";
            // 
            // textBox_J2
            // 
            this.textBox_J2.Location = new System.Drawing.Point(644, 90);
            this.textBox_J2.Name = "textBox_J2";
            this.textBox_J2.Size = new System.Drawing.Size(88, 23);
            this.textBox_J2.TabIndex = 38;
            this.textBox_J2.Text = "0";
            this.textBox_J2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(596, 93);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 17);
            this.label5.TabIndex = 37;
            this.label5.Text = "J2：";
            // 
            // textBox_J1
            // 
            this.textBox_J1.Location = new System.Drawing.Point(644, 59);
            this.textBox_J1.Name = "textBox_J1";
            this.textBox_J1.Size = new System.Drawing.Size(88, 23);
            this.textBox_J1.TabIndex = 36;
            this.textBox_J1.Text = "0";
            this.textBox_J1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(596, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 17);
            this.label4.TabIndex = 35;
            this.label4.Text = "J1：";
            // 
            // groupIncrementalMove
            // 
            this.groupIncrementalMove.Controls.Add(this.numStep);
            this.groupIncrementalMove.Controls.Add(this.lblstepIncrement);
            this.groupIncrementalMove.Controls.Add(this.rad_Move_Joints);
            this.groupIncrementalMove.Controls.Add(this.rad_Move_wrt_Tool);
            this.groupIncrementalMove.Controls.Add(this.rad_Move_wrt_Reference);
            this.groupIncrementalMove.Controls.Add(this.btnTXneg);
            this.groupIncrementalMove.Controls.Add(this.btnTXpos);
            this.groupIncrementalMove.Controls.Add(this.btnTYneg);
            this.groupIncrementalMove.Controls.Add(this.btnTYpos);
            this.groupIncrementalMove.Controls.Add(this.btnRZpos);
            this.groupIncrementalMove.Controls.Add(this.btnTZneg);
            this.groupIncrementalMove.Controls.Add(this.btnRZneg);
            this.groupIncrementalMove.Controls.Add(this.btnTZpos);
            this.groupIncrementalMove.Controls.Add(this.btnRYpos);
            this.groupIncrementalMove.Controls.Add(this.btnRXneg);
            this.groupIncrementalMove.Controls.Add(this.btnRYneg);
            this.groupIncrementalMove.Controls.Add(this.btnRXpos);
            this.groupIncrementalMove.Location = new System.Drawing.Point(361, 31);
            this.groupIncrementalMove.Name = "groupIncrementalMove";
            this.groupIncrementalMove.Size = new System.Drawing.Size(229, 240);
            this.groupIncrementalMove.TabIndex = 61;
            this.groupIncrementalMove.TabStop = false;
            this.groupIncrementalMove.Text = "Incremental Move";
            // 
            // numStep
            // 
            this.numStep.DecimalPlaces = 1;
            this.numStep.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numStep.Location = new System.Drawing.Point(93, 71);
            this.numStep.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numStep.Name = "numStep";
            this.numStep.Size = new System.Drawing.Size(96, 23);
            this.numStep.TabIndex = 48;
            this.numStep.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // lblstepIncrement
            // 
            this.lblstepIncrement.AutoSize = true;
            this.lblstepIncrement.Location = new System.Drawing.Point(6, 75);
            this.lblstepIncrement.Name = "lblstepIncrement";
            this.lblstepIncrement.Size = new System.Drawing.Size(71, 17);
            this.lblstepIncrement.TabIndex = 47;
            this.lblstepIncrement.Text = "Step (mm):";
            // 
            // rad_Move_Joints
            // 
            this.rad_Move_Joints.AutoSize = true;
            this.rad_Move_Joints.Location = new System.Drawing.Point(11, 53);
            this.rad_Move_Joints.Name = "rad_Move_Joints";
            this.rad_Move_Joints.Size = new System.Drawing.Size(90, 21);
            this.rad_Move_Joints.TabIndex = 2;
            this.rad_Move_Joints.Text = "Joint Move";
            this.rad_Move_Joints.UseVisualStyleBackColor = true;
            this.rad_Move_Joints.CheckedChanged += new System.EventHandler(this.rad_Move_Joints_CheckedChanged);
            // 
            // rad_Move_wrt_Tool
            // 
            this.rad_Move_wrt_Tool.AutoSize = true;
            this.rad_Move_wrt_Tool.Location = new System.Drawing.Point(11, 34);
            this.rad_Move_wrt_Tool.Name = "rad_Move_wrt_Tool";
            this.rad_Move_wrt_Tool.Size = new System.Drawing.Size(52, 21);
            this.rad_Move_wrt_Tool.TabIndex = 1;
            this.rad_Move_wrt_Tool.Text = "Tool";
            this.rad_Move_wrt_Tool.UseVisualStyleBackColor = true;
            this.rad_Move_wrt_Tool.CheckedChanged += new System.EventHandler(this.rad_Move_wrt_Tool_CheckedChanged);
            // 
            // rad_Move_wrt_Reference
            // 
            this.rad_Move_wrt_Reference.AutoSize = true;
            this.rad_Move_wrt_Reference.Location = new System.Drawing.Point(11, 16);
            this.rad_Move_wrt_Reference.Name = "rad_Move_wrt_Reference";
            this.rad_Move_wrt_Reference.Size = new System.Drawing.Size(84, 21);
            this.rad_Move_wrt_Reference.TabIndex = 0;
            this.rad_Move_wrt_Reference.Text = "Reference";
            this.rad_Move_wrt_Reference.UseVisualStyleBackColor = true;
            this.rad_Move_wrt_Reference.CheckedChanged += new System.EventHandler(this.rad_Move_wrt_Reference_CheckedChanged);
            // 
            // btnTXneg
            // 
            this.btnTXneg.Location = new System.Drawing.Point(37, 96);
            this.btnTXneg.Margin = new System.Windows.Forms.Padding(1);
            this.btnTXneg.Name = "btnTXneg";
            this.btnTXneg.Size = new System.Drawing.Size(60, 23);
            this.btnTXneg.TabIndex = 27;
            this.btnTXneg.Text = "X-";
            this.btnTXneg.UseVisualStyleBackColor = true;
            this.btnTXneg.Click += new System.EventHandler(this.btnTXneg_Click);
            // 
            // btnTXpos
            // 
            this.btnTXpos.Location = new System.Drawing.Point(142, 96);
            this.btnTXpos.Margin = new System.Windows.Forms.Padding(1);
            this.btnTXpos.Name = "btnTXpos";
            this.btnTXpos.Size = new System.Drawing.Size(60, 23);
            this.btnTXpos.TabIndex = 28;
            this.btnTXpos.Text = "X+";
            this.btnTXpos.UseVisualStyleBackColor = true;
            this.btnTXpos.Click += new System.EventHandler(this.btnTXneg_Click);
            // 
            // btnTYneg
            // 
            this.btnTYneg.Location = new System.Drawing.Point(37, 119);
            this.btnTYneg.Margin = new System.Windows.Forms.Padding(1);
            this.btnTYneg.Name = "btnTYneg";
            this.btnTYneg.Size = new System.Drawing.Size(60, 23);
            this.btnTYneg.TabIndex = 29;
            this.btnTYneg.Text = "Y-";
            this.btnTYneg.UseVisualStyleBackColor = true;
            this.btnTYneg.Click += new System.EventHandler(this.btnTXneg_Click);
            // 
            // btnTYpos
            // 
            this.btnTYpos.Location = new System.Drawing.Point(142, 119);
            this.btnTYpos.Margin = new System.Windows.Forms.Padding(1);
            this.btnTYpos.Name = "btnTYpos";
            this.btnTYpos.Size = new System.Drawing.Size(60, 23);
            this.btnTYpos.TabIndex = 30;
            this.btnTYpos.Text = "Y+";
            this.btnTYpos.UseVisualStyleBackColor = true;
            this.btnTYpos.Click += new System.EventHandler(this.btnTXneg_Click);
            // 
            // btnRZpos
            // 
            this.btnRZpos.Location = new System.Drawing.Point(142, 211);
            this.btnRZpos.Margin = new System.Windows.Forms.Padding(1);
            this.btnRZpos.Name = "btnRZpos";
            this.btnRZpos.Size = new System.Drawing.Size(60, 23);
            this.btnRZpos.TabIndex = 38;
            this.btnRZpos.Text = "rZ+";
            this.btnRZpos.UseVisualStyleBackColor = true;
            this.btnRZpos.Click += new System.EventHandler(this.btnTXneg_Click);
            // 
            // btnTZneg
            // 
            this.btnTZneg.Location = new System.Drawing.Point(37, 142);
            this.btnTZneg.Margin = new System.Windows.Forms.Padding(1);
            this.btnTZneg.Name = "btnTZneg";
            this.btnTZneg.Size = new System.Drawing.Size(60, 23);
            this.btnTZneg.TabIndex = 31;
            this.btnTZneg.Text = "Z-";
            this.btnTZneg.UseVisualStyleBackColor = true;
            this.btnTZneg.Click += new System.EventHandler(this.btnTXneg_Click);
            // 
            // btnRZneg
            // 
            this.btnRZneg.Location = new System.Drawing.Point(37, 211);
            this.btnRZneg.Margin = new System.Windows.Forms.Padding(1);
            this.btnRZneg.Name = "btnRZneg";
            this.btnRZneg.Size = new System.Drawing.Size(60, 23);
            this.btnRZneg.TabIndex = 37;
            this.btnRZneg.Text = "rZ-";
            this.btnRZneg.UseVisualStyleBackColor = true;
            this.btnRZneg.Click += new System.EventHandler(this.btnTXneg_Click);
            // 
            // btnTZpos
            // 
            this.btnTZpos.Location = new System.Drawing.Point(142, 142);
            this.btnTZpos.Margin = new System.Windows.Forms.Padding(1);
            this.btnTZpos.Name = "btnTZpos";
            this.btnTZpos.Size = new System.Drawing.Size(60, 23);
            this.btnTZpos.TabIndex = 32;
            this.btnTZpos.Text = "Z+";
            this.btnTZpos.UseVisualStyleBackColor = true;
            this.btnTZpos.Click += new System.EventHandler(this.btnTXneg_Click);
            // 
            // btnRYpos
            // 
            this.btnRYpos.Location = new System.Drawing.Point(142, 188);
            this.btnRYpos.Margin = new System.Windows.Forms.Padding(1);
            this.btnRYpos.Name = "btnRYpos";
            this.btnRYpos.Size = new System.Drawing.Size(60, 23);
            this.btnRYpos.TabIndex = 36;
            this.btnRYpos.Text = "rY+";
            this.btnRYpos.UseVisualStyleBackColor = true;
            this.btnRYpos.Click += new System.EventHandler(this.btnTXneg_Click);
            // 
            // btnRXneg
            // 
            this.btnRXneg.Location = new System.Drawing.Point(37, 165);
            this.btnRXneg.Margin = new System.Windows.Forms.Padding(1);
            this.btnRXneg.Name = "btnRXneg";
            this.btnRXneg.Size = new System.Drawing.Size(60, 23);
            this.btnRXneg.TabIndex = 33;
            this.btnRXneg.Text = "rX-";
            this.btnRXneg.UseVisualStyleBackColor = true;
            this.btnRXneg.Click += new System.EventHandler(this.btnTXneg_Click);
            // 
            // btnRYneg
            // 
            this.btnRYneg.Location = new System.Drawing.Point(37, 188);
            this.btnRYneg.Margin = new System.Windows.Forms.Padding(1);
            this.btnRYneg.Name = "btnRYneg";
            this.btnRYneg.Size = new System.Drawing.Size(60, 23);
            this.btnRYneg.TabIndex = 35;
            this.btnRYneg.Text = "rY-";
            this.btnRYneg.UseVisualStyleBackColor = true;
            this.btnRYneg.Click += new System.EventHandler(this.btnTXneg_Click);
            // 
            // btnRXpos
            // 
            this.btnRXpos.Location = new System.Drawing.Point(142, 165);
            this.btnRXpos.Margin = new System.Windows.Forms.Padding(1);
            this.btnRXpos.Name = "btnRXpos";
            this.btnRXpos.Size = new System.Drawing.Size(60, 23);
            this.btnRXpos.TabIndex = 34;
            this.btnRXpos.Text = "rX+";
            this.btnRXpos.UseVisualStyleBackColor = true;
            this.btnRXpos.Click += new System.EventHandler(this.btnTXneg_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(984, 652);
            this.Controls.Add(this.groupIncrementalMove);
            this.Controls.Add(this.button15);
            this.Controls.Add(this.button14);
            this.Controls.Add(this.textBox_Rz);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.textBox_Ry);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.textBox_Rx);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.textBox_Tz);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.textBox_Ty);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.textBox_Tx);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.textBox_J6);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.textBox_J5);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBox_J4);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBox_J3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBox_J2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox_J1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.serverDiagnosticsCtrl1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "OPC UA 测试";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupIncrementalMove.ResumeLayout(false);
            this.groupIncrementalMove.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStep)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Opc.Ua.Server.Controls.ServerDiagnosticsCtrl serverDiagnosticsCtrl1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem writeToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button15;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.TextBox textBox_Rz;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textBox_Ry;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBox_Rx;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBox_Tz;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBox_Ty;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBox_Tx;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBox_J6;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBox_J5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox_J4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox_J3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_J2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_J1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupIncrementalMove;
        private System.Windows.Forms.NumericUpDown numStep;
        private System.Windows.Forms.Label lblstepIncrement;
        private System.Windows.Forms.RadioButton rad_Move_Joints;
        private System.Windows.Forms.RadioButton rad_Move_wrt_Tool;
        private System.Windows.Forms.RadioButton rad_Move_wrt_Reference;
        private System.Windows.Forms.Button btnTXneg;
        private System.Windows.Forms.Button btnTXpos;
        private System.Windows.Forms.Button btnTYneg;
        private System.Windows.Forms.Button btnTYpos;
        private System.Windows.Forms.Button btnRZpos;
        private System.Windows.Forms.Button btnTZneg;
        private System.Windows.Forms.Button btnRZneg;
        private System.Windows.Forms.Button btnTZpos;
        private System.Windows.Forms.Button btnRYpos;
        private System.Windows.Forms.Button btnRXneg;
        private System.Windows.Forms.Button btnRYneg;
        private System.Windows.Forms.Button btnRXpos;
    }
}

