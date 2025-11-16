using Nebulae.RimWorld.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Core.Emit
{
    public partial class Emission
    {
        internal sealed class EmitWithOpCode : Emission
        {
            public EmitWithOpCode(OpCode opCode) : base(opCode) { }


            public override void Emit(ILGenerator generator)
            {
                generator.Emit(OpCode);
            }

            public override bool Equals(object obj)
            {
                return obj is EmitWithOpCode other
                    && OpCode.Equals(other.OpCode);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(OpCode);
            }

            public override string ToString()
            {
                return OpCode.ToString();
            }
        }

        internal sealed class EmitWithLocal : Emission
        {
            public readonly LocalBuilder Local;


            public EmitWithLocal(OpCode opCode, LocalBuilder local) : base(opCode)
            {
                Local = local;
            }


            public override void Emit(ILGenerator generator)
            {
                generator.Emit(OpCode, Local);
            }

            public override bool Equals(object obj)
            {
                return obj is EmitWithLocal other
                    && OpCode.Equals(other.OpCode)
                    && Local == other.Local;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(OpCode, Local);
            }

            public override string ToString()
            {
                return $"{OpCode} {Local}";
            }
        }

        internal sealed class EmitWithType : Emission
        {
            public readonly Type Type;

            public EmitWithType(OpCode opCode, Type type) : base(opCode)
            {
                Type = type;
            }


            public override void Emit(ILGenerator generator)
            {
                generator.Emit(OpCode, Type);
            }

            public override bool Equals(object obj)
            {
                return obj is EmitWithType other
                    && OpCode.Equals(other.OpCode)
                    && Type == other.Type;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(OpCode, Type);
            }

            public override string ToString()
            {
                return $"{OpCode} {Type.AsLog()}";
            }
        }

        internal sealed class EmitWithField : Emission
        {
            public readonly FieldInfo Field;


            public EmitWithField(OpCode opCode, FieldInfo field) : base(opCode)
            {
                Field = field;
            }


            public override void Emit(ILGenerator generator)
            {
                generator.Emit(OpCode, Field);
            }

            public override bool Equals(object obj)
            {
                return obj is EmitWithField other
                    && OpCode.Equals(other.OpCode)
                    && Field == other.Field;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(OpCode, Field);
            }

            public override string ToString()
            {
                return $"{OpCode} {Field.AsLog()}";
            }
        }

        internal sealed class EmitWithMethod : Emission
        {
            public readonly MethodInfo Method;


            public EmitWithMethod(OpCode opCode, MethodInfo method) : base(opCode)
            {
                Method = method;
            }


            public override void Emit(ILGenerator generator)
            {
                generator.Emit(OpCode, Method);
            }

            public override bool Equals(object obj)
            {
                return obj is EmitWithMethod other
                    && OpCode.Equals(other.OpCode)
                    && Method == other.Method;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(OpCode, Method);
            }

            public override string ToString()
            {
                return $"{OpCode} {Method.AsLog()}";
            }
        }

        internal sealed class EmitWithConstructor : Emission
        {
            public readonly ConstructorInfo Constructor;


            public EmitWithConstructor(OpCode opCode, ConstructorInfo constructor) : base(opCode)
            {
                Constructor = constructor;
            }


            public override void Emit(ILGenerator generator)
            {
                generator.Emit(OpCode, Constructor);
            }

            public override bool Equals(object obj)
            {
                return obj is EmitWithConstructor other
                    && OpCode.Equals(other.OpCode)
                    && Constructor == other.Constructor;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(OpCode, Constructor);
            }

            public override string ToString()
            {
                return $"{OpCode} {Constructor.AsLog()}";
            }
        }

        internal sealed class EmitWithLabel : Emission
        {
            public readonly Label Label;


            public EmitWithLabel(OpCode opCode, Label label) : base(opCode)
            {
                Label = label;
            }


            public override void Emit(ILGenerator generator)
            {
                generator.Emit(OpCode, Label);
            }

            public override bool Equals(object obj)
            {
                return obj is EmitWithLabel other
                    && OpCode.Equals(other.OpCode)
                    && Label.Equals(other.Label);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(OpCode, Label);
            }

            public override string ToString()
            {
                return $"{OpCode} {Label}";
            }
        }

        internal sealed class EmitWithLabels : Emission
        {
            public readonly Label[] Labels;


            public EmitWithLabels(OpCode opCode, Label[] labels) : base(opCode)
            {
                Labels = labels;
            }


            public override void Emit(ILGenerator generator)
            {
                generator.Emit(OpCode, Labels);
            }

            public override bool Equals(object obj)
            {
                return obj is EmitWithLabels other
                    && OpCode.Equals(other.OpCode)
                    && Labels.SequenceEqual(other.Labels);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(OpCode, Labels);
            }

            public override string ToString()
            {
                return $"{OpCode} {string.Join(", ", Labels)}";
            }
        }

        internal sealed class EmitWithByte : Emission
        {
            public readonly byte Value;

            public EmitWithByte(OpCode opCode, byte value) : base(opCode)
            {
                Value = value;
            }


            public override void Emit(ILGenerator generator)
            {
                generator.Emit(OpCode, Value);
            }

            public override bool Equals(object obj)
            {
                return obj is EmitWithByte other
                    && OpCode.Equals(other.OpCode)
                    && Value == other.Value;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(OpCode, Value);
            }

            public override string ToString()
            {
                return $"{OpCode} {Value}";
            }
        }

        internal sealed class EmitWithSByte : Emission
        {
            public readonly sbyte Value;


            public EmitWithSByte(OpCode opCode, sbyte value) : base(opCode)
            {
                Value = value;
            }


            public override void Emit(ILGenerator generator)
            {
                generator.Emit(OpCode, Value);
            }

            public override bool Equals(object obj)
            {
                return obj is EmitWithSByte other
                    && OpCode.Equals(other.OpCode)
                    && Value == other.Value;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(OpCode, Value);
            }

            public override string ToString()
            {
                return $"{OpCode} {Value}";
            }
        }

        internal sealed class EmitWithInt16 : Emission
        {
            public readonly short Value;


            public EmitWithInt16(OpCode opCode, short value) : base(opCode)
            {
                Value = value;
            }


            public override void Emit(ILGenerator generator)
            {
                generator.Emit(OpCode, Value);
            }

            public override bool Equals(object obj)
            {
                return obj is EmitWithInt16 other
                    && OpCode.Equals(other.OpCode)
                    && Value == other.Value;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(OpCode, Value);
            }

            public override string ToString()
            {
                return $"{OpCode} {Value}";
            }
        }

        internal sealed class EmitWithInt32 : Emission
        {
            public readonly int Value;


            public EmitWithInt32(OpCode opCode, int value) : base(opCode)
            {
                Value = value;
            }


            public override void Emit(ILGenerator generator)
            {
                generator.Emit(OpCode, Value);
            }

            public override bool Equals(object obj)
            {
                return obj is EmitWithInt32 other
                    && OpCode.Equals(other.OpCode)
                    && Value == other.Value;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(OpCode, Value);
            }

            public override string ToString()
            {
                return $"{OpCode} {Value}";
            }
        }

        internal sealed class EmitWithInt64 : Emission
        {
            public readonly long Value;


            public EmitWithInt64(OpCode opCode, long value) : base(opCode)
            {
                Value = value;
            }


            public override void Emit(ILGenerator generator)
            {
                generator.Emit(OpCode, Value);
            }

            public override bool Equals(object obj)
            {
                return obj is EmitWithInt64 other
                    && OpCode.Equals(other.OpCode)
                    && Value == other.Value;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(OpCode, Value);
            }

            public override string ToString()
            {
                return $"{OpCode} {Value}";
            }
        }

        internal sealed class EmitWithSingle : Emission
        {
            public readonly float Value;


            public EmitWithSingle(OpCode opCode, float value) : base(opCode)
            {
                Value = value;
            }


            public override void Emit(ILGenerator generator)
            {
                generator.Emit(OpCode, Value);
            }

            public override bool Equals(object obj)
            {
                return obj is EmitWithSingle other
                    && OpCode.Equals(other.OpCode)
                    && Value == other.Value;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(OpCode, Value);
            }

            public override string ToString()
            {
                return $"{OpCode} {Value}";
            }
        }

        internal sealed class EmitWithDouble : Emission
        {
            public readonly double Value;


            public EmitWithDouble(OpCode opCode, double value) : base(opCode)
            {
                Value = value;
            }


            public override void Emit(ILGenerator generator)
            {
                generator.Emit(OpCode, Value);
            }

            public override bool Equals(object obj)
            {
                return obj is EmitWithDouble other
                    && OpCode.Equals(other.OpCode)
                    && Value == other.Value;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(OpCode, Value);
            }

            public override string ToString()
            {
                return $"{OpCode} {Value}";
            }
        }

        internal sealed class EmitWithString : Emission
        {
            public readonly string Value;


            public EmitWithString(OpCode opCode, string value) : base(opCode)
            {
                Value = value;
            }


            public override void Emit(ILGenerator generator)
            {
                generator.Emit(OpCode, Value);
            }

            public override bool Equals(object obj)
            {
                return obj is EmitWithString other
                    && OpCode.Equals(other.OpCode)
                    && Value.Equals(other.Value);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(OpCode, Value);
            }

            public override string ToString()
            {
                return $"{OpCode} \"{Value}\"";
            }
        }

        internal sealed class EmitWithSignature : Emission
        {
            public readonly SignatureHelper Signature;


            public EmitWithSignature(OpCode opCode, SignatureHelper signature) : base(opCode)
            {
                Signature = signature;
            }


            public override void Emit(ILGenerator generator)
            {
                generator.Emit(OpCode, Signature);
            }

            public override bool Equals(object obj)
            {
                return obj is EmitWithSignature other
                    && OpCode.Equals(other.OpCode)
                    && Signature.Equals(other.Signature);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(OpCode, Signature);
            }

            public override string ToString()
            {
                return $"{OpCode} {Signature}";
            }
        }
    }
}
