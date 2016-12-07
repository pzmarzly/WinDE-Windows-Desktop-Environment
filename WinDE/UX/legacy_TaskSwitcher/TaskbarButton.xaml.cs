/*
 * Created by SharpDevelop.
 * User: jatom
 * Date: 12/05/2016
 * Time: 21:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using WinDE.Core.Shell;
using System.Windows.Media.Imaging;
namespace WinDE
{
	/// <summary>
	/// Interaction logic for TaskbarButton.xaml
	/// </summary>
	public partial class TaskbarButton : UserControl
	{

       private readonly shWindow window;

 

       //Contructor with a MerulaShell.windows.window as input

       public TaskbarButton(shWindow window)

       {

          

           InitializeComponent();

 
           try{
           this.window = window;

           window.TitleChanged += WindowTitleChanged; //when the title of the window changes

           SetProperties(); //set the window properties
           }
           catch(Exception ex)
           {
           	MessageBox.Show(ex.ToString());
           }

       }

 

       private delegate void DelegateVoid();

 

       void WindowTitleChanged(object sender, EventArgs e)

       {

           Dispatcher.Invoke(new DelegateVoid(SetTitle)); //invoke beacause merula shell runs in another thread

       }

 

       private void SetTitle()

       {

           lblTitle.Text = window.Title; // sets the title in the textblock

       }

 

       private void SetProperties()

       {
		
       	imgIcon.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(window.ProgramIcon.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(window.ProgramIcon.Width, window.ProgramIcon.Height)); // sets the icon of the window

           lblTitle.Text = window.Title; // sets the title in the textblock

       }
        private bool active;

 

       private void UserControl1_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)

       {

           Background = System.Windows.Media.Brushes.Lime; //set a nice active color

           if(active)
           {           	//when active minimize and maximize
               window.MaximizeMinimize(); //minimize or maximize
           }

           else

           {

               active = true; // set active

               window.SetToForeground(); //set window to foreground
				
           }

           InvokeActivated(new EventArgs());

       }

 

       public void SetNonActive()

       {

           active = false; //set active to false

           Background = System.Windows.Media.Brushes.Transparent; //reset color to white

       }

 

       public event EventHandler Activated; //event to notify the mainwindow

 

       public void InvokeActivated(EventArgs e)

       {

           EventHandler handler = Activated;

           if (handler != null) handler(this, e);

       }
       

   }
}