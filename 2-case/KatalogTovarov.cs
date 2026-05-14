using System.Collections.Generic;

namespace _2_case
{
    public static class KatalogTovarov
    {
        public static List<Tovar> PoluchitVseTovary()
        {
            return new List<Tovar>
            {
                new Tovar
                {
                    Id = 1,
                    Nazvanie = "Ноутбук 15\"",
                    Opisanie = "Процессор современный, память 16 ГБ, SSD 512 ГБ. Подходит для учёбы и работы.",
                    Tsena = 54990m
                },
                new Tovar
                {
                    Id = 2,
                    Nazvanie = "Беспроводные наушники",
                    Opisanie = "Bluetooth 5, шумоподавление, до 24 часов работы.",
                    Tsena = 4990m
                },
                new Tovar
                {
                    Id = 3,
                    Nazvanie = "Смартфон 6.5\"",
                    Opisanie = "Две SIM, камера 48 Мп, аккумулятор 5000 мА·ч.",
                    Tsena = 18990m
                },
                new Tovar
                {
                    Id = 4,
                    Nazvanie = "Умные часы",
                    Opisanie = "Пульс, шаги, уведомления, защита от воды.",
                    Tsena = 7990m
                },
                new Tovar
                {
                    Id = 5,
                    Nazvanie = "Портативная колонка",
                    Opisanie = "Мощность 20 Вт, Bluetooth, до 12 часов автономности.",
                    Tsena = 3490m
                }
            };
        }

        public static Tovar NaytiPoId(int id)
        {
            foreach (var tovar in PoluchitVseTovary())
            {
                if (tovar.Id == id)
                    return tovar;
            }
            return null;
        }
    }
}
