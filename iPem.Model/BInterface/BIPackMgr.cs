using LSCService;
using System;

namespace iPem.Model {
    public abstract class BIPackMgr {
        public static GetFsuInfoAckPackage GetFsuInfo(string uri, GetFsuInfoPackage package, int timeout = 5000) {
            var service = new LSCServiceService() { Url = uri, Timeout = timeout };
            var xmlData = service.invoke(package.ToXml());
            return new GetFsuInfoAckPackage(xmlData);
        }
    }
}
