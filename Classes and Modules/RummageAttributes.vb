Namespace My
    <AttributeUsage(AttributeTargets.[Class] Or AttributeTargets.Struct Or AttributeTargets.[Enum] Or AttributeTargets.[Delegate] Or AttributeTargets.[Interface], _
        Inherited:=False, AllowMultiple:=False), RummageKeepUsersReflectionSafe()> _
    Public NotInheritable Class RummageKeepReflectionSafeAttribute
        Inherits Attribute
    End Class

    <AttributeUsage(AttributeTargets.Class, Inherited:=False, AllowMultiple:=False)> _
    Public NotInheritable Class RummageKeepUsersReflectionSafeAttribute
        Inherits Attribute
    End Class

    <RummageKeepReflectionSafe()> _
    Partial Class MySettings
    End Class
End Namespace