using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace Dao.Dynamic {


    public class DynamicDelegateBuilder {

        public const string AssemblyName = "Dao.Dynamic.DynamicDelegate";
        public const string ModuleName = "Dao.Dynamic.DynamicDelegate";
        public const string FileName = "Dao.Dynamic.DynamicDelegate.dll";
        public const string TypeName = "Dao.Dynamic.DynamicDelegate{0}";

        AssemblyName _assemblyName;

        AssemblyBuilder _assemblyBuilder;

        public AssemblyBuilder AssemblyBuilder
        {
            get { return _assemblyBuilder; }
        }

        ModuleBuilder _modelBuilder;

        public ModuleBuilder ModelBuilder
        {
            get { return _modelBuilder; }
        }

        public DynamicDelegateBuilder() {
            _assemblyName = new AssemblyName(AssemblyName);

            _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
               _assemblyName,
               AssemblyBuilderAccess.RunAndSave,
               AppDomain.CurrentDomain.BaseDirectory);

            _modelBuilder = _assemblyBuilder.DefineDynamicModule(ModuleName, FileName);
        }

        public Type CreateType(Type type) {
            if (type.BaseType != typeof(MulticastDelegate))
                throw new ArgumentException("");


            TypeBuilder tb = _modelBuilder.DefineType(
                "Type" + Guid.NewGuid().GetHashCode(),
                TypeAttributes.Public,
                null);

            FieldBuilder fb = tb.DefineField("handler", typeof(Action<object[]>), FieldAttributes.Private);

            BuildConstructor(tb, fb);

            BuildMethod(tb, fb, type);

            return tb.CreateType();
        }

        public object CreateObject(Type type, Action<object[]> action) {
            Type type2 = CreateType(type);
            _assemblyBuilder.Save(FileName);
            return Activator.CreateInstance(type2, action);
        }

        public Delegate CreateDelegate(Type type, Action<object[]> action) {
            object obj = CreateObject(type, action);
            MethodInfo method = obj.GetType().GetMethod("Invoke");
            return Delegate.CreateDelegate(type, method);
        }

        private void BuildMethod(TypeBuilder tb, FieldBuilder fb, Type type) {

            MethodInfo method = type.GetMethod("Invoke");

            Type returnType = method.ReturnType;
            Type[] parameters = method.GetParameters().Select(x => x.ParameterType).ToArray();

            MethodBuilder mb = tb.DefineMethod("Invoke", MethodAttributes.Public, CallingConventions.Standard,
                 returnType, parameters);

            ILGenerator il = mb.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fb);

            il.Emit(OpCodes.Ldc_I4, parameters.Length);
            il.Emit(OpCodes.Newarr, typeof(object[]));

            //for (int i = 0; i < parameters.Length; i++) {
            //    il.Emit(OpCodes.Dup);
            //    il.Emit(OpCodes.Ldc_I4, i);
            //    //il.Emit(OpCodes.Ldc_I4, i + 1);
            //    il.Emit(OpCodes.Ldarg,i+1);
            //    il.Emit(OpCodes.Stelem);
            //}

            il.Emit(OpCodes.Call, typeof(Action<object[]>).GetMethod("Invoke"));

            il.Emit(OpCodes.Ret);
        }

        private void BuildConstructor(TypeBuilder tb, FieldBuilder fb) {
            var cb = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new Type[] { typeof(Action<object[]>) });
            ILGenerator il = cb.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fb);
            il.Emit(OpCodes.Ret);
        }

    }

}
