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

		private RandomGameMC _randomGame;
		private DiceGame _diceGame;

		public MainWindow() {
			InitializeComponent();
			DataContext = this;
			Replications = 1000000;
			ChartSettings = new ChartSettings(1000, 300000);
			Random seeder = new Random();
			_diceGame = new DiceGame(seeder);
			_randomGame = new RandomGameMC(_diceGame);
			_randomGame.ChartSettings = ChartSettings;
			ReadyToStart();
		}

		public ChartSettings ChartSettings { get; set; }
		public int Replications { get; set; }

		private void ReadyToStart() {
			_randomGame.Stop = false;
			StartBtn.IsEnabled = true;
			StopBtn.IsEnabled = false;
		}


		private void StartSimulation(object sender, RoutedEventArgs e) {
			_randomGame.Stop = false;
			StartBtn.IsEnabled = false;
			StopBtn.IsEnabled = true;
			FirstPlayerChart.Clear();
			SecondPlayerChart.Clear();
			TextOutput.Text = "Simulation is running ...";

			BackgroundWorker worker = new BackgroundWorker {
				WorkerReportsProgress = true,
				WorkerSupportsCancellation = true
			};

			_randomGame.Worker = worker;
			worker.DoWork += delegate(object o, DoWorkEventArgs args) {
				_randomGame.Simulate(Replications);
			};
			worker.ProgressChanged += UpdateChartsOutput;
			worker.RunWorkerCompleted += delegate(object o, RunWorkerCompletedEventArgs args) {
				ReadyToStart();
				LogTextOutput();
			};
			worker.RunWorkerAsync();
		}

		private void StopSimulation(object sender, RoutedEventArgs e) {
			_randomGame.Stop = true;
			StartBtn.IsEnabled = true;
			StopBtn.IsEnabled = false;
		}

		private void UpdateChartsOutput(object sender, ProgressChangedEventArgs e) {
			//TODO toto doriesit aby sa tu nemusel volat DiceGame
			double firstPlayerWinChance = (double)_randomGame.DiceGame.FirstPlayerWins / _randomGame.ActualReplication;
			FirstPlayerChart.AddChartValue(_randomGame.ActualReplication, firstPlayerWinChance);

			double secondPlayerWinChance = (double)_randomGame.DiceGame.SecondPlayerWins / _randomGame.ActualReplication;
			SecondPlayerChart.AddChartValue(_randomGame.ActualReplication, secondPlayerWinChance);
		}

		private void LogTextOutput() {
			TextOutput.Text = _randomGame.TextResult();
		}
	}
}
