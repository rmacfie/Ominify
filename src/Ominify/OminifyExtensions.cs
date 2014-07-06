using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ominify
{
    public static class OminifyExtensions
    {
        public static Action<Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>> UseOminifier(
            this Action<Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>> app, Action<OminifyOptions> withOptions)
        {
            var options = new OminifyOptions();
            withOptions.Invoke(options);
            return app.UseOminifier(options);
        }

        public static Action<Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>> UseOminifier(
            this Action<Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>> app, OminifyOptions options)
        {
            Ominifier.Initialize(options);
            app(next => new Ominifier(next).Invoke);
            return app;
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