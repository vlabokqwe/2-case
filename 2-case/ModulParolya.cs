using System;
using System.Security.Cryptography;

namespace _2_case
{
    public static class ModulParolya
    {
        private const int ChisloIteratsiy = 10000;
        private const int RazmerSoli = 16;
        private const int RazmerHash = 32;

        public static string SozdatNovuyuSol()
        {
            var sol = new byte[RazmerSoli];
            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(sol);
            return Convert.ToBase64String(sol);
        }

        public static string PoluchitHashIzParolya(string parol, string solBase64)
        {
            if (string.IsNullOrEmpty(parol) || string.IsNullOrEmpty(solBase64))
                return string.Empty;
            var sol = Convert.FromBase64String(solBase64);
            using (var pbkdf2 = new Rfc2898DeriveBytes(parol, sol, ChisloIteratsiy))
                return Convert.ToBase64String(pbkdf2.GetBytes(RazmerHash));
        }

        public static bool ParoliSovpadayut(string vvedennyyParol, string solBase64, string ozhidaemyyHash)
        {
            if (string.IsNullOrEmpty(ozhidaemyyHash) || string.IsNullOrEmpty(solBase64))
                return false;
            var novyyHash = PoluchitHashIzParolya(vvedennyyParol, solBase64);
            return string.Equals(novyyHash, ozhidaemyyHash, StringComparison.Ordinal);
        }
    }
}
