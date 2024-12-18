using System.Configuration;
using System.Windows;
using System.Windows.Controls;

namespace JeuFleurSae
{
    public partial class Parametre : Window
    {
        public Parametre()
        {
            InitializeComponent();

        }

        // Annuler : Fermer la fenêtre sans appliquer les changements
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Appliquer les paramètres
        private void Appliquer_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.NiveauSon = sliderSon.Value;
            DialogResult = true;

            // Vérifier que l'élément sélectionné dans la ComboBox est valide
            string difficulty = null;
            if (ComboBoxDifficulte.SelectedItem is ComboBoxItem selectedItem)
            {
                difficulty = selectedItem.Content.ToString();
            }

            // Récupérer la configuration des touches
            string configuredKeys = KeyDisplay.Text;
        }
    }
}
