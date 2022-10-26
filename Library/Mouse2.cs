using System.Runtime.InteropServices;

namespace Library
{
	public class Mouse2
	{
		[DllImport("user32.dll")]
		private static extern void mouse_event(uint dwFlags, int dx, int dy, int dwData, IntPtr dwExtraInfo);

		[DllImport("user32.dll")]
		private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, IntPtr dwExtraInfo);


		private const int MouseEventRightDown = 0x0008;
		private const int MouseEventRightUp = 0x0010;

		private const int MouseEventLeftDown = 0x0002;
		private const int MouseEventLeftUp = 0x0004;

		private const int MouseEventWheel = 0x0800;

		private const int MouseEventMove = 0x0001;

		public const int MOUSEEVENTF_MOVE = 0x0001; /* mouse move */
		public const int MOUSEEVENTF_LEFTDOWN = 0x0002; /* left button down */
		public const int MOUSEEVENTF_LEFTUP = 0x0004; /* left button up */
		public const int MOUSEEVENTF_RIGHTDOWN = 0x0008; /* right button down */
		public const int MOUSEEVENTF_RIGHTUP = 0x0010; /* right button up */
		public const int MOUSEEVENTF_MIDDLEDOWN = 0x0020; /* middle button down */
		public const int MOUSEEVENTF_MIDDLEUP = 0x0040; /* middle button up */
		public const int MOUSEEVENTF_XDOWN = 0x0080; /* x button down */
		public const int MOUSEEVENTF_XUP = 0x0100; /* x button down */
		public const int MOUSEEVENTF_WHEEL = 0x0800; /* wheel button rolled */
		public const int MOUSEEVENTF_HWHEEL = 0x01000; /* hwheel button rolled */
		public const int MOUSEEVENTF_MOVE_NOCOALESCE = 0x2000; /* do not coalesce mouse moves */
		public const int MOUSEEVENTF_VIRTUALDESK = 0x4000; /* map to entire virtual desktop */
		public const int MOUSEEVENTF_ABSOLUTE = 0x8000; /* absolute move */

		public const int INPUT_MOUSE = 0;
		public const int INPUT_KEYBOARD = 1;
		public const int INPUT_HARDWARE = 2;


		[DllImport("User32.dll", SetLastError = true)]
		public static extern int GetSystemMetrics(int nIndex);

		public const int SM_XVIRTUALSCREEN = 76;
		public const int SM_YVIRTUALSCREEN = 77;
		public const int SM_CXVIRTUALSCREEN = 78;
		public const int SM_CYVIRTUALSCREEN = 79;

		public const int KEYEVENTF_EXTENDEDKEY = 0x0001;
		public const int KEYEVENTF_KEYUP = 0x0002;
		public const int KEYEVENTF_UNICODE = 0x0004;
		public const int KEYEVENTF_SCANCODE = 0x0008;

		[StructLayout(LayoutKind.Sequential)]
		public struct MOUSEINPUT
		{
			public int dx;
			public int dy;
			public int mouseData;
			public int dwFlags;
			public int time;
			public IntPtr dwExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct KEYBDINPUT
		{
			public short wVk;
			public short wScan;
			public int dwFlags;
			public int time;
			public IntPtr dwExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct HARDWAREINPUT
		{
			public int uMsg;
			public short wParamL;
			public short wParamH;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct INPUT
		{
			public int type;
			public INPUTUNION inputUnion;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct INPUTUNION
		{
			[FieldOffset(0)]
			public MOUSEINPUT mi;
			[FieldOffset(0)]
			public KEYBDINPUT ki;
			[FieldOffset(0)]
			public HARDWAREINPUT hi;
		}

		[DllImport("User32.dll", SetLastError = true)]
		public static extern int SendInput(int nInputs, [MarshalAs(UnmanagedType.LPArray)] INPUT[] pInput, int cbSize);


		public static void SendLeftClick(int posX, int posY)
		{
			mouse_event(MouseEventLeftDown, posX, posY, 0, new IntPtr());
			mouse_event(MouseEventLeftUp, posX, posY, 0, new IntPtr());
		}

		public static void Wheel(UInt32 amount)
		{
			uint posX = Convert.ToUInt32(Cursor.Position.X);
			uint posY = Convert.ToUInt32(Cursor.Position.Y);

			mouse_event(MouseEventWheel, posX, posY, amount, new IntPtr());
		}

		public static void Click(int x, int y, int timeDown)
		{
			Cursor.Position = new Point(x, y);
			LeftDown();
			Thread.Sleep(timeDown);
			LeftUp();
		}


		public static void Drag(int dx, int dy, int delay_ms, int partsCount)
		{
			LeftDown();
			Thread.Sleep(delay_ms);
			for (int i = 0; i < partsCount; i++)
			{
				Move(dx / partsCount, dy / partsCount);
				Thread.Sleep(delay_ms);
			}
			LeftUp();
		}

		public static void Move(int dx, int dy)
		{
			mouse_event(MouseEventMove, dx, dy, 0, new IntPtr());
		}

		public static void LeftDown()
		{
			int posX = Cursor.Position.X;
			int posY = Cursor.Position.Y;

			mouse_event(MouseEventLeftDown, posX, posY, 0, new IntPtr());
		}

		public static void LeftUp()
		{
			int posX = Cursor.Position.X;
			int posY = Cursor.Position.Y;

			mouse_event(MouseEventLeftUp, posX, posY, 0, new IntPtr());
		}
	}
}
