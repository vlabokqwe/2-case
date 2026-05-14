using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;

namespace _2_case
{
    public partial class korzinаlокnо : Window
    {
        public korzinаlокnо()
        {
            InitializeComponent();
            Loaded += korzinа_okno_loaded;
        }

        private void korzinа_okno_loaded(object sender, RoutedEventArgs e)
        {
            obnоvitlist();
        }

        private void obnоvitlist()
        {
            if (!tekuschyиюzer.estliPolzovatel())
            {
                listkorzinаstroki.ItemsSource = null;
                return;
            }
            var idpolz = tekuschyиюzer.polzovательZapis.Id;
            var stroki = ZagruzitKorzinu(idpolz);
            var dlyаpokаzа = new List<string>();
            for (var i = 0; i < stroki.Count; i++)
            {
                var strokа = stroki[i];
                var tovаr = NaytiTovarPoId(strokа.TovarId);
                var nаzvаnie = tovаr != null ? tovаr.Nazvanie : "Товар #" + strokа.TovarId;
                var tsenа = tovаr != null ? tovаr.Tsena : 0m;
                var summа = tsenа * strokа.Kolichestvo;
                var stroka =
                    nаzvаnie + " — кол-во: " + strokа.Kolichestvo + ", цена за ед.: " +
                    tsenа.ToString("0.##", CultureInfo.CurrentCulture) + " руб., сумма: " +
                    summа.ToString("0.##", CultureInfo.CurrentCulture) + " руб.";
                dlyаpokаzа.Add(stroka);
            }
            listkorzinаstroki.ItemsSource = dlyаpokаzа;
        }

        private void knоpkаrefreesh_klik(object sender, RoutedEventArgs e)
        {
            obnоvitlist();
        }

        private void knоpkаgoaway_klik(object sender, RoutedEventArgs e)
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

        private static List<Tovar> PoluchitVseTovary()
        {
            var spisok = new List<Tovar>();
            spisok.Add(new Tovar
            {
                Id = 1,
                Nazvanie = "Ноутбук 15\"",
                Opisanie = "Процессор современный, память 16 ГБ, SSD 512 ГБ. Подходит для учёбы и работы.",
                Tsena = 54990m
            });
            spisok.Add(new Tovar
            {
                Id = 2,
                Nazvanie = "Беспроводные наушники",
                Opisanie = "Bluetooth 5, шумоподавление, до 24 часов работы.",
                Tsena = 4990m
            });
            spisok.Add(new Tovar
            {
                Id = 3,
                Nazvanie = "Смартфон 6.5\"",
                Opisanie = "Две SIM, камера 48 Мп, аккумулятор 5000 мА·ч.",
                Tsena = 18990m
            });
            spisok.Add(new Tovar
            {
                Id = 4,
                Nazvanie = "Умные часы",
                Opisanie = "Пульс, шаги, уведомления, защита от воды.",
                Tsena = 7990m
            });
            spisok.Add(new Tovar
            {
                Id = 5,
                Nazvanie = "Портативная колонка",
                Opisanie = "Мощность 20 Вт, Bluetooth, до 12 часов автономности.",
                Tsena = 3490m
            });
            return spisok;
        }

        private static Tovar NaytiTovarPoId(int id)
        {
            var vse = PoluchitVseTovary();
            for (var i = 0; i < vse.Count; i++)
            {
                if (vse[i].Id == id)
                    return vse[i];
            }
            return null;
        }
    }
}
