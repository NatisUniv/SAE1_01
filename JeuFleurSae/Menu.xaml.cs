﻿using System;
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

namespace JeuFleurSae
{
    /// <summary>
    /// Logique d'interaction pour Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void boutJouer_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).preLancement();
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).ControlContent.Content = null;
        }

        private void boutParametre_Click(object sender, RoutedEventArgs e)
        {
            Parametre settingsWindow = new Parametre();
            settingsWindow.ShowDialog();
        }
    }
}
