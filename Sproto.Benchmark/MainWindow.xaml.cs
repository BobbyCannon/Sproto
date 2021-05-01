#region References

using System.Windows;

#endregion

namespace Sproto.Benchmark
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region Constructors

		public MainWindow()
		{
			InitializeComponent();

			ViewModel = new MainViewModel();
			DataContext = ViewModel;
		}

		#endregion

		#region Properties

		public MainViewModel ViewModel { get; }

		#endregion
	}
}