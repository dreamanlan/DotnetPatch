@echo on
move /Y DotnetPatch.exe DotnetPatchBak.exe
ilmerge /target:winexe /out:DotnetPatch.exe DotnetPatchBak.exe Dsl.dll
