using System.Text;
using Microsoft.CodeAnalysis;

namespace EfficientUnionGenerator;
using static Constants;

public sealed record TypeCandidateDefinition(
        string TypeName,
        bool IsUnmanaged,
        string ConstructorParameterName,
        ulong EnumBitPattern
        )
    {
        public static TypeCandidateDefinition? Create(IMethodSymbol ctor)
        {
            if (ctor.MethodKind != MethodKind.Constructor)
            {
                return null;
            }
            if (ctor.DeclaredAccessibility != Accessibility.Public)
            {
                return null;
            }
            if (ctor.Parameters.Length != 1)
            {
                return null;
            }

            var param = ctor.Parameters[0];
            var typeName = param.Type.ToDisplayString();
            var isUnmanaged = param.Type.IsUnmanagedType;
            var constructorParameterName = param.Name;
            var enumBitPattern = ctor.GetAttributes()
                .FirstOrDefault(static attr => attr.AttributeClass?.ToDisplayString() == $"{AttributeNamespace}.{EnumBitPatternAttributeName}")
                ?.ConstructorArguments[0]
                .Value as ulong?
                ?? 0;
            return new TypeCandidateDefinition(typeName, isUnmanaged, constructorParameterName, enumBitPattern);
    }

    public string FieldName { get; } = Helpers.CreateSafeName(TypeName);
}
