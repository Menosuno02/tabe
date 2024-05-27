using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabeNuget
{
    public class KeysModel
    {
        public string Audience { get; set; }
        public string CacheRedisKey { get; set; }
        public string EncryptKey { get; set; }
        public string GoogleApiKey { get; set; }
        public string Issuer { get; set; }
        public string MySql { get; set; }
        public string SecretKey { get; set; }
        public string SignalRKey { get; set; }
        public string StorageAccountKey { get; set; }
        public string TabeAPI { get; set; }
        public string TabeMailPass { get; set; }
    }
}
