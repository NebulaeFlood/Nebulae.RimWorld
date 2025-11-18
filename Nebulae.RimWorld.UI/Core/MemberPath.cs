using Nebulae.RimWorld.Collections;
using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Core.Emit;
using Nebulae.RimWorld.UI.Core.Ximl.Services;
using Nebulae.RimWorld.Utilities;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using static Nebulae.RimWorld.UI.Core.PathMember;

namespace Nebulae.RimWorld.UI.Core
{
    /// <summary>
    /// 成员路径
    /// </summary>
    public sealed class MemberPath : RoughLinkedListBase<PathMember>, IEquatable<MemberPath>
    {
        /// <summary>
        /// 该 <see cref="MemberPath"/> 解析的路径字符串
        /// </summary>
        public readonly string Path;


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 获取包含该路径第一个成员的节点
        /// </summary>
        public RoughLinkedListNode<PathMember> Head => head;

        /// <summary>
        /// 获取一个值，该值指示路径是否始于静态成员
        /// </summary>
        public bool IsStatic => head.Item.Info.IsStatic;

        /// <summary>
        /// 获取一个值，该值是此路径的最后一个成员的类型
        /// </summary>
        public Type ResultType => tail.Item.Info.ValueType;

        /// <summary>
        /// 获取一个值，该值是声明此路径第一个成员的类型
        /// </summary>
        public Type RootType => head.Item.Info.DeclaringType;

        /// <summary>
        /// 获取包含该路径最后一个成员的节点
        /// </summary>
        public RoughLinkedListNode<PathMember> Tail => tail;

        #endregion


        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        static MemberPath()
        {
            var accessor = new DynamicMethod("MemberPath[]<--{Self}.GetValue", typeof(object), new Type[] { typeof(MemberPath), typeof(object) }, typeof(MemberPath), skipVisibility: true);
            var accessorIL = accessor.GetILGenerator();

            accessorIL.Emit(OpCodes.Ldarg_1);
            accessorIL.Emit(OpCodes.Ret);

            SelfAccessor = (MemberAccessor)accessor.CreateDelegate(typeof(MemberAccessor));
        }

        private MemberPath(string path)
        {
            Path = path;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Staitc Methods
        //
        //------------------------------------------------------

        #region Public Staitc Methods

        /// <summary>
        /// 解析成员路径
        /// </summary>
        /// <param name="property">依赖属性</param>
        /// <returns>由 <paramref name="property"/> 解析的 <see cref="MemberPath"/>。</returns>
        public static MemberPath Resolve(DependencyProperty property)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            var accessor = new DynamicMethod("GetValue", typeof(object), new Type[] { typeof(object) }, true);
            var modifier = new DynamicMethod("SetValue", null, new Type[] { typeof(object), typeof(object) }, true);


            var il = accessor.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, typeof(DependencyObject));
            il.Emit(OpCodes.Call, PathMemberInfo.DependencyObjectGetValueMethod);
            il.Emit(OpCodes.Ret);

            var accessorDelegate = (MemberAccessor)accessor.CreateDelegate(typeof(MemberAccessor));


            il = modifier.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, typeof(DependencyObject));
            il.Emit(OpCodes.Ldsfld, property.GetIdentifier());
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, PathMemberInfo.DependencyObjectSetValueMethod);
            il.Emit(OpCodes.Ret);

            var modifierDelegate = (MemberModifier)modifier.CreateDelegate(typeof(MemberModifier));


            var memberPath = new MemberPath($"({property})");
            var member = new PathMember(new PathMemberInfo(property)) { accessor = accessorDelegate, modifier = modifierDelegate };

            memberPath.InsertLast(member);
            memberPath.count++;

            return memberPath;
        }

        /// <summary>
        /// 解析成员路径
        /// </summary>
        /// <param name="rootType">包含根成员的类型</param>
        /// <param name="path">成员路径</param>
        /// <param name="typeResolver">XIML 类型解析器</param>
        /// <returns>由 <paramref name="path"/> 在 <paramref name="rootType"/> 类型中解析的 <see cref="MemberPath"/>。</returns>
        public static MemberPath Resolve(Type rootType, string path, IXimlTypeResolver typeResolver)
        {
            if (rootType is null)
            {
                throw new ArgumentNullException(nameof(rootType));
            }

            if (typeResolver is null)
            {
                throw new ArgumentNullException(nameof(typeResolver));
            }

            if (string.IsNullOrEmpty(path))
            {
                var memberPath = new MemberPath("{Self}");
                var member = new PathMember(new PathMemberInfo(typeof(object))) { accessor = SelfAccessor };

                memberPath.InsertLast(member);
                memberPath.count++;

                return memberPath;
            }

            return PathCache.GetOrAdd(new CacheKey(rootType, path, typeResolver.GetType()), new PathResolver(typeResolver).Resolve);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 判断指定对象是否等于当前对象
        /// </summary>
        /// <param name="obj">要比较的对象</param>
        /// <returns>若指定的对象等于当前对象，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public override bool Equals(object obj)
        {
            if (obj is not MemberPath other)
            {
                return false;
            }

            if (head.Item.Info.DeclaringType != other.head.Item.Info.DeclaringType)
            {
                return false;
            }

            return Path.Equals(other.Path);
        }

        /// <summary>
        /// 判断指定对象是否等于当前对象
        /// </summary>
        /// <param name="other">要比较的对象</param>
        /// <returns>若指定的对象等于当前对象，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Equals(MemberPath other)
        {
            if (other is null)
            {
                return false;
            }

            if (head.Item.Info.DeclaringType != other.head.Item.Info.DeclaringType)
            {
                return false;
            }

            return Path.Equals(other.Path);
        }

        /// <summary>
        /// 获取当前对象的哈希代码
        /// </summary>
        /// <returns>当前对象的哈希代码。</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(head.Item.Info.DeclaringType, Path);
        }

        /// <summary>
        /// 通过路径从给定对象获取值
        /// </summary>
        /// <param name="target">要通过路径获取值的对象</param>
        /// <returns>该路径通过 <paramref name="target"/> 获取的值。</returns>
        public object GetValue(object target)
        {
            try
            {
                return tail.Item.accessor(this, target);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Cannot get the value of path '{Path}' from the object '{target.AsLog()}'.", e);
            }
        }

        /// <summary>
        /// 通过路径从给定对象获取指定成员的值
        /// </summary>
        /// <param name="target">要通过路径获取值的对象</param>
        /// <param name="member">路径中的成员</param>
        /// <returns>该路径通过 <paramref name="target"/> 获取的 <paramref name="member"/> 的值。</returns>
        public object GetValue(object target, PathMember member)
        {
            try
            {
                return member.accessor(this, target);
            }
            catch (Exception e)
            {
                if (member is null)
                {
                    throw new ArgumentNullException(nameof(member));
                }

                var node = head;

                while (node is not null)
                {
                    if (node.Item == member)
                    {
                        throw new InvalidOperationException($"Cannot get the value of member '{member}' in path '{Path}' from the object '{target.AsLog()}'.", e);
                    }
                }

                throw new ArgumentException($"The given member '{member}' is not in the path '{Path}'.", nameof(member));
            }
        }

        /// <summary>
        /// 通过路径为给定对象设置值
        /// </summary>
        /// <param name="target">要通过路径设置值的对象</param>
        /// <param name="value">要设置的值</param>
        public void SetValue(object target, object value)
        {
            try
            {
                tail.Item.modifier(this, target, value);
            }
            catch (Exception e)
            {
                if (tail.Item.Info.IsReadonly)
                {
                    throw new InvalidOperationException($"Cannot set value '{value.AsLog()}' to path '{Path}' on the object '{target.AsLog()}' since the final member is read-only.");
                }

                throw new InvalidOperationException($"Cannot set value '{value.AsLog()}' to path '{Path}' on the object '{target.AsLog()}'.", e);
            }
        }

        /// <summary>
        /// 通过路径为给定对象设置指定成员的值
        /// </summary>
        /// <param name="target">要通过路径设置值的对象</param>
        /// <param name="member">路径中的成员</param>
        /// <param name="value">要设置的值</param>
        public void SetValue(object target, PathMember member, object value)
        {
            try
            {
                member.modifier(this, target, value);
            }
            catch (Exception e)
            {
                if (member is null)
                {
                    throw new ArgumentNullException(nameof(member));
                }

                var node = head;

                while (node is not null)
                {
                    if (node.Item == member)
                    {
                        throw new InvalidOperationException($"Cannot set value '{value.AsLog()}' to member '{member}' in path '{Path}' on the object '{target.AsLog()}'.", e);
                    }
                }

                if (tail.Item.Info.IsReadonly)
                {
                    throw new InvalidOperationException($"Cannot set value '{value.AsLog()}' to member '{member}' in path '{Path}' on the object '{target.AsLog()}' since it is read-only.");
                }

                throw new ArgumentException($"The given member '{member}' is not in the path '{Path}'.", nameof(member));
            }
        }

        /// <summary>
        /// 获取表示当前对象的字符串
        /// </summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            if (count is 1)
            {
                return head.Item.Info.ToString();
            }

            var builder = new StringBuilder();
            var node = head;

            while (node is not null)
            {
                builder.Append(node.Item.Info.ToString()).Append('.');
                node = node.Next;
            }

            return builder.ToString(0, builder.Length - 1);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

        private void Compile()
        {
            var emissions = new EmissionList();

            if (!head.Item.Info.IsStatic)
            {
                emissions.Emit(OpCodes.Ldarg_1);

                var declaringType = head.Item.Info.DeclaringType;

                if (declaringType.IsValueType)
                {
                    emissions.Emit(OpCodes.Unbox_Any, declaringType);
                }
                else
                {
                    emissions.Emit(OpCodes.Castclass, declaringType);
                }
            }

            var curr = head;
            int indexerIndex = 0;

            while (curr is not null)
            {
                var memberInfo = curr.Item.Info;

                switch (memberInfo.Type)
                {
                    case PathMemberType.DependencyProperty:

                        emissions.Emit(OpCodes.Ldsfld, ((DependencyProperty)memberInfo.Metadata).GetIdentifier());
                        emissions.Emit(OpCodes.Call, PathMemberInfo.DependencyObjectGetValueMethod);

                        if (memberInfo.ValueType.IsValueType)
                        {
                            emissions.Emit(OpCodes.Unbox, memberInfo.ValueType);
                        }
                        else
                        {
                            emissions.Emit(OpCodes.Castclass, memberInfo.ValueType);
                        }

                        break;
                    case PathMemberType.Indexer:

                        var indexer = (PropertyInfo)memberInfo.Metadata;

                        var parameters = _indexerParameters[indexerIndex];
                        var parameterCount = parameters.Length;

                        emissions.Emit(OpCodes.Ldarg_0);
                        emissions.Emit(OpCodes.Ldfld, IndexerParametersField);
                        emissions.Emit(OpCodes.Ldc_I4, indexerIndex);
                        emissions.Emit(OpCodes.Ldelem_Ref);
                        emissions.Emit(OpCodes.Stloc_0);

                        for (int i = 0; i < parameterCount; i++)
                        {
                            emissions.Emit(OpCodes.Ldloc_0);
                            emissions.Emit(OpCodes.Ldc_I4, i);
                            emissions.Emit(OpCodes.Ldelem_Ref);

                            var parameterType = parameters[i].GetType();

                            if (parameterType.IsValueType)
                            {
                                emissions.Emit(OpCodes.Unbox_Any, parameterType);
                            }
                            else
                            {
                                emissions.Emit(OpCodes.Castclass, parameterType);
                            }
                        }

                        var indexerGetter = indexer.GetGetMethod(true);

                        if (indexerGetter.IsVirtual)
                        {
                            emissions.Emit(OpCodes.Callvirt, indexerGetter);
                        }
                        else
                        {
                            emissions.Emit(OpCodes.Call, indexerGetter);
                        }

                        indexerIndex++;

                        break;
                    case PathMemberType.Field:

                        emissions.Emit(memberInfo.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, (FieldInfo)memberInfo.Metadata);

                        break;
                    default:    // Property

                        var propertyGetter = ((PropertyInfo)memberInfo.Metadata).GetGetMethod(true);

                        if (propertyGetter.IsVirtual)
                        {
                            emissions.Emit(OpCodes.Callvirt, propertyGetter);
                        }
                        else
                        {
                            emissions.Emit(OpCodes.Call, propertyGetter);
                        }

                        break;
                }

                Initialize(curr.Item, emissions);

                curr = curr.Next;
            }

            if (_indexerCount > 0)
            {
                Array.Resize(ref _indexerParameters, _indexerCount);
            }
        }

        private void Initialize(PathMember member, EmissionList emissions)
        {
            var dyanmicMethodPrefix = $"MemberPath[{Path}]<--{member.Info}.";

            var accessor = new DynamicMethod(dyanmicMethodPrefix + "GetValue", typeof(object), new Type[] { typeof(MemberPath), typeof(object) }, typeof(MemberPath), skipVisibility: true);
            var accessorIL = accessor.GetILGenerator();

            var memberInfo = member.Info;


            if (memberInfo.IsReadonly)
            {
                emissions.Emit(accessorIL);

                if (memberInfo.ValueType.IsValueType)
                {
                    accessorIL.Emit(OpCodes.Box, memberInfo.ValueType);
                }

                accessorIL.Emit(OpCodes.Ret);

                if (_indexerCount > 0)
                {
                    accessorIL.DeclareLocal(typeof(object[]));
                }

                member.accessor = (MemberAccessor)accessor.CreateDelegate(typeof(MemberAccessor));
                return;
            }


            var modifier = new DynamicMethod(dyanmicMethodPrefix + "SetValue", null, new Type[] { typeof(MemberPath), typeof(object), typeof(object) }, typeof(MemberPath), skipVisibility: true);
            var modifierIL = modifier.GetILGenerator();

            if (_indexerCount > 0)
            {
                accessorIL.DeclareLocal(typeof(object[]));
                modifierIL.DeclareLocal(typeof(object[]));
            }

            if (memberInfo.Type is PathMemberType.DependencyProperty)
            {
                var castValue = emissions.Recall();
                var callGetValue = emissions.Recall();

                foreach (var emission in emissions)
                {
                    emission.Emit(accessorIL);
                    emission.Emit(modifierIL);
                }

                emissions.Save(callGetValue);
                emissions.Save(castValue);

                callGetValue.Emit(accessorIL);
                accessorIL.Emit(OpCodes.Ret);

                modifierIL.Emit(OpCodes.Ldarg_2);
                modifierIL.Emit(OpCodes.Call, PathMemberInfo.DependencyObjectSetValueMethod);
                modifierIL.Emit(OpCodes.Ret);
            }
            else
            {
                var loadValue = emissions.Recall();

                foreach (var emission in emissions)
                {
                    emission.Emit(accessorIL);
                    emission.Emit(modifierIL);
                }

                emissions.Save(loadValue);


                loadValue.Emit(accessorIL);

                if (memberInfo.ValueType.IsValueType)
                {
                    accessorIL.Emit(OpCodes.Box, memberInfo.ValueType);
                }

                accessorIL.Emit(OpCodes.Ret);


                modifierIL.Emit(OpCodes.Ldarg_2);

                if (memberInfo.ValueType.IsValueType)
                {
                    modifierIL.Emit(OpCodes.Unbox_Any, memberInfo.ValueType);
                }
                else if (memberInfo.ValueType != typeof(object))
                {
                    modifierIL.Emit(OpCodes.Castclass, memberInfo.ValueType);
                }

                if (memberInfo.Type is PathMemberType.Field)
                {
                    modifierIL.Emit(memberInfo.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, (FieldInfo)memberInfo.Metadata);
                }
                else
                {
                    // 属性和索引器都使用 PropertyInfo
                    modifierIL.Emit(loadValue.OpCode, ((PropertyInfo)memberInfo.Metadata).GetSetMethod(true));
                }

                modifierIL.Emit(OpCodes.Ret);
            }

            member.accessor = (MemberAccessor)accessor.CreateDelegate(typeof(MemberAccessor));
            member.modifier = (MemberModifier)modifier.CreateDelegate(typeof(MemberModifier));
        }

        private bool Match(IndexerParameter[] x, ParameterInfo[] y, out object[] arguments)
        {
            if (x.Length != y.Length)
            {
                arguments = null;
                return false;
            }

            int count = x.Length;

            for (int i = 0; i < count; i++)
            {
                var type = x[i].Type;

                if (type is null)
                {
                    continue;
                }

                if (type != y[i].ParameterType)
                {
                    arguments = null;
                    return false;
                }
            }

            arguments = new object[count];

            for (int i = 0; i < count; i++)
            {
                arguments[i] = Convert.ChangeType(x[i].Value, y[i].ParameterType);
            }

            return true;
        }

        private void ResolveAttachedProperty(string path, IXimlTypeResolver typeResolver)
        {
            var segments = path.Split('.', StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length != 2)
            {
                throw new InvalidOperationException($"Cannot resolve attached property from '{path}', the correct format of an attached property is '(TypeName.PropertyName)'.");
            }

            var ownerType = typeResolver.Resolve(segments[0]);
            var property = DependencyProperty.Search(segments[1], ownerType);

            InsertLast(new PathMember(new PathMemberInfo(property)));
            count++;
        }

        private void ResolveIndexer(string path, Type ownerType, IXimlTypeResolver typeResolver)
        {
            if (path.Length < 1)
            {
                throw new InvalidOperationException("Cannot resolve indexer from an empty string.");
            }

            var segments = path.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var count = segments.Length;

            if (count < 1)
            {
                throw new InvalidOperationException($"Cannot resolve indexer from '{path}', the correct format of an indexer's arguments is 'Arg1, Arg2,...' or '(Type1) Arg1, (Type) Arg2,...'.");
            }

            var parameters = new IndexerParameter[count];

            for (int i = 0; i < count; i++)
            {
                var parameter = segments[i].Trim();

                if (parameter[0] is '(')
                {
                    int front = parameter.IndexOf(')');

                    if (front < 0)
                    {
                        throw new InvalidOperationException($"Cannot resolve indexer from '{path}', the correct format of an indexer's arguments is 'Arg1, Arg2,...' or '(Type1) Arg1, (Type) Arg2,...'.");
                    }

                    var typeName = parameter[1..front];
                    var valueStr = parameter[(front + 1)..].Trim();

                    parameters[i] = new IndexerParameter(typeResolver.Resolve(typeName), valueStr);
                }
                else
                {
                    parameters[i] = new IndexerParameter(null, parameter);
                }
            }

            PropertyInfo[] properties = head is null
                ? ownerType.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                : ownerType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            for (int i = properties.Length - 1; i >= 0; i--)
            {
                var property = properties[i];
                var propertyParameters = property.GetIndexParameters();

                if (Match(parameters, propertyParameters, out var arguments))
                {
                    if (_indexerCount + 1 > _indexerParameters.Length)
                    {
                        Array.Resize(ref _indexerParameters, _indexerCount + 3);
                    }

                    _indexerParameters[_indexerCount] = arguments;

                    InsertLast(new PathMember(new PathMemberInfo(property, PathMemberType.Indexer)));
                    this.count++;

                    _indexerCount++;
                    return;
                }
            }

            throw new InvalidOperationException($"Cannot find any specified non-static indexer in type '{ownerType.AsLog()}'.");
        }

        private void ResolveMember(string path, Type ownerType)
        {
            MemberInfo[] memberInfos = head is null
                ? ownerType.GetMember(path, MemberTypes.Field | MemberTypes.Property, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                : ownerType.GetMember(path, MemberTypes.Field | MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (memberInfos.Length < 1)
            {
                throw new InvalidOperationException($"Cannot find any non-static member named '{path}' in type '{ownerType.AsLog()}'.");
            }

            if (memberInfos.Length > 1)
            {
                throw new InvalidOperationException($"Cannot resolve the path '{path}', there are multiple members named '{path}' in type '{ownerType.AsLog()}'.");
            }

            var memberInfo = memberInfos[0];

            if (memberInfo.MemberType is MemberTypes.Field)
            {
                InsertLast(new PathMember(new PathMemberInfo((FieldInfo)memberInfo)));
            }
            else
            {
                if (DependencyProperty.TrySearch(path, ownerType, out var property))
                {
                    InsertLast(new PathMember(new PathMemberInfo(property)));
                }
                else
                {
                    InsertLast(new PathMember(new PathMemberInfo((PropertyInfo)memberInfo, PathMemberType.Property)));
                }
            }

            count++;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Static Fields
        //
        //------------------------------------------------------

        #region Private Static Fields

        private static readonly FieldInfo IndexerParametersField = typeof(MemberPath).GetField(nameof(_indexerParameters), BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly ConcurrentDictionary<CacheKey, MemberPath> PathCache = new();
        private static readonly MemberAccessor SelfAccessor;

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private int _indexerCount = 0;
        private object[][] _indexerParameters = Array.Empty<object[][]>();

        #endregion


        //------------------------------------------------------
        //
        //  Private Structs
        //
        //------------------------------------------------------

        #region Private Structs

        private readonly struct CacheKey : IEquatable<CacheKey>
        {
            public readonly Type RootType;
            public readonly string Path;
            public readonly Type TypeResolverType;


            public CacheKey(Type rootType, string path, Type typeResolverType)
            {
                RootType = rootType;
                Path = path;
                TypeResolverType = typeResolverType;
            }


            public override bool Equals(object obj)
            {
                return obj is CacheKey other
                    && RootType == other.RootType
                    && Path.Equals(other.Path)
                    && TypeResolverType == other.TypeResolverType;
            }

            public bool Equals(CacheKey other)
            {
                return RootType == other.RootType
                    && Path.Equals(other.Path)
                    && TypeResolverType == other.TypeResolverType;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(RootType, Path, TypeResolverType);
            }
        }

        private readonly struct IndexerParameter
        {
            public readonly Type Type;
            public readonly string Value;

            public IndexerParameter(Type type, string value)
            {
                Type = type;
                Value = value;
            }
        }

        private readonly struct PathResolver
        {
            public readonly IXimlTypeResolver TypeResolver;


            public PathResolver(IXimlTypeResolver typeResolver)
            {
                TypeResolver = typeResolver;
            }


            public MemberPath Resolve(CacheKey key)
            {
                Type rootType = key.RootType;
                string path = key.Path;

                int back = 0;
                int length = path.Length;

                MemberPath memberPath = new MemberPath(path);

                for (int front = 0; front < length; front++)
                {
                    char c = path[front];

                    // 解析附加属性
                    if (c is '(')
                    {
                        back = front;
                        front = path.IndexOf(')', back);

                        if (front < 0)
                        {
                            throw new InvalidOperationException($"Cannot resolve the path '{path}' since it has an incorrect format.");
                        }

                        try
                        {
                            memberPath.ResolveAttachedProperty(path[(back + 1)..(front - 1)], TypeResolver);
                        }
                        catch (Exception e)
                        {
                            throw new InvalidOperationException($"Cannot resolve the path '{path}'.", e);
                        }

                        back = front + 1;
                    }
                    // 解析索引器
                    else if (c is '[')
                    {
                        if (back < front)
                        {
                            try
                            {
                                memberPath.ResolveMember(
                                    path[back..front],
                                    memberPath.count < 1 ? rootType : memberPath.tail.Item.Info.ValueType);
                            }
                            catch (Exception e)
                            {
                                throw new InvalidOperationException($"Cannot resolve the path '{path}'.", e);
                            }
                        }

                        back = front;
                        front = path.IndexOf(']', back);

                        if (front < 0)
                        {
                            throw new InvalidOperationException($"Cannot resolve the path '{path}', the indexer must have a closing bracket ']'.");
                        }

                        try
                        {
                            memberPath.ResolveIndexer(
                                path[(back + 1)..front],
                                memberPath.count < 1 ? rootType : memberPath.tail.Item.Info.ValueType,
                                TypeResolver);
                        }
                        catch (Exception e)
                        {
                            throw new InvalidOperationException($"Cannot resolve the path '{path}'.", e);
                        }

                        back = front + 2;
                    }
                    // 解析成员
                    else if (c is '.' && back < front)
                    {
                        try
                        {
                            memberPath.ResolveMember(
                                path[back..front],
                                memberPath.count < 1 ? rootType : memberPath.tail.Item.Info.ValueType);
                        }
                        catch (Exception e)
                        {
                            throw new InvalidOperationException($"Cannot resolve the path '{path}'.", e);
                        }

                        back = front + 1;
                    }
                }

                if (back < length)
                {
                    try
                    {
                        memberPath.ResolveMember(
                            path[back..length],
                            memberPath.count < 1 ? rootType : memberPath.tail.Item.Info.ValueType);
                    }
                    catch (Exception e)
                    {
                        throw new InvalidOperationException($"Cannot resolve the path '{path}'.", e);
                    }
                }

                if (memberPath.count < 1)
                {
                    throw new NotSupportedException($"Cannot resolve the path '{path}'.");
                }

                memberPath.Compile();
                return memberPath;
            }
        }

        #endregion
    }
}
