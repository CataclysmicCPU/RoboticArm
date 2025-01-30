using dynamixel_sdk;
using XInputDotNetPure;
using System.Windows.Input;

namespace read_write {
    internal class RobotArm {

        public const int BAUDRATE = 1000000;
        public const string PORT = "COM3";
        public static int BULK_WRITE_MOVE = 0;
        public static int PORT_NUM;

        public const int COMM_SUCCESS = 0;
        public const int COMM_TX_FAIL = -1001;

        public static readonly byte[] SHOULDER_SERVOS = { 1, 2, 3 };
        public static readonly byte[] ELBOW_SERVOS = { 4, 5 };
        public static readonly byte[] WRIST_SERVOS = { 6, 7, 8 };
        public static readonly byte CLAW_SERVO = 9;
        public static readonly byte GOAL_POSITION_ADRESS = 116;
        public static readonly byte LEN_GOAL_POSITION = 4;
        public static int[]? SERVO_POS;

        static void Main(string[] args) {
            Console.WriteLine("//////////////Arm_Init/////////////");
            PORT_NUM = dynamixel.portHandler(PORT);
            BULK_WRITE_MOVE = dynamixel.groupSyncWrite(PORT_NUM, ClawUtils.PROTOCOL_VERSION, GOAL_POSITION_ADRESS, LEN_GOAL_POSITION);
            ArmInitialiser RobotInitialiser = new ArmInitialiser(BAUDRATE, PORT, BULK_WRITE_MOVE);
            ClawUtils Arm = new ClawUtils(BAUDRATE, PORT, BULK_WRITE_MOVE);
            RobotInitialiser.EnableTorque();
            SERVO_POS = RobotInitialiser.servoPos;
            int dxl_comm_result = COMM_TX_FAIL;
            Console.WriteLine("Arm Init Sucessful!");

            Console.WriteLine("////////////GamePad_Init////////////");
            GamePad gamePad = new GamePad();
            GamePadState state = GamePad.GetState(PlayerIndex.One);
            Console.WriteLine("IsConnected {0} Packet #{1}", state.IsConnected, state.PacketNumber);
            if (state.IsConnected == false) { 
                Console.WriteLine("ERROR: Controller not connected, please ensure it is connected and try again");
                RobotInitialiser.DisableTorque();
                return; 
            }
            Console.WriteLine("GamePad Functioning!");

            ushort LStickX = 0;
            ushort LStickY = 0;
            ushort RStickX = 0;
            ushort RStickY = 0;

            while (true) {
                state = GamePad.GetState(PlayerIndex.One);
                if (state.Buttons.X == ButtonState.Pressed) {
                    break;
                }
                if (state.Buttons.LeftShoulder == ButtonState.Pressed) {
                    Arm.OpenClaw();
                }

                if (state.Buttons.RightShoulder == ButtonState.Pressed) {
                    Arm.CloseClaw();
                }
                if (state.ThumbSticks.Left.X > LStickX || state.ThumbSticks.Left.X < LStickX) {
                    dynamixel.groupSyncWriteAddParam(BULK_WRITE_MOVE, WRIST_SERVOS[2], (uint)(SERVO_POS[7] + (state.ThumbSticks.Left.X * 100)), LEN_GOAL_POSITION);
                    SERVO_POS[7] = (int)(SERVO_POS[7] + (state.ThumbSticks.Left.X * 100));
                }

                if (state.ThumbSticks.Left.Y > LStickY || state.ThumbSticks.Left.Y < LStickY) {
                    dynamixel.groupSyncWriteAddParam(BULK_WRITE_MOVE, WRIST_SERVOS[1], (uint)(SERVO_POS[6] + (state.ThumbSticks.Left.Y * 70)), LEN_GOAL_POSITION);
                    SERVO_POS[6] = (int)(SERVO_POS[6] + (state.ThumbSticks.Left.Y * 70));
                }

                if (state.DPad.Left == ButtonState.Pressed) {
                    dynamixel.groupSyncWriteAddParam(BULK_WRITE_MOVE, WRIST_SERVOS[0], (uint)(SERVO_POS[5] + 70), LEN_GOAL_POSITION);
                    SERVO_POS[5] = (int)(SERVO_POS[5] + 70);
                }
                if (state.DPad.Right == ButtonState.Pressed) {
                    dynamixel.groupSyncWriteAddParam(BULK_WRITE_MOVE, WRIST_SERVOS[0], (uint)(SERVO_POS[5] - 70), LEN_GOAL_POSITION);
                    SERVO_POS[5] = (int)(SERVO_POS[5] - 70);
                }

                if (state.DPad.Down == ButtonState.Pressed) {
                    dynamixel.groupSyncWriteAddParam(BULK_WRITE_MOVE, ELBOW_SERVOS[0], (uint)(SERVO_POS[4] + 40), LEN_GOAL_POSITION);
                    dynamixel.groupSyncWriteAddParam(BULK_WRITE_MOVE, ELBOW_SERVOS[1], (uint)(SERVO_POS[3] + 40), LEN_GOAL_POSITION);
                    SERVO_POS[4] = (int)(SERVO_POS[4] + 40);
                    SERVO_POS[3] = (int)(SERVO_POS[3] + 40);
                }
                if (state.DPad.Up == ButtonState.Pressed) {
                    dynamixel.groupSyncWriteAddParam(BULK_WRITE_MOVE, ELBOW_SERVOS[0], (uint)(SERVO_POS[4] - 40), LEN_GOAL_POSITION);
                    dynamixel.groupSyncWriteAddParam(BULK_WRITE_MOVE, ELBOW_SERVOS[1], (uint)(SERVO_POS[3] - 40), LEN_GOAL_POSITION);
                    SERVO_POS[4] = (int)(SERVO_POS[4] - 40);
                    SERVO_POS[3] = (int)(SERVO_POS[3] - 40);
                } else {
                }

                if (state.ThumbSticks.Right.Y > RStickY || state.ThumbSticks.Right.Y < RStickY) {
                    dynamixel.groupSyncWriteAddParam(BULK_WRITE_MOVE, SHOULDER_SERVOS[2], (uint)(SERVO_POS[2] + (state.ThumbSticks.Right.Y * 70)), LEN_GOAL_POSITION);
                    dynamixel.groupSyncWriteAddParam(BULK_WRITE_MOVE, SHOULDER_SERVOS[1], (uint)(SERVO_POS[1] + (state.ThumbSticks.Right.Y * 70)), LEN_GOAL_POSITION);
                    SERVO_POS[2] = (int)(SERVO_POS[2] + (state.ThumbSticks.Right.Y * 70));
                    SERVO_POS[1] = (int)(SERVO_POS[1] + (state.ThumbSticks.Right.Y * 70));
                }

                if (state.ThumbSticks.Right.X > RStickX || state.ThumbSticks.Right.X < RStickX) {
                    dynamixel.groupSyncWriteAddParam(BULK_WRITE_MOVE, SHOULDER_SERVOS[0], (uint)(SERVO_POS[0] + (state.ThumbSticks.Right.X * 70)), LEN_GOAL_POSITION);
                    SERVO_POS[0] = (int)(SERVO_POS[0] + (state.ThumbSticks.Right.X * 70));
                }
                dynamixel.groupSyncWriteTxPacket(BULK_WRITE_MOVE);
                if ((dxl_comm_result = dynamixel.getLastTxRxResult(PORT_NUM, ClawUtils.PROTOCOL_VERSION)) != COMM_SUCCESS) {
                    dynamixel.printTxRxResult(ClawUtils.PROTOCOL_VERSION, dxl_comm_result);
                }
                dynamixel.groupSyncWriteClearParam(BULK_WRITE_MOVE);
                Thread.Sleep(100);
            }
            Console.WriteLine("Detorquing motors");
            RobotInitialiser.DisableTorque();
            return;
        }
    }
}
