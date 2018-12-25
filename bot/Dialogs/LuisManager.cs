using LuisBot.Table;
using LuisBot.Table.Luis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LuisBot.Dialogs
{
    /// <summary>
    /// Classe que gerencia as configuracoes do LUIS
    /// </summary>
    [Serializable]
    public class LuisManager
    {
        //Chave do Web.Config com as configuracoes do STORAGE
        private readonly string azureWebConfigStorage = ConfigurationManager.AppSettings["AzureWebConfigStorage"];

        //Wrapper que trata a tabela
        private readonly ITableWrapper table;
        
        /// <summary>
        /// ctor
        /// </summary>
        public LuisManager()
        {
            //cria o wrapper da tabela
            table = new TableWrapper(azureWebConfigStorage);

            //cria o dicionario de servicos QnA
            QnaService = new Dictionary<string, QnAMakerService>();
        }

        /// <summary>
        /// Nome do LUIS
        /// </summary>
        public string LuisNome { get; set; }

        /// <summary>
        /// AppID do LUIS
        /// </summary>
        public string LuisAppId { get; set; }

        /// <summary>
        /// KeyID do LUIS
        /// </summary>
        public string LuisApiKey { get; set; }

        /// <summary>
        /// Host Region do LUIS
        /// </summary>
        public string LuisHostRegion { get; set; }

        /// <summary>
        /// Dicionario com todas as bases QnA desse LUIS
        /// </summary>
        public Dictionary<string, QnAMakerService> QnaService { get; set; }
        
        /// <summary>
        /// Inicializador do gerenciador do LUIS
        /// </summary>
        /// <returns></returns>
        public async Task InitAsync()
        {
            //seta o AppID
            LuisAppId = ConfigurationManager.AppSettings["LuisAppId"];

            //Testa se existe
            if (String.IsNullOrEmpty(LuisAppId))
                throw new ArgumentNullException("LuisAppId", "Cahve do web.config LuisAppId nao prenchida");

            //recuera a entidade do tables com as configuracoes do LUIS
            var luisEntity = await table.Get<LuisEntity>(LuisEntity.STR_PARTITION_KEY, LuisAppId);

            //verifica se existe
            if (luisEntity == null)
                throw new ArgumentNullException("luisEntity", "Nao existe configuracao para esse AppId do LUIS");

            //recupera o NOME
            LuisNome = luisEntity.Nome;

            //Recupera a App Key
            LuisApiKey = luisEntity.ApiKey;

            //Recupera ao Host Region
            LuisHostRegion = luisEntity.HostRegion;

            //recupera as KB relacionadas a esse LUIS AppID
            var luisKBs = await table.List<QnAKBEntity>(LuisAppId);

            //percorre a lista de KBs
            foreach (var luisKB in luisKBs)
            {
                //Adiciona ao dicionario os servicos
                QnaService.Add
                (
                    luisKB.LuisIntent,
                    new QnAMakerService
                    (
                        "https://" + luisKB.QnADomain + ".azurewebsites.net",
                        luisKB.QnAKBID,
                        luisKB.QnAEndPointKey
                    )
                );
            }
        }
    }
}