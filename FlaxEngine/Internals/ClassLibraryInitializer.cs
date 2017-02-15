// Flax Engine scripting API

using System;
using System.Runtime.CompilerServices;

namespace FlaxEngine
{
    internal static class ClassLibraryInitializer
    {
        private static void Init()
        {
            UnhandledExceptionHandler.RegisterUECatcher();
            FlaxLogWriter.Init();
            GUI.Style.Current = GUI.Style.CreateDefault();
        }
    }
}