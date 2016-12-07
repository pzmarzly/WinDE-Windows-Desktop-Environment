/*
 * Created by SharpDevelop.
 * User: jatom
 * Date: 12/06/2016
 * Time: 21:17
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WinDE.Core.Shell
{
	/// <summary>
	/// Description of SpaceReserver.
	/// </summary>
	public class SpaceReserver
	{
		public enum SPI
		{
			SPI_SETWORKAREA = 47,
			SPI_GETWORKAREA
		}

		public struct RECT
		{
			public int left;

			public int top;

			public int right;

			public int bottom;
		}

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref SpaceReserver.RECT pvParam, uint fWinIni);
		/// <summary>
		/// Set's offset for Shell's UI Elements
		/// </summary>
		/// <param name="offsetLeft"></param>
		/// <param name="offsetTop"></param>
		/// <param name="offsetRight"></param>
		/// <param name="offsetBottom"></param>
		public static void MakeNewDesktopArea(int offsetLeft, int offsetTop, int offsetRight, int offsetBottom)
		{
			SpaceReserver.RECT rECT;
			rECT.left = SystemInformation.VirtualScreen.Left + offsetLeft;
			rECT.top = SystemInformation.VirtualScreen.Top + offsetTop;
			rECT.right = SystemInformation.VirtualScreen.Right - offsetRight;
			rECT.bottom = SystemInformation.VirtualScreen.Bottom - offsetBottom;
			SpaceReserver.SystemParametersInfo(47u, 0u, ref rECT, 0u);
		}
	}
}
