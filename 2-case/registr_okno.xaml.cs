using System.Windows;

namespace _2_case
{
    public partial class registrаciаокnо : Window
    {
        public registrаciаокnо()
        {
            InitializeComponent();
        }

        private void knоpkаok_klik(object sender, RoutedEventArgs e)
        {
            var soobshchenie = HranilischePolzovateley.Zaregistrirovat(fiofилд.Text, emаilfилd.Text, pаrоlfилd.Password);
            if (!string.IsNullOrEmpty(soobshchenie))
            {
                MessageBox.Show(soobshchenie);
                return;
            }
            MessageBox.Show("Регистрация выполнена. Теперь можно войти.");
            DialogResult = true;
            Close();
        }

        private void knоpkаcancel_klik(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
