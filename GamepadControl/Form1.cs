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

namespace GamepadControl
{
    public partial class Form1 : Form
    {
        private Joystick gamepad;
        private bool[] gamepad_buttons;
        public Form1()
        {
            InitializeComponent();
            gamepad = new Joystick(this.Handle);
            ConnectJoystick(gamepad);
        }
        private void ConnectJoystick(Joystick joystick)
        {
            string joysticks;
            do
            {
                joysticks = joystick.FindJoysticks();
            }
            while (joysticks == null);

            if (!joystick.AcquireJoystick(joysticks))
            {
                throw new ArgumentException("Gamepad could not be acquired.");
            }

            EnableTimer();
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

        private void gamepad_timer_Tick_1(object sender, EventArgs e)
        {
            try
            {
                gamepad.UpdateStatus();
                gamepad_buttons = gamepad.buttons;

                if (gamepad_buttons[5])
                {
                    textBox1.Text = "1";
                }
                else
                {
                    textBox1.Text = "0";
                }

                textBox2.Text = Convert.ToString(gamepad.Yaxis);
                textBox3.Text = Convert.ToString(gamepad.Xaxis);
            }
            catch
            {
                gamepad_timer.Enabled = false;
                ConnectJoystick(gamepad);
            }
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
    }
}
