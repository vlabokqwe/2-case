using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;

namespace _2_case
{
    public static class HranilischeKorziny
    {
        private static readonly object Zamok = new object();

        public static List<KorzinaStroka> ZagruzitKorzinu(int idPolzovatelya)
        {
            lock (Zamok)
            {
                var put = PapkaDannyh.PutKFailuKorziny(idPolzovatelya);
                if (!File.Exists(put))
                    return new List<KorzinaStroka>();
                using (var potok = File.OpenRead(put))
                {
                    var serializer = new DataContractJsonSerializer(typeof(List<KorzinaStroka>));
                    var obekt = serializer.ReadObject(potok);
                    return obekt as List<KorzinaStroka> ?? new List<KorzinaStroka>();
                }
            }
        }

        public static void SohranitKorzinu(int idPolzovatelya, List<KorzinaStroka> stroki)
        {
            lock (Zamok)
            {
                var put = PapkaDannyh.PutKFailuKorziny(idPolzovatelya);
                using (var potok = File.Create(put))
                {
                    var serializer = new DataContractJsonSerializer(typeof(List<KorzinaStroka>));
                    serializer.WriteObject(potok, stroki ?? new List<KorzinaStroka>());
                }
            }
        }

        public static void DobavitTovar(int idPolzovatelya, int idTovara, int kolichestvo)
        {
            if (kolichestvo <= 0)
                kolichestvo = 1;
            lock (Zamok)
            {
                var put = PapkaDannyh.PutKFailuKorziny(idPolzovatelya);
                List<KorzinaStroka> spisok;
                if (!File.Exists(put))
                    spisok = new List<KorzinaStroka>();
                else
                    using (var potok = File.OpenRead(put))
                    {
                        var serializer = new DataContractJsonSerializer(typeof(List<KorzinaStroka>));
                        spisok = serializer.ReadObject(potok) as List<KorzinaStroka> ?? new List<KorzinaStroka>();
                    }
                var stroka = spisok.FirstOrDefault(s => s.TovarId == idTovara);
                if (stroka == null)
                    spisok.Add(new KorzinaStroka { TovarId = idTovara, Kolichestvo = kolichestvo });
                else
                    stroka.Kolichestvo += kolichestvo;
                using (var potokZapis = File.Create(put))
                {
                    var serializer = new DataContractJsonSerializer(typeof(List<KorzinaStroka>));
                    serializer.WriteObject(potokZapis, spisok);
                }
            }
        }
    }
}
