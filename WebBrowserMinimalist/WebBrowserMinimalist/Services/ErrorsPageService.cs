using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace WebBrowserMinimalist.Services
{
    public class ErrorsPageService
    {
        Dictionary<CoreWebView2WebErrorStatus, string> ErrorToPages = new Dictionary<CoreWebView2WebErrorStatus, string>() {
            { CoreWebView2WebErrorStatus.CannotConnect, "" },
            { CoreWebView2WebErrorStatus.CertificateCommonNameIsIncorrect, "" },
        };

        Dictionary<CoreWebView2WebErrorStatus, SymbolRegular> erroToIcon = new Dictionary<CoreWebView2WebErrorStatus, SymbolRegular>() {
           
            { CoreWebView2WebErrorStatus.Unknown, SymbolRegular.Shield24 },
            {CoreWebView2WebErrorStatus.CannotConnect, SymbolRegular.PlugDisconnected24 },
            { CoreWebView2WebErrorStatus.CertificateCommonNameIsIncorrect, SymbolRegular.ShieldError24 },
            { CoreWebView2WebErrorStatus.CertificateExpired, SymbolRegular.ShieldError24 },
            { CoreWebView2WebErrorStatus.CertificateIsInvalid, SymbolRegular.ShieldError24 },
            { CoreWebView2WebErrorStatus.CertificateRevoked, SymbolRegular.ShieldError24 },
            { CoreWebView2WebErrorStatus.ClientCertificateContainsErrors, SymbolRegular.ShieldError24 },
            { CoreWebView2WebErrorStatus.ConnectionAborted, SymbolRegular.PlugDisconnected24 },
            { CoreWebView2WebErrorStatus.ConnectionReset, SymbolRegular.Connected20 },
            { CoreWebView2WebErrorStatus.Disconnected, SymbolRegular.PlugDisconnected24 },
            { CoreWebView2WebErrorStatus.ErrorHttpInvalidServerResponse, SymbolRegular.ChatDismiss24 },
            { CoreWebView2WebErrorStatus.HostNameNotResolved, SymbolRegular.DocumentDismiss24 },
            { CoreWebView2WebErrorStatus.OperationCanceled, SymbolRegular.Square24 },
            { CoreWebView2WebErrorStatus.RedirectFailed, SymbolRegular.ArrowReset24 },
            { CoreWebView2WebErrorStatus.Timeout, SymbolRegular.TimerOff24},
            { CoreWebView2WebErrorStatus.UnexpectedError, SymbolRegular.Question24 },
            { CoreWebView2WebErrorStatus.ValidAuthenticationCredentialsRequired, SymbolRegular.PersonProhibited28 },
            { CoreWebView2WebErrorStatus.ValidProxyAuthenticationRequired, SymbolRegular.Server24 },
            

        };

     
        public SymbolRegular GetSymbol(CoreWebView2WebErrorStatus webErrorStatus)
        {
            if (!erroToIcon.ContainsKey(webErrorStatus)) return SymbolRegular.Shield24;
            else
                return erroToIcon.GetValueOrDefault(webErrorStatus);
        }
    }
}
