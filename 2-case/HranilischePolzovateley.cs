using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;

namespace _2_case
{
    public static class HranilischePolzovateley
    {
        private static readonly object Zamok = new object();

        private static List<Polzovatel> ZagruzitVnutri()
        {
            var put = PapkaDannyh.PutKFailuPolzovateli();
            if (!File.Exists(put))
                return new List<Polzovatel>();
            try
            {
                using (var potok = File.OpenRead(put))
                {
                    var serializer = new DataContractJsonSerializer(typeof(List<Polzovatel>));
                    var obekt = serializer.ReadObject(potok);
                    return obekt as List<Polzovatel> ?? new List<Polzovatel>();
                }
            }
            catch
            {
                return new List<Polzovatel>();
            }
        }

        private static void SohranitVnutri(List<Polzovatel> spisok)
        {
            var put = PapkaDannyh.PutKFailuPolzovateli();
            using (var potok = File.Create(put))
            {
                var serializer = new DataContractJsonSerializer(typeof(List<Polzovatel>));
                serializer.WriteObject(potok, spisok);
            }
        }

        public static bool EstLiEmail(string email)
        {
            lock (Zamok)
            {
                var spisok = ZagruzitVnutri();
                return spisok.Any(p => string.Equals(p.Email, email, StringComparison.OrdinalIgnoreCase));
            }
        }

        public static string Zaregistrirovat(string fio, string email, string parol)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(parol))
                return "Заполните email и пароль.";
            lock (Zamok)
            {
                var spisok = ZagruzitVnutri();
                if (spisok.Any(p => string.Equals(p.Email, email, StringComparison.OrdinalIgnoreCase)))
                    return "Такой email уже зарегистрирован.";
                var sol = ModulParolya.SozdatNovuyuSol();
                var hash = ModulParolya.PoluchitHashIzParolya(parol, sol);
                var novyyId = spisok.Count == 0 ? 1 : spisok.Max(p => p.Id) + 1;
                spisok.Add(new Polzovatel
                {
                    Id = novyyId,
                    Fio = fio ?? string.Empty,
                    Email = email.Trim(),
                    ParolHash = hash,
                    ParolSol = sol,
                    PutKFoto = string.Empty
                });
                SohranitVnutri(spisok);
                return string.Empty;
            }
        }

        public static Polzovatel NaytiPoEmailIParolyu(string email, string parol)
        {
            if (string.IsNullOrWhiteSpace(email) || parol == null)
                return null;
            lock (Zamok)
            {
                var spisok = ZagruzitVnutri();
                var polzovatel = spisok.FirstOrDefault(p =>
                    string.Equals(p.Email, email.Trim(), StringComparison.OrdinalIgnoreCase));
                if (polzovatel == null)
                    return null;
                if (!ModulParolya.ParoliSovpadayut(parol, polzovatel.ParolSol, polzovatel.ParolHash))
                    return null;
                return polzovatel;
            }
        }

        public static Polzovatel NaytiPoId(int id)
        {
            lock (Zamok)
            {
                var spisok = ZagruzitVnutri();
                return spisok.FirstOrDefault(p => p.Id == id);
            }
        }

        public static string ObnovitProfil(Polzovatel obnovlennyy)
        {
            if (obnovlennyy == null)
                return "Нет данных.";
            lock (Zamok)
            {
                var spisok = ZagruzitVnutri();
                var indeks = spisok.FindIndex(p => p.Id == obnovlennyy.Id);
                if (indeks < 0)
                    return "Пользователь не найден.";
                var drugoy = spisok.FirstOrDefault(p =>
                    p.Id != obnovlennyy.Id &&
                    string.Equals(p.Email, obnovlennyy.Email, StringComparison.OrdinalIgnoreCase));
                if (drugoy != null)
                    return "Этот email уже занят другим пользователем.";
                var staryy = spisok[indeks];
                spisok[indeks] = new Polzovatel
                {
                    Id = staryy.Id,
                    Fio = obnovlennyy.Fio ?? string.Empty,
                    Email = obnovlennyy.Email.Trim(),
                    ParolHash = staryy.ParolHash,
                    ParolSol = staryy.ParolSol,
                    PutKFoto = obnovlennyy.PutKFoto ?? string.Empty
                };
                SohranitVnutri(spisok);
                return string.Empty;
            }
        }

        public static string SmenitParol(int idPolzovatelya, string staryyParol, string novyyParol)
        {
            if (string.IsNullOrWhiteSpace(novyyParol))
                return "Новый пароль не может быть пустым.";
            lock (Zamok)
            {
                var spisok = ZagruzitVnutri();
                var indeks = spisok.FindIndex(p => p.Id == idPolzovatelya);
                if (indeks < 0)
                    return "Пользователь не найден.";
                var polzovatel = spisok[indeks];
                if (!ModulParolya.ParoliSovpadayut(staryyParol, polzovatel.ParolSol, polzovatel.ParolHash))
                    return "Старый пароль указан неверно.";
                var novayaSol = ModulParolya.SozdatNovuyuSol();
                var novyyHash = ModulParolya.PoluchitHashIzParolya(novyyParol, novayaSol);
                spisok[indeks] = new Polzovatel
                {
                    Id = polzovatel.Id,
                    Fio = polzovatel.Fio,
                    Email = polzovatel.Email,
                    ParolHash = novyyHash,
                    ParolSol = novayaSol,
                    PutKFoto = polzovatel.PutKFoto
                };
                SohranitVnutri(spisok);
                return string.Empty;
            }
        }
    }
}
