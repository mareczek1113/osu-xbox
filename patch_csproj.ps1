$p = 'D:\osu-xbox-scratch\uwp-framework\UwpFramework.csproj'
$raw = [IO.File]::ReadAllText($p)
$nl = if ($raw.Contains("`r`n")) { "`r`n" } else { "`n" }
$block = @'
  <Target Name="UwpUmbrellaLink" AfterTargets="SetupOSSpecificProps">
    <!-- Direct-call list minus version!/iphlpapi!: those APIs do not exist for UWP, so they must be loaded lazily instead of imported. -->
    <ReadLinesFromFile File="@(DirectPInvokeList)">
      <Output TaskParameter="Lines" ItemName="_WinApiLine" />
    </ReadLinesFromFile>
    <ItemGroup>
      <_WinApiKept Include="@(_WinApiLine)" Condition="!$([System.String]::new('%(Identity)').StartsWith('version!')) And !$([System.String]::new('%(Identity)').StartsWith('iphlpapi!'))" />
    </ItemGroup>
    <WriteLinesToFile File="$(MSBuildProjectDirectory)\obj\WindowsAPIs.uwp.txt" Lines="@(_WinApiKept)" Overwrite="true" />
    <ItemGroup>
      <DirectPInvokeList Remove="@(DirectPInvokeList)" />
      <DirectPInvokeList Include="$(MSBuildProjectDirectory)\obj\WindowsAPIs.uwp.txt" />
      <LinkerArg Remove="&quot;version.lib&quot;;&quot;crypt32.lib&quot;;&quot;iphlpapi.lib&quot;;&quot;ncrypt.lib&quot;;&quot;ws2_32.lib&quot;" />
      <LinkerArg Include="&quot;WindowsApp.lib&quot;" />
    </ItemGroup>
  </Target>
'@ -replace "`r?`n", $nl
$new = [regex]::Replace($raw, '(?s)  <Target Name="UwpUmbrellaLink".*?</Target>' + '\r?\n', { param($m) $block + $nl })
if ($new -eq $raw) { 'NO CHANGE'; exit 1 }
[IO.File]::WriteAllText($p, $new, (New-Object Text.UTF8Encoding($false)))
'patched'
