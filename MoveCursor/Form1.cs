using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MoveCursor
{
	public partial class Form1 : Form
	{

		// Mouse Hook Variables
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

		private const int MOUSEEVENT_LEFTDOWN = 0x02;
		private const int MOUSEEVENT_LEFTUP = 0x04;
		private const int MOUSEEVENT_MIDDLEDOWN = 0x20;
		private const int MOUSEEVENT_MIDDLEUP = 0x40;
		private const int MOUSEEVENT_RIGHTDOWN = 0x08;
		private const int MOUSEEVENT_RIGHTUP = 0x10;

		// Keybaord Hook Variables
		const int WH_KEYBOARD_LL = 13; // Number of global LowLevel- hook on the keyboard
		const int WM_KEYDOWN = 0x100; // Messages pressing

		private LowLevelKeyboardProc _proc;

		private static IntPtr hhook = IntPtr.Zero;

		private bool locateEnable = false;
		private Timer cursorPointTimer = new Timer();
		private Timer botTimer = new Timer();


		public Form1()
		{
			_proc = hookProc;
			InitializeComponent();
		}

		// Keyboard Hook Methods

		// GLOBAL HOOK
		[DllImport("user32.dll")]
		static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance, uint threadId);

		[DllImport("user32.dll")]
		static extern bool UnhookWindowsHookEx(IntPtr hInstance);

		[DllImport("user32.dll")]
		static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

		[DllImport("kernel32.dll")]
		static extern IntPtr LoadLibrary(string lpFileName);

		private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

		public void SetHook()
		{
			IntPtr hInstance = LoadLibrary("User32");
			hhook = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, hInstance, 0);
		}

		public void UnHook()
		{
			UnhookWindowsHookEx(hhook);
		}

		public IntPtr hookProc(int code, IntPtr wParam, IntPtr lParam)
		{
			if (code >= 0 && wParam == (IntPtr)WM_KEYDOWN)
			{
				int vkCode = Marshal.ReadInt32(lParam);

				if (vkCode.ToString() == "112") //162 is ASCI CTRL
				{
					startLooping();
				}
				else if(vkCode.ToString() == "113")
				{
					toolStripStatusLabel1.Text = String.Format("x:{0}, y:{1}", System.Windows.Forms.Control.MousePosition.X.ToString(), System.Windows.Forms.Control.MousePosition.Y.ToString());
				}
			}
			return CallNextHookEx(hhook, code, (int)wParam, lParam);
		}

		// End of keybaord Hook

		private void button1_Click(object sender, EventArgs e)
		{
			if (button1.Text == "Start")
			{
				botTimer.Interval = Int32.Parse(textBox7.Text);
				botTimer.Tick += startClicker;
				button1.Text = "Stop";
				botTimer.Enabled = true;
			}
			else
			{
				botTimer.Enabled = false;
				button1.Text = "Start";
			}
		}

		private void startLooping()
		{
			if (textBoxIsEmpty())
			{
				MessageBox.Show("Please filled up the coordinate");
				return;
			}
			if (!botTimer.Enabled)
			{
				botTimer.Interval = Int32.Parse(textBox7.Text);
				botTimer.Tick += startClicker;
				button1.Text = "Stop";
				botTimer.Enabled = true;
			}
			else
			{
				botTimer.Enabled = false;
				button1.Text = "Start";
			}
		}

		private bool textBoxIsEmpty()
		{
			if(textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "" || textBox5.Text == "" || textBox6.Text == "" || textBox7.Text == "")
			{
				return true;
			} else
			{
				return false;
			}
		}

		private void MoveCursor()
		{
			MoveCursor(Int32.Parse(textBox1.Text), Int32.Parse(textBox2.Text));
			System.Threading.Thread.Sleep(Int32.Parse(textBox7.Text));
			mouse_event(MOUSEEVENT_LEFTDOWN, 0, 0, 0, 0);
			mouse_event(MOUSEEVENT_LEFTUP, 0, 0, 0, 0);
			System.Threading.Thread.Sleep(Int32.Parse(textBox7.Text));
			MoveCursor(Int32.Parse(textBox3.Text), Int32.Parse(textBox4.Text));
			System.Threading.Thread.Sleep(Int32.Parse(textBox7.Text));
			mouse_event(MOUSEEVENT_LEFTDOWN, 0, 0, 0, 0);
			mouse_event(MOUSEEVENT_LEFTUP, 0, 0, 0, 0);
			System.Threading.Thread.Sleep(Int32.Parse(textBox7.Text));
			MoveCursor(Int32.Parse(textBox5.Text), Int32.Parse(textBox6.Text));
			System.Threading.Thread.Sleep(Int32.Parse(textBox7.Text));
			mouse_event(MOUSEEVENT_LEFTDOWN, 0, 0, 0, 0);
			mouse_event(MOUSEEVENT_LEFTUP, 0, 0, 0, 0);
			//{X=3256,Y=343}
			//{ X = 2274,Y = 709}
		}

		private void MoveCursor(int x, int y)
		{
			this.Cursor = new Cursor(Cursor.Current.Handle);
			Cursor.Position = new Point(x, y);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			locateEnable = !locateEnable;
			if(locateEnable)
			{
				toolStripStatusLabel1.Text = "Position to any one of the button";
				cursorPointTimer.Interval = 5000;
				cursorPointTimer.Tick += new EventHandler(outputMouseCursor);
				cursorPointTimer.Enabled = true;
			}
		}

		private void outputMouseCursor(object sender, EventArgs e) {
			cursorPointTimer.Enabled = false;
			toolStripStatusLabel1.Text = String.Format("x:{0}, y:{1}", System.Windows.Forms.Control.MousePosition.X.ToString(), System.Windows.Forms.Control.MousePosition.Y.ToString());
		}

		private void startClicker(object sender, EventArgs e)
		{
			MoveCursor();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			SetHook();
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			UnHook();
		}
	}
}
