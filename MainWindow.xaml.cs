﻿using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Animation;



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
        private Point _dragStartPoint;
        public ObservableCollection<TodoItem> TodoItems { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            TodoItems = new ObservableCollection<TodoItem>();
            TodoListBox.ItemsSource = TodoItems;
            LoadTodoList();
            UpdateWindowTitle();

            PositionWindowBottomRight();
            this.Topmost = true;
        }

        private void PositionWindowBottomRight()
        {
            // Get the dimensions of the primary screen
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            // Set the window's position to the bottom-right corner
            this.Left = screenWidth - this.Width - 15;
            this.Top = screenHeight - this.Height - 40;
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
                UpdateWindowTitle();
                SaveTodoList();
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is TodoItem item)
            {
                FadeOutAndRemoveItem(item);
                SaveTodoList();
            }
        }

        private void FadeOutAndRemoveItem(TodoItem item)
        {
            var listBoxItem = TodoListBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
            if (listBoxItem != null)
            {
                var storyboard = new Storyboard();
                var fadeOutAnimation = new DoubleAnimation
                {
                    From = 1.0,
                    To = 0.0,
                    Duration = new Duration(TimeSpan.FromSeconds(1))
                };
                Storyboard.SetTarget(fadeOutAnimation, listBoxItem);
                Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath("Opacity"));
                storyboard.Children.Add(fadeOutAnimation);

                storyboard.Completed += (s, e) =>
                {
                    TodoItems.Remove(item);
                    UpdateWindowTitle();
                };
                storyboard.Begin();
            }
            else
            {
                // If ListBoxItem is null, just remove the item directly
                TodoItems.Remove(item);
                UpdateWindowTitle();
            }
        }

        private void ClearListButton_Click(object sender, RoutedEventArgs e)
        {
            TodoItems.Clear();
            UpdateWindowTitle();
            SaveTodoList();
        }

        private void UpdateWindowTitle()
        {
            if (TodoItems.Count == 0)
            {
                this.Title = "All done!";
            }
            else
            {
                this.Title = $"{TodoItems.Count} tasks remaining!";
            }
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
                UpdateWindowTitle();
            }
        }

        private void SaveTodoList()
        {
            var json = JsonConvert.SerializeObject(TodoItems, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        private void TodoListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
        }

        private void TodoListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPos = e.GetPosition(null);
                if (Math.Abs(currentPos.X - _dragStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(currentPos.Y - _dragStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    ListBox listBox = sender as ListBox;
                    TodoItem selectedItem = (TodoItem)listBox.SelectedItem;
                    if (selectedItem != null)
                    {
                        // Start the drag operation
                        DragDrop.DoDragDrop(listBox, selectedItem, DragDropEffects.Move);
                    }
                }
            }
        }

        private void TodoListBox_DragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(TodoItem)))
            {
                e.Effects = DragDropEffects.None;
            }
            else
            {
                e.Effects = DragDropEffects.Move;
            }
            e.Handled = true;
        }

        private void TodoListBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TodoItem)))
            {
                TodoItem droppedData = e.Data.GetData(typeof(TodoItem)) as TodoItem;
                TodoItem target = ((FrameworkElement)e.OriginalSource).DataContext as TodoItem;

                int oldIndex = TodoItems.IndexOf(droppedData);
                int newIndex = TodoItems.IndexOf(target);

                if (oldIndex != -1 && newIndex != -1)
                {
                    TodoItems.Move(oldIndex, newIndex);
                    SaveTodoList(); // Save updated order
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            SaveTodoList();
        }
    }
}
