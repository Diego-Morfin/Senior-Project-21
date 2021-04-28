using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System;
using System.Collections.Generic;
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
using Microsoft.Win32;
using System.IO;

namespace WpfTutorialSamples.Audio_and_Video
{
	public partial class AudioVideoPlayerCompleteSample : Window
	{
		private bool mediaPlayerIsPlaying = false;
		private bool userIsDraggingSlider = false;

		public AudioVideoPlayerCompleteSample()
		{
			InitializeComponent();

			LoadFilmList();

			DispatcherTimer timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromMilliseconds(10);
			timer.Tick += timer_Tick;
			timer.Start();
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			if ((mePlayer.Source != null) && (mePlayer.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
			{
				sliProgress.Minimum = 0;
				sliProgress.Maximum = mePlayer.NaturalDuration.TimeSpan.TotalSeconds;
				sliProgress.Value = mePlayer.Position.TotalSeconds;

				if (mePlayer.Position == TimeSpan.FromSeconds(sliProgress.Maximum))
				{
					mePlayer.Position = TimeSpan.FromSeconds(0);
				}
			}
		}

		private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Media files (*.mp4;*.mp3;*.mpg;*.mpeg)|*.mp4;*.mp3;*.mpg;*.mpeg|All files (*.*)|*.*";

			if (openFileDialog.ShowDialog() == true)
			{
				mePlayer.Source = new Uri(openFileDialog.FileName);
				mePlayer.Play();
				Play.Content = "Pause";
			}
		}

		private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (mePlayer != null) && (mePlayer.Source != null);
		}

		private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if ("Play" == Play.Content.ToString())
			{
				mePlayer.Play();
				Play.Content = "Pause";
				mediaPlayerIsPlaying = true;
			}
			else
			{
				mePlayer.Pause();
				Play.Content = "Play";
			}
		}

		private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = mediaPlayerIsPlaying;
		}

		private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			mePlayer.Pause();
		}

		private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = mediaPlayerIsPlaying;
		}

		private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			mePlayer.Stop();
			mediaPlayerIsPlaying = false;
		}

		private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
		{
			userIsDraggingSlider = true;
		}

		private void sliProgress_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			userIsDraggingSlider = false;
			mePlayer.Position = TimeSpan.FromSeconds(sliProgress.Value);
		}

		private void sliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			lblProgressStatus.Text = TimeSpan.FromSeconds(sliProgress.Value).ToString(@"hh\:mm\:ss");
		}

		private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			mePlayer.Volume += (e.Delta > 0) ? 0.1 : -0.1;
		}

		private void back(object sender, RoutedEventArgs e)
		{
			double currPos = mePlayer.Position.TotalSeconds;
			if (currPos < 10)
			{
				mePlayer.Position = TimeSpan.FromSeconds(0); //go to this position
			}
			else
			{
				mePlayer.Position = TimeSpan.FromSeconds(currPos - 10); //go to this position
			}
		}

		private void slow(object sender, RoutedEventArgs e)
		{
			mePlayer.SpeedRatio = .25;
		}

		private void fast(object sender, RoutedEventArgs e)
		{
			mePlayer.SpeedRatio = 1.25;
		}
		private void normal(object sender, RoutedEventArgs e)
		{
			mePlayer.SpeedRatio = 1;
		}

		private void forward(object sender, RoutedEventArgs e)
		{
			double currPos = mePlayer.Position.TotalSeconds;
			if (mePlayer.Position > TimeSpan.FromSeconds(sliProgress.Maximum - 10))
			{
				mePlayer.Position = TimeSpan.FromSeconds(sliProgress.Maximum);
			}
			else
			{
				mePlayer.Position = TimeSpan.FromSeconds(currPos + 10); //go to this position
			}
		}

		private void LoadFilmList()
		{
			FilmListName.Items.Clear();
			FilmListGenre.Items.Clear();
			FilmListLength.Items.Clear();
			FilmListRating.Items.Clear();

			using (var reader = new StreamReader(@"C:\Users\banan\Desktop\CS-4600\Senior-Project\TestVideos\films.txt"))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					char[] delimiters = { ':', '\t' };
					string[] words = line.Split(delimiters);
					if (words.Length == 2)
					{
						FilmListName.Items.Add(words[0]);
					}
					else
					{
						FilmListName.Items.Add(words[0]);
						FilmListGenre.Items.Add(words[1]);
						FilmListRating.Items.Add(words[2]);
						FilmListLength.Items.Add(words[3]);

						FillMovieList(words[0], words[1], words[2], words[3]);
					}
				}
			}
		}

		string filmName, filmGenre, filmRating, filmLength;

		struct filmStruct
		{
			public string sFilmName;
			public string sFilmGenre;
			public string sfilmRating;
			public string sfilmLength;
		}

		List<filmStruct> filmList = new List<filmStruct>();

		private void FillMovieList(string Name, string Genre, string Rating, string Length)
        {
			filmList.Add(new filmStruct());

			var filmStruct = filmList[filmList.Count-1];

			filmStruct.sFilmName = Name;
			filmStruct.sFilmGenre = Genre;
			filmStruct.sfilmRating = Rating;
			filmStruct.sfilmLength = Length;

			filmList[filmList.Count-1] = filmStruct;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
			//string path = @"C:\Users\banan\Desktop\CS-4600\Senior-Project\TestVideos\films.txt";
			//System.IO.File.WriteAllText(path, string.Empty);

			//if (!File.Exists(path))
			//{
			//	using (StreamWriter sw = File.CreateText(path))
			//	{
			//		foreach (var line in FilmList.Items)
			//			sw.WriteLine(line.ToString());
			//	}
			//}
			//else
			//{
			//	using (StreamWriter sw = File.AppendText(path))
			//	{
			//		foreach (var line in FilmList.Items)
			//			sw.WriteLine(line.ToString());
			//	}
			//}
		}

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
			FilmListName.Items.Add(nameTextBox.Text);
			FilmListLength.Items.Add(lengthTextBox.Text);
			FilmListGenre.Items.Add(genreTextBox.Text);
			FilmListRating.Items.Add(ratingTextBox.Text);
		}

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
			FilmListName.Items.RemoveAt(FilmListName.Items.IndexOf(FilmListName.SelectedItem));
			FilmListLength.Items.RemoveAt(FilmListLength.Items.IndexOf(FilmListName.SelectedItem));
			FilmListGenre.Items.RemoveAt(FilmListGenre.Items.IndexOf(FilmListName.SelectedItem));
			FilmListRating.Items.RemoveAt(FilmListRating.Items.IndexOf(FilmListName.SelectedItem));
		}
        private void FilmListName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void FilmListRating_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void FilmListGenre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void FilmListLength_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

		static int minHelper(int x, int y, int z)
		{
			if (x <= y && x <= z)
				return x;
			if (y <= x && y <= z)
				return y;
			else
				return z;
		}

        private void GenreComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void LengthComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void RatingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
			
        }

        private int EditDistance(string word1, String word2, int m, int n) {
			int[,] dp = new int[m + 1, n + 1]; // creates table
			for (int i = 0; i <= m; i++)
			{
				for (int j = 0; j <= n; j++) // fills bottom up
				{
					if (i == 0) // checks if first string is empty
						dp[i, j] = j;
					else if (j == 0) // checks if second string is empty
						dp[i, j] = i;
					else if (word1[i - 1] == word2[j - 1]) //compares last char
						dp[i, j] = dp[i - 1, j - 1];
					else
						dp[i, j] = 1
							   + minHelper(dp[i, j - 1], // Insert
								 dp[i - 1, j],           // Remove
								 dp[i - 1, j - 1]);      // Replace
					}
				}
				return dp[m, n];
		}

		struct DistanceStruct
		{
			public string sFilmName;
			public int sDistance;
		}

		private int convertRating(String rating)
        {
			if (rating == "1/10")
            {
				return 1;
            }else if (rating == "2/10")
            {
				return 2;
            }
			else if (rating == "3/10")
			{
				return 3;
			}
			else if (rating == "4/10")
			{
				return 4;
			}
			else if (rating == "5/10")
			{
				return 5;
			}
			else if (rating == "6/10")
			{
				return 6;
			}
			else if (rating == "7/10")
			{
				return 7;
			}
			else if (rating == "8/10")
			{
				return 8;
			}
			else if (rating == "9/10")
			{
				return 9;
			}
			else if (rating == "10/10")
			{
				return 10;
			}
			return -1;
        }

		private int convertLength(String length)
        {
			if (length == "1h-0m")
            {
				return 1;
            }else if (length == "1h-30m")
            {
				return 2;
            }
			else if (length == "2h-0m")
			{
				return 3;
			}
			else if (length == "2h-30m")
			{
				return 4;
			}
			else if (length == "3h-0m")
			{
				return 5;
			}
			else if (length == "3h-30m")
			{
				return 6;
			}
			else if (length == "4h-0m")
			{
				return 7;
			}
			else if (length == "4h-30m")
			{
				return 8;
			}
			return 0;
        }
		
		private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
			int smallestDistance = 999;
			int count = filmList.Count - 1;
			DistanceStruct distanceList = new DistanceStruct();
			String word1 = NameTextBox.Text;
			int constraint1 = convertLength(cmbLength.Text);
			String constraint2 = cmbGenre.Text;
			int constraint3 = convertRating(cmbRating.Text);



			List<string> closestDistances = new List<string>();

			LoadFilmList();
			for (int i = 0; i < filmList.Count-1; i++)
			{
				int ratingInt = convertRating(filmList[i].sfilmRating);
				int lengthInt = convertLength(filmList[i].sfilmLength);
				if (constraint2 == filmList[i].sFilmGenre && 
					constraint3 <= ratingInt && 
					constraint1 >= lengthInt)
				{
					String word2 = filmList[i].sFilmName; // grabs film name from list of movies
					word2 = word2.ToLower();
					word1 = word1.ToLower();
					int distance = EditDistance(word1, word2, word1.Length, word2.Length); // O(m*n)

					if (distance < smallestDistance)
					{
						smallestDistance = distance;
						distanceList.sFilmName = word2; // store the smallest distance found
						distanceList.sDistance = distance;
						closestDistances.Add(word2);
					}
					if (distance == 0 || count == 0)
					{
						break;
					}
					count -= 1;
				}
			}

			StringBuilder message = new StringBuilder();
			foreach (String movie in closestDistances)
			{
				message.AppendLine(movie + "\n");
			}
			MessageBox.Show(message.ToString());
		}
	}
}

/* TODO
 * fix the save function
 */
