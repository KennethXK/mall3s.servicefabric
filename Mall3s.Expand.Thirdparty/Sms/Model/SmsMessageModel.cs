using Mall3s.Dependency;

namespace Mall3s.Expand.Thirdparty.Sms.Model
{
    [SuppressSniffer]
    public class SmsMessageModel
    {
        public string RequestId { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
    }
}
