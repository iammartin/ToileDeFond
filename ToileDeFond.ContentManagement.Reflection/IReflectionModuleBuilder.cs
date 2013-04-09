using System;
using System.Reflection;

namespace ToileDeFond.ContentManagement.Reflection
{
    public interface IReflectionModuleBuilder
    {
        //Module BuildModuleFromAssembly(Assembly assembly);
        string GetContentTypeNameFromType(Type type);
        string GetModuleNameFromAssembly(Assembly assembly);
        string GetModuleVersionFromAssembly(Assembly assembly);
        Module UpdateModuleFromAssembly(Assembly assembly, Module module = null);
        //ContentType UpdateContentTypeFromType(Module module, Type type);
        
    }
}