/*
 * Created by SharpDevelop.
 * User: jatom
 * Date: 12/04/2016
 * Time: 19:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

using System.Windows.Interop;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Media.Imaging;
using WinDE.Core.Shell;
namespace WinDE
{
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class Icon : UserControl
	{
		public string Name, Path;
		public Bitmap icon;
		public Icon(string Name, Bitmap icon, string Path)
		{
			InitializeComponent();
			this.icon = icon;
			this.Name = Name;
			this.Path = Path;
			this.Loaded += Icon_Loaded;
		}

		void Icon_Loaded(object sender, RoutedEventArgs e)
		{
			appname.Content = Name;
			SetIcon();
		}
		void AppButton_Click(object sender, RoutedEventArgs e)
		{
			try{
			Process.Start(Path);
			}
			catch(Exception ex)
			{
				MessageBox.Show("Can't run Appx!");
			}
		}
		void SetIcon()
		{
			try{
				icon.Save("ic.bmp");
				ImageSource s = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
      icon.GetHbitmap(),
      IntPtr.Zero,
      Int32Rect.Empty,
      BitmapSizeOptions.FromEmptyOptions());
				ic.Background = new ImageBrush(s);
				imgname.Source = s;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}
		public ImageSource imageSourceForImageControl(Bitmap yourBitmap)
		{
 		ImageSourceConverter c = new ImageSourceConverter();
 		return (ImageSource)c.ConvertFrom(yourBitmap);
		}
	}
}