using System;
using System.IO;

namespace _2_case
{
    public static class PapkaDannyh
    {
        public static string PoluchitKorenPapki()
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

        public static string PutKFailuPolzovateli()
        {
            return Path.Combine(PoluchitKorenPapki(), "polzovateli.json");
        }

        public static string PutKFailuKorziny(int idPolzovatelya)
        {
            return Path.Combine(PoluchitKorenPapki(), "korzina_" + idPolzovatelya + ".json");
        }

        public static string PapkaDlyaFoto()
        {
            return Path.Combine(PoluchitKorenPapki(), "Foto");
        }
    }
}
