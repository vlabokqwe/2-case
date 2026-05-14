using System.Runtime.Serialization;

namespace _2_case
{
    [DataContract]
    public class KorzinaStroka
    {
        [DataMember]
        public int TovarId { get; set; }

        [DataMember]
        public int Kolichestvo { get; set; }
    }
}
