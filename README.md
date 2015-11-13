# ikende.data
asp.net mvc data binding



   public class ModelBinder : IModelBinder
    {
    
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType == typeof(string) || bindingContext.ModelType.IsValueType)
                return IKende.Data.BinderHelper.GetValue(bindingContext.ModelType, bindingContext.ModelName);
            return IKende.Data.BinderHelper.GetObject(bindingContext.ModelType, null);
        }
    }
    public class ModelBinderProvider : IModelBinderProvider
    {
    
        public IModelBinder GetBinder(Type modelType)
        {
            return new ModelBinder();
        }
    }
    
    protected void Application_Start(){
    
    ModelBinderProviders.BinderProviders.Add(new Codes.ModelBinderProvider());
    
    }
