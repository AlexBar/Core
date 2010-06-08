﻿namespace ViewModelGenerator
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    public class ViewModelFactory
    {
        private const string Name = "ViewModels";
        private const string AssemblyName = Name + "Assembly";

        private const BindingFlags PropertyBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty;
        private const MethodAttributes PropertyMethodsAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

        private static readonly Type typeType = typeof(Type);
        private static readonly Type objectType = typeof(object);

        private static readonly Type stringType = typeof(string);
        private static readonly Type dateTimeType = typeof(DateTime);
        private static readonly Type decimalType = typeof(decimal);
        private static readonly Type[] extraKnownTypes = new[] { stringType, dateTimeType, decimalType };

        private static readonly Type typeConverterType = typeof(TypeConverterAttribute);
        private static readonly Type entityConverterType = typeof(EntityConverter);

        private static readonly Type viewModelInterfaceType = typeof(IViewModel);

        private static readonly AssemblyBuilder assemblyBuilder = CreateAssemblyBuilder();
        private static readonly ModuleBuilder moduleBuilder = CreateModuleBuilder();

        public void Save()
        {
            assemblyBuilder.Save(assemblyBuilder.GetName().Name + ".dll", PortableExecutableKinds.ILOnly, ImageFileMachine.I386);
        }

        public Type CreateDisplayModel(Type modelType)
        {
            TypeBuilder typeBuilder = CreateTypeBuilder(modelType, "DisplayModel");

            return typeBuilder.CreateType();
        }

        public Type CreateEditModel(Type modelType)
        {
            TypeBuilder typeBuilder = CreateTypeBuilder(modelType, "EditModel");

            return typeBuilder.CreateType();
        }

        private static TypeBuilder CreateTypeBuilder(Type modelType, string suffix)
        {
            string typeName = CreateTypeName(modelType, suffix);

            TypeBuilder typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit, objectType, new[] { viewModelInterfaceType });

            ApplyEntityConverterAttribute(typeBuilder);

            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
            typeBuilder.AddInterfaceImplementation(viewModelInterfaceType);

            AddPremitiveTypeProperties(typeBuilder, modelType);

            return typeBuilder;
        }

        private static void ApplyEntityConverterAttribute(TypeBuilder typeBuilder)
        {
            Type[] ctorParameters = new[] { typeType };
            ConstructorInfo ctor = typeConverterType.GetConstructor(ctorParameters);

            CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder(ctor, new object[] { entityConverterType });

            typeBuilder.SetCustomAttribute(attributeBuilder);
        }

        private static void AddPremitiveTypeProperties(TypeBuilder typeBuilder, IReflect modelType)
        {
            foreach (PropertyInfo property in modelType.GetProperties(PropertyBindingFlags).Where(p => IsKnownType(p.PropertyType)))
            {
                Type propertyType = property.PropertyType;
                string propertyName = property.Name;

                FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.PrivateScope | FieldAttributes.Private);

                string getName = "get_" + propertyName;

                MethodBuilder getMethodBuilder = typeBuilder.DefineMethod(getName, PropertyMethodsAttributes, propertyType, Type.EmptyTypes);
                ILGenerator getMethodIL = getMethodBuilder.GetILGenerator();

                getMethodIL.Emit(OpCodes.Ldarg_0);
                getMethodIL.Emit(OpCodes.Ldfld, fieldBuilder);
                getMethodIL.Emit(OpCodes.Ret);

                string setName = "set_" + propertyName;

                MethodBuilder setMethodBuilder = typeBuilder.DefineMethod(setName, PropertyMethodsAttributes, null, new[] { propertyType });
                ILGenerator setMethodIL = setMethodBuilder.GetILGenerator();

                setMethodIL.Emit(OpCodes.Ldarg_0);
                setMethodIL.Emit(OpCodes.Ldarg_1);
                setMethodIL.Emit(OpCodes.Stfld, fieldBuilder);
                setMethodIL.Emit(OpCodes.Ret);

                PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, propertyType, new[] { propertyType });

                propertyBuilder.SetGetMethod(getMethodBuilder);
                propertyBuilder.SetSetMethod(setMethodBuilder);

                if (propertyName.Equals("Id"))
                {
                    MethodInfo convertMethod = typeof(Convert).GetMethods().First(m => m.Name.Equals("ToString") && m.GetParameters()[0].ParameterType.Equals(propertyType));

                    MethodBuilder methodBuilder = typeBuilder.DefineMethod("ToString", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, CallingConventions.Standard, stringType, Type.EmptyTypes);
                    ILGenerator methodIL = methodBuilder.GetILGenerator();

                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Ldfld, fieldBuilder);
                    methodIL.Emit(OpCodes.Call, convertMethod);
                    methodIL.Emit(OpCodes.Ret);
                }
            }
        }

        private static bool IsKnownType(Type type)
        {
            Type actualType = Nullable.GetUnderlyingType(type) ?? type;

            return actualType.IsPrimitive || extraKnownTypes.Contains(actualType);
        }

        private static string CreateTypeName(Type modelType, string suffix)
        {
            return modelType.FullName.Replace(".", "$") + "$" + suffix;
        }

        private static ModuleBuilder CreateModuleBuilder()
        {
            ModuleBuilder builder = assemblyBuilder.DefineDynamicModule(assemblyBuilder.GetName().Name, AssemblyName + ".dll");

            return builder;
        }

        private static AssemblyBuilder CreateAssemblyBuilder()
        {
            AssemblyName assemblyName = new AssemblyName(AssemblyName)
                                            {
                                                Version = typeof(ViewModelFactory).Assembly.GetName().Version
                                            };

            AssemblyBuilder builder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);

            return builder;
        }
    }
}