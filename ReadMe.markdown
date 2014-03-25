# Ominify #

Simple minification middleware for OWIN.


### Usage ###

The following assumes that you want the YuiCompressor to handle the minification.

First, install the Ominify.Yui Nuget package.

    Install-Package Ominify.Yui

Secondly, configure the Ominifier in your OWIN startup class.

    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            appBuilder
                .UseOminifier(x =>
                {
                    x.EnableBundling = true;
                    x.AutoRefreshOnFileChanges = true;
                    x.MinifyBundles = true;

                    x.AddPackage(new YuiCssPackage("/content/css/minified.css").With(
                        "/content/css/normalize-3.0.0.css", // Actual files
                        "/content/css/font-awesome-4.0.3.css",
                        "/content/css/layout.css"
                        ));
                    
                    x.AddPackage(new YuiJsPackage("/content/js/minified.js").With(
                        "/content/js/jquery-2.1.0.js",
                        "/content/js/angular-1.2.0.js"
                        ));
                })
                .UseNancy();
        }
    }

Then, you can get an url to the package by calling Ominifier.GetUrl(packagePath). The url will have a timestamp corresponding to the last modification time of any of the included files. When a browser calls the url, Ominify will return all the included files, minified and combined into one result.

    var url = Ominifier.GetUrl("/content/css/minified.css");
    // returns: /content/css/minified.css?t=986439696586564

Or, get the whole element with Ominifier.GetElement(packagePath):

    var htmlElement = Ominifier.GetElement("/content/css/minified.css");
    // returns: <link rel="stylesheet" href="/content/css/minified.css?t=986439696586564" />

In a Razor view (using NancyFx for example) you can output the element like so:

    @Html.Raw(Ominifier.GetElement("/content/css/minified.css"))
    
With NancyFx specifically (haven't tried other frameworks) you need to have the following in your web.config so that the view engine can find Ominify:

    <system.web.webPages.razor>
        <pages>
            <namespaces>
                <add namespace="Ominify" />
            </namespaces>
        </pages>
    </system.web.webPages.razor>
    
    <razor>
        <assemblies>
            <add assembly="Ominify.Core" />
        </assemblies>
        <namespaces>
            <add namespace="Ominify" />
        </namespaces>
    </razor>
    
    
    