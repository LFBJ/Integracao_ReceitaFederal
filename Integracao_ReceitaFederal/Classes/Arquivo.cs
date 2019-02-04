using System;
using System.IO;
using System.Linq;

namespace Integracao_ReceitaFederal
{
    class Arquivo
    {
        
        private string diretorio_arquivo_out_log { get; set; }
        private string diretorio_arquivo_erro_xml { get; set; }
        private string diretorio_arquivo_erro_tentativas { get; set; }
        private string diretorio_arquivo_out { get; set; }
        private string diretorio_arquivo_in { get; set; }
        private string diretorio_arquivo_in_log { get; set; }
        private string nome_arquivo_in { get; set; }
        private FileInfo[] arquivosnapasta { get; set; }
        private StreamWriter writer { get; set; }
        private TextWriter oldOut { get; set;}



        public int QtdArquivosNaPasta(string Diretorio)
        {
            var diretorio = new System.IO.DirectoryInfo(@Diretorio);
            arquivosnapasta = diretorio.GetFiles();
            return arquivosnapasta.Length;
        }
        public string NomeArquivoOrdenado()
        {
            var arquivosOrdenados = arquivosnapasta.OrderBy(x => x.CreationTime);
            var primeiroArquivo = arquivosOrdenados.First();
            diretorio_arquivo_in = primeiroArquivo.FullName;
            nome_arquivo_in = Path.GetFileNameWithoutExtension(primeiroArquivo.FullName);
            return nome_arquivo_in;
        }
        public string RetornaDiretorio()
        {
            return diretorio_arquivo_in;
        }
        public void CriarArquivo_OutLog(string Diretorio_Out_log, string inId, string datetime)
        {
            oldOut = Console.Out;
            diretorio_arquivo_out_log = (@Diretorio_Out_log + inId + "_" + datetime + ".xml");
            writer = new StreamWriter(diretorio_arquivo_out_log);
            Console.SetOut(writer);
        }
        public void EncerraCriaArquivo_OutLog()
        {
            Console.SetOut(oldOut);
            writer.Close();
        }
        public void MoveArquivo_InLog(string Diretorio_In_log, string datetime)
        {
            diretorio_arquivo_in_log = (@Diretorio_In_log + nome_arquivo_in + "_" + datetime + ".xml");
            File.Move(diretorio_arquivo_in, diretorio_arquivo_in_log);
        }
        public void CopiaArquivo_Out(string Diretorio_Out, string inId, string datetime)
        {
            diretorio_arquivo_out = (@Diretorio_Out + inId + "_" + datetime + ".xml");
            File.Copy(diretorio_arquivo_out_log, diretorio_arquivo_out);
        }
        public void MoveArquivo_ErroTentativas(string Diretorio_Erro_Tentativas,string datetime)
        {
            diretorio_arquivo_erro_tentativas = (@Diretorio_Erro_Tentativas + nome_arquivo_in + "_" + datetime + ".xml");
            File.Move(diretorio_arquivo_in, diretorio_arquivo_erro_tentativas);
        }
        public void MoveArquivo_ErroXML(string Diretorio_Erro_XML, string datetime)
        {
            diretorio_arquivo_erro_xml = (@Diretorio_Erro_XML + nome_arquivo_in + "_" + datetime + ".xml");
            File.Move(diretorio_arquivo_in, diretorio_arquivo_erro_xml);
        }
        public void MoverTodosArquivo(string origem, string destino)
        {
            DirectoryInfo dir = new DirectoryInfo(@origem);
            foreach (FileInfo f in dir.GetFiles("*.xml"))
            {
                File.Move(f.FullName, destino + f.Name);
            }
        }
    }
}
