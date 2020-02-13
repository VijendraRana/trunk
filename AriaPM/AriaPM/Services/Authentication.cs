using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;

namespace AriaPM.Services
{
    public class Authentication : IAuthenticaiton
    {
        public async Task<int?> IsAuthenticated(string userId, string password)
        {
            int? result = null;
            DBConnection dBConnection = new DBConnection();
            ObservableCollection<Params> paramList = new ObservableCollection<Params>();
            Params param = new Params();
            param.Name = "username";
            param.Value = userId;
            paramList.Add(param);
            param = new Params();
            param.Name = "password";
            param.Value = password;
            paramList.Add(param);
            var queryResult = dBConnection.GetQueryResult("SELECT is_admin FROM store_name where store_name = @username AND password = @password;", paramList);

            if (queryResult != null && ((DataSet)queryResult).Tables != null)
            {
                DataTable dataTable = ((DataSet)queryResult).Tables[0];
                if (dataTable.Rows.Count > 0)
                    result = Convert.ToInt32(dataTable.Rows[0]["is_admin"]);
            }
            return await Task.FromResult(result);
        }
    }
}
