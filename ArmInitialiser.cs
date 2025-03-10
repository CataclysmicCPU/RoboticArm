using dynamixel_sdk;
using System;
using System.Runtime.InteropServices;

namespace read_write {
    internal class ArmInitialiser {

        public const int ADDR_PRO_TORQUE_ENABLE = 64;

        // Protocol version
        public const int PROTOCOL_VERSION = 2;                   // See which protocol version is used in the Dynamixel

        // Default setting
        public int BAUDRATE;
        public string DEVICENAME;              // Check which port is being used on your controller
                                               // ex) Windows: "COM1"   Linux: "/dev/ttyUSB0" Mac: "/dev/tty.usbserial-*"

        public const int TORQUE_ENABLE = 1;                  // Value for enabling the torque
        public const int TORQUE_DISABLE = 0;                  // Dynamixel moving status threshold

        public const byte ESC_ASCII_VALUE = 0x1b;

        public const int COMM_SUCCESS = 0;                   // Communication Success result value
        public const int COMM_TX_FAIL = -1001;

        public int dxl_comm_result = COMM_TX_FAIL;
        public int port_num;

        public int[] servoPos = new int[10];

        byte dxl_error = 0;

        public ArmInitialiser(int BAUDRATE, string DEVICENAME, int PORT_NUM) {
            this.BAUDRATE = BAUDRATE;
            this.DEVICENAME = DEVICENAME;
            port_num = PORT_NUM;

            // Initialize PacketHandler Structs
            dynamixel.packetHandler();

            // Open port
            if (dynamixel.openPort(port_num)) {
                Console.WriteLine("Succeeded to open the port!");
            } else {
                Console.WriteLine("Failed to open the port!");
                Console.WriteLine("Press any key to terminate...");
                Console.ReadKey();
                return;
            }

            // Set port baudrate
            if (dynamixel.setBaudRate(port_num, BAUDRATE)) {
                Console.WriteLine("Succeeded to change the baudrate!");
            } else {
                Console.WriteLine("Failed to change the baudrate!");
                Console.WriteLine("Press any key to terminate...");
                Console.ReadKey();
                return;
            }
            for (int i = 1; i <= 8; i++) {
                servoPos[i] = (int)dynamixel.read2ByteTxRx(port_num, PROTOCOL_VERSION, (byte)i, 132);
                Console.WriteLine("Servo Position " + i + " is: " + servoPos[i]);
            }
            servoPos[9] = 3000;
        }
        public void EnableTorque() {
            for (int i = 1; i <= 9; i++) {
                dynamixel.ping(port_num, PROTOCOL_VERSION, (byte)i);
                // Enable Dynamixel Torque
                dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, (byte)i, ADDR_PRO_TORQUE_ENABLE, TORQUE_ENABLE);
                if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, PROTOCOL_VERSION)) != COMM_SUCCESS) {
                    Console.WriteLine(Marshal.PtrToStringAnsi(dynamixel.getTxRxResult(PROTOCOL_VERSION, dxl_comm_result)));
                } else if ((dxl_error = dynamixel.getLastRxPacketError(port_num, PROTOCOL_VERSION)) != 0) {
                    Console.WriteLine(Marshal.PtrToStringAnsi(dynamixel.getRxPacketError(PROTOCOL_VERSION, dxl_error)));
                } else {
                    Console.WriteLine("Dynamixel has successfully torqued motor " + i);
                }
            }
        }
        public void DisableTorque() {
            for (int i = 1; i <= 9; i++) {
                dynamixel.ping(port_num, PROTOCOL_VERSION, (byte)i);
                // Enable Dynamixel Torque
                dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, (byte)i, ADDR_PRO_TORQUE_ENABLE, TORQUE_DISABLE);
                if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, PROTOCOL_VERSION)) != COMM_SUCCESS) {
                    Console.WriteLine(Marshal.PtrToStringAnsi(dynamixel.getTxRxResult(PROTOCOL_VERSION, dxl_comm_result)));
                } else if ((dxl_error = dynamixel.getLastRxPacketError(port_num, PROTOCOL_VERSION)) != 0) {
                    Console.WriteLine(Marshal.PtrToStringAnsi(dynamixel.getRxPacketError(PROTOCOL_VERSION, dxl_error)));
                } else {
                    Console.WriteLine("Dynamixel has successfully shutdown motor " + i);
                }
            }
        }
        public void RestartMotors() {
            for (int i = 1; i <= 9; i++) {
                dynamixel.ping(port_num, PROTOCOL_VERSION, (byte)i);
                // Enable Dynamixel Torque
                dynamixel.reboot(port_num, PROTOCOL_VERSION, (byte)i);
                if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, PROTOCOL_VERSION)) != COMM_SUCCESS) {
                    Console.WriteLine(Marshal.PtrToStringAnsi(dynamixel.getTxRxResult(PROTOCOL_VERSION, dxl_comm_result)));
                } else if ((dxl_error = dynamixel.getLastRxPacketError(port_num, PROTOCOL_VERSION)) != 0) {
                    Console.WriteLine(Marshal.PtrToStringAnsi(dynamixel.getRxPacketError(PROTOCOL_VERSION, dxl_error)));
                } else {
                    Console.WriteLine("Dynamixel has successfully rebooted motor " + i);
                }
            }
        }
    }
}
