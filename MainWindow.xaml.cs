using Journal2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Journal2
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public BindingList<Group>? Groups { get; set; }
		public BindingList<Student>? Students { get; set; }

		public BindingList<Student> VisibleStudents { get; set; } = new();
		public DataTable TestDataTable { get; set; } = new();

		ApplicationContext db;
		Group? selectedGroup;
		public Student? SelectedStudent { get; set; }
		private string connectionString = @"Server=(localdb)\mssqllocaldb;Database=JournalDatabase;Trusted_Connection=True;";
		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;
			ReloadDatabase();
		}

		private void ListChanged(object sender, ListChangedEventArgs e) => db.SaveChanges();

		private void AddGroupButton_Click(object sender, RoutedEventArgs e)
		{
			string name = GroupNameTextbox.Text;
			if (name == null || name.Length < 1) return;
			Groups.Add(new Group { Name = name });
		}

		private void DeleteGroupButton_Click(object sender, RoutedEventArgs e)
		{
			if (selectedGroup == null) return;
			Groups.Remove(selectedGroup);
			selectedGroup = null;
		}

		private void SettingsButton_Click(object sender, RoutedEventArgs e)
		{
			SettingsWindow settingsWindow = new SettingsWindow();
			settingsWindow.ConnectionStringBox.Text = connectionString;
			if(settingsWindow.ShowDialog() == true)
			{
				connectionString = settingsWindow.ConnectionString;
				ReloadDatabase();
			}
		}

		private void GroupsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			selectedGroup = GroupsBox.SelectedItem as Group;
			JournalGrid.IsEnabled = selectedGroup != null;
			UpdateVisibleStudents();
			//if(selectedGroup != null) VisibleStudents = selectedGroup.Students;
		}

		private void AddStudentButton_Click(object sender, RoutedEventArgs e)
		{
			if (selectedGroup == null) return;
			string name = StudentNameTextbox.Text;
			if (name == null || name.Length < 1) return;
			Student student = new() { Name = name, GroupId = selectedGroup.Id };
			Students.Add(student);
			UpdateVisibleStudents();
		}

		private void DeleteStudentButton_Click(object sender, RoutedEventArgs e)
		{
			if (selectedGroup == null) return;
			if (SelectedStudent == null) return;
			Students.Remove((Student)SelectedStudent);
			UpdateVisibleStudents();
		}

		private void UpdateVisibleStudents()
		{
			if (selectedGroup == null) return;
			VisibleStudents.Clear();
			foreach (Student s in Students.Where(s => s.GroupId == selectedGroup.Id))
			{
				VisibleStudents.Add(s);
			}
		}

		private void JournalGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			db.SaveChanges();
		}

		private void ReloadDatabase()
		{
			if (db != null)
			{
				db.Groups.Local.Reset();
				db.Students.Local.Reset();
				db.Dispose();
			}
			db = new(connectionString);

			db.Groups.Load();
			Groups = db.Groups.Local.ToBindingList();
			Groups.ListChanged += ListChanged;
			GroupsBox.ItemsSource = Groups;

			db.Students.Load();
			Students = db.Students.Local.ToBindingList();
			Students.AllowRemove = true;
			Students.ListChanged += ListChanged;
			UpdateVisibleStudents();
		}
	}
}