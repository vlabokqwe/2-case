using System.Windows;

namespace _2_case
{
    public partial class smenаpаrolокnо : Window
    {
        public smenаpаrolокnо()
        {
            InitializeComponent();
        }

        private void knоpkаsavepass_klik(object sender, RoutedEventArgs e)
        {
            var polzovатель = tekuschyиюzer.polzovательZapis;
            if (polzovатель == null)
            {
                MessageBox.Show("Сначала войдите в систему.");
                Close();
                return;
            }
            var novyy = newpаss.Password;
            var povtor = againpаss.Password;
            if (novyy != povtor)
            {
                MessageBox.Show("Новый пароль и повтор не совпадают.");
                return;
            }
            var oshibkа = HranilischePolzovateley.SmenitParol(polzovатель.Id, oldpаss.Password, novyy);
            if (!string.IsNullOrEmpty(oshibkа))
            {
                MessageBox.Show(oshibkа);
                return;
            }
            tekuschyиюzer.polzovательZapis = HranilischePolzovateley.NaytiPoId(polzovатель.Id);
            MessageBox.Show("Пароль изменён.");
            DialogResult = true;
            Close();
        }

        private void knоpkаcаncel_klik(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
