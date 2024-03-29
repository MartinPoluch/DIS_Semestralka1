﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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
using Microsoft.Win32;
using SimulationCore;
using Path = System.IO.Path;

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
			_tableCreatorMc = new TableCreatorMC(_diceGame);
			_gameMC = new RandomGameMC(_diceGame, GameMode.AllRandom) {
				ChartSettings = ChartSettings,
				GameTable = _tableCreatorMc
			};
			
			EnableControls();
			ExportFileName = "SimulationOutput";
			CreatingTable = false;
		}
		

		public ChartSettings ChartSettings { get; set; }
		public int Replications { get; set; }

		public int TableReplications { get; set; }

		public string ExportFileName { get; set; }

		public bool CreatingTable { get; set; }

		private void EnableControls() {
			StartBtn.IsEnabled = true;
			StopBtn.IsEnabled = false;
			GameModeGroup.IsEnabled = true;
			ChartGroup.IsEnabled = true;
			TableGroup.IsEnabled = true;
			ReplicationInput.IsEnabled = true;
		}

		private void DisableControls() {
			StartBtn.IsEnabled = false;
			StopBtn.IsEnabled = true;
			GameModeGroup.IsEnabled = false;
			ChartGroup.IsEnabled = false;
			TableGroup.IsEnabled = false;
			ReplicationInput.IsEnabled = false;
		}

		private bool CheckBoxSimulationInput() {
			return false;
		}

		private void CalculateStep() {
			if (Replications <= 100) {
				ChartSettings.Step = 1;
			}
			else {
				double sqrt = Math.Sqrt(Replications);
				ChartSettings.Step = (int)Math.Ceiling(sqrt);
			}
		}

		private void StartSimulation(object sender, RoutedEventArgs e) {
			if (((_gameMC.GameMode == GameMode.UnlimitedTable) || (_gameMC.GameMode == GameMode.LimitedTable)) && (_tableCreatorMc.WinChances.Count == 0)) {
				MessageBox.Show("Table is not created, you need to create table first.");
				return;
			}

			if (Replications <= 0) {
				MessageBox.Show("Number of replications should be higher then zero.");
				return;
			}

			if ((ChartSettings.Step == 0) || (StepInput.Text.Trim() == "") || (StepInput.Text == "0")) {
				CalculateStep();
			}

			_gameMC.Stop = false;
			_gameMC.ChartSettings = ChartSettings;
			_tableCreatorMc.Stop = false;
			DisableControls();
			FirstPlayerChart.Clear();
			SecondPlayerChart.Clear();
			TextOutput.Text = $"Simulation is running ...";
			TextOutput.Text += $"\nReplications: {Replications}\nSkip results: {ChartSettings.SkipReplications}\nStep: {ChartSettings.Step}";
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
				EnableControls();
				LogTextOutput();
			};
			worker.RunWorkerAsync();
		}

		private void StopSimulation(object sender, RoutedEventArgs e) {
			if (CreatingTable) {
				CreatingTable = false;
				_tableCreatorMc.Stop = true;
			}
			else {
				_gameMC.Stop = true;
			}
			
			EnableControls();
		}

		private void UpdateChartsOutput(object sender, ProgressChangedEventArgs e) {
			int[] results = (int[]) e.UserState;
			double firstPlayersWins = results[0];
			double secondPlayersWins = results[1];
			int actualReplications = results[2] + 1;
			double firstPlayerWinChance = (firstPlayersWins / actualReplications) * 100;
			FirstPlayerChart.AddChartValue(actualReplications, firstPlayerWinChance);

			double secondPlayerWinChance = (secondPlayersWins / actualReplications) * 100;
			SecondPlayerChart.AddChartValue(actualReplications, secondPlayerWinChance);
		}

		private void LogTextOutput() {
			TextOutput.Text = _gameMC.TextResult();
			TextOutput.Text += $"\nReplications: {Replications}\nSkip results: {ChartSettings.SkipReplications}\nStep: {ChartSettings.Step}";
		}

		private void CreateWinChanceTable(object sender, RoutedEventArgs e) {
			CreatingTable = true;
			DisableControls();
			TextOutput.Text = "Creating table ...";
			BackgroundWorker worker = new BackgroundWorker {
				WorkerReportsProgress = true,
				WorkerSupportsCancellation = true
			};

			_tableCreatorMc.Worker = worker;
			worker.DoWork += delegate (object o, DoWorkEventArgs args) {
				_tableCreatorMc.CreateTable(TableReplications);
			};
			worker.ProgressChanged += delegate (object o, ProgressChangedEventArgs args) {
				TextOutput.Text = $"Creating table ... processing combination {args.UserState}";
			};
			worker.RunWorkerCompleted += delegate(object o, RunWorkerCompletedEventArgs args) {
				EnableControls();
				if (_tableCreatorMc.WinChances.Count == 0) {
					MessageBox.Show("Creation of table was stopped.");
				}
				else {
					MessageBox.Show("Table was successfully created");
				}
				CreatingTable = false;
			};
			worker.RunWorkerAsync();
		}

		private void ExportTableToFile(object sender, RoutedEventArgs e) {
			DisableControls();
			TextOutput.Text = "Exporting table ...";
			BackgroundWorker worker = new BackgroundWorker {
				WorkerReportsProgress = true,
				WorkerSupportsCancellation = true
			};
		
			worker.DoWork += delegate (object o, DoWorkEventArgs args) {
				string extension = ".csv";
				_tableCreatorMc.WriteWinChancesToFile($"Table_{ExportFileName}{extension}");
				_tableCreatorMc.WriteBestResponsesToFile($"Responses_{ExportFileName}{extension}");
			};
			worker.RunWorkerCompleted += delegate (object o, RunWorkerCompletedEventArgs args) {
				EnableControls();
				MessageBox.Show("File was successfully exported");
			};
			worker.RunWorkerAsync();
		}

		private void RadioButton_Click(object sender, RoutedEventArgs e) {
			if (Equals(sender, AllRandomRb)) {
				_gameMC.GameMode = GameMode.AllRandom;
			}
			else if (Equals(sender, LimitedTableRb)) {
				_gameMC.GameMode = GameMode.LimitedTable;
			}
			else if (Equals(sender, OwnStrategyRb)) {
				_gameMC.GameMode = GameMode.OwnStrategy;
			}
			else if (Equals(sender, UnLimitedTableRb)) {
				_gameMC.GameMode = GameMode.UnlimitedTable;
			}
			else {
				MessageBox.Show($"Radio button error. Game mode not detected.\n {sender.ToString()}");
			}
			TextOutput.Text = $"Game mode changed to {_gameMC.GameMode.ToString()}";
		}

		private void ImportTableFromFile(object sender, RoutedEventArgs e) {
			bool success = false;
			OpenFileDialog openFileDialog = new OpenFileDialog();
			if (openFileDialog.ShowDialog() == true) {
				DisableControls();
				TextOutput.Text = "Importing table ...";
				BackgroundWorker worker = new BackgroundWorker {
					WorkerReportsProgress = true,
					WorkerSupportsCancellation = true
				};

				worker.DoWork += delegate (object o, DoWorkEventArgs args) {
					try {
						_tableCreatorMc.ReadWinChancesFromFile(openFileDialog.FileName);
						success = true;
					}
					catch (Exception exception) {
						MessageBox.Show($"Cannot import file, file has from format.\n{exception.Message}");
					} 
					
				};
				worker.RunWorkerCompleted += delegate (object o, RunWorkerCompletedEventArgs args) {
					EnableControls();
					if (success) {
						MessageBox.Show("File was successfully imported");
					}
				};
				worker.RunWorkerAsync();
			}
		}

		public void CheckNumericInput(object sender, TextCompositionEventArgs e) {
			Regex _regex = new Regex("[^0-9.-]+");
			e.Handled = _regex.IsMatch(e.Text);
		}
	}
}
