using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using AriaPM.Models;
using AriaPM.Services;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AriaPM.ViewModels
{
    public class LoginViewModel
    {
        public IAuthenticaiton _service => DependencyService.Get<IAuthenticaiton>() ?? new Authentication();

        public async Task<int?> IsAuthenticated(string userId, string password)
        {
            int? result = null;
            try
            {
                result = await _service.IsAuthenticated(userId, password);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return result;
        }
    }
}