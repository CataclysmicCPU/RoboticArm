using dynamixel_sdk;
using XInputDotNetPure;
using System.Windows.Input;
using System.Runtime.InteropServices;

namespace read_write {
    internal class RobotArm {

        public const int BAUDRATE = 1000000;
        public const string PORT = "COM3";
        public static int BULK_WRITE_MOVE = 0;
        public static int PORT_NUM;

        public const int COMM_SUCCESS = 0;
        public const int COMM_TX_FAIL = -1001;
        public const int PROTOCOL_VERSION = 2;

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
            BULK_WRITE_MOVE = dynamixel.groupSyncWrite(PORT_NUM, PROTOCOL_VERSION, GOAL_POSITION_ADRESS, LEN_GOAL_POSITION);

            ArmInitialiser ArmInitialiser = new ArmInitialiser(BAUDRATE, PORT, BULK_WRITE_MOVE);
            ArmInitialiser.EnableTorque();
            dynamixel.write4ByteTxRx(PORT_NUM, 2, RobotArm.CLAW_SERVO, 116, 3000);
            Console.WriteLine("Claw Opened");

            SERVO_POS = ArmInitialiser.servoPos;
            Console.WriteLine("Arm Init Sucessful!");

            Console.WriteLine("////////////GamePad_Init////////////");
            GamePad gamePad = new GamePad();
            GamePadState state = GamePad.GetState(PlayerIndex.One);
            Console.WriteLine("IsConnected {0} Packet #{1}", state.IsConnected, state.PacketNumber);
            if (state.IsConnected == false) {
                Console.WriteLine("ERROR: Controller not connected, please ensure it is connected and try again");
                ArmInitialiser.DisableTorque();
                return;
            }
            Console.WriteLine("GamePad Functioning!");

            while (true) {
                state = GamePad.GetState(PlayerIndex.One);

                if (state.Buttons.A == ButtonState.Pressed) {
                    ArmInitialiser.RestartMotors();
                    break;
                }

                if (state.Buttons.X == ButtonState.Pressed) { break; }

                if (state.ThumbSticks.Left.X != 0) {
                    dynamixel.groupSyncWriteAddParam(BULK_WRITE_MOVE, WRIST_SERVOS[2], (uint)(SERVO_POS[WRIST_SERVOS[2]] + (state.ThumbSticks.Left.X * 100)), LEN_GOAL_POSITION);
                    SERVO_POS[WRIST_SERVOS[2]] = (int)(SERVO_POS[WRIST_SERVOS[2]] + (state.ThumbSticks.Left.X * 100));
                    Console.WriteLine("LStickX: " + state.ThumbSticks.Left.X);
                }

                if (state.ThumbSticks.Left.Y != 0) {
                    dynamixel.groupSyncWriteAddParam(BULK_WRITE_MOVE, WRIST_SERVOS[1], (uint)(SERVO_POS[WRIST_SERVOS[1]] + (state.ThumbSticks.Left.Y * 70)), LEN_GOAL_POSITION);
                    SERVO_POS[WRIST_SERVOS[1]] = (int)(SERVO_POS[WRIST_SERVOS[1]] + (state.ThumbSticks.Left.Y * 70));
                    Console.WriteLine("LStickY: " + state.ThumbSticks.Left.Y);
                }

                if (state.DPad.Left == ButtonState.Pressed) {
                    dynamixel.groupSyncWriteAddParam(BULK_WRITE_MOVE, WRIST_SERVOS[0], (uint)(SERVO_POS[WRIST_SERVOS[0]] + 70), LEN_GOAL_POSITION);
                    SERVO_POS[WRIST_SERVOS[0]] = (int)(SERVO_POS[WRIST_SERVOS[0]] + 70);
                    Console.WriteLine("DPad Left Pressed");
                }

                if (state.DPad.Right == ButtonState.Pressed) {
                    dynamixel.groupSyncWriteAddParam(BULK_WRITE_MOVE, WRIST_SERVOS[0], (uint)(SERVO_POS[WRIST_SERVOS[0]] - 70), LEN_GOAL_POSITION);
                    SERVO_POS[WRIST_SERVOS[0]] = (int)(SERVO_POS[WRIST_SERVOS[0]] - 70);
                    Console.WriteLine("DPad Right Pressed");
                }

                if (state.DPad.Down == ButtonState.Pressed) {
                    dynamixel.groupSyncWriteAddParam(BULK_WRITE_MOVE, ELBOW_SERVOS[0], (uint)(SERVO_POS[ELBOW_SERVOS[0]] + 15), LEN_GOAL_POSITION);
                    dynamixel.groupSyncWriteAddParam(BULK_WRITE_MOVE, ELBOW_SERVOS[1], (uint)(SERVO_POS[ELBOW_SERVOS[1]] - 15), LEN_GOAL_POSITION);
                    SERVO_POS[ELBOW_SERVOS[0]] = (int)(SERVO_POS[ELBOW_SERVOS[0]] + 15);
                    SERVO_POS[ELBOW_SERVOS[1]] = (int)(SERVO_POS[ELBOW_SERVOS[1]] - 15);
                    Console.WriteLine("DPad Down Pressed");
                }

                if (state.DPad.Up == ButtonState.Pressed) {
                    dynamixel.groupSyncWriteAddParam(BULK_WRITE_MOVE, ELBOW_SERVOS[0], (uint)(SERVO_POS[ELBOW_SERVOS[0]] - 15), LEN_GOAL_POSITION);
                    dynamixel.groupSyncWriteAddParam(BULK_WRITE_MOVE, ELBOW_SERVOS[1], (uint)(SERVO_POS[ELBOW_SERVOS[1]] + 15), LEN_GOAL_POSITION);
                    SERVO_POS[ELBOW_SERVOS[0]] = (int)(SERVO_POS[ELBOW_SERVOS[0]] - 15);
                    SERVO_POS[ELBOW_SERVOS[1]] = (int)(SERVO_POS[ELBOW_SERVOS[1]] + 15);
                    Console.WriteLine("DPad Up  Pressed");
                }

                if (state.ThumbSticks.Right.Y != 0) {
                    dynamixel.groupSyncWriteAddParam(BULK_WRITE_MOVE, SHOULDER_SERVOS[2], (uint)(SERVO_POS[2] + (state.ThumbSticks.Right.Y * 25)), LEN_GOAL_POSITION);
                    dynamixel.groupSyncWriteAddParam(BULK_WRITE_MOVE, SHOULDER_SERVOS[1], (uint)(SERVO_POS[1] + (state.ThumbSticks.Right.Y * 25)), LEN_GOAL_POSITION);
                    SERVO_POS[2] = (int)(SERVO_POS[2] + (state.ThumbSticks.Right.Y * 25));
                    SERVO_POS[1] = (int)(SERVO_POS[1] + (state.ThumbSticks.Right.Y * 25));
                    Console.WriteLine("RStickY: " + state.ThumbSticks.Right.Y);
                }

                if (state.ThumbSticks.Right.X != 0) {
                    dynamixel.groupSyncWriteAddParam(BULK_WRITE_MOVE, SHOULDER_SERVOS[0], (uint)(SERVO_POS[SHOULDER_SERVOS[0]] + (state.ThumbSticks.Right.X * 25)), LEN_GOAL_POSITION);
                    SERVO_POS[SHOULDER_SERVOS[0]] = (int)(SERVO_POS[SHOULDER_SERVOS[0]] + (state.ThumbSticks.Right.X * 40));
                    Console.WriteLine("RStickX: " + state.ThumbSticks.Right.X);
                }

                if (state.Buttons.RightShoulder == ButtonState.Pressed && SERVO_POS[CLAW_SERVO] <= 3000) {
                    dynamixel.groupSyncWriteAddParam(BULK_WRITE_MOVE, CLAW_SERVO, (uint)(SERVO_POS[CLAW_SERVO] + 10), LEN_GOAL_POSITION);
                    SERVO_POS[CLAW_SERVO] = (int)(SERVO_POS[CLAW_SERVO] + 20);
                    Console.WriteLine("Right Shoulder Pressed");
                }

                if (state.Buttons.LeftShoulder == ButtonState.Pressed && SERVO_POS[CLAW_SERVO] >= 1810) {
                    dynamixel.groupSyncWriteAddParam(BULK_WRITE_MOVE, CLAW_SERVO, (uint)(SERVO_POS[CLAW_SERVO] - 10), LEN_GOAL_POSITION);
                    SERVO_POS[CLAW_SERVO] = (int)(SERVO_POS[CLAW_SERVO] - 20);
                    Console.WriteLine("Left Shoulder Pressed");
                }

                dynamixel.groupSyncWriteTxPacket(BULK_WRITE_MOVE);
                if (dynamixel.getLastTxRxResult(PORT_NUM, PROTOCOL_VERSION) == COMM_TX_FAIL) {
                    Console.WriteLine("Failed to process request!");
                    return;
                }

                dynamixel.groupSyncWriteClearParam(BULK_WRITE_MOVE);
                Thread.Sleep(20);
            }
            Console.WriteLine("Detorquing motors");
            ArmInitialiser.DisableTorque();
            return;
        }
    }
}
