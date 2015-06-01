using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace StackOverflow_30243034
{
    class Program
    {
        static void Main(string[] args)
        {
            //global::AutoMapper.Configuration;
            AutoMapper.Configuration.CreateTypeMapExpression;

            ObjectFactory.Initialize(init =>
            {
                init.AddRegistry<ConfigurationRegistry>();
            });

            var configuration1 = ObjectFactory.GetInstance<IConfiguration>();
            var configuration2 = ObjectFactory.GetInstance<IConfiguration>();
            configuration1.ShouldBeTheSameAs(configuration2);

            var configurationProvider = ObjectFactory.GetInstance<IConfigurationProvider>();
            configurationProvider.ShouldBeTheSameAs(configuration1);

            var configuration = ObjectFactory.GetInstance<Configuration>();
            configuration.ShouldBeTheSameAs(configuration1);

            configuration1.CreateMap<Source, Destination>();

            var engine = ObjectFactory.GetInstance<IMappingEngine>();

            var destination = engine.Map<Source, Destination>(new Source { Value = 15 });

            destination.Value.ShouldEqual(15);
        }
    }
}
