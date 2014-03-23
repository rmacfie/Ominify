namespace Ominify
{
    using System;
    using Owin;

    public static class OminifyExtensions
    {
        public static IAppBuilder UseOminifier(this IAppBuilder app, Action<OminifyOptions> withOptions)
        {
            var options = new OminifyOptions();
            withOptions.Invoke(options);
            return app.UseOminifier(options);
        }

        public static IAppBuilder UseOminifier(this IAppBuilder app, OminifyOptions options)
        {
            Ominifier.Initialize(options);
            return app.Use(typeof(Ominifier));
        }

        public static T With<T>(this T package, string filePath) where T : OminifyPackage
        {
            package.AddFilePath(filePath);
            return package;
        }

        public static T With<T>(this T package, params string[] filePaths) where T : OminifyPackage
        {
            package.AddFilePaths(filePaths);
            return package;
        }
    }
}