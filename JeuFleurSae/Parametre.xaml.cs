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
            // Récupérer la valeur du slider (niveau de son) comme double pour conserver la précision
            double soundLevel = sliderSon.Value; // pas de cast vers int pour conserver la précision

            // Vérifier que l'élément sélectionné dans la ComboBox est valide
            string difficulty = null;
            if (ComboBoxDifficulte.SelectedItem is ComboBoxItem selectedItem)
            {
                difficulty = selectedItem.Content.ToString();
            }

            // Récupérer la configuration des touches
            string configuredKeys = KeyDisplay.Text;

            // Appliquer les paramètres (affichage ici, mais tu peux enregistrer ou utiliser ces valeurs)
            MessageBox.Show($"Paramètres appliqués:\nSon: {soundLevel}\nDifficulté: {difficulty}\nTouches: {configuredKeys}");
        }
    }
}
