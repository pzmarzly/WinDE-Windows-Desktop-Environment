/*
 * Created by SharpDevelop.
 * User: jatom
 * Date: 04.12.2016
 * Time: 18:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using WinDE.Core.Shell;
namespace WinDE
{
	/// <summary>
	/// For the sake of SharpDevelop, this must be named "Window1.xaml".
	/// So I will use tht name as a startup form for WinDE ;)
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
			Apps ss = new Apps();
			TaskSwitcher s = new TaskSwitcher();
			SpaceReserver.MakeNewDesktopArea(0, 0, 0, 50);
			s.Show();
			this.Hide();
		}
	}
}