using System;
using System.IO;

namespace SezXmlSendler.Model
{
    public class SendlersCongif
    {
        public static readonly string PathConfig = Path.Combine(Environment.CurrentDirectory, "appsettings.json");
        public SendlerParam[] SendlerParam { get; set; }
    }
}