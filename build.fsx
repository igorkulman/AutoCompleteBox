// include Fake lib
#r @"tools\FAKE\tools\FakeLib.dll"
#load "packages/SourceLink.Fake.0.3.4/tools/SourceLink.fsx"
open Fake
open SourceLink
 
RestorePackages()

// Properties
let buildDir = "./AutoCompleteBox/bin/Release"
let testDir  = @".\test\"
let packagesDir = @".\packages"
let packagingRoot = "./packaging/"
let packagesVersion = "1.2.0.4"

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

Target "SourceLink" (fun _ ->            
        use repo = new GitRepo(__SOURCE_DIRECTORY__)
        let proj = VsProj.LoadRelease "AutoCompleteBox/AutoCompleteBox.csproj"    
        logfn "source linking %s" proj.OutputFilePdb
        let files = proj.Compiles -- "**/AssemblyInfo.cs" -- "**/xamltypeinfo.g.cs"        
        repo.VerifyChecksums files
        proj.VerifyPdbChecksums files
        proj.CreateSrcSrv "https://raw.github.com/igorkulman/AutoCompleteBox/{0}/%var2%" repo.Revision (repo.Paths files)
        Pdbstr.exec proj.OutputFilePdb proj.OutputFilePdbSrcSrv    
)

Target "Default" (fun _ ->
    trace "Build completed"
)

// Dependencies
"Clean"  
  ==> "Build"
  ==> "SourceLink"
  ==> "CreateNugetPackage"
  ==> "Default"
 
// start build
Run "Default"