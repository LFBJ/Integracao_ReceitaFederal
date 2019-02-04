# Integracao_ReceitaFederal
Integração entre XML com a API ReceitaWS para retornar dados da empresa baseados no CNPJ

#Para utilização, basta criar um arquivo .xml na pasta "In" com as informações abaixo

<?xml version='1.0' encoding='utf-8'?>
<body>
<dadoscliente>
<id></id> // APENAS NECESSARIO SE VOCE FOR ATUALIZAR SEU SISTEMA COM BASE NO RETORNO DO XML DA RECEITA FEDERAL.
<cnpj></cnpj> //DIGITE O CNPJ AQUI DO CLIENTE
</dadoscliente>
</body>
#Alterar os parametros de email no App.config

#Executar a aplicação
