using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Opc.Ua.Server;
using Opc.Ua.Hsl;

namespace WindowsFormsAppServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 配置opc ua的日志输出
            Opc.Ua.Utils.SetTraceLog(Application.StartupPath + @"\Logs\Opc.Ua.Huibo.txt", true);
            // http://117.48.203.204:62547/DataAccessServer
            // opc.tcp://localhost:62547/DataAccessServer
            server = new OpcUaServer("opc.tcp://localhost:62547/DataAccessServer", new DataAccessServer());//117.48.203.204
            StandardServer server2 = server.AppInstance.Server as StandardServer;
            dataAccessServer = server.AppInstance.Server as DataAccessServer;

            serverDiagnosticsCtrl1.Initialize(server2, server.AppConfig);
        }




        public OpcUaServer server { get; set; }
        public DataAccessServer dataAccessServer { get; set; }

        private void writeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //server.WriteNode("ns=2;s=1:Device B?Name", Guid.NewGuid().ToString("N"));
        }
        

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            dataAccessServer.CurrentNodeManager.SetEnable(checkBox1.Checked);
        }


        private double[] RobotLocation = new double[6];

        private void button14_Click(object sender, EventArgs e)
        {
            string data = Clipboard.GetText();
            try
            {
                string[] list = data.Split(',');
                textBox_J1.Text = list[0];
                textBox_J2.Text = list[1];
                textBox_J3.Text = list[2];
                textBox_J4.Text = list[3];
                textBox_J5.Text = list[4];
                textBox_J6.Text = list[5];
            }
            catch
            {
                MessageBox.Show("数据格式不对");
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            string data = Clipboard.GetText();
            try
            {
                string[] list = data.Split(',');
                textBox_Tx.Text = list[0];
                textBox_Ty.Text = list[1];
                textBox_Tz.Text = list[2];
                textBox_Rx.Text = list[3];
                textBox_Ry.Text = list[4];
                textBox_Rz.Text = list[5];
            }
            catch
            {
                MessageBox.Show("数据格式不对");
            }
        }



        #region MyRegion

        private void Set_Incremental_Buttons_Joints()
        {
            // update label units for the step:
            lblstepIncrement.Text = "Step (deg):";

            // Text to display on the positive motion buttons for Incremental Joint movement:
            btnTXpos.Text = "+J1";
            btnTYpos.Text = "+J2";
            btnTZpos.Text = "+J3";
            btnRXpos.Text = "+J4";
            btnRYpos.Text = "+J5";
            btnRZpos.Text = "+J6";

            // Text to display on the positive motion buttons for Incremental Joint movement:
            btnTXneg.Text = "-J1";
            btnTYneg.Text = "-J2";
            btnTZneg.Text = "-J3";
            btnRXneg.Text = "-J4";
            btnRYneg.Text = "-J5";
            btnRZneg.Text = "-J6";
        }

        private void Set_Incremental_Buttons_Cartesian()
        {
            // update label units for the step:
            lblstepIncrement.Text = "Step (mm):";

            // Text to display on the positive motion buttons for incremental Cartesian movements:
            btnTXpos.Text = "+Tx";
            btnTYpos.Text = "+Ty";
            btnTZpos.Text = "+Tz";
            btnRXpos.Text = "+Rx";
            btnRYpos.Text = "+Ry";
            btnRZpos.Text = "+Rz";

            // Text to display on the negative motion buttons for incremental Cartesian movements:
            btnTXneg.Text = "-Tx";
            btnTYneg.Text = "-Ty";
            btnTZneg.Text = "-Tz";
            btnRXneg.Text = "-Rx";
            btnRYneg.Text = "-Ry";
            btnRZneg.Text = "-Rz";
        }


        private void rad_Move_wrt_Reference_CheckedChanged(object sender, EventArgs e)
        {
            // 机坐标
            Set_Incremental_Buttons_Cartesian();
            dataAccessServer.CurrentNodeManager.RobotA.SetRobotMode(2);
        }

        private void rad_Move_wrt_Tool_CheckedChanged(object sender, EventArgs e)
        {
            // 工具坐标
            Set_Incremental_Buttons_Cartesian();
            dataAccessServer.CurrentNodeManager.RobotA.SetRobotMode(3);
        }
        private void rad_Move_Joints_CheckedChanged(object sender, EventArgs e)
        {
            // 关节
            Set_Incremental_Buttons_Joints();
            dataAccessServer.CurrentNodeManager.RobotA.SetRobotMode(1);
        }

        private void btnTXneg_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                try
                {
                    Incremental_Move(btn.Text); // send the text of the button as parameter
                }
                catch (Exception ex)
                {
                    MessageBox.Show("异常：" + ex.Message);
                }
            }
        }

        private double[] GetCurrentJoints()
        {
            double[] joints = new double[6];
            joints[0] = Convert.ToDouble(textBox_J1.Text);
            joints[1] = Convert.ToDouble(textBox_J2.Text);
            joints[2] = Convert.ToDouble(textBox_J3.Text);
            joints[3] = Convert.ToDouble(textBox_J4.Text);
            joints[4] = Convert.ToDouble(textBox_J5.Text);
            joints[5] = Convert.ToDouble(textBox_J6.Text);
            return joints;
        }

        private void ShowCurrentJoints(double[] joints)
        {
            textBox_J1.Text = joints[0].ToString();
            textBox_J2.Text = joints[1].ToString();
            textBox_J3.Text = joints[2].ToString();
            textBox_J4.Text = joints[3].ToString();
            textBox_J5.Text = joints[4].ToString();
            textBox_J6.Text = joints[5].ToString();
        }


        private double[] GetCurrentPose()
        {
            double[] joints = new double[6];
            joints[0] = Convert.ToDouble(textBox_Tx.Text);
            joints[1] = Convert.ToDouble(textBox_Ty.Text);
            joints[2] = Convert.ToDouble(textBox_Tz.Text);
            joints[3] = Convert.ToDouble(textBox_Rx.Text);
            joints[4] = Convert.ToDouble(textBox_Ry.Text);
            joints[5] = Convert.ToDouble(textBox_Rz.Text);
            return joints;
        }

        private void ShowCurrentPose(double[] joints)
        {
            textBox_Tx.Text = joints[0].ToString();
            textBox_Ty.Text = joints[1].ToString();
            textBox_Tz.Text = joints[2].ToString();
            textBox_Rx.Text = joints[3].ToString();
            textBox_Ry.Text = joints[4].ToString();
            textBox_Rz.Text = joints[5].ToString();
        }

        /// <summary>
        /// Move the the robot relative to the TCP
        /// </summary>
        /// <param name="movement"></param>
        private void Incremental_Move(string button_name)
        {
            if (button_name.Length < 3)
            {
                return;
            }

            // get the the sense of motion the first character as '+' or '-'
            double move_step = 0.0;
            if (button_name[0] == '+')
            {
                move_step = +Convert.ToDouble(numStep.Value);
            }
            else if (button_name[0] == '-')
            {
                move_step = -Convert.ToDouble(numStep.Value);
            }
            else
            {
               MessageBox.Show("Internal problem! Unexpected button name");
                return;
            }

            //////////////////////////////////////////////
            //////// if we are moving in the joint space:
            if (rad_Move_Joints.Checked)
            {
                // get all the exsist joints
                double[] joints = GetCurrentJoints();

                // get the moving axis (1, 2, 3, 4, 5 or 6)
                int joint_id = Convert.ToInt32(button_name[2].ToString()) - 1; // important, double array starts at 0

                joints[joint_id] = joints[joint_id] + move_step;

                // 设置点就可以
                ShowCurrentJoints(joints);
                // 发送给opc ua 服务器
                dataAccessServer.CurrentNodeManager.RobotA.SetRobotLocation(joints);
            }
            else
            {
                //////////////////////////////////////////////
                //////// if we are moving in the cartesian space
                // Button name examples: +Tx, -Tz, +Rx, +Ry, +Rz

                int move_id = 0;

                string[] move_types = new string[6] { "Tx", "Ty", "Tz", "Rx", "Ry", "Rz" };

                for (int i = 0; i < 6; i++)
                {
                    if (button_name.EndsWith(move_types[i]))
                    {
                        move_id = i;
                        break;
                    }
                }
                double[] move_xyzwpr = new double[6] { 0, 0, 0, 0, 0, 0 };
                move_xyzwpr[move_id] = move_step;
                Mat movement_pose = Mat.FromTxyzRxyz(move_xyzwpr);

                // the the current position of the robot (as a 4x4 matrix)
                Mat robot_pose = Mat.FromTxyzRxyz(GetCurrentPose());

                // Calculate the new position of the robot
                Mat new_robot_pose;
                bool is_TCP_relative_move = rad_Move_wrt_Tool.Checked;
                if (is_TCP_relative_move)
                {
                    // 工具参考系
                    // if the movement is relative to the TCP we must POST MULTIPLY the movement
                    new_robot_pose = robot_pose * movement_pose;
                }
                else
                {
                    // if the movement is relative to the reference frame we must PRE MULTIPLY the XYZ translation:
                    // new_robot_pose = movement_pose * robot_pose;
                    // Note: Rotation applies from the robot axes.


                    Mat transformation_axes = new Mat(robot_pose);
                    transformation_axes.setPos(0, 0, 0);
                    Mat movement_pose_aligned = transformation_axes.inv() * movement_pose * transformation_axes;
                    new_robot_pose = robot_pose * movement_pose_aligned;


                    /*
                    void NodeRobot::Set_HX_Delta(const Matrix4x4 deltaHX, const Matrix4x4 axesHwrtHB){
                        Matrix4x4 ROTadapt, invROTadapt, deltaHX_aligned;
                        Matrix4x4 invHX, newHX;
                        INV_4x4(invHX, HX);
                        MULT_4x4(ROTadapt, axesHwrtHB, invHX);//not sure if it is te right sense
                        H_SET_P(ROTadapt, 0,0,0);
                        INV_4x4(invROTadapt,ROTadapt);
                        MULT_4x4_3(deltaHX_aligned, ROTadapt, deltaHX, invROTadapt);
                        MULT_4x4(newHX, HX, deltaHX_aligned);
                        this->Set_HX_if_close(newHX);
                    }
                    */
                }

                // Then, we can do the movement:
                ShowCurrentPose(new_robot_pose.ToTxyzRxyz());
                // 发送给opc ua 服务器
                dataAccessServer.CurrentNodeManager.RobotA.SetRobotLocation(new_robot_pose.ToTxyzRxyz());




                // Some tips:
                // retrieve the current pose of the robot: the active TCP with respect to the current reference frame
                // Tip 1: use
                // ROBOT.setFrame()
                // to set a reference frame (object link or pose)
                //
                // Tip 2: use
                // ROBOT.setTool()
                // to set a tool frame (object link or pose)
                //
                // Tip 3: use
                // ROBOT.MoveL_Collision(j1, new_move)
                // to test if a movement is feasible by the robot before doing the movement
                //
            }

        }


        #endregion


    }
}
