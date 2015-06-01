using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;

namespace StackOverflow_29494255
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var container = new UnityContainer();

            UnityRegistration(container);

            var normalModelController = container.Resolve<NormalModelController>();
            var specialModelController = container.Resolve<SpecialModelController>();
            var anotherModelController = container.Resolve<AnotherModelController>();
        }

        private static void UnityRegistration(IUnityContainer container)
        {
            container.RegisterType(
                typeof(IDataTransformerSelector<,>), 
                typeof(DataTransformerSelector<,>));
            container.RegisterType(
                typeof(IDataTransformerManager<,>),
                typeof(DataTransformerManager<,>));

            container.RegisterType<IDataTransformer<EntitiesModel>, TrackCreatedTransformer>
                ("TrackCreatedTransformer");
            container.RegisterType<IDataTransformer<EntitiesModel>, TrackChangedTransformer>
                ("TrackChangedTransformer");
            container.RegisterType<IDataTransformer<EntitiesModel>, TrackClosedTransformer>
                ("TrackClosedTransformer");
            container.RegisterType<IDataTransformer<EntitiesModel>, TrackSignedTransformer>
                ("TrackSignedTransformer");
        }
    }

    public interface IDataTransformerManager<TModel, TContext> {}
    public class DataTransformerManager<TModel, TContext> : 
        IDataTransformerManager<TModel, TContext>,
        IDataTransformer<TContext>
        where TContext : OpenAccessContext, new()
    {
        private readonly IEnumerable<IDataTransformer<TContext>> _transformers;

        public DataTransformerManager(
            IDataTransformerSelector<TModel, TContext> dataTransformerSelector, 
            params IDataTransformer<TContext>[] allTransformers)
        {
            _transformers = dataTransformerSelector.SelectTransformers(allTransformers);
        }
    }

    public interface IDataTransformerSelector<TModel, TContext>
        where TContext : OpenAccessContext, new()
    {
        IEnumerable<IDataTransformer<TContext>> 
            SelectTransformers(IEnumerable<IDataTransformer<TContext>> allTransformers);
    }

    public class DataTransformerSelector<TModel, TContext> :
        IDataTransformerSelector<TModel, TContext>
        where TContext : OpenAccessContext, new()
    {
        public IEnumerable<IDataTransformer<TContext>> 
            SelectTransformers(IEnumerable<IDataTransformer<TContext>> allTransformers)
        {
            // Custom logic to map the model type to the transformers that apply
            if (typeof(TModel) == typeof(NormalModel))
            {
                return allTransformers.Where(transformer =>
                    transformer.GetType() == typeof(TrackCreatedTransformer) ||
                    transformer.GetType() == typeof(TrackChangedTransformer)).ToArray();
            }
            
            if (typeof(TModel) == typeof(SpecialModel))
            {
                return allTransformers.Where(transformer =>
                    transformer.GetType() == typeof(TrackCreatedTransformer) ||
                    transformer.GetType() == typeof(TrackChangedTransformer) ||
                    transformer.GetType() == typeof(TrackClosedTransformer)).ToArray();
            }
            
            if (typeof (TModel) == typeof (AnotherModel))
            {
                return allTransformers.Where(transformer =>
                    transformer.GetType() == typeof (TrackCreatedTransformer) ||
                    transformer.GetType() == typeof (TrackChangedTransformer) ||
                    transformer.GetType() == typeof (TrackClosedTransformer) ||
                    transformer.GetType() == typeof (TrackSignedTransformer)).ToArray();
            }

            throw new InvalidOperationException("Unknown model type: " + typeof(TModel));
        }
    }

    public class NormalModelController
    {
        public NormalModelController(
            IDataTransformerManager<NormalModel, EntitiesModel> dataTransformerManager)
        {}
    }

    public class SpecialModelController
    {
        public SpecialModelController(
            IDataTransformerManager<SpecialModel, EntitiesModel> dataTransformerManager)
        {}
    }

    public class AnotherModelController 
    {
        public AnotherModelController(
            IDataTransformerManager<AnotherModel, EntitiesModel> dataTransformerManager)
        {}
    }

    public interface IDataTransformer<TContext> { }
    public class TrackCreatedTransformer : IDataTransformer<EntitiesModel> { }
    public class TrackChangedTransformer : IDataTransformer<EntitiesModel> { }
    public class TrackClosedTransformer : IDataTransformer<EntitiesModel> { }
    public class TrackSignedTransformer : IDataTransformer<EntitiesModel> { }

    public class OpenAccessContext { }
    public class EntitiesModel : OpenAccessContext { }

    public class NormalModel { }
    public class SpecialModel { }
    public class AnotherModel { }
}
