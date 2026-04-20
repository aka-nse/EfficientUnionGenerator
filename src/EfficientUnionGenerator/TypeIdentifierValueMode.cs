namespace EfficientUnionGenerator;
using static TypeIdentifierValueMode;

[Flags]
public enum TypeIdentifierValueMode
{
    AutoAssign     = 0b0,
    ExplicitAssign = 0b1,

    SetWhenCreate   = 0b00,
    LeaveWhenCreate = 0b10,

    ResetWhenGet = 0b000,
    LeaveWhenGet = 0b100,
}


public static class TypeIdentifierValueModeExtensions
{
    extension(TypeIdentifierValueMode mode)
    {
        public bool IsAutoAssign => (mode & ExplicitAssign) == 0;
        public bool IsExplicitAssign => (mode & ExplicitAssign) != 0;
        public bool IsSetWhenCreate => (mode & LeaveWhenCreate) == 0;
        public bool IsLeaveWhenCreate => (mode & LeaveWhenCreate) != 0;
        public bool IsResetWhenGet => (mode & LeaveWhenGet) == 0;
        public bool IsLeaveWhenGet => (mode & LeaveWhenGet) != 0;
    }
}