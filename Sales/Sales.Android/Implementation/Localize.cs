[assembly: Xamarin.Forms.Dependency(typeof(Sales.Droid.Implementation.Localize))]
namespace Sales.Droid.Implementation
{
    using System.Globalization;
    using System.Threading;
    using Helpers;
    using Interfaces;

    public class Localize : ILocalize
    {
        public CultureInfo GetCurrentCultureInfo()
        {
            var netLanguage = "en";
            var androidLocale = Java.Util.Locale.Default;
            netLanguage = AndroidToDotnetLanguage(androidLocale.ToString().Replace("_", "-"));
            System.Globalization. CultureInfo  ci =  null ;
            try
            {
                ci =  new  System.Globalization. CultureInfo (netLanguage);
            }
            catch  ( CultureNotFoundException  e1)
            {
                try
                {
                    var  fallback = ToDotnetFallbackLanguage( new   PlatformCulture (netLanguage));
                    ci =  new  System.Globalization. CultureInfo (fallback);
                }                  catch  ( CultureNotFoundException  e2)
                {
                    ci =  new  System.Globalization. CultureInfo ( "en" );
                }
            }
            return  ci;
        } 

        public void SetLocale(CultureInfo ci) {
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        string AndroidToDotnetLanguage(string androidLanguage)
        {
            var netLanguage = androidLanguage;
            switch  (androidLanguage)
            {
                case   "ms-BN" :    // "Malaysian (Brunei)" not supported .NET culture                  
                case   "ms-MY" :    // "Malaysian (Malaysia)" not supported .NET culture                  
                case   "ms-SG" :    // "Malaysian (Singapore)" not supported .NET culture                     
                    netLanguage =  "ms" ;  // closest supported                      
                break ;
                case   "in-ID" :   // "Indonesian (Indonesia)" has different code in  .NET                     
                    netLanguage =  "id-ID" ;  // correct code for .NET                      
                break ;
                case   "gsw-CH" :   // "Schwiizertüütsch (Swiss German)" not supported .NET culture                     
                    netLanguage =  "de-CH" ;  // closest supported                      
                break ;
            }
            return  netLanguage;
        } 

        string ToDotnetFallbackLanguage(PlatformCulture platCulture)
        {
            var netLanguage = platCulture.LanguageCode;              
            switch  (platCulture.LanguageCode)
            {
                case   "gsw" :
                    netLanguage =  "de-CH" ;
                break ;
            }
            return  netLanguage;
        }
    }

}