/*
 * Created by SharpDevelop.
 * User: jatom
 * Date: 12/05/2016
 * Time: 19:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WinDE.Core.Shell;
namespace WinDE
{
	/// <summary>
	/// Interaction logic for TaskSwitcher.xaml
	/// </summary>
	public partial class TaskSwitcher : Window
	{
		private ManageWindows windowManager;
		public bool AppsLaunched;
		public TaskSwitcher()
		{
			InitializeComponent();
			try{
			windowManager = new ManageWindows(); //create a new windowmanager / only one needed
			Closed += MainWindow_Closed; // on close event
			windowManager.WindowListChanged += WindowManagerWindowListChanged; //when the list of windows is changed
           	LoadWindows(); //load the windows
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			this.Loaded+= TaskSwitcher_Loaded;
		}

		void TaskSwitcher_Loaded(object sender, RoutedEventArgs e)
		{
			this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
			this.Top = System.Windows.SystemParameters.PrimaryScreenHeight - this.Height;
		}
		
 		private delegate void DelegateVoid();

       void WindowManagerWindowListChanged(object sender, EventArgs e)
       {
           Dispatcher.Invoke(new DelegateVoid(LoadWindows));
       }

	   void ButtonActivated(object sender, EventArgs e)
       {
           var senderButton = (TaskbarButton) sender;
           var otherButtons = TaskPanel.Children.OfType<TaskbarButton>().Where(b => b != senderButton);
           foreach (var otherButton in otherButtons)
           {
               otherButton.SetNonActive();

           }
       }

       void MainWindow_Closed(object sender, EventArgs e)
       {
          Environment.Exit(0); 
       }

 

 

       private void LoadWindows()

       {
       	try{
           ClearTasks();//delete old tasks

           var windows = windowManager.GetWindows();//windowManager.GetWindows() returns all the active windows

           foreach (var window in windows)

           { //foreach window add a taskbar button
					
                var button = new TaskbarButton(window);

               button.Activated += ButtonActivated; //add a event to the taskbarbutton

               TaskPanel.Children.Add(button);

           }
       	}
       	catch(Exception ex)
       	{
       		MessageBox.Show(ex.ToString());
       	}
       }

 

       private void ClearTasks() //delete old tasks

       {

           TaskPanel.Children.Clear();

       }
		void Window_StateChanged(object sender, EventArgs e)
		{
			this.WindowState = WindowState.Normal;
		}
		void LaunchApps(object sender, RoutedEventArgs e)
		{
			AppsLaunched =! AppsLaunched;
			if(!AppsLaunched)
				Apps.staticapp.Show();
			else{
				
				Apps.staticapp.Hide();
			}
		}
	}

}