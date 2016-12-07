/*
 * Created by SharpDevelop.
 * User: jatom
 * Date: 04.12.2016
 * Time: 18:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using WinDE.Core.Shell;
namespace WinDE
{
	/// <summary>
	/// Interaction logic for Apps.xaml
	/// Also, Load Apps and icons.
	/// </summary>
	public partial class Apps : Window
	{
		public static Apps staticapp;
		public static List<xApp> AppsList = new List<xApp>();
		public Apps()
		{

			InitializeComponent();
			GetItems();
			this.Loaded += Apps_Loaded;
			staticapp = this;
			this.StateChanged += Apps_StateChanged;
		}

		void Apps_StateChanged(object sender, EventArgs e)
		{
Rectangle bounds = Screen.GetBounds(System.Drawing.Point.Empty);
using(Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
{
    using(Graphics g = Graphics.FromImage(bitmap))
    {
         g.CopyFromScreen(System.Drawing.Point.Empty, System.Drawing.Point.Empty, bounds.Size);
    }
    ImageSource s = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
      bitmap.GetHbitmap(),
      IntPtr.Zero,
      Int32Rect.Empty,
      BitmapSizeOptions.FromEmptyOptions());
    bg.Background = new ImageBrush(s);
		}
		}

		void Show_Activated(object sender, RoutedEventArgs e)
		{
			Rectangle bounds = Screen.GetBounds(System.Drawing.Point.Empty);
using(Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
{
    using(Graphics g = Graphics.FromImage(bitmap))
    {
         g.CopyFromScreen(System.Drawing.Point.Empty, System.Drawing.Point.Empty, bounds.Size);
    }
    ImageSource s = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
      bitmap.GetHbitmap(),
      IntPtr.Zero,
      Int32Rect.Empty,
      BitmapSizeOptions.FromEmptyOptions());
    bg.Background = new ImageBrush(s);
		}
		}
void Apps_Loaded(object sender, RoutedEventArgs e)
		{
			PlaceApps();
			Rectangle bounds = Screen.GetBounds(System.Drawing.Point.Empty);
using(Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
{
    using(Graphics g = Graphics.FromImage(bitmap))
    {
         g.CopyFromScreen(System.Drawing.Point.Empty, System.Drawing.Point.Empty, bounds.Size);
    }
    ImageSource s = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
      bitmap.GetHbitmap(),
      IntPtr.Zero,
      Int32Rect.Empty,
      BitmapSizeOptions.FromEmptyOptions());
    bg.Background = new ImageBrush(s);
}
			
		}
		public void GetItems()
		{	
    		try
    		{
    			foreach (string d in Directory.GetDirectories(@"C:\Users\"+Environment.UserName+@"\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\"))
        		{
            		foreach (string f in Directory.GetFiles(d))
            		{
            			if(!f.EndsWith(".ini"))
            			{
            				AppsList.Add(new xApp(ExtractIcon.GetName(f), System.Drawing.Icon.ExtractAssociatedIcon(f).ToBitmap(), f));
                		
                		
                		
            			}
                	}
            	}
    			foreach (string d in Directory.GetDirectories(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs"))
        		{
            		foreach (string f in Directory.GetFiles(d))
            		{
            			if(!f.EndsWith(".ini"))
            			{
            				if(!AppsList.Any(x => x.name == (ExtractIcon.GetName(f))))
            				{
            				AppsList.Add(new xApp(ExtractIcon.GetName(f), System.Drawing.Icon.ExtractAssociatedIcon(f).ToBitmap(), f));
            				}
            			}
            			
            			
                	}
            	}
    		}
    		catch (Exception ex)
    		{
    			System.Windows.MessageBox.Show(ex.Message);
    		}
		}
		void PlaceApps()
		{
			AppsGrid.Items.Clear();
			foreach(xApp x in AppsList)
				AppsGrid.Items.Add(new WinDE.Icon(x.name, x.icon, x.Path));

		}
		void updatelist(object sender, TextChangedEventArgs e)
		{
			if(SearchBox.Text != " " || SearchBox.Text != "")
			{
			AppsGrid.Items.Clear();
			List<xApp> CacheList = new List<xApp>();
			CacheList = Apps.AppsList.Where(x => x.name.ToLower().Contains(SearchBox.Text.ToLower())).ToList<xApp>();
			
			foreach(xApp x in CacheList)
				
				AppsGrid.Items.Add(new WinDE.Icon(x.name, x.icon, x.Path));
			}
			else{
				
				PlaceApps();
			}
			
		}
	}
	public class xApp
	{
		public string name;
		public Bitmap icon;
		public string Path;
		public xApp(string name, Bitmap icon, string Path)
		{
			this.name = name;
			this.icon = icon;
			this.Path = Path;
		}
	}
}