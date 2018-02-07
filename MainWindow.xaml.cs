using System;
using System.Windows;

namespace sdproject
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			var parser = new GraphParser(AppDomain.CurrentDomain.BaseDirectory + @"..\..\Templates\borneo.xmile");
			var graph = parser.CreateGraph();
			InitializeComponent();
		}
	}
}
