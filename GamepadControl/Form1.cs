using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Microsoft.DirectX.DirectInput;

namespace GamepadControl
{
    public partial class Form1 : Form
    {
        private Joystick gamepad;
        private bool[] gamepad_buttons;
        private bool gamepad_connected = false;

        private bool safety_switch = true;
        private int throttle = 0;
        private int turn = 0;
        private string motor_mode = "O";
        public Form1()
        {
            InitializeComponent();
            gamepad = new Joystick(Handle);
            toolTip1.SetToolTip(textBox5, "FF - Forward Fast - быстрый ход;\nFS - Forward Slow - медленный ход;\nO - остановка/простой;\nRS - Reverse Slow - медленный задний ход;\nRF - Reverse Fast - быстрый задний ход;\nEB - Emergency Braking - экстренное торможение.");
        }
        private void InfoMessage(string message)
        {
            textBox4.Text = message;
        }
        private void ConnectJoystick()
        {
            if(gamepad_connected)
            {
                return;
            }

            pictureBox2.Image = Properties.Resources.yellow; // Indicates that the program is searching for a joystick ~13.05.2022
            InfoMessage("Поиск геймпада...");

            string joysticks;
            joysticks = gamepad.FindJoysticks();

            if(joysticks == null)
            {
                InfoMessage("Геймпад не найден");
                pictureBox2.Image = Properties.Resources.red;
                return;
            }

            if (!gamepad.AcquireJoystick(joysticks))
            {
                throw new ArgumentException("Gamepad could not be acquired.");
            }

            EnableTimer();
            pictureBox2.Image = Properties.Resources.green;
            button1.Enabled = false;
            button2.Enabled = true;
            gamepad_connected = true;
            InfoMessage("Экстренное торможение");
        }
        private void DisconnectJoystick()
        {
            if (!gamepad_connected)
            {
                return;
            }

            pictureBox2.Image = Properties.Resources.yellow;

            gamepad.ReleaseJoystick();
            pictureBox2.Image = Properties.Resources.red;
            button1.Enabled = true;
            button2.Enabled = false;
            gamepad_connected = false;
            gamepad_timer.Enabled = false;
            InfoMessage("Геймпад не подключён");
        }

        private void EnableTimer()
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new ThreadStart(delegate ()
                {
                    gamepad_timer.Enabled = true;
                }));
            }
            else
                gamepad_timer.Enabled = true;
        }
        private void EmergencyBrake()
        {
            InfoMessage("Экстренное торможение");
            motor_mode = "EB";
        }

        private void UpdateJoystickPicture(double vertical_raw, double horizontal_raw)
        {
            vertical_raw = -(gamepad.Yaxis - 32768) / 327;
            vertical_raw = Math.Round(vertical_raw); 
            int new_y = Convert.ToInt16(vertical_raw);             

            horizontal_raw = -(gamepad.Yaxis - 32768) / 327; 
            horizontal_raw = Math.Round(horizontal_raw);              
            int new_x = Convert.ToInt16(horizontal_raw);

            int base_x = 115;
            int base_y = 354;

            new_x = base_x + new_x / 4;
            new_y = base_y + new_y / 4;
            int cur_x = Convert.ToInt16(pictureBox4.Location.X);
            int cur_y = Convert.ToInt16(pictureBox4.Location.Y);

            pictureBox4.Location.Offset(cur_x - new_x, cur_y - new_y);
        }

        private void gamepad_timer_Tick_1(object sender, EventArgs e)
        {
            try
            {
                InfoMessage("");

                gamepad.UpdateStatus();
                gamepad_buttons = gamepad.buttons;
                safety_switch = Convert.ToBoolean(gamepad_buttons[6]);

                if (safety_switch)
                {
                    textBox1.Text = "Нажат";
                    pictureBox3.Visible = true;
                }
                else
                {
                    textBox1.Text = "Отжат";
                    pictureBox3.Visible = false;
                }

                // Acquiring the throttle ~13.05.2022
                double throttle_raw = -(gamepad.Yaxis - 32768) / 327; // Functionally similar to mapping [0; 65536] to [100; -100]
                throttle_raw = Math.Round(throttle_raw);              // Now make it an integer to get a clean percentage...
                throttle = Convert.ToInt16(throttle_raw);             // And transfer it to an integer variable.

                // Acquiring the turn ~13.05.2022
                double turn_raw = -(gamepad.Xaxis - 32768) / 1092;     // Functionally similar to mapping [0; 65536] to [-30; 30]
                turn_raw = Math.Round(turn_raw);                     // Same as the throttle but for degrees.
                turn = Convert.ToInt16(turn_raw);

                if(throttle != 0 || turn != 0)
                {
                    pictureBox4.Visible = true;
                    UpdateJoystickPicture(gamepad.Yaxis, gamepad.Xaxis);
                }
                else
                {
                    pictureBox4.Visible = false;
                }

                // Determine the motor mode from the throttle.
                if(throttle <= 100 && throttle >= 71)
                {
                    motor_mode = "FF";
                }
                else if(throttle <= 70 && throttle >= 10)
                {
                    motor_mode = "FS";
                }
                else if(throttle < 10 && throttle > -10)
                {
                    motor_mode = "O";
                }
                else if (throttle <= -10 && throttle >= -70)
                {
                    motor_mode = "RS";
                }
                if (throttle <= -71 && throttle >= -100)
                {
                    motor_mode = "RF";
                }

                if (!safety_switch && (throttle != 0 || turn != 0))
                {
                    EmergencyBrake();
                }

                textBox2.Text = Convert.ToString(gamepad.Yaxis);
                textBox3.Text = Convert.ToString(gamepad.Xaxis);
                textBox7.Text = Convert.ToString(throttle);
                textBox5.Text = motor_mode;
                textBox6.Text = Convert.ToString(turn);

                try
                {
                    /*
                    SendToArduino(motor_mode);
                    SendToArduino(turn)
                    */
                }
                catch
                {
                    EmergencyBrake();
                    InfoMessage("Нет соединения с Arduino");
                }


            }
            catch
            {
                gamepad_timer.Enabled = false;
                EmergencyBrake();
                DisconnectJoystick();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(gamepad == null)
            {
                gamepad = new Joystick(Handle);
            }
            pictureBox2.Image = Properties.Resources.yellow;
            ConnectJoystick();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = Properties.Resources.yellow;
            DisconnectJoystick();
        }
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
