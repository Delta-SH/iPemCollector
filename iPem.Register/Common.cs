using System;

namespace iPem.Register {
    public static class Common {
        public static RSA_Key GetRSAKey() {
            return new RSA_Key {
                Public = @"<RSAKeyValue><Modulus>tZUVInFeKdIih+jcrZeRY/yl5Dz8XJQtbTJcI/HtGYRMv80kfxZ9/5dXT+qyyzxGWd46TDD55EM/91s2SCnkuyBUKZBxTItvwNIOO8kNWxaqvaTQPMOTI9H0ckXGBNFOFbvtJzs3K6LgBe4jyOKodaeDAc67ZxpKRZ0A3LTmhUU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>",
                Private = @"<RSAKeyValue><Modulus>tZUVInFeKdIih+jcrZeRY/yl5Dz8XJQtbTJcI/HtGYRMv80kfxZ9/5dXT+qyyzxGWd46TDD55EM/91s2SCnkuyBUKZBxTItvwNIOO8kNWxaqvaTQPMOTI9H0ckXGBNFOFbvtJzs3K6LgBe4jyOKodaeDAc67ZxpKRZ0A3LTmhUU=</Modulus><Exponent>AQAB</Exponent><P>4vuI9TnisqvQJkuVoyp+VQOhnO+sP/qZ1SimFOlSK8H439QJmYQ9HDhLVkZz/v1Gg/H6lnxZjnO3qsQgBNgS/w==</P><Q>zMu7064pURe5iRVYiQF/YRYwAEUl7JfLJmTe5fDyCqtZbGz9Don4z4itz/Bxr55yAaOj73PSQqB0np0Vs7Bbuw==</Q><DP>QdmhiStK9nTcBVAmUFjyn61XBJWPzlvgpzOMw0JRYXp7vkvgoRX5OKeoS5ZS7qYCACOChIf831P48+TEOUOKdw==</DP><DQ>YePobfBLs4VhnBLl9OcQWfnfC+IBlKuh4UJKASNArrTk05ztAOwWUC0G1+QYk1drKzlq/OQh1tMXq1FutCtSTQ==</DQ><InverseQ>JdXDkfTBRJNTCxTL8EAdKokPsmSR0AdSv3O1AsbTzxT5JQKoxq1mt2tPE2ElWz6dEqHF0Q0Vc1qJ5YjDm0cRdg==</InverseQ><D>styN++Zl1ZYKs4tZzJiO+0mDcwXro5nGAVjoz/NfFJJwM0H0IEdch2Zg9/R8d5sAxUcUb7aSgBkKKjIrjvvNNCi8cCJofSrnJUxAH0JX723OcuCvY1gowNJagoUZIhitK3WVvGSYpSjjKejI26iKn8aKmhC7N8POyTXI/mh6mYU=</D></RSAKeyValue>"
            };
        }

        public static string Separator {
            get { return "┆"; }
        }
    }
}
