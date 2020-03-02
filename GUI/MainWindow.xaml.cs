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

		private RandomGameMC _gameMC;
		private TableCreatorMC _tableCreatorMc;
		private DiceGame _diceGame;

		public MainWindow() {
			InitializeComponent();
			DataContext = this;
			Replications = 1000000;
			TableReplications = 100;
			ChartSettings = new ChartSettings(1000, 300000);
			Random seeder = new Random();
			_diceGame = new DiceGame(seeder);
			_gameMC = new RandomGameMC(_diceGame) {ChartSettings = ChartSettings};
			_tableCreatorMc = new TableCreatorMC(_diceGame);
			ReadyToStart();
		}

		public ChartSettings ChartSettings { get; set; }
		public int Replications { get; set; }

		public int TableReplications { get; set; }

		private void ReadyToStart() {
			_gameMC.Stop = false;
			StartBtn.IsEnabled = true;
			StopBtn.IsEnabled = false;
			CreateTableBtn.IsEnabled = true;
		}


		private void StartSimulation(object sender, RoutedEventArgs e) {
			_gameMC.Stop = false;
			StartBtn.IsEnabled = false;
			StopBtn.IsEnabled = true;
			FirstPlayerChart.Clear();
			SecondPlayerChart.Clear();
			TextOutput.Text = "Simulation is running ...";

			BackgroundWorker worker = new BackgroundWorker {
				WorkerReportsProgress = true,
				WorkerSupportsCancellation = true
			};

			_gameMC.Worker = worker;
			worker.DoWork += delegate(object o, DoWorkEventArgs args) {
				_gameMC.Simulate(Replications);
			};
			worker.ProgressChanged += UpdateChartsOutput;
			worker.RunWorkerCompleted += delegate(object o, RunWorkerCompletedEventArgs args) {
				ReadyToStart();
				LogTextOutput();
			};
			worker.RunWorkerAsync();
		}

		private void StopSimulation(object sender, RoutedEventArgs e) {
			_gameMC.Stop = true;
			StartBtn.IsEnabled = true;
			StopBtn.IsEnabled = false;
		}

		private void UpdateChartsOutput(object sender, ProgressChangedEventArgs e) {
			//TODO toto doriesit aby sa tu nemusel volat DiceGame
			double firstPlayerWinChance = (double)_gameMC.DiceGame.FirstPlayerWins / _gameMC.ActualReplication;
			FirstPlayerChart.AddChartValue(_gameMC.ActualReplication, firstPlayerWinChance);

			double secondPlayerWinChance = (double)_gameMC.DiceGame.SecondPlayerWins / _gameMC.ActualReplication;
			SecondPlayerChart.AddChartValue(_gameMC.ActualReplication, secondPlayerWinChance);
		}

		private void LogTextOutput() {
			TextOutput.Text = _gameMC.TextResult();
		}

		private void CreateWinChanceTable(object sender, RoutedEventArgs e) {
			//TODO znefunkci buttony start 
			_tableCreatorMc.WinChancesDict.Clear();
			BackgroundWorker worker = new BackgroundWorker {
				WorkerReportsProgress = true,
				WorkerSupportsCancellation = true
			};

			worker.DoWork += delegate (object o, DoWorkEventArgs args) {
				_tableCreatorMc.CreateTable(TableReplications);
			};
			worker.RunWorkerCompleted += delegate (object o, RunWorkerCompletedEventArgs args) {
				ReadyToStart();
				MessageBox.Show("Done");
			};
			worker.RunWorkerAsync();
		}
	}
}
