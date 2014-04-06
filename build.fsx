// include Fake lib
#r @"tools\FAKE\tools\FakeLib.dll"
open Fake
 
RestorePackages()

// Properties
let buildDir = @".\build\"
let testDir  = @".\test\"
let packagesDir = @".\packages"
let packagingRoot = "./packaging/"
let packagesVersion = "1.2.0.3"

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; testDir; packagingRoot]
)

Target "Build" (fun _ ->
    !! @"AutoCompleteBox\AutoCompleteBox.csproj"
      |> MSBuildRelease buildDir "Build"
      |> Log "AppBuild-Output: "
)

Target "CreateNugetPackage" (fun _ ->    
    NuGet (fun p -> 
        {p with                  
            Project = "AutoCompleteBoxWinRT"          
            OutputPath = packagingRoot
            WorkingDir = buildDir
            Version = packagesVersion
            Dependencies =
                ["Rx-Main", GetPackageVersion "./packages/" "Rx-Main"
                 "Rx-Core", GetPackageVersion "./packages/" "Rx-Core"
                 "Rx-XAML", "2.1.30214.0"
                 "Rx-Interfaces", GetPackageVersion "./packages/" "Rx-Interfaces"
                 "Rx-Linq", GetPackageVersion "./packages/" "Rx-Linq"
                 "Rx-PlatformServices", GetPackageVersion "./packages/" "Rx-PlatformServices"
                 "winrtxamltoolkit", GetPackageVersion "./packages/" "winrtxamltoolkit"]
            Publish = false
            }) "autocompletebox.nuspec"
)

Target "Default" (fun _ ->
    trace "Build completed"
)

// Dependencies
"Clean"  
  ==> "Build"
  ==> "CreateNugetPackage"
  ==> "Default"
 
// start build
Run "Default"