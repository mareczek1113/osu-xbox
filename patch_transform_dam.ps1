# AOT: string-based TransformTo/MakeTransform look the member up by name through reflection (TransformCustom.findAccessor).
# Annotate the target type parameter so ILC keeps its properties/fields at every call site.
$dir = 'D:\osu-xbox-port\osu-framework\osu.Framework\Graphics'
$ext = "$dir\TransformableExtensions.cs"
$cus = "$dir\Transforms\TransformCustom.cs"
$ann = '[DynamicallyAccessedMembers(TransformReflection.Members)] '

$t = [IO.File]::ReadAllText($ext)
if ($t -notmatch 'TransformReflection') {
    $t = $t.Replace('TransformTo<TThis, TValue>(this TThis t, string', "TransformTo<${ann}TThis, TValue>(this TThis t, string")
    $t = $t.Replace('TransformTo<TThis, TValue, TEasing>(this TThis t, string', "TransformTo<${ann}TThis, TValue, TEasing>(this TThis t, string")
    $t = $t.Replace('MakeTransform<TThis, TValue>(this TThis t, string', "MakeTransform<${ann}TThis, TValue>(this TThis t, string")
    $t = $t.Replace('MakeTransform<TThis, TEasing, TValue>(this TThis t, string', "MakeTransform<${ann}TThis, TEasing, TValue>(this TThis t, string")
    $t = $t.Replace("using System;`r`n", "using System;`r`nusing System.Diagnostics.CodeAnalysis;`r`n")
    $t = $t.Replace("using System;`n", "using System;`nusing System.Diagnostics.CodeAnalysis;`n")
    [IO.File]::WriteAllText($ext, $t)
    "patched TransformableExtensions.cs"
}

$c = [IO.File]::ReadAllText($cus)
if ($c -notmatch 'TransformReflection') {
    $c = $c.Replace('internal class TransformCustom<TValue, TEasing, T> : Transform<TValue, TEasing, T>', "internal class TransformCustom<TValue, TEasing, ${ann}T> : Transform<TValue, TEasing, T>")
    $c = $c.Replace('internal class TransformCustom<TValue, T> : TransformCustom<TValue, DefaultEasingFunction, T>', "internal class TransformCustom<TValue, ${ann}T> : TransformCustom<TValue, DefaultEasingFunction, T>")
    $c = $c.Replace("using System.Diagnostics;`r`n", "using System.Diagnostics;`r`nusing System.Diagnostics.CodeAnalysis;`r`n")
    $c = $c.Replace("using System.Diagnostics;`n", "using System.Diagnostics;`nusing System.Diagnostics.CodeAnalysis;`n")
    $c = $c.Replace('    /// <summary>
    /// A transform which operates on arbitrary fields or properties of a given target.', '    internal static class TransformReflection
    {
        public const DynamicallyAccessedMemberTypes Members = DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties
                                                              | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields;
    }

    /// <summary>
    /// A transform which operates on arbitrary fields or properties of a given target.')
    [IO.File]::WriteAllText($cus, $c)
    "patched TransformCustom.cs"
}
Select-String -Path $ext, $cus -Pattern 'TransformReflection|CodeAnalysis' | ForEach-Object { "$($_.Filename):$($_.LineNumber): $($_.Line.Trim())" }
