using dynamixel_sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace read_write {
    internal class ClawUtils {

        public int BAUDRATE;
        public string DEVICENAME;

        public const int PROTOCOL_VERSION = 2;

        public static readonly ushort CLAW_CLOSE = 1864;
        public static readonly ushort CLAW_OPEN = 3000;

        public readonly ushort WRITE_TARGET_POSITION = 116;

        public const int COMM_SUCCESS = 0;
        public const int COMM_TX_FAIL = -1001;
        public int dxl_comm_result = COMM_TX_FAIL;
        byte dxl_error = 0;

        public static int port_num;

        public ClawUtils(int BAUDRATE, string DEVICENAME, int PORT_NUM) {
            this.BAUDRATE = BAUDRATE;
            this.DEVICENAME = DEVICENAME;
            port_num = PORT_NUM;
        }

        public void OpenClaw() {
            dynamixel.write4ByteTxRx(port_num, PROTOCOL_VERSION, RobotArm.CLAW_SERVO, WRITE_TARGET_POSITION, CLAW_OPEN);
            if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, PROTOCOL_VERSION)) != COMM_SUCCESS) {
                Console.WriteLine(Marshal.PtrToStringAnsi(dynamixel.getTxRxResult(PROTOCOL_VERSION, dxl_comm_result)));
            } else if ((dxl_error = dynamixel.getLastRxPacketError(port_num, PROTOCOL_VERSION)) != 0) {
                Console.WriteLine(Marshal.PtrToStringAnsi(dynamixel.getRxPacketError(PROTOCOL_VERSION, dxl_error)));
            } else {
                Console.WriteLine("Claw Opened");
            }
        }

        public void CloseClaw() {
            dynamixel.write4ByteTxRx(port_num, PROTOCOL_VERSION, RobotArm.CLAW_SERVO, WRITE_TARGET_POSITION, CLAW_CLOSE);
            if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, PROTOCOL_VERSION)) != COMM_SUCCESS) {
                Console.WriteLine(Marshal.PtrToStringAnsi(dynamixel.getTxRxResult(PROTOCOL_VERSION, dxl_comm_result)));
            } else if ((dxl_error = dynamixel.getLastRxPacketError(port_num, PROTOCOL_VERSION)) != 0) {
                Console.WriteLine(Marshal.PtrToStringAnsi(dynamixel.getRxPacketError(PROTOCOL_VERSION, dxl_error)));
            } else {
                Console.WriteLine("Claw Closed");
            }
        }
    }
}
