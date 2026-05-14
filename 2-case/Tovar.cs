using System.Runtime.Serialization;

namespace _2_case
{
    [DataContract]
    public class Tovar
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Nazvanie { get; set; }

        [DataMember]
        public string Opisanie { get; set; }

        [DataMember]
        public decimal Tsena { get; set; }
    }
}
