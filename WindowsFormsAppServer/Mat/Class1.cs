using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsAppServer
{
    /// <summary>
    /// Matrix class for robotics. 
    /// 简单矩阵类用于均匀运算
    /// </summary>
    public class Mat // simple matrix class for homogeneous operations
    {
        #region 基础属性


        public int rows;
        public int cols;
        public double[,] mat;

        public Mat L;
        public Mat U;


        #endregion

        #region MatException Class



        //  Class used for Matrix exceptions
        public class MatException : Exception
        {
            public MatException(string Message)
                : base(Message)
            { }
        }


        #endregion

        #region 构造方法

        /// <summary>
        /// Matrix class constructor for any size matrix
        /// 任何大小矩阵的矩阵类构造函数，初始为 0
        /// </summary>
        /// <param name="Rows">dimension 1 size (rows)</param>
        /// <param name="Cols">dimension 2 size (columns)</param>
        public Mat(int Rows, int Cols)         // Matrix Class constructor
        {
            rows = Rows;
            cols = Cols;
            mat = new double[rows, cols];
        }

        /// <summary>
        /// Matrix class constructor for a 4x4 homogeneous matrix
        /// 4x4均匀矩阵的矩阵类构造函数
        /// </summary>
        /// <param name="nx">Position [0,0]</param>
        /// <param name="ox">Position [0,1]</param>
        /// <param name="ax">Position [0,2]</param>
        /// <param name="tx">Position [0,3]</param>
        /// <param name="ny">Position [1,0]</param>
        /// <param name="oy">Position [1,1]</param>
        /// <param name="ay">Position [1,2]</param>
        /// <param name="ty">Position [1,3]</param>
        /// <param name="nz">Position [2,0]</param>
        /// <param name="oz">Position [2,1]</param>
        /// <param name="az">Position [2,2]</param>
        /// <param name="tz">Position [2,3]</param>
        public Mat(double nx, double ox, double ax, double tx, double ny, double oy, double ay, double ty, double nz, double oz, double az, double tz)         // Matrix Class constructor
        {
            rows = 4;
            cols = 4;
            mat = new double[rows, cols];
            mat[0, 0] = nx; mat[1, 0] = ny; mat[2, 0] = nz; mat[3, 0] = 0.0;
            mat[0, 1] = ox; mat[1, 1] = oy; mat[2, 1] = oz; mat[3, 1] = 0.0;
            mat[0, 2] = ax; mat[1, 2] = ay; mat[2, 2] = az; mat[3, 2] = 0.0;
            mat[0, 3] = tx; mat[1, 3] = ty; mat[2, 3] = tz; mat[3, 3] = 1.0;

        }

        /// <summary>
        /// Matrix class constructor for a 4x4 homogeneous matrix as a copy from another matrix
        /// </summary>
        /// <param name="pose">矩阵</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Mat(Mat pose)
        {
            rows = pose.rows;
            cols = pose.cols;

            mat = new double[rows, cols];

            // 复制
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    mat[i, j] = pose[i, j];
        }

        /// <summary>
        /// Matrix class constructor for a 4x1 vector [x,y,z,1]
        /// 构造一个4*1的一维矩阵
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <param name="z">z coordinate</param>
        public Mat(double x, double y, double z)
        {
            rows = 4;
            cols = 1;
            mat = new double[rows, cols];
            mat[0, 0] = x;
            mat[1, 0] = y;
            mat[2, 0] = z;
            mat[3, 0] = 1.0;
        }


        #endregion

        #region 静态方法

        #region 加减乘除运算操作


        // Operators
        public static Mat operator -(Mat m)
        { return Mat.Multiply(-1, m); }

        public static Mat operator +(Mat m1, Mat m2)
        { return Mat.Add(m1, m2); }

        public static Mat operator -(Mat m1, Mat m2)
        { return Mat.Add(m1, -m2); }

        public static Mat operator *(Mat m1, Mat m2)
        { return Mat.StrassenMultiply(m1, m2); }

        public static Mat operator *(double n, Mat m)
        { return Mat.Multiply(n, m); }


        #endregion


        //----------------------------------------------------
        //--------     Generic matrix usage    ---------------
        /// <summary>
        /// Return a translation matrix
        ///                 |  1   0   0   X |
        /// transl(X,Y,Z) = |  0   1   0   Y |
        ///                 |  0   0   1   Z |
        ///                 |  0   0   0   1 |
        /// </summary>
        /// <param name="x">translation along X (mm)</param>
        /// <param name="y">translation along Y (mm)</param>
        /// <param name="z">translation along Z (mm)</param>
        /// <returns></returns>
        public static Mat transl(double x, double y, double z)
        {
            Mat mat = Mat.IdentityMatrix(4, 4);
            mat.setPos(x, y, z);
            return mat;
        }

        /// <summary>
        /// Return a X-axis rotation matrix
        ///            |  1  0        0        0 |
        /// rotx(rx) = |  0  cos(rx) -sin(rx)  0 |
        ///            |  0  sin(rx)  cos(rx)  0 |
        ///            |  0  0        0        1 |
        /// </summary>
        /// <param name="rx">rotation around X axis (in radians)</param>
        /// <returns></returns>
        public static Mat rotx(double rx)
        {
            double cx = Math.Cos(rx);
            double sx = Math.Sin(rx);
            return new Mat(1, 0, 0, 0, 0, cx, -sx, 0, 0, sx, cx, 0);
        }

        /// <summary>
        /// Return a Y-axis rotation matrix
        ///            |  cos(ry)  0   sin(ry)  0 |
        /// roty(ry) = |  0        1   0        0 |
        ///            | -sin(ry)  0   cos(ry)  0 |
        ///            |  0        0   0        1 |
        /// </summary>
        /// <param name="ry">rotation around Y axis (in radians)</param>
        /// <returns></returns>
        public static Mat roty(double ry)
        {
            double cy = Math.Cos(ry);
            double sy = Math.Sin(ry);
            return new Mat(cy, 0, sy, 0, 0, 1, 0, 0, -sy, 0, cy, 0);
        }

        /// <summary>
        /// Return a Z-axis rotation matrix
        ///            |  cos(rz)  -sin(rz)   0   0 |
        /// rotz(rx) = |  sin(rz)   cos(rz)   0   0 |
        ///            |  0         0         1   0 |
        ///            |  0         0         0   1 |
        /// </summary>
        /// <param name="rz">rotation around Z axis (in radians)</param>
        /// <returns></returns>
        public static Mat rotz(double rz)
        {
            double cz = Math.Cos(rz);
            double sz = Math.Sin(rz);
            return new Mat(cz, -sz, 0, 0, sz, cz, 0, 0, 0, 0, 1, 0);
        }



        /// <summary>
        /// Calculates the pose from the position and euler angles ([x,y,z,r,p,w] vector)
        /// The result is the same as calling: H = transl(x,y,z)*rotz(w*pi/180)*roty(p*pi/180)*rotx(r*pi/180)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        /// <param name="p"></param>
        /// <param name="r"></param>
        /// <returns>Homogeneous matrix (4x4)</returns>
        static public Mat FromXYZRPW(double x, double y, double z, double w, double p, double r)
        {
            double a = r * Math.PI / 180.0;
            double b = p * Math.PI / 180.0;
            double c = w * Math.PI / 180.0;
            double ca = Math.Cos(a);
            double sa = Math.Sin(a);
            double cb = Math.Cos(b);
            double sb = Math.Sin(b);
            double cc = Math.Cos(c);
            double sc = Math.Sin(c);
            return new Mat(cb * cc, cc * sa * sb - ca * sc, sa * sc + ca * cc * sb, x, cb * sc, ca * cc + sa * sb * sc, ca * sb * sc - cc * sa, y, -sb, cb * sa, ca * cb, z);
        }

        /// <summary>
        /// Calculates the pose from the position and euler angles ([x,y,z,r,p,w] vector)
        //  The result is the same as calling: H = transl(x,y,z)*rotz(w*pi/180)*roty(p*pi/180)*rotx(r*pi/180)
        /// </summary>
        /// <param name="xyzwpr"></param>
        /// <returns>Homogeneous matrix (4x4)</returns>
        static public Mat FromXYZRPW(double[] xyzwpr)
        {
            if (xyzwpr.Length < 6)
            {
                return null;
            }
            return FromXYZRPW(xyzwpr[0], xyzwpr[1], xyzwpr[2], xyzwpr[3], xyzwpr[4], xyzwpr[5]);
        }

        /// <summary>
        /// Calculates the pose from the position and euler angles ([x,y,z,rx,ry,rz] array)
        /// The result is the same as calling: H = transl(x,y,z)*rotx(rx*pi/180)*roty(ry*pi/180)*rotz(rz*pi/180)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="rx"></param>
        /// <param name="ry"></param>
        /// <param name="rz"></param>
        /// <returns>Homogeneous matrix (4x4)</returns>
        public static Mat FromTxyzRxyz(double x, double y, double z, double rx, double ry, double rz)
        {
            double a = rx * Math.PI / 180.0;
            double b = ry * Math.PI / 180.0;
            double c = rz * Math.PI / 180.0;
            double crx = Math.Cos(a);
            double srx = Math.Sin(a);
            double cry = Math.Cos(b);
            double sry = Math.Sin(b);
            double crz = Math.Cos(c);
            double srz = Math.Sin(c);
            return new Mat(cry * crz, -cry * srz, sry, x, crx * srz + crz * srx * sry, crx * crz - srx * sry * srz, -cry * srx, y, srx * srz - crx * crz * sry, crz * srx + crx * sry * srz, crx * cry, z);
        }

        /// <summary>
        /// Calculates the pose from the position and euler angles ([x,y,z,rx,ry,rz] array)
        /// The result is the same as calling: H = transl(x,y,z)*rotx(rx*pi/180)*roty(ry*pi/180)*rotz(rz*pi/180)
        /// </summary>
        /// <returns>Homogeneous matrix (4x4)</returns>
        public static Mat FromTxyzRxyz(double[] xyzwpr)
        {
            if (xyzwpr.Length < 6)
            {
                return null;
            }
            return FromTxyzRxyz(xyzwpr[0], xyzwpr[1], xyzwpr[2], xyzwpr[3], xyzwpr[4], xyzwpr[5]);
        }




        /// <summary>
        /// Returns the quaternion of a pose (4x4 matrix)
        /// 返回姿势的四元数（4x4矩阵）
        /// </summary>
        /// <param name="Ti"></param>
        /// <returns></returns>
        public static double[] ToQuaternion(Mat Ti)
        {
            double[] q = new double[4];
            double a = (Ti[0, 0]);
            double b = (Ti[1, 1]);
            double c = (Ti[2, 2]);
            double sign2 = 1.0;
            double sign3 = 1.0;
            double sign4 = 1.0;
            if ((Ti[2, 1] - Ti[1, 2]) < 0)
            {
                sign2 = -1;
            }
            if ((Ti[0, 2] - Ti[2, 0]) < 0)
            {
                sign3 = -1;
            }
            if ((Ti[1, 0] - Ti[0, 1]) < 0)
            {
                sign4 = -1;
            }
            q[0] = 0.5 * Math.Sqrt(Math.Max(a + b + c + 1, 0));
            q[1] = 0.5 * sign2 * Math.Sqrt(Math.Max(a - b - c + 1, 0));
            q[2] = 0.5 * sign3 * Math.Sqrt(Math.Max(-a + b - c + 1, 0));
            q[3] = 0.5 * sign4 * Math.Sqrt(Math.Max(-a - b + c + 1, 0));
            return q;
        }

        /// <summary>
        /// Returns the pose (4x4 matrix) from quaternion data
        /// 从四元数据返回姿势（4x4矩阵）
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static Mat FromQuaternion(double[] qin)
        {
            double qnorm = Math.Sqrt(qin[0] * qin[0] + qin[1] * qin[1] + qin[2] * qin[2] + qin[3] * qin[3]);
            double[] q = new double[4];
            q[0] = qin[0] / qnorm;
            q[1] = qin[1] / qnorm;
            q[2] = qin[2] / qnorm;
            q[3] = qin[3] / qnorm;
            Mat pose = new Mat(1 - 2 * q[2] * q[2] - 2 * q[3] * q[3], 2 * q[1] * q[2] - 2 * q[3] * q[0], 2 * q[1] * q[3] + 2 * q[2] * q[0], 0, 2 * q[1] * q[2] + 2 * q[3] * q[0], 1 - 2 * q[1] * q[1] - 2 * q[3] * q[3], 2 * q[2] * q[3] - 2 * q[1] * q[0], 0, 2 * q[1] * q[3] - 2 * q[2] * q[0], 2 * q[2] * q[3] + 2 * q[1] * q[0], 1 - 2 * q[1] * q[1] - 2 * q[2] * q[2], 0);
            return pose;
        }

        /// <summary>
        /// Converts a pose to an ABB target
        /// 将姿势转换为ABB目标
        /// </summary>
        /// <param name="H"></param>
        /// <returns></returns>
        public static double[] ToABB(Mat H)
        {
            double[] q = ToQuaternion(H);
            double[] xyzq1234 = { H[0, 3], H[1, 3], H[2, 3], q[0], q[1], q[2], q[3] };
            return xyzq1234;
        }

        /// <summary>
        /// Calculates the pose from the position and euler angles ([x,y,z,r,p,w] vector)
        /// 从位置和欧拉角（[x，y，z，r，p，w]矢量）计算姿态
        /// Note: The difference between FromUR and FromXYZWPR is that the first one uses radians for the orientation and the second one uses degres
        /// The result is the same as calling: H = transl(x,y,z)*rotx(rx)*roty(ry)*rotz(rz)
        /// </summary>
        /// <param name="xyzwpr">The position and euler angles array</param>
        /// <returns>Homogeneous matrix (4x4)</returns>
        public static Mat FromUR(double[] xyzwpr)
        {
            double x = xyzwpr[0];
            double y = xyzwpr[1];
            double z = xyzwpr[2];
            double w = xyzwpr[3];
            double p = xyzwpr[4];
            double r = xyzwpr[5];
            double angle = Math.Sqrt(w * w + p * p + r * r);
            if (angle < 1e-6)
            {
                return Identity4x4();
            }
            double c = Math.Cos(angle);
            double s = Math.Sin(angle);
            double ux = w / angle;
            double uy = p / angle;
            double uz = r / angle;
            return new Mat(ux * ux + c * (1 - ux * ux), ux * uy * (1 - c) - uz * s, ux * uz * (1 - c) + uy * s, x, ux * uy * (1 - c) + uz * s, uy * uy + (1 - uy * uy) * c, uy * uz * (1 - c) - ux * s, y, ux * uz * (1 - c) - uy * s, uy * uz * (1 - c) + ux * s, uz * uz + (1 - uz * uz) * c, z);
        }



        #endregion

        #region MyRegion

        #endregion

        //----------------------------------------------------
        //------ Pose to xyzrpw and xyzrpw to pose------------
        /// <summary>
        /// Calculates the equivalent position and euler angles ([x,y,z,r,p,w] vector) of the given pose 
        /// 计算给定姿势的等效位置和欧拉角（[x，y，z，r，p，w]矢量
        /// Note: transl(x,y,z)*rotz(w*pi/180)*roty(p*pi/180)*rotx(r*pi/180)
        /// See also: FromXYZRPW()
        /// </summary>
        /// <returns>XYZWPR translation and rotation in mm and degrees</returns>
        public double[] ToXYZRPW()
        {
            double[] xyzwpr = new double[6];
            double x = mat[0, 3];
            double y = mat[1, 3];
            double z = mat[2, 3];
            double w, p, r;
            if (mat[2, 0] > (1.0 - 1e-6))
            {
                p = -Math.PI * 0.5;
                r = 0;
                w = Math.Atan2(-mat[1, 2], mat[1, 1]);
            }
            else if (mat[2, 0] < -1.0 + 1e-6)
            {
                p = 0.5 * Math.PI;
                r = 0;
                w = Math.Atan2(mat[1, 2], mat[1, 1]);
            }
            else
            {
                p = Math.Atan2(-mat[2, 0], Math.Sqrt(mat[0, 0] * mat[0, 0] + mat[1, 0] * mat[1, 0]));
                w = Math.Atan2(mat[1, 0], mat[0, 0]);
                r = Math.Atan2(mat[2, 1], mat[2, 2]);
            }
            xyzwpr[0] = x;
            xyzwpr[1] = y;
            xyzwpr[2] = z;
            xyzwpr[3] = r * 180.0 / Math.PI;
            xyzwpr[4] = p * 180.0 / Math.PI;
            xyzwpr[5] = w * 180.0 / Math.PI;
            return xyzwpr;
        }


        /// <summary>
        /// Calculates the equivalent position and euler angles ([x,y,z,rx,ry,rz] array) of a pose 
        /// 计算姿势的等效位置和欧拉角（[x，y，z，rx，ry，rz]数组
        /// Note: Pose = transl(x,y,z)*rotx(rx*pi/180)*roty(ry*pi/180)*rotz(rz*pi/180)
        /// See also: FromTxyzRxyz()
        /// </summary>
        /// <returns>XYZWPR translation and rotation in mm and degrees</returns>
        public double[] ToTxyzRxyz()
        {
            double[] xyzwpr = new double[6];
            double x = mat[0, 3];
            double y = mat[1, 3];
            double z = mat[2, 3];
            double rx1 = 0;
            double ry1 = 0;
            double rz1 = 0;


            double a = mat[0, 0];
            double b = mat[0, 1];
            double c = mat[0, 2];
            double d = mat[1, 2];
            double e = mat[2, 2];

            if (c == 1)
            {
                ry1 = 0.5 * Math.PI;
                rx1 = 0;
                rz1 = Math.Atan2(mat[1, 0], mat[1, 1]);
            }
            else if (c == -1)
            {
                ry1 = -Math.PI / 2;
                rx1 = 0;
                rz1 = Math.Atan2(mat[1, 0], mat[1, 1]);
            }
            else
            {
                double sy = c;
                double cy1 = +Math.Sqrt(1 - sy * sy);

                double sx1 = -d / cy1;
                double cx1 = e / cy1;

                double sz1 = -b / cy1;
                double cz1 = a / cy1;

                rx1 = Math.Atan2(sx1, cx1);
                ry1 = Math.Atan2(sy, cy1);
                rz1 = Math.Atan2(sz1, cz1);
            }

            xyzwpr[0] = x;
            xyzwpr[1] = y;
            xyzwpr[2] = z;
            xyzwpr[3] = rx1 * 180.0 / Math.PI;
            xyzwpr[4] = ry1 * 180.0 / Math.PI;
            xyzwpr[5] = rz1 * 180.0 / Math.PI;
            return xyzwpr;
        }


        /// <summary>
        /// Calculates the equivalent position and euler angles ([x,y,z,r,p,w] vector) of the given pose in Universal Robots format
        /// 计算通用机器人格式中给定姿势的等效位置和欧拉角([x，y，z，r，p，w]矢量)
        /// Note: The difference between ToUR and ToXYZWPR is that the first one uses radians for the orientation and the second one uses degres
        /// Note: transl(x,y,z)*rotx(rx*pi/180)*roty(ry*pi/180)*rotz(rz*pi/180)
        /// See also: FromXYZRPW()
        /// </summary>
        /// <returns>XYZWPR translation and rotation in mm and radians</returns>
        public double[] ToUR()
        {
            double[] xyzwpr = new double[6];
            double x = mat[0, 3];
            double y = mat[1, 3];
            double z = mat[2, 3];
            double angle = Math.Acos(Math.Min(Math.Max((mat[0, 0] + mat[1, 1] + mat[2, 2] - 1) / 2, -1), 1));
            double rx = mat[2, 1] - mat[1, 2];
            double ry = mat[0, 2] - mat[2, 0];
            double rz = mat[1, 0] - mat[0, 1];
            if (angle == 0)
            {
                rx = 0;
                ry = 0;
                rz = 0;
            }
            else
            {
                rx = rx * angle / (2 * Math.Sin(angle));
                ry = ry * angle / (2 * Math.Sin(angle));
                rz = rz * angle / (2 * Math.Sin(angle));
            }
            xyzwpr[0] = x;
            xyzwpr[1] = y;
            xyzwpr[2] = z;
            xyzwpr[3] = rx;
            xyzwpr[4] = ry;
            xyzwpr[5] = rz;
            return xyzwpr;
        }



        /// <summary>
        /// Converts a matrix into a one-dimensional array of doubles
        /// </summary>
        /// <returns>one-dimensional array</returns>
        public double[] ToDoubles()
        {
            int cnt = 0;
            double[] array = new double[rows * cols];
            for (int j = 0; j < cols; j++)
            {
                for (int i = 0; i < rows; i++)
                {
                    array[cnt] = mat[i, j];
                    cnt = cnt + 1;
                }
            }
            return array;
        }

        /// <summary>
        /// Check if the matrix is square
        /// </summary>
        public Boolean IsSquare()
        {
            return (rows == cols);
        }

        public Boolean Is4x4()
        {
            if (cols != 4 || rows != 4)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check if the matrix is homogeneous (4x4)
        /// </summary>
        public Boolean IsHomogeneous()
        {
            if (!Is4x4())
            {
                return false;
            }
            return true;
            /*
            test = self[0:3,0:3];
            test = test*test.tr()
            test[0,0] = test[0,0] - 1.0
            test[1,1] = test[1,1] - 1.0
            test[2,2] = test[2,2] - 1.0
            zero = 0.0
            for x in range(3):
                for y in range(3):
                    zero = zero + abs(test[x,y])
            if zero > 1e-4:
                return False
            return True
            */
        }

        /// <summary>
        /// Returns the inverse of a homogeneous matrix (4x4 matrix)
        /// </summary>
        /// <returns>Homogeneous matrix (4x4)</returns>
        public Mat inv()
        {
            if (!IsHomogeneous())
            {
                throw new MatException("Can't invert a non-homogeneous matrix");
            }
            double[] xyz = this.Pos();
            Mat mat_xyz = new Mat(xyz[0], xyz[1], xyz[2]);
            Mat hinv = this.Duplicate();
            hinv.setPos(0, 0, 0);
            hinv = hinv.Transpose();
            Mat new_pos = rotate(hinv, mat_xyz);
            hinv[0, 3] = -new_pos[0, 0];
            hinv[1, 3] = -new_pos[1, 0];
            hinv[2, 3] = -new_pos[2, 0];
            return hinv;
        }

        /// <summary>
        /// Rotate a vector given a matrix (rotation matrix or homogeneous matrix)
        /// </summary>
        /// <param name="pose">4x4 homogeneous matrix or 3x3 rotation matrix</param>
        /// <param name="vector">4x1 or 3x1 vector</param>
        /// <returns></returns>
        static public Mat rotate(Mat pose, Mat vector)
        {
            if (pose.cols < 3 || pose.rows < 3 || vector.rows < 3)
            {
                throw new MatException("Invalid matrix size");
            }
            Mat pose3x3 = pose.Duplicate();
            Mat vector3 = vector.Duplicate();
            pose3x3.rows = 3;
            pose3x3.cols = 3;
            vector3.rows = 3;
            return pose3x3 * vector3;
        }

        /// <summary>
        /// Returns the XYZ position of the Homogeneous matrix
        /// </summary>
        /// <returns>XYZ position</returns>
        public double[] Pos()
        {
            if (!Is4x4())
            {
                return null;
            }
            double[] xyz = new double[3];
            xyz[0] = mat[0, 3]; xyz[1] = mat[1, 3]; xyz[2] = mat[2, 3];
            return xyz;
        }

        /// <summary>
        /// Sets the 4x4 position of the Homogeneous matrix
        /// </summary>
        /// <param name="xyz">XYZ position</param>
        public void setPos(double[] xyz)
        {
            if (!Is4x4() || xyz.Length < 3)
            {
                return;
            }
            mat[0, 3] = xyz[0]; mat[1, 3] = xyz[1]; mat[2, 3] = xyz[2];
        }

        /// <summary>
        /// Sets the 4x4 position of the Homogeneous matrix
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="z">Z position</param>
        public void setPos(double x, double y, double z)
        {
            if (!Is4x4())
            {
                return;
            }
            mat[0, 3] = x; mat[1, 3] = y; mat[2, 3] = z;
        }


        public double this[int iRow, int iCol]      // Access this matrix as a 2D array
        {
            get { return mat[iRow, iCol]; }
            set { mat[iRow, iCol] = value; }
        }

        public Mat GetCol(int k)
        {
            Mat m = new Mat(rows, 1);
            for (int i = 0; i < rows; i++) m[i, 0] = mat[i, k];
            return m;
        }

        public void SetCol(Mat v, int k)
        {
            for (int i = 0; i < rows; i++) mat[i, k] = v[i, 0];
        }

        public Mat Duplicate()                   // Function returns the copy of this matrix
        {
            Mat matrix = new Mat(rows, cols);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    matrix[i, j] = mat[i, j];
            return matrix;
        }

        public static Mat ZeroMatrix(int iRows, int iCols)       // Function generates the zero matrix
        {
            Mat matrix = new Mat(iRows, iCols);
            for (int i = 0; i < iRows; i++)
                for (int j = 0; j < iCols; j++)
                    matrix[i, j] = 0;
            return matrix;
        }

        /// <summary>
        /// 计算生成单位矩阵
        /// | 1 0 0 0 |
        /// | 0 1 0 0 |
        /// | 0 0 1 0 |
        /// | 0 0 0 1 |
        /// </summary>
        /// <param name="iRows"></param>
        /// <param name="iCols"></param>
        /// <returns></returns>
        public static Mat IdentityMatrix(int iRows, int iCols)   // Function generates the identity matrix
        {
            Mat matrix = ZeroMatrix(iRows, iCols);
            for (int i = 0; i < Math.Min(iRows, iCols); i++)
                matrix[i, i] = 1;
            return matrix;
        }

        /// <summary>
        /// Returns an identity 4x4 matrix (homogeneous matrix)
        /// 返回一个身份4x4矩阵（均匀矩阵）
        /// </summary>
        /// <returns></returns>
        public static Mat Identity4x4()
        {
            return Mat.IdentityMatrix(4, 4);
        }

        /*
        public static Mat Parse(string ps)                        // Function parses the matrix from string
        {
            string s = NormalizeMatrixString(ps);
            string[] rows = Regex.Split(s, "\r\n");
            string[] nums = rows[0].Split(' ');
            Mat matrix = new Mat(rows.Length, nums.Length);
            try
            {
                for (int i = 0; i < rows.Length; i++)
                {
                    nums = rows[i].Split(' ');
                    for (int j = 0; j < nums.Length; j++) matrix[i, j] = double.Parse(nums[j]);
                }
            }
            catch (FormatException exc) { throw new MatException("Wrong input format!"); }
            return matrix;
        }*/

        public override string ToString()                           // Function returns matrix as a string
        {
            string s = "";
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++) s += String.Format("{0,5:0.00}", mat[i, j]) + " ";
                s += "\r\n";
            }
            return s;
        }

        /// <summary>
        /// Transpose a matrix
        /// </summary>
        /// <returns></returns>
        public Mat Transpose()
        {
            return Transpose(this);
        }
        public static Mat Transpose(Mat m)              // Matrix transpose, for any rectangular matrix
        {
            Mat t = new Mat(m.cols, m.rows);
            for (int i = 0; i < m.rows; i++)
                for (int j = 0; j < m.cols; j++)
                    t[j, i] = m[i, j];
            return t;
        }

        private static void SafeAplusBintoC(Mat A, int xa, int ya, Mat B, int xb, int yb, Mat C, int size)
        {
            for (int i = 0; i < size; i++)          // rows
                for (int j = 0; j < size; j++)     // cols
                {
                    C[i, j] = 0;
                    if (xa + j < A.cols && ya + i < A.rows) C[i, j] += A[ya + i, xa + j];
                    if (xb + j < B.cols && yb + i < B.rows) C[i, j] += B[yb + i, xb + j];
                }
        }

        private static void SafeAminusBintoC(Mat A, int xa, int ya, Mat B, int xb, int yb, Mat C, int size)
        {
            for (int i = 0; i < size; i++)          // rows
                for (int j = 0; j < size; j++)     // cols
                {
                    C[i, j] = 0;
                    if (xa + j < A.cols && ya + i < A.rows) C[i, j] += A[ya + i, xa + j];
                    if (xb + j < B.cols && yb + i < B.rows) C[i, j] -= B[yb + i, xb + j];
                }
        }

        private static void SafeACopytoC(Mat A, int xa, int ya, Mat C, int size)
        {
            for (int i = 0; i < size; i++)          // rows
                for (int j = 0; j < size; j++)     // cols
                {
                    C[i, j] = 0;
                    if (xa + j < A.cols && ya + i < A.rows) C[i, j] += A[ya + i, xa + j];
                }
        }

        private static void AplusBintoC(Mat A, int xa, int ya, Mat B, int xb, int yb, Mat C, int size)
        {
            for (int i = 0; i < size; i++)          // rows
                for (int j = 0; j < size; j++) C[i, j] = A[ya + i, xa + j] + B[yb + i, xb + j];
        }

        private static void AminusBintoC(Mat A, int xa, int ya, Mat B, int xb, int yb, Mat C, int size)
        {
            for (int i = 0; i < size; i++)          // rows
                for (int j = 0; j < size; j++) C[i, j] = A[ya + i, xa + j] - B[yb + i, xb + j];
        }

        private static void ACopytoC(Mat A, int xa, int ya, Mat C, int size)
        {
            for (int i = 0; i < size; i++)          // rows
                for (int j = 0; j < size; j++) C[i, j] = A[ya + i, xa + j];
        }

        private static Mat StrassenMultiply(Mat A, Mat B)                // Smart matrix multiplication
        {
            if (A.cols != B.rows) throw new MatException("Wrong dimension of matrix!");

            Mat R;

            int msize = Math.Max(Math.Max(A.rows, A.cols), Math.Max(B.rows, B.cols));

            if (msize < 32)
            {
                R = ZeroMatrix(A.rows, B.cols);
                for (int i = 0; i < R.rows; i++)
                    for (int j = 0; j < R.cols; j++)
                        for (int k = 0; k < A.cols; k++)
                            R[i, j] += A[i, k] * B[k, j];
                return R;
            }

            int size = 1; int n = 0;
            while (msize > size) { size *= 2; n++; };
            int h = size / 2;


            Mat[,] mField = new Mat[n, 9];

            /*
             *  8x8, 8x8, 8x8, ...
             *  4x4, 4x4, 4x4, ...
             *  2x2, 2x2, 2x2, ...
             *  . . .
             */

            int z;
            for (int i = 0; i < n - 4; i++)          // rows
            {
                z = (int)Math.Pow(2, n - i - 1);
                for (int j = 0; j < 9; j++) mField[i, j] = new Mat(z, z);
            }

            SafeAplusBintoC(A, 0, 0, A, h, h, mField[0, 0], h);
            SafeAplusBintoC(B, 0, 0, B, h, h, mField[0, 1], h);
            StrassenMultiplyRun(mField[0, 0], mField[0, 1], mField[0, 1 + 1], 1, mField); // (A11 + A22) * (B11 + B22);

            SafeAplusBintoC(A, 0, h, A, h, h, mField[0, 0], h);
            SafeACopytoC(B, 0, 0, mField[0, 1], h);
            StrassenMultiplyRun(mField[0, 0], mField[0, 1], mField[0, 1 + 2], 1, mField); // (A21 + A22) * B11;

            SafeACopytoC(A, 0, 0, mField[0, 0], h);
            SafeAminusBintoC(B, h, 0, B, h, h, mField[0, 1], h);
            StrassenMultiplyRun(mField[0, 0], mField[0, 1], mField[0, 1 + 3], 1, mField); //A11 * (B12 - B22);

            SafeACopytoC(A, h, h, mField[0, 0], h);
            SafeAminusBintoC(B, 0, h, B, 0, 0, mField[0, 1], h);
            StrassenMultiplyRun(mField[0, 0], mField[0, 1], mField[0, 1 + 4], 1, mField); //A22 * (B21 - B11);

            SafeAplusBintoC(A, 0, 0, A, h, 0, mField[0, 0], h);
            SafeACopytoC(B, h, h, mField[0, 1], h);
            StrassenMultiplyRun(mField[0, 0], mField[0, 1], mField[0, 1 + 5], 1, mField); //(A11 + A12) * B22;

            SafeAminusBintoC(A, 0, h, A, 0, 0, mField[0, 0], h);
            SafeAplusBintoC(B, 0, 0, B, h, 0, mField[0, 1], h);
            StrassenMultiplyRun(mField[0, 0], mField[0, 1], mField[0, 1 + 6], 1, mField); //(A21 - A11) * (B11 + B12);

            SafeAminusBintoC(A, h, 0, A, h, h, mField[0, 0], h);
            SafeAplusBintoC(B, 0, h, B, h, h, mField[0, 1], h);
            StrassenMultiplyRun(mField[0, 0], mField[0, 1], mField[0, 1 + 7], 1, mField); // (A12 - A22) * (B21 + B22);

            R = new Mat(A.rows, B.cols);                  // result

            /// C11
            for (int i = 0; i < Math.Min(h, R.rows); i++)          // rows
                for (int j = 0; j < Math.Min(h, R.cols); j++)     // cols
                    R[i, j] = mField[0, 1 + 1][i, j] + mField[0, 1 + 4][i, j] - mField[0, 1 + 5][i, j] + mField[0, 1 + 7][i, j];

            /// C12
            for (int i = 0; i < Math.Min(h, R.rows); i++)          // rows
                for (int j = h; j < Math.Min(2 * h, R.cols); j++)     // cols
                    R[i, j] = mField[0, 1 + 3][i, j - h] + mField[0, 1 + 5][i, j - h];

            /// C21
            for (int i = h; i < Math.Min(2 * h, R.rows); i++)          // rows
                for (int j = 0; j < Math.Min(h, R.cols); j++)     // cols
                    R[i, j] = mField[0, 1 + 2][i - h, j] + mField[0, 1 + 4][i - h, j];

            /// C22
            for (int i = h; i < Math.Min(2 * h, R.rows); i++)          // rows
                for (int j = h; j < Math.Min(2 * h, R.cols); j++)     // cols
                    R[i, j] = mField[0, 1 + 1][i - h, j - h] - mField[0, 1 + 2][i - h, j - h] + mField[0, 1 + 3][i - h, j - h] + mField[0, 1 + 6][i - h, j - h];

            return R;
        }

        // function for square matrix 2^N x 2^N

        private static void StrassenMultiplyRun(Mat A, Mat B, Mat C, int l, Mat[,] f)    // A * B into C, level of recursion, matrix field
        {
            int size = A.rows;
            int h = size / 2;

            if (size < 32)
            {
                for (int i = 0; i < C.rows; i++)
                    for (int j = 0; j < C.cols; j++)
                    {
                        C[i, j] = 0;
                        for (int k = 0; k < A.cols; k++) C[i, j] += A[i, k] * B[k, j];
                    }
                return;
            }

            AplusBintoC(A, 0, 0, A, h, h, f[l, 0], h);
            AplusBintoC(B, 0, 0, B, h, h, f[l, 1], h);
            StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 1 + 1], l + 1, f); // (A11 + A22) * (B11 + B22);

            AplusBintoC(A, 0, h, A, h, h, f[l, 0], h);
            ACopytoC(B, 0, 0, f[l, 1], h);
            StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 1 + 2], l + 1, f); // (A21 + A22) * B11;

            ACopytoC(A, 0, 0, f[l, 0], h);
            AminusBintoC(B, h, 0, B, h, h, f[l, 1], h);
            StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 1 + 3], l + 1, f); //A11 * (B12 - B22);

            ACopytoC(A, h, h, f[l, 0], h);
            AminusBintoC(B, 0, h, B, 0, 0, f[l, 1], h);
            StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 1 + 4], l + 1, f); //A22 * (B21 - B11);

            AplusBintoC(A, 0, 0, A, h, 0, f[l, 0], h);
            ACopytoC(B, h, h, f[l, 1], h);
            StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 1 + 5], l + 1, f); //(A11 + A12) * B22;

            AminusBintoC(A, 0, h, A, 0, 0, f[l, 0], h);
            AplusBintoC(B, 0, 0, B, h, 0, f[l, 1], h);
            StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 1 + 6], l + 1, f); //(A21 - A11) * (B11 + B12);

            AminusBintoC(A, h, 0, A, h, h, f[l, 0], h);
            AplusBintoC(B, 0, h, B, h, h, f[l, 1], h);
            StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 1 + 7], l + 1, f); // (A12 - A22) * (B21 + B22);

            /// C11
            for (int i = 0; i < h; i++)          // rows
                for (int j = 0; j < h; j++)     // cols
                    C[i, j] = f[l, 1 + 1][i, j] + f[l, 1 + 4][i, j] - f[l, 1 + 5][i, j] + f[l, 1 + 7][i, j];

            /// C12
            for (int i = 0; i < h; i++)          // rows
                for (int j = h; j < size; j++)     // cols
                    C[i, j] = f[l, 1 + 3][i, j - h] + f[l, 1 + 5][i, j - h];

            /// C21
            for (int i = h; i < size; i++)          // rows
                for (int j = 0; j < h; j++)     // cols
                    C[i, j] = f[l, 1 + 2][i - h, j] + f[l, 1 + 4][i - h, j];

            /// C22
            for (int i = h; i < size; i++)          // rows
                for (int j = h; j < size; j++)     // cols
                    C[i, j] = f[l, 1 + 1][i - h, j - h] - f[l, 1 + 2][i - h, j - h] + f[l, 1 + 3][i - h, j - h] + f[l, 1 + 6][i - h, j - h];
        }

        public static Mat StupidMultiply(Mat m1, Mat m2)                  // Stupid matrix multiplication
        {
            if (m1.cols != m2.rows) throw new MatException("Wrong dimensions of matrix!");

            Mat result = ZeroMatrix(m1.rows, m2.cols);
            for (int i = 0; i < result.rows; i++)
                for (int j = 0; j < result.cols; j++)
                    for (int k = 0; k < m1.cols; k++)
                        result[i, j] += m1[i, k] * m2[k, j];
            return result;
        }
        private static Mat Multiply(double n, Mat m)                          // Multiplication by constant n
        {
            Mat r = new Mat(m.rows, m.cols);
            for (int i = 0; i < m.rows; i++)
                for (int j = 0; j < m.cols; j++)
                    r[i, j] = m[i, j] * n;
            return r;
        }
        private static Mat Add(Mat m1, Mat m2)         // Add matrix
        {
            if (m1.rows != m2.rows || m1.cols != m2.cols) throw new MatException("Matrices must have the same dimensions!");
            Mat r = new Mat(m1.rows, m1.cols);
            for (int i = 0; i < r.rows; i++)
                for (int j = 0; j < r.cols; j++)
                    r[i, j] = m1[i, j] + m2[i, j];
            return r;
        }

        public static string NormalizeMatrixString(string matStr)   // From Andy - thank you! :)
        {
            // Remove any multiple spaces
            while (matStr.IndexOf("  ") != -1)
                matStr = matStr.Replace("  ", " ");

            // Remove any spaces before or after newlines
            matStr = matStr.Replace(" \r\n", "\r\n");
            matStr = matStr.Replace("\r\n ", "\r\n");

            // If the data ends in a newline, remove the trailing newline.
            // Make it easier by first replacing \r\n’s with |’s then
            // restore the |’s with \r\n’s
            matStr = matStr.Replace("\r\n", "|");
            while (matStr.LastIndexOf("|") == (matStr.Length - 1))
                matStr = matStr.Substring(0, matStr.Length - 1);

            matStr = matStr.Replace("|", "\r\n");
            return matStr.Trim();
        }



    }
}
