using System.IO;
using System.Text;

namespace SezXmlSendler.Model
{
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}