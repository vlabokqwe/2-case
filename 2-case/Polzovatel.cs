using System.Runtime.Serialization;

namespace _2_case
{
    [DataContract]
    public class Polzovatel
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Fio { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string ParolHash { get; set; }

        [DataMember]
        public string ParolSol { get; set; }

        [DataMember]
        public string PutKFoto { get; set; }
    }
}
