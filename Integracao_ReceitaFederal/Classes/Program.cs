using System;
using System.Configuration;

namespace Integracao_ReceitaFederal
{
    class Program
    {
        static void Main(string[] args)
        {
            //PEGA OS PARAMETROS DO APP.CONFIG
            string Endereco_API = ConfigurationManager.AppSettings.Get("Endereco_API");
            string Diretorio_In = ConfigurationManager.AppSettings.Get("Diretorio_In");
            string Diretorio_Out = ConfigurationManager.AppSettings.Get("Diretorio_Out");
            string Diretorio_In_log = ConfigurationManager.AppSettings.Get("Diretorio_In_log");
            string Diretorio_Out_log = ConfigurationManager.AppSettings.Get("Diretorio_Out_log");
            string Diretorio_Erro_Tentativas = ConfigurationManager.AppSettings.Get("Diretorio_Erro_Tentativas");
            string Diretorio_Erro_XML = ConfigurationManager.AppSettings.Get("Diretorio_Erro_XML");
            int Tentativas_Falha = Convert.ToInt32(ConfigurationManager.AppSettings.Get("Tentativas_Falha"));
            int Tempo_espera_segs = Convert.ToInt32(ConfigurationManager.AppSettings.Get("Tempo_espera_segs"));

            int tentativas = 0;
            var arquivo = new Arquivo();
            var Qntarquivos = arquivo.QtdArquivosNaPasta(@Diretorio_In);

            while (Qntarquivos > 0)
            {
                Qntarquivos = arquivo.QtdArquivosNaPasta(@Diretorio_In);
                if (Qntarquivos > 0)
                {
                    var datetime = DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss");
                    tentativas = 0;
                    var nome_arquivo_in = arquivo.NomeArquivoOrdenado();
                    var diretorio_arquivo_in = arquivo.RetornaDiretorio();
                    try
                    {
                        var xml = new XML(diretorio_arquivo_in);
                        var inId = xml.RetornaId();
                        var inCnpj = xml.RetornaCnpj();
                        while (tentativas < Tentativas_Falha)
                        {
                            try
                            {
                                var api_receitaws = new API_ReceitaWS();
                                api_receitaws.RequisicaoAPI(Endereco_API, inCnpj);
                                using (var resposta = api_receitaws.DisponibilidadeAPI())
                                {
                                    var post = api_receitaws.RetornoAPIJson();
                                    
                                    if (post.Status == "OK" && (post.Logradouro!="" && post.Bairro!=""))
                                    {
                                        arquivo.CriarArquivo_OutLog(@Diretorio_Out_log, inId, datetime);
                                        var Endereco1 = post.Logradouro;
                                        var Endereco2 = post.Numero + ", " + post.Bairro;
                                        if (post.Complemento != "")
                                        {
                                            Endereco2 = Endereco2 + " - " + post.Complemento;
                                        }
                                        Console.WriteLine("<?xml version=\'1.0\' encoding=\'utf-8\'?>");
                                        Console.WriteLine("<body>");
                                        Console.WriteLine("<dadoscliente>");
                                        Console.WriteLine("<id>" + inId + "</id>");
                                        Console.WriteLine("<cnpj>" + inCnpj + "</cnpj>");
                                        Console.WriteLine("<nome>" + post.Nome + "</nome>");
                                        Console.WriteLine("<endereco1>" + Endereco1 + "</endereco1>");
                                        Console.WriteLine("<endereco2>" + Endereco2 + "</endereco2>");
                                        Console.WriteLine("<status>" + post.Status + "</status>");
                                        Console.WriteLine("</dadoscliente>");
                                        Console.Write("</body>");
                                        arquivo.EncerraCriaArquivo_OutLog();
                                        arquivo.MoveArquivo_InLog(Diretorio_In_log, datetime);
                                        arquivo.CopiaArquivo_Out(Diretorio_Out, inId, datetime);
                                        Console.WriteLine("Sucesso - arquivo: " + nome_arquivo_in + " ID: " + inId);
                                    }
                                    else if (post.Status == "ERROR")
                                    {
                                        ///CNPJ ERRADO///
                                        arquivo.CriarArquivo_OutLog(@Diretorio_Out_log, inId, datetime);
                                        Console.WriteLine("<?xml version=\'1.0\' encoding=\'utf-8\'?>");
                                        Console.WriteLine("<body>");
                                        Console.WriteLine("<dadoscliente>");
                                        Console.WriteLine("<id>" + inId + "</id>");
                                        Console.WriteLine("<cnpj>" + inCnpj + "</cnpj>");
                                        Console.WriteLine("<Status>" + post.Status + "</Status>");
                                        Console.WriteLine("<Message>" + post.Message + "</Message>");
                                        Console.WriteLine("</dadoscliente>");
                                        Console.Write("</body>");
                                        //var email = new Email();
                                        //var titulo = "CNPJ: " + inCnpj + " - não encontrado na Receita Federal";
                                        //var corpo = "Código: " + inId + " CNPJ: " + inCnpj + " Erro: " + post.Message;
                                        //email.EnviaEmail(titulo, corpo);
                                        //arquivo.EncerraCriaArquivo_OutLog();
                                        arquivo.MoveArquivo_InLog(Diretorio_In_log, datetime);
                                        arquivo.CopiaArquivo_Out(Diretorio_Out, inId, datetime);
                                        Console.WriteLine("Sucesso - arquivo: " + nome_arquivo_in + " CNPJ: " + inCnpj + " Motivo: " + post.Message);
                                    }
                                    else
                                    {
                                        /// EMPRESA SEM DADOS
                                        arquivo.MoveArquivo_ErroXML(Diretorio_Erro_XML, datetime);
                                        Console.WriteLine("Erro - Arquivo: " + nome_arquivo_in + " CNPJ: " + inCnpj + "  sem dados na receita.");
                                    }
                                    tentativas = Tentativas_Falha;
                                    System.Threading.Thread.Sleep(Tempo_espera_segs * 1000);
                                }
                            }
                            catch
                            {
                                tentativas++;
                                Console.WriteLine("Falha arquivo: " + nome_arquivo_in + " - Tentativa: " + tentativas + "/" + Tentativas_Falha);
                                System.Threading.Thread.Sleep(Tempo_espera_segs * 1000);
                                if (tentativas == Tentativas_Falha)
                                {
                                    arquivo.MoveArquivo_ErroTentativas(Diretorio_Erro_Tentativas, datetime);
                                    Console.WriteLine("");
                                    Console.WriteLine("Falha no processamento do  arquivo: " + nome_arquivo_in + " ID: " + inId);
                                    tentativas = Tentativas_Falha;
                                }
                            }

                        }
                    }
                    catch
                    {
                        arquivo.MoveArquivo_ErroXML(Diretorio_Erro_XML, datetime);
                        Console.WriteLine("Erro Arquivo: " + nome_arquivo_in + " fora da estrutura padrao do XML");
                    }
                }
                else
                {
                    Console.WriteLine("Sem arquivos na pasta");
                }
            }
            arquivo.MoverTodosArquivo(Diretorio_Erro_Tentativas, Diretorio_In);
        }
    }
}


