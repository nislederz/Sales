[assembly: Xamarin.Forms.Dependency(typeof(Sales.iOS.Implementation.Localize))]

namespace Sales.iOS.Implementation
{
    using System.Globalization;
    using System.Threading;
    using Foundation;
    using Helpers;
    using Interfaces;

    public class Localize : ILocalize
    {
        public CultureInfo GetCurrentCultureInfo()
        {
            var netLanguage = "en"; if (NSLocale.PreferredLanguages.Length > 0) {
                var pref = NSLocale.PreferredLanguages[0];
                netLanguage = iOSToDotnetLanguage(pref);
            }
            System.Globalization.CultureInfo ci =  null ;
            try
            {
                ci =  new  System.Globalization.CultureInfo(netLanguage);
            }
            catch  (CultureNotFoundException e1)
            {                                                
                 try         
                 {
                    var fallback = ToDotnetFallbackLanguage(new PlatformCulture(netLanguage));
                    ci = new System.Globalization.CultureInfo(fallback);
                 }catch (CultureNotFoundException e2)
                 {                                         
                    ci =  new  System.Globalization.CultureInfo( "en" );
                 }
            }
            return  ci;
        } 

        public void SetLocale(CultureInfo ci) {
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        string iOSToDotnetLanguage(string iOSLanguage)
        {
            var netLanguage = iOSLanguage;
            switch (iOSLanguage)
            {
                case "ms-MY":
                case "ms-SG":
                    netLanguage = "ms";
                    break;
                case "gsw-CH":
                    netLanguage = "de-CH";
                    break;
            }

            return netLanguage;
        }

        string ToDotnetFallbackLanguage(PlatformCulture platCulture){
             var netLanguage = platCulture.LanguageCode;               
             switch  (platCulture.LanguageCode)
             {
                case   "pt" :
                    netLanguage =  "pt-PT" ;
                break ;
                case   "gsw" :
                    netLanguage =  "de-CH" ;                       
                break ;                                                      
             } 
             return netLanguage;
        }
    }
}