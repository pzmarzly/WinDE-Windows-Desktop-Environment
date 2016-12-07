/*
Classes from Merula Shell (R.I.P :[ )
Debugged and refreshed to work with new versions of windows.
All of MerulaShell Libs will be deleted after migration
 */
using System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace WinDE.Core.Shell
{
	/// <summary>
	/// Class Made to manage shell's windows static Shell object is important, because we get all windows by it. (Singleton ftw).
	/// </summary>
	public class ManageWindows
	{
		private Shell shell;

		public event EventHandler WindowListChanged;

		public ManageWindows()
		{
			this.shell = Shell.GetInstance();
			this.shell.WindowListChanged += new EventHandler(this.ShellWindowListChanged);
		}

		public void InvokeWindowListChanged(EventArgs e)
		{
			EventHandler windowListChanged = this.WindowListChanged;
			if (windowListChanged != null) {
				windowListChanged(this, e);
			}
		}

		private void ShellWindowListChanged(object sender, EventArgs e)
		{
			this.InvokeWindowListChanged(new EventArgs());
		}

		public void MinimizeWindow()
		{
		}

		public List<shWindow> GetWindows()
		{
			GetWindows getWindows = new GetWindows();
			return getWindows.GetActiveWindows();
		}

		public void ShowWindow()
		{
		}

		public void CloseWindow()
		{
		}

		public void AddException(IntPtr handle)
		{
			this.shell.AddException(handle);
		}
	}
	/// <summary>
	/// Shell Class starts after program start. it also handles exceptions
	/// </summary>
	public class Shell
	{
		private static Shell shell;

		private readonly WindowManager windowCenter;

		public event EventHandler WindowListChanged;

		public void InvokeWindowListChanged(EventArgs e)
		{
			EventHandler windowListChanged = this.WindowListChanged;
			if (windowListChanged != null) {
				windowListChanged(this, e);
			}
		}

		private Shell()
		{
			this.windowCenter = new WindowManager();
			this.windowCenter.WindowListChanged += new EventHandler(this.WindowCenterWindowListChanged);
			ShellReady.SetShellReadyEvent();
		}

		private void WindowCenterWindowListChanged(object sender, EventArgs e)
		{
			this.InvokeWindowListChanged(new EventArgs());
		}

		public static Shell GetInstance()
		{
			Shell arg_14_0;
			if ((arg_14_0 = Shell.shell) == null) {
				arg_14_0 = (Shell.shell = new Shell());
			}
			return arg_14_0;
		}

		public List<shWindow> GetActiveWindows()
		{
			return this.windowCenter.Windows;
		}

		public void AddException(IntPtr handle)
		{
			this.windowCenter.AddException(handle);
		}

		public static void HideWindow(string name)
		{
			IntPtr intPtr = ShellCommands.FindWindow(name, null);
			if ((int)intPtr != 0) {
				ShellCommands.ShowWindow(intPtr, 0);
			}
		}
	}
	/// <summary>
	/// Window Manager is revived straight from Merula Shell
	/// </summary>
	internal class WindowManager
	{
		private delegate bool EnumWindowsCallback(IntPtr hwnd, int lParam);

		private int windowCount;

		private readonly List<IntPtr> exceptionList;

		private bool stopped;

		private static readonly int GWL_STYLE = -16;

		private static readonly ulong WS_VISIBLE = 268435456uL;

		private static readonly ulong WS_BORDER = 8388608uL;

		private static readonly ulong TARGETWINDOW = WindowManager.WS_BORDER | WindowManager.WS_VISIBLE;

		private static readonly int WS_EX_TOOLWINDOW = 128;

		private static readonly int WS_EX_APPWINDOW = 262144;

		private static readonly int GW_OWNER = 4;

		private static readonly int GWL_EXSTYLE = -20;

		public event EventHandler WindowListChanged;

		internal List<shWindow> Windows {
			get;
			set;
		}

		private void InvokeWindowListChanged(EventArgs e)
		{
			EventHandler windowListChanged = this.WindowListChanged;
			if (windowListChanged != null) {
				windowListChanged(this, e);
			}
		}

		public WindowManager()
		{
			this.exceptionList = new List<IntPtr>();
			this.LoadWindows();
			this.WindowListChanged += new EventHandler(this.WindowCenterWindowListChanged);
			Thread thread = new Thread(new ThreadStart(this.CheckWindows)) {
				Priority = ThreadPriority.Lowest
			};
			thread.Start();
		}

		private void WindowCenterWindowListChanged(object sender, EventArgs e)
		{
			this.LoadWindows();
		}

		private void LoadWindows()
		{
			this.DestroyWindows();
			this.Windows = new List<shWindow>();
			WindowManager.EnumWindows(new WindowManager.EnumWindowsCallback(this.Callback), 0);
		}

		private void DestroyWindows()
		{
			if (this.Windows == null) {
				return;
			}
			foreach (shWindow current in this.Windows) {
				current.Destroy();
			}
		}

		private void CheckWindows()
		{
			while (!this.stopped) {
				this.windowCount = 0;
				WindowManager.EnumWindows(new WindowManager.EnumWindowsCallback(this.CountCallback), 0);
				if (this.windowCount != this.Windows.Count) {
					this.InvokeWindowListChanged(new EventArgs());
				}
				Thread.Sleep(40);
			}
		}

		public void AddException(IntPtr handle)
		{
			this.exceptionList.Add(handle);
		}

		~WindowManager()
		{
			this.stopped = true;
		}

		private bool Callback(IntPtr hwnd, int lParam)
		{
			if (this.IsTaskBarWindow(hwnd)) {
			shWindow item = new shWindow(hwnd);
				this.Windows.Add(item);
			}
			return true;
		}

		private bool IsTaskBarWindow(IntPtr hwnd)
		{
			if (!this.exceptionList.Contains(hwnd) && (WindowManager.GetWindowLongA(hwnd, WindowManager.GWL_STYLE) & WindowManager.TARGETWINDOW) == WindowManager.TARGETWINDOW && WindowManager.IsWindowVisible(hwnd) && WindowManager.GetParent(hwnd) == IntPtr.Zero) {
				bool flag = WindowManager.GetWindow(hwnd, WindowManager.GW_OWNER) == IntPtr.Zero;
				int windowLong = WindowManager.GetWindowLong(hwnd, WindowManager.GWL_EXSTYLE);
				if (!flag) {
					return false;
				}
				if ((windowLong & WindowManager.WS_EX_TOOLWINDOW) == 0) {
					return true;
				}
			}
			return false;
		}

		private bool CountCallback(IntPtr hwnd, int lParam)
		{
			if (this.IsTaskBarWindow(hwnd)) {
				this.windowCount++;
			}
			return true;
		}

		[DllImport("user32.dll")]
		private static extern int EnumWindows(WindowManager.EnumWindowsCallback lpEnumFunc, int lParam);

		[DllImport("user32.dll")]
		private static extern ulong GetWindowLongA(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		private static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern IntPtr GetParent(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern IntPtr GetWindow(IntPtr hWnd, int wFlag);

		[DllImport("user32.dll")]
		public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
	}
	/// <summary>
	/// Class that describes Window
	/// Windows's api for loading icons added, also reinvented for use with WPF Image UIcomponent better (Convertable through bitmap sources conversion)
	/// </summary>
	public class shWindow
	{
		private bool stopped;

		private bool isMinimized;

		private IntPtr HWND_TOPMOST = (IntPtr)(-1);

		private int SWP_NOSIZE = 1;

		public event EventHandler TitleChanged;

		public IntPtr Handler {
			get;
			private set;
		}

		public Thumbnail Thumbnail {
			get;
			private set;
		}

		public string Title {
			get;
			private set;
		}

		public System.Drawing.Bitmap ProgramIcon {
			get;
			private set;
		}

		public shWindow(IntPtr handler)
		{
			this.Handler = handler;
			this.Title = this.GetTitle();
			this.SetIcon();
			this.Thumbnail = new Thumbnail(handler);
			Thread thread = new Thread(new ThreadStart(this.CheckTitle));
			thread.Start();
		}

		public void InvokeTitleChanged(EventArgs e)
		{
			EventHandler titleChanged = this.TitleChanged;
			if (titleChanged != null) {
				titleChanged(this, e);
			}
		}

		private void CheckTitle()
		{
			this.isMinimized = shWindow.IsIconic(this.Handler);
			while (!this.stopped) {
				if (!this.Title.Equals(this.GetTitle())) {
					this.Title = this.GetTitle();
					this.InvokeTitleChanged(new EventArgs());
				}
				if (shWindow.IsIconic(this.Handler) != this.isMinimized) {
					if (shWindow.IsIconic(this.Handler)) {
						this.HideWindow();
					}
					this.isMinimized = shWindow.IsIconic(this.Handler);
				}
				Thread.Sleep(40);
			}
		}

		private void SetIcon()
		{
			try{
			int processId = 0;
			shWindow.GetWindowThreadProcessId(this.Handler, out processId);
			if(processId == 0 || processId == 4)
			{
			
			}
			else
			{
			ProgramIcon = GetSmallWindowIcon(Handler);
			}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		private string GetTitle()
		{
			StringBuilder stringBuilder = new StringBuilder(200);
			shWindow.GetWindowText(this.Handler, stringBuilder, stringBuilder.Capacity);
			//MessageBox.Show(stringBuilder.ToString());
			return stringBuilder.ToString();
		}

/// <summary>
/// 64 bit version maybe loses significant 64-bit specific information
/// </summary>
		static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
{
    if (IntPtr.Size == 4)
        return new IntPtr((long)GetClassLong32(hWnd, nIndex));
    else
        return GetClassLong64(hWnd, nIndex);
}


		const uint WM_GETICON = 0x007f;
		static IntPtr ICON_SMALL2 = new IntPtr(2);
		static IntPtr IDI_APPLICATION = new IntPtr(0x7F00);
		const int GCL_HICON = -14;

		public static System.Drawing.Bitmap GetSmallWindowIcon(IntPtr hWnd)
		{
    try
    {
        IntPtr hIcon = default(IntPtr);

        hIcon = SendMessage(hWnd, WM_GETICON, shWindow.ICON_SMALL2, IntPtr.Zero);

        if (hIcon == IntPtr.Zero)
            hIcon = GetClassLongPtr(hWnd, GCL_HICON);

        if (hIcon == IntPtr.Zero)
            hIcon = LoadIcon(IntPtr.Zero, (IntPtr)0x7F00/*IDI_APPLICATION*/);

        if (hIcon != IntPtr.Zero)
            return new Bitmap(System.Drawing.Icon.FromHandle(hIcon).ToBitmap(), 16, 16);
        else
            return null;
    }
    catch (Exception)
    {
        return null;
    }
}
		public void Destroy()
		{
			this.stopped = true;
			this.ProgramIcon = null;
			this.Thumbnail.Unregister();
			this.Thumbnail = null;
			this.Title = null;
		}

		public void MaximizeMinimize()
		{
			if (!shWindow.IsIconic(this.Handler)) {
				shWindow.CloseWindow(this.Handler);
				return;
			}
			shWindow.ShowWindow(this.Handler, 4);
			shWindow.SetForegroundWindow(this.Handler);
		}

		public void HideWindow()
		{
			shWindow.MoveWindow(this.Handler, 0, -50, 0, 0, false);
		}

		public void SetToForeground()
		{
			shWindow.SetForegroundWindow(this.Handler);
			if (shWindow.IsIconic(this.Handler)) {
				this.MaximizeMinimize();
			}
		}

		public void SetTopMost()
		{
			shWindow.SetWindowPos(this.Handler, this.HWND_TOPMOST, 0, 0, 0, 0, this.SWP_NOSIZE);
		}

		public void Close()
		{
			shWindow.ShowWindow(this.Handler, 0);
		}

		[DllImport("user32.dll")]
		private static extern void GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll")]
		private static extern bool IsIconic(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		private static extern bool CloseWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

		[DllImport("user32.dll")]
		private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int processId);

		[DllImport("user32.dll")]
		private static extern bool EndTask(IntPtr hWnd, bool fShutDown, bool fForce);

		[DllImport("user32.dll")]
		public static extern int SetWindowPos(IntPtr hwnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);
		
		[DllImport("user32.dll")]
		static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconName);

		[DllImport("user32.dll", EntryPoint = "GetClassLong")]
		static extern uint GetClassLong32(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
		static extern IntPtr GetClassLong64(IntPtr hWnd, int nIndex);

	}
	public class Thumbnail
	{
		internal struct PSIZE
		{
			public int x;

			public int y;
		}

		internal struct DWM_THUMBNAIL_PROPERTIES
		{
			public int dwFlags;

			public Thumbnail.Rect rcDestination;

			public Thumbnail.Rect rcSource;

			public byte opacity;

			public bool fVisible;

			public bool fSourceClientAreaOnly;
		}

		public struct Rect
		{
			public int Left;

			public int Top;

			public int Right;

			public int Bottom;

			internal Rect(int left, int top, int right, int bottom)
			{
				this.Left = left;
				this.Top = top;
				this.Right = right;
				this.Bottom = bottom;
			}
		}

		private readonly IntPtr windowHandler;

		private IntPtr thumb = IntPtr.Zero;

		private static readonly int DWM_TNP_VISIBLE = 8;

		private static readonly int DWM_TNP_OPACITY = 4;

		private static readonly int DWM_TNP_RECTDESTINATION = 1;

		private static readonly int DWM_TNP_SOURCECLIENTAREAONLY = 16;

		public IntPtr Thumb {
			get {
				return this.thumb;
			}
		}

		public Thumbnail(IntPtr windowHandler)
		{
			this.windowHandler = windowHandler;
		}

		public System.Windows.Point GetThumb(IntPtr userHandle, Thumbnail.Rect area, int widthArea, int heightArea)
		{
			if (this.thumb == IntPtr.Zero) {
				Thumbnail.DwmRegisterThumbnail(userHandle, this.windowHandler, out this.thumb);
			}
			return this.UpdateThumb(area, widthArea, heightArea);
		}

		public void Unregister()
		{
			if (this.thumb == IntPtr.Zero) {
				return;
			}
			Thumbnail.DwmUnregisterThumbnail(this.thumb);
			this.thumb = IntPtr.Zero;
		}

		private System.Windows.Point UpdateThumb(Thumbnail.Rect area, int width, int height)
		{
			if (this.thumb != IntPtr.Zero) {
				Thumbnail.PSIZE pSIZE;
				Thumbnail.DwmQueryThumbnailSourceSize(this.thumb, out pSIZE);
				Thumbnail.DWM_THUMBNAIL_PROPERTIES dWM_THUMBNAIL_PROPERTIES = default(Thumbnail.DWM_THUMBNAIL_PROPERTIES);
				dWM_THUMBNAIL_PROPERTIES.dwFlags = (Thumbnail.DWM_TNP_VISIBLE | Thumbnail.DWM_TNP_RECTDESTINATION | Thumbnail.DWM_TNP_OPACITY | Thumbnail.DWM_TNP_SOURCECLIENTAREAONLY);
				dWM_THUMBNAIL_PROPERTIES.fVisible = true;
				dWM_THUMBNAIL_PROPERTIES.fSourceClientAreaOnly = true;
				dWM_THUMBNAIL_PROPERTIES.opacity = 255;
				dWM_THUMBNAIL_PROPERTIES.rcDestination.Bottom = height;
				dWM_THUMBNAIL_PROPERTIES.rcDestination = area;
				double num = (double)height / (double)pSIZE.y;
				int num2 = (int)((double)pSIZE.x * num);
				if (num2 < width) {
					dWM_THUMBNAIL_PROPERTIES.rcDestination.Left = dWM_THUMBNAIL_PROPERTIES.rcDestination.Left + (width / 2 - num2 / 2);
					dWM_THUMBNAIL_PROPERTIES.rcDestination.Right = dWM_THUMBNAIL_PROPERTIES.rcDestination.Right + (width / 2 - num2 / 2);
				}
				num = (double)width / (double)pSIZE.x;
				int num3 = (int)((double)pSIZE.y * num);
				if (num3 < height) {
					dWM_THUMBNAIL_PROPERTIES.rcDestination.Top = dWM_THUMBNAIL_PROPERTIES.rcDestination.Top + (height / 2 - num3 / 2);
					dWM_THUMBNAIL_PROPERTIES.rcDestination.Bottom = dWM_THUMBNAIL_PROPERTIES.rcDestination.Bottom + (height / 2 - num3 / 2);
				}
				Thumbnail.DwmUpdateThumbnailProperties(this.thumb, ref dWM_THUMBNAIL_PROPERTIES);
				return new System.Windows.Point((double)num2, (double)num3);
			}
			return new System.Windows.Point(0.0, 0.0);
		}

		~Thumbnail()
		{
			if (this.thumb != IntPtr.Zero) {
				Thumbnail.DwmUnregisterThumbnail(this.thumb);
			}
		}

		[DllImport("dwmapi.dll")]
		private static extern int DwmRegisterThumbnail(IntPtr dest, IntPtr src, out IntPtr thumb);

		[DllImport("dwmapi.dll")]
		private static extern int DwmUnregisterThumbnail(IntPtr thumb);

		[DllImport("dwmapi.dll")]
		private static extern int DwmQueryThumbnailSourceSize(IntPtr thumb, out Thumbnail.PSIZE size);

		[DllImport("dwmapi.dll")]
		private static extern int DwmUpdateThumbnailProperties(IntPtr hThumb, ref Thumbnail.DWM_THUMBNAIL_PROPERTIES props);
	}
	
	internal class GetWindows
	{
		private Shell shell;

		public GetWindows()
		{
			this.shell = Shell.GetInstance();
		}

		public List<shWindow> GetActiveWindows()
		{
			return this.shell.GetActiveWindows();
		}
	}
	internal class ShellReady
	{
		public enum EventAccessRights
		{
			EVENT_ALL_ACCESS = 2031619,
			EVENT_MODIFY_STATE = 2
		}

		private const uint EVENT_MODIFY_STATE = 2u;

		[DllImport("kernel32.dll")]
		public static extern IntPtr OpenEvent(uint dwDesiredAcess, bool bInheritHandle, string lpName);

		[DllImport("kernel32.dll")]
		public static extern bool SetEvent(IntPtr hEevent);

		[DllImport("kernel32.dll")]
		public static extern bool CloseHandle(IntPtr hObject);

		public static bool SetShellReadyEvent()
		{
			IntPtr intPtr = (IntPtr)0;
			intPtr = ShellReady.OpenEvent(2u, false, "ShellDesktopSwitchEvent");
			if (intPtr == IntPtr.Zero) {
				return false;
			}
			ShellReady.SetEvent(intPtr);
			ShellReady.CloseHandle(intPtr);
			return true;
		}
	}
	internal class ShellCommands
	{
		public enum ShowStyle
		{
			Hide,
			ShowNormal,
			ShowMinimized,
			ShowMaximized,
			Maximize = 3,
			ShowNormalNoActivate,
			Show,
			Minimize,
			ShowMinNoActivate,
			ShowNoActivate,
			Restore,
			ShowDefault,
			ForceMinimized
		}

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll")]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
	}
	public static class PowerOptions
	{
		public static void Shutdown()
		{
			Process.Start(new ProcessStartInfo("shutdown.exe", "-s -t 0") {
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden
			});
		}

		public static void Restart()
		{
			Process.Start(new ProcessStartInfo("shutdown.exe", "-r -t 0") {
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden
			});
		}

		public static void Hibernate()
		{
			Process.Start(new ProcessStartInfo("shutdown.exe", "-h") {
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden
			});
		}
	}
}
