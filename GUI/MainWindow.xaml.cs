using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SimulationCore;

namespace GUI {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		private DiceGameMC _simCore;

		public MainWindow() {
			InitializeComponent();
			DataContext = this;
			Random seeder = new Random();
			_simCore = new DiceGameMC(seeder);
			ReadyToStart();
			InitializeCharts();
		}

		private void InitializeCharts() {
			Replications = 1000;
			Step = 1;
			SkipReplications = 300;
		}

		public int Replications { get; set; }

		public int Step { get; set; }

		public int SkipReplications { get; set; }

		private void ReadyToStart() {
			_simCore.Stop = false;
			StartBtn.IsEnabled = true;
			StopBtn.IsEnabled = false;
		}


		private void StartSimulation(object sender, RoutedEventArgs e) {
			_simCore.Stop = false;
			StartBtn.IsEnabled = false;
			StopBtn.IsEnabled = true;
			FirstPlayerChart.ChartValues.Clear(); //TODO co s osou X, jej max hodnota bude ina
			SecondPlayerChart.ChartValues.Clear();

			BackgroundWorker worker = new BackgroundWorker {
				WorkerReportsProgress = true,
				WorkerSupportsCancellation = true
			};

			_simCore.Worker = worker;
			worker.DoWork += delegate(object o, DoWorkEventArgs args) {
				_simCore.Simulate(Replications);
			};
			worker.ProgressChanged += UpdateChartsOutput;
			worker.RunWorkerCompleted += delegate(object o, RunWorkerCompletedEventArgs args) {
				ReadyToStart();
			};
			worker.RunWorkerAsync();
		}

		private void StopSimulation(object sender, RoutedEventArgs e) {
			_simCore.Stop = true;
			StartBtn.IsEnabled = true;
			StopBtn.IsEnabled = false;
		}

		private void UpdateChartsOutput(object sender, ProgressChangedEventArgs e) {
			int actualReplication = (int) e.UserState; 

			if ((actualReplication > SkipReplications) && (actualReplication % Step == 0)) {
				double firstPlayerWinChance = (double)_simCore.FirstPlayerWins / _simCore.ActualReplication;
				FirstPlayerChart.AddChartValue(_simCore.ActualReplication, firstPlayerWinChance);

				double secondPlayerWinChance = (double)_simCore.SecondPlayerWins / _simCore.ActualReplication;
				SecondPlayerChart.AddChartValue(_simCore.ActualReplication, secondPlayerWinChance);
			}
		}

		private void TextOutput() {

		}
	}
}
