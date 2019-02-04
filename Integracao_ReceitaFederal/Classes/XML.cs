using System;
using System.Xml;

namespace Integracao_ReceitaFederal
{
    class XML
    {
        private string Id { get; set; }
        private string Cnpj { get; set; }

        public XML(string diretorio)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(diretorio);
            Id = Convert.ToString(doc.SelectSingleNode("body/dadoscliente/id").InnerText);
            Cnpj = Convert.ToString(doc.SelectSingleNode("body/dadoscliente/cnpj").InnerText);
        }
        public string RetornaCnpj()
        {
            Cnpj = Cnpj.Replace(".", "");
            Cnpj = Cnpj.Replace("/", "");
            Cnpj = Cnpj.Replace("-", "");
            return Cnpj;
        }
        public string RetornaId()
        {
            return Id;
        }
    }
}
