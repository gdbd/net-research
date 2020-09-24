using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;

namespace Components
{
    class Program
    {
        static void Main(string[] args)
        {
            var components = new ComponentsContainer();
            components.Print();
            Console.Read();
        }
    }

    public class ComponentsContainer
    {
        public ComponentsContainer()
        {
            // var agrCatalog = new AggregateCatalog();
            var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            // agrCatalog.Catalogs.Add(catalog);

            var container = new CompositionContainer(catalog);

            container.ComposeParts(this);
        }


        [ImportMany("commands", RequiredCreationPolicy = CreationPolicy.NonShared)]
        private IEnumerable<Lazy<IMyComponent, object>> Commands { get; set; }

        [ImportMany("actions", RequiredCreationPolicy = CreationPolicy.NonShared)]
        private IEnumerable<Lazy<IMyComponent, object>> Actions { get; set; }

        internal void Print()
        {
            Console.WriteLine("commands: " + Commands.Aggregate("", (cur, next) => cur + $"{next.Value.Name }; "));
            Console.WriteLine("actions: " + Actions.Aggregate("", (cur, next) => cur + $"{next.Value.Name }; "));
        }
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ComponentAttribute : ExportAttribute
    {
        /// <summary>
        /// Инициализирует экземпляр класса <see cref="ComponentAttribute"/>
        /// </summary>
        public ComponentAttribute()
            : base(typeof(IMyComponent))
        {
        }

        /// <summary>
        /// Инициализирует экземпляр класса <see cref="ComponentAttribute"/>
        /// </summary>
        /// <param name="name">Системное имя компонента</param>
        /// <param name="componentType">Тип компонента</param>
        public ComponentAttribute(string name, Type componentType)
            : base(name, typeof(IMyComponent))
        {
        }

    }

    public interface IMyComponent
    {
        string Name { get; }
    }

    [Component("commands", typeof(IMyComponent))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MyCommand1 : IMyComponent
    {
        public string Name => "MyCommand1";
    }

    [Component("commands", typeof(IMyComponent))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MyCommand2 : IMyComponent
    {
        public string Name => "MyCommand2";
    }

    [Component("actions", typeof(IMyComponent))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MyAction1 : IMyComponent
    {
        public string Name => "MyAction1";
    }
}