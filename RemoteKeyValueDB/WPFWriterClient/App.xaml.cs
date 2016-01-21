using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApplication1
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow wnd = new MainWindow();
            if(e.Args==null)
            wnd.WPFClientStart(e.Args[0], e.Args[1]);
            else
            wnd.WPFClientStart("/Performance", "8089");
            wnd.Show();
        }
    }
}
