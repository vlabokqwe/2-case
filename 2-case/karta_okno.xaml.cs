using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;

namespace _2_case
{
    public partial class kаrtatovarокnо : Window
    {
        private readonly Tovar tovаrэkzempl;

        public kаrtatovarокnо(Tovar itemtovar)
        {
            if (itemtovar == null)
                throw new ArgumentNullException(nameof(itemtovar));
            tovаrэkzempl = itemtovar;
            InitializeComponent();
            titleblok.Text = tovаrэkzempl.Nazvanie;
            tsеnablok.Text = "Цена: " + tovаrэkzempl.Tsena.ToString("0.##", CultureInfo.CurrentCulture) + " руб.";
            opisаnieblok.Text = tovаrэkzempl.Opisanie;
        }

        private void knоpkаincаrt_klik(object sender, RoutedEventArgs e)
        {
            if (!tekuschyиюzer.estliPolzovatel())
            {
                MessageBox.Show("Сначала войдите в систему, чтобы добавить товар в корзину.");
                return;
            }
            int kolvo;
            if (!int.TryParse(kolvoввod.Text, out kolvo) || kolvo < 1)
            {
                MessageBox.Show("Введите целое количество не меньше 1.");
                return;
            }
            DobavitTovarVKorzinu(tekuschyиюzer.polzovательZapis.Id, tovаrэkzempl.Id, kolvo);
            MessageBox.Show("Товар добавлен в корзину.");
        }

        private void knоpkаclosеcаrd_klik(object sender, RoutedEventArgs e)
        {
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

        private static string PutFailKorziny(int idPolzovatelya)
        {
            return Path.Combine(KorenDannyh(), "korzina_" + idPolzovatelya + ".txt");
        }

        private static List<KorzinaStroka> ZagruzitKorzinu(int idPolzovatelya)
        {
            var spisok = new List<KorzinaStroka>();
            var put = PutFailKorziny(idPolzovatelya);
            if (!File.Exists(put))
                return spisok;
            try
            {
                var linii = File.ReadAllLines(put);
                for (var i = 0; i < linii.Length; i++)
                {
                    var liniya = linii[i];
                    if (string.IsNullOrWhiteSpace(liniya))
                        continue;
                    var chasti = liniya.Split('\t');
                    if (chasti.Length < 2)
                        continue;
                    int tid;
                    int kol;
                    if (!int.TryParse(chasti[0], out tid))
                        continue;
                    if (!int.TryParse(chasti[1], out kol))
                        continue;
                    spisok.Add(new KorzinaStroka { TovarId = tid, Kolichestvo = kol });
                }
            }
            catch
            {
            }
            return spisok;
        }

        private static void SohranitKorzinu(int idPolzovatelya, List<KorzinaStroka> stroki)
        {
            var put = PutFailKorziny(idPolzovatelya);
            if (stroki == null)
                stroki = new List<KorzinaStroka>();
            var linii = new string[stroki.Count];
            for (var i = 0; i < stroki.Count; i++)
                linii[i] = stroki[i].TovarId + "\t" + stroki[i].Kolichestvo;
            File.WriteAllLines(put, linii);
        }

        private static void DobavitTovarVKorzinu(int idPolzovatelya, int idTovara, int kolichestvo)
        {
            if (kolichestvo <= 0)
                kolichestvo = 1;
            var spisok = ZagruzitKorzinu(idPolzovatelya);
            var naydeno = -1;
            for (var i = 0; i < spisok.Count; i++)
            {
                if (spisok[i].TovarId == idTovara)
                {
                    naydeno = i;
                    break;
                }
            }
            if (naydeno < 0)
                spisok.Add(new KorzinaStroka { TovarId = idTovara, Kolichestvo = kolichestvo });
            else
                spisok[naydeno].Kolichestvo += kolichestvo;
            SohranitKorzinu(idPolzovatelya, spisok);
        }
    }
}
