using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LuisBot.Table
{
    /// <summary>
    /// Classe de logger, que registra no mesmo repositorio das configuracoes
    /// </summary>
    
    public class Logger
    {
        #region Singleton
        //instancia
        private static Logger instance;

        //instancia do logger
        public static Logger Instance
        {
            get
            {
                if (instance == null)
                    instance = new Logger();
                return instance;
            }
        }

        #endregion

        //Chave do Web.Config com as configuracoes do STORAGE
        private readonly string azureWebConfigStorage = ConfigurationManager.AppSettings["AzureWebConfigStorage"];
       
        //Wrapper que trata a tabela
        private readonly ITableWrapper table;

        //ctor
        private Logger()
        {
            //cria o wrapper da tabela
            table = new TableWrapper(azureWebConfigStorage);
        }


        public async Task LogarLuisQna(  string luisAppID, 
                            object context, 
                            object result, 
                            string qnaMakerAnswer)
        {            
            try
            {
                LogEntity log = new LogEntity(luisAppID);

                if (context != null)
                    log.Context = JsonConvert.SerializeObject(context);

                if (result != null)
                    log.LuisResult = JsonConvert.SerializeObject(result);

                if (!String.IsNullOrWhiteSpace(qnaMakerAnswer))
                    log.QnAResult = qnaMakerAnswer;
                await table.Insert(log);
                log = null;
            }
            catch (Exception)
            {
                //depois colocr handle para gravar em outro lugar
            }
        }
    }
}