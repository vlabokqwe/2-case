using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Windows;

namespace _2_case
{
    public partial class registrаciаокnо : Window
    {
        private const int ChisloIteratsiy = 10000;
        private const int RazmerSoli = 16;
        private const int RazmerHash = 32;

        public registrаciаокnо()
        {
            InitializeComponent();
        }

        private void knоpkаok_klik(object sender, RoutedEventArgs e)
        {
            var soobshchenie = Zaregistrirovat(fiofилд.Text, emаilfилd.Text, pаrоlfилd.Password);
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

        private static void SohranitPolzovateley(List<Polzovatel> spisok)
        {
            var put = PutFailPolzovateli();
            var linii = new string[spisok.Count];
            for (var i = 0; i < spisok.Count; i++)
            {
                var p = spisok[i];
                linii[i] = p.Id + "\t" + (p.Fio ?? "") + "\t" + (p.Email ?? "") + "\t" + (p.ParolHash ?? "") + "\t" +
                           (p.ParolSol ?? "") + "\t" + (p.PutKFoto ?? "");
            }
            File.WriteAllLines(put, linii);
        }

        private static string SozdatNovuyuSol()
        {
            var sol = new byte[RazmerSoli];
            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(sol);
            return Convert.ToBase64String(sol);
        }

        private static string PoluchitHashIzParolya(string parol, string solBase64)
        {
            if (string.IsNullOrEmpty(parol) || string.IsNullOrEmpty(solBase64))
                return string.Empty;
            var sol = Convert.FromBase64String(solBase64);
            using (var pbkdf2 = new Rfc2898DeriveBytes(parol, sol, ChisloIteratsiy))
                return Convert.ToBase64String(pbkdf2.GetBytes(RazmerHash));
        }

        private static string Zaregistrirovat(string fio, string email, string parol)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(parol))
                return "Заполните email и пароль.";
            var spisok = ZagruzitPolzovateley();
            var emailTrim = email.Trim();
            for (var i = 0; i < spisok.Count; i++)
            {
                if (string.Equals(spisok[i].Email, emailTrim, StringComparison.OrdinalIgnoreCase))
                    return "Такой email уже зарегистрирован.";
            }
            var sol = SozdatNovuyuSol();
            var hash = PoluchitHashIzParolya(parol, sol);
            var novyyId = 1;
            for (var i = 0; i < spisok.Count; i++)
            {
                if (spisok[i].Id >= novyyId)
                    novyyId = spisok[i].Id + 1;
            }
            spisok.Add(new Polzovatel
            {
                Id = novyyId,
                Fio = fio ?? string.Empty,
                Email = emailTrim,
                ParolHash = hash,
                ParolSol = sol,
                PutKFoto = string.Empty
            });
            SohranitPolzovateley(spisok);
            return string.Empty;
        }
    }
}
