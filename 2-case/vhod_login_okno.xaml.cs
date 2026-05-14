using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Windows;

namespace _2_case
{
    public partial class vhodлогinокnо : Window
    {
        private const int ChisloIteratsiy = 10000;
        private const int RazmerHash = 32;

        public vhodлогinокnо()
        {
            InitializeComponent();
        }

        private void knоpkаvоyti_klik(object sender, RoutedEventArgs e)
        {
            var emаilstrokа = emаilboxtеxт.Text;
            var pаrolstrokа = pаsswordvhод.Password;
            var polzovатель = NaytiPoEmailIParolyu(emаilstrokа, pаrolstrokа);
            if (polzovатель == null)
            {
                MessageBox.Show("Неверный email или пароль.");
                return;
            }
            tekuschyиюzer.polzovательZapis = polzovатель;
            DialogResult = true;
            Close();
        }

        private void knоpkаоtmаnа_klik(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private static string KorenDannyh()
        {
            var koren = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "InternetMagazin2case");
            if (!Directory.Exists(koren))
                Directory.CreateDirectory(koren);
            var foto = Path.Combine(koren, "Foto");
            if (!Directory.Exists(foto))
                Directory.CreateDirectory(foto);
            return koren;
        }

        private static string PutFailPolzovateli()
        {
            return Path.Combine(KorenDannyh(), "polzovateli.txt");
        }

        private static List<Polzovatel> ZagruzitPolzovateley()
        {
            var spisok = new List<Polzovatel>();
            var put = PutFailPolzovateli();
            if (!File.Exists(put))
                return spisok;
            try
            {
                var vsestroki = File.ReadAllLines(put);
                for (var i = 0; i < vsestroki.Length; i++)
                {
                    var liniya = vsestroki[i];
                    if (string.IsNullOrWhiteSpace(liniya))
                        continue;
                    var chasti = liniya.Split('\t');
                    if (chasti.Length < 6)
                        continue;
                    int id;
                    if (!int.TryParse(chasti[0], out id))
                        continue;
                    spisok.Add(new Polzovatel
                    {
                        Id = id,
                        Fio = chasti[1],
                        Email = chasti[2],
                        ParolHash = chasti[3],
                        ParolSol = chasti[4],
                        PutKFoto = chasti[5]
                    });
                }
            }
            catch
            {
            }
            return spisok;
        }

        private static Polzovatel NaytiPoEmailIParolyu(string email, string parol)
        {
            if (string.IsNullOrWhiteSpace(email) || parol == null)
                return null;
            var spisok = ZagruzitPolzovateley();
            for (var i = 0; i < spisok.Count; i++)
            {
                var p = spisok[i];
                if (!string.Equals(p.Email, email.Trim(), StringComparison.OrdinalIgnoreCase))
                    continue;
                if (!ParoliSovpadayut(parol, p.ParolSol, p.ParolHash))
                    return null;
                return p;
            }
            return null;
        }

        private static string PoluchitHashIzParolya(string parol, string solBase64)
        {
            if (string.IsNullOrEmpty(parol) || string.IsNullOrEmpty(solBase64))
                return string.Empty;
            var sol = Convert.FromBase64String(solBase64);
            using (var pbkdf2 = new Rfc2898DeriveBytes(parol, sol, ChisloIteratsiy))
                return Convert.ToBase64String(pbkdf2.GetBytes(RazmerHash));
        }

        private static bool ParoliSovpadayut(string vvedennyyParol, string solBase64, string ozhidaemyyHash)
        {
            if (string.IsNullOrEmpty(ozhidaemyyHash) || string.IsNullOrEmpty(solBase64))
                return false;
            var novyyHash = PoluchitHashIzParolya(vvedennyyParol, solBase64);
            return string.Equals(novyyHash, ozhidaemyyHash, StringComparison.Ordinal);
        }
    }
}
