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
using GUI.Core;
using SimulationCore;

namespace GUI {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		private RandomGameMC randomGame;
		private DiceGame _diceGame;

		public MainWindow() {
			InitializeComponent();
			DataContext = this;
			Replications = 1000000;
			ChartSettings = new ChartSettings(1000, 300000);
			Random seeder = new Random();
			_diceGame = new DiceGame(seeder);
			randomGame = new RandomGameMC(_diceGame);
			randomGame.ChartSettings = ChartSettings;
			ReadyToStart();
		}


		public ChartSettings ChartSettings { get; set; }
		public int Replications { get; set; }

		private void ReadyToStart() {
			randomGame.Stop = false;
			StartBtn.IsEnabled = true;
			StopBtn.IsEnabled = false;
		}


		private void StartSimulation(object sender, RoutedEventArgs e) {
			randomGame.Stop = false;
			StartBtn.IsEnabled = false;
			StopBtn.IsEnabled = true;
			FirstPlayerChart.ChartValues.Clear(); //TODO co s osou X, jej max hodnota bude ina
			SecondPlayerChart.ChartValues.Clear();

			BackgroundWorker worker = new BackgroundWorker {
				WorkerReportsProgress = true,
				WorkerSupportsCancellation = true
			};

			randomGame.Worker = worker;
			worker.DoWork += delegate(object o, DoWorkEventArgs args) {
				randomGame.Simulate(Replications);
			};
			worker.ProgressChanged += UpdateChartsOutput;
			worker.RunWorkerCompleted += delegate(object o, RunWorkerCompletedEventArgs args) {
				ReadyToStart();
				LogTextOutput();
			};
			worker.RunWorkerAsync();
		}

		private void StopSimulation(object sender, RoutedEventArgs e) {
			randomGame.Stop = true;
			StartBtn.IsEnabled = true;
			StopBtn.IsEnabled = false;
		}

		private void UpdateChartsOutput(object sender, ProgressChangedEventArgs e) {
			double firstPlayerWinChance = (double)randomGame.DiceGame.FirstPlayerWins / randomGame.ActualReplication;
			FirstPlayerChart.AddChartValue(randomGame.ActualReplication, firstPlayerWinChance);

			double secondPlayerWinChance = (double)randomGame.DiceGame.SecondPlayerWins / randomGame.ActualReplication;
			SecondPlayerChart.AddChartValue(randomGame.ActualReplication, secondPlayerWinChance);
		}

		private void LogTextOutput() {
			TextOutput.Text = randomGame.TextResult();
		}
	}
}
