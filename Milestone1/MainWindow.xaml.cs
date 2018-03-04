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
using Npgsql; 

namespace Milestone1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class Business
        {
            public string name { get; set; }
            public string state { get; set; }
            public string city { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            initColumns(); 
            addStates(); 
        }

        private string buildConnStr()
        {
            return "Host=localhost; Username=postgres; Password=postgres991537; Database = milestone1db";
        }

        public void addStates() //fills in State dropdown menu 
        {
            using (var conn = new NpgsqlConnection(buildConnStr()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT distinct state FROM business ORDER BY state;";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            statelist.Items.Add(reader.GetString(0)); 
                        }
                    }
                }
                conn.Close(); 
            }
        }

        public void addCities(string stateSelection) //fills in city dropdown based on state selection 
        {
            citylist.Items.Clear(); 

            using (var conn = new NpgsqlConnection(buildConnStr()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT distinct city FROM business WHERE state = '" + stateSelection +"';"; 
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            citylist.Items.Add(reader.GetString(0));
                        }
                    }
                }
                conn.Close();
            }
        }

        public void initColumns()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "Business Name";
            col1.Binding = new Binding("name"); 
            col1.Width = 255;
            businessGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "State";
            col2.Binding = new Binding("state");
            col2.Width = 75;
            businessGrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Header = "City";
            col3.Binding = new Binding("city");
            col2.Width = 150; 
            businessGrid.Columns.Add(col3);
        }

        //updates businessGrid when a state is selected 
        private void statelist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            businessGrid.Items.Clear();

            //checks if selection is valid / no runtime errors
            if (statelist.SelectedIndex > -1)
            {
                using (var conn = new NpgsqlConnection(buildConnStr()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT name,state,city FROM business WHERE state = '" + statelist.SelectedItem.ToString() + "';";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                businessGrid.Items.Add(new Business() { name = reader.GetString(0), state = reader.GetString(1), city = reader.GetString(2) });
                            }
                        }
                    }
                    conn.Close();
                }
                addCities(statelist.SelectedItem.ToString()); 
                //updates citylist dropdown depending on selected state
            }
        }

        //update businessGrid when a State AND City are selected 
        private void citylist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            businessGrid.Items.Clear();

            //checks if selection is valid / no runtime errors
            if (citylist.SelectedIndex > -1)
            {
                using (var conn = new NpgsqlConnection(buildConnStr()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT name,state,city FROM business WHERE state = '" + statelist.SelectedItem.ToString() + 
                                          "' AND city = '" + citylist.SelectedItem.ToString() + "';";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                businessGrid.Items.Add(new Business() { name = reader.GetString(0), state = reader.GetString(1), city = reader.GetString(2) });
                            }
                        }
                    }
                    conn.Close();
                }
            }
        }
    }
}
