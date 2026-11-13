using System;
using System.Collections.Generic;

namespace IlInterpreter
{
    public interface IScript
    {
        string Name { get; }
        ReadOnlyMemory<byte> Il { get; }
    }

    public class ScriptValidationException : Exception
    {
        public ScriptValidationException(string message) : base(message) { }
    }

    public class ScriptRuntimeException : Exception
    {
        public ScriptRuntimeException(string message) : base(message) { }
    }
}

namespace IlInterpreter.Interpreter
{
    public class HostBinding
    {
        public static Action<string> Warn;
        private readonly List<Type> m_RegisteredTypes = new List<Type>();
        public IEnumerable<Type> RegisteredTypes => m_RegisteredTypes;
        public Func<string, Type> AutoBindResolver { get; set; }
        public HostBinding AllowBcl() => this;
        public HostBinding AllowType(Type t) { if (t != null) m_RegisteredTypes.Add(t); return this; }
        public HostBinding AllowType<T>() => AllowType(typeof(T));
        public HostBinding AllowTypeStruct<T>() => AllowType(typeof(T));
        public HostBinding Allow(string typeName, string memberName, Func<object, object[], object> invoke, int arity) => this;
        public HostBinding AllowConstructor(string typeName, Func<object, object[], object> invoke, int arity) => this;
        public HostBinding AllowNonPublicInstanceMembers(Type t) => this;
    }
}
