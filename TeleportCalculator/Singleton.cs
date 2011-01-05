using System;
using System.Reflection;
using System.IO;

namespace TeleportCalculator
{
    /// <summary>
    /// ��������.
    /// </summary>
    /// <typeparam name="T">����������� �����-��������.
    /// ������ ���� ����������� ������� ������.</typeparam>
    public class Singleton<T> where T : class
    {
        private static T _self = null;

		protected Singleton()
		{
		}
		//protected Singleton(object data)
		//{
		//}

        ~Singleton()
        {
            _self = null;
        }

        public static T Instance()
        {
            Type baseType = typeof(T).BaseType;

            // �������������� ����� ������ ���� �������� (��������, ��������) ������ Singleton
            while (baseType.Name != "Singleton`1" && baseType != null)
            {
                baseType = baseType.BaseType;
            }

            if (baseType == null)
                throw new ArgumentException();

            if (_self == null)
            {
                ConstructorInfo ci = typeof(T).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
                                                                null,
                                                                new Type[] {},
                                                                new ParameterModifier[] { });

                _self = (T)ci.Invoke(new object[] {});
            }

            return _self;
        }
        public static T Instance(object data)
        {
            Type baseType = typeof(T).BaseType;

            // �������������� ����� ������ ���� �������� (��������, ��������) ������ Singleton
            while (baseType.Name != "Singleton`1" && baseType != null)
            {
                baseType = baseType.BaseType;
            }

            if (baseType == null)
                throw new ArgumentException();

            if (_self == null)
            {
                ConstructorInfo ci = typeof(T).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
                                                                null,
                                                                new Type[] {typeof(object)},
                                                                new ParameterModifier[] { });

                _self = (T)ci.Invoke(new object[] {data});
            }

            return _self;
        }
    }
}