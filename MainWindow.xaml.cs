using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace TaskManagerApp
{
    public class TodoItem : INotifyPropertyChanged
    {
        private bool _isChecked;
        public string Text { get; set; }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public partial class MainWindow : Window
    {
        private const string FilePath = "todoList.json";
        public ObservableCollection<TodoItem> TodoItems { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            TodoItems = new ObservableCollection<TodoItem>();
            TodoListBox.ItemsSource = TodoItems;
            LoadTodoList();
        }

        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewItem();
        }

        private void NewItemTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true; // Prevents the default action (e.g., adding a new line)
                AddNewItem();
            }
        }

        private void AddNewItem()
        {
            if (!string.IsNullOrWhiteSpace(NewItemTextBox.Text))
            {
                TodoItems.Add(new TodoItem { Text = NewItemTextBox.Text });
                NewItemTextBox.Clear();
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is TodoItem item)
            {
                if (item.IsChecked)
                {
                    TodoItems.Remove(item);
                    TodoItems.Add(item);
                }
            }
        }

        private void ClearListButton_Click(object sender, RoutedEventArgs e)
        {
            TodoItems.Clear();
        }

        private void LoadTodoList()
        {
            if (File.Exists(FilePath))
            {
                var json = File.ReadAllText(FilePath);
                var items = JsonConvert.DeserializeObject<ObservableCollection<TodoItem>>(json);
                foreach (var item in items)
                {
                    TodoItems.Add(item);
                }
            }
        }

        private void SaveTodoList()
        {
            var json = JsonConvert.SerializeObject(TodoItems, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            SaveTodoList();
        }
    }
}
