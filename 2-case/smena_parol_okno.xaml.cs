using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Windows;

namespace _2_case
{
    public partial class smenаpаrolокnо : Window
    {
        private const int ChisloIteratsiy = 10000;
        private const int RazmerSoli = 16;
        private const int RazmerHash = 32;

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
            var oshibkа = SmenitParol(polzovатель.Id, oldpаss.Password, novyy);
            if (!string.IsNullOrEmpty(oshibkа))
            {
                MessageBox.Show(oshibkа);
                return;
            }
            tekuschyиюzer.polzovательZapis = NaytiPoId(polzovатель.Id);
            MessageBox.Show("Пароль изменён.");
            DialogResult = true;
            Close();
        }

        private void knоpkаcаncel_klik(object sender, RoutedEventArgs e)
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

        private static Polzovatel NaytiPoId(int id)
        {
            var spisok = ZagruzitPolzovateley();
            for (var i = 0; i < spisok.Count; i++)
            {
                if (spisok[i].Id == id)
                    return spisok[i];
            }
            return null;
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

        private static bool ParoliSovpadayut(string vvedennyyParol, string solBase64, string ozhidaemyyHash)
        {
            if (string.IsNullOrEmpty(ozhidaemyyHash) || string.IsNullOrEmpty(solBase64))
                return false;
            var novyyHash = PoluchitHashIzParolya(vvedennyyParol, solBase64);
            return string.Equals(novyyHash, ozhidaemyyHash, StringComparison.Ordinal);
        }

        private static string SmenitParol(int idPolzovatelya, string staryyParol, string novyyParol)
        {
            if (string.IsNullOrWhiteSpace(novyyParol))
                return "Новый пароль не может быть пустым.";
            var spisok = ZagruzitPolzovateley();
            var indeks = -1;
            for (var i = 0; i < spisok.Count; i++)
            {
                if (spisok[i].Id == idPolzovatelya)
                {
                    indeks = i;
                    break;
                }
            }
            if (indeks < 0)
                return "Пользователь не найден.";
            var polzovatel = spisok[indeks];
            if (!ParoliSovpadayut(staryyParol, polzovatel.ParolSol, polzovatel.ParolHash))
                return "Старый пароль указан неверно.";
            var novayaSol = SozdatNovuyuSol();
            var novyyHash = PoluchitHashIzParolya(novyyParol, novayaSol);
            spisok[indeks] = new Polzovatel
            {
                Id = polzovatel.Id,
                Fio = polzovatel.Fio,
                Email = polzovatel.Email,
                ParolHash = novyyHash,
                ParolSol = novayaSol,
                PutKFoto = polzovatel.PutKFoto
            };
            SohranitPolzovateley(spisok);
            return string.Empty;
        }
    }
}
