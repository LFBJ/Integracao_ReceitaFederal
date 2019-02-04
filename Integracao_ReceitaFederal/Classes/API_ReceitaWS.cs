using Newtonsoft.Json;
using System.IO;
using System.Net;


namespace Integracao_ReceitaFederal
{
    class API_ReceitaWS
    {
        public string Nome { get; set; }
        public string Bairro { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string Cnpj { get; set; }
        public string Motivo_situacao { get; set; }
        private WebResponse Disponibilidade { get; set; }


        public void RequisicaoAPI(string Endereco_API, string inCnpj)
        {
            var requisicaoWeb = WebRequest.CreateHttp(Endereco_API + inCnpj);
            requisicaoWeb.Method = "GET";
            requisicaoWeb.UserAgent = "RequisicaoWeb";
            Disponibilidade = requisicaoWeb.GetResponse();
        }
        public WebResponse DisponibilidadeAPI()
        {
            return Disponibilidade;
        }
        public API_ReceitaWS RetornoAPIJson()
        {
            var streamDados = Disponibilidade.GetResponseStream();
            StreamReader readerapi = new StreamReader(streamDados);
            object objResponse = readerapi.ReadToEnd();
            var post = JsonConvert.DeserializeObject<API_ReceitaWS>(objResponse.ToString());
            streamDados.Close();
            Disponibilidade.Close();
            return post;
        }

    }
}
