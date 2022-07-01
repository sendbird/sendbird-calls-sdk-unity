// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using System.Reflection;

namespace Sendbird.Calls
{
    internal abstract class SingletonAbstract<TClassType> where TClassType : class
    {
        private static TClassType _internalInstance = null;

        internal static TClassType Instance
        {
            get
            {
                if (_internalInstance == null)
                    CreateInstance();

                return _internalInstance;
            }
        }

        private static void CreateInstance()
        {
            if (_internalInstance != null)
                return;

            _internalInstance = (TClassType)Activator.CreateInstance(typeof(TClassType), true);
            ConstructorInfo[] constructorInfos = typeof(TClassType).GetConstructors();
            if (0 < constructorInfos.Length) Logger.LogError(Logger.CategoryType.Common, $"you must be implement private constructor:{typeof(TClassType).Name}");
        }
    }
}