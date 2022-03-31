using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GamepadControl
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 display = new Form1();
            Application.Run(display);
            /*
            Joystick gamepad = new Joystick();
            string joysticks = null;
            
            do
            {
                joysticks = gamepad.FindJoysticks();
            }
            while (joysticks == null);
            
            if(!gamepad.AcquireJoystick(joysticks))
            {
                throw new ArgumentException("Gamepad could not be acquired.");
            }
            
            while(true)
            {
                gamepad.UpdateStatus();

                if(gamepad.buttons[5])
                {
                    display.textBox1.Text = "1";
                }
                else
                {
                    display.textBox1.Text = "0";
                }

                display.textBox2.Text = Convert.ToString(gamepad.Yaxis);
                display.textBox3.Text = Convert.ToString(gamepad.Xaxis);

            }
            */
        }
    }
}
