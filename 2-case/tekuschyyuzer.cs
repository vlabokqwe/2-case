namespace _2_case
{
    public static class tekuschyиюzer
    {
        public static Polzovatel polzovательZapis { get; set; }

        public static bool estliPolzovatel()
        {
            return polzovательZapis != null;
        }

        public static void ochistitSesiju()
        {
            polzovательZapis = null;
        }
    }
}
