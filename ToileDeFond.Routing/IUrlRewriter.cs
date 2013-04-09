using System.Web;

namespace ToileDeFond.Routing
{
    public interface IUrlRewriter
    {
        void UrlRewrite(HttpApplication httpApplication);
    }

    //TODO: Faire le menu du site (dans _layout.html) à l'aide des routes
    //Je crois que les routes ne sont pas suffisantes pour faire un menu
    //Un object menu, qui contiendrait des menuitem qui pointerait vers des routes ou url externe, etc.
}
