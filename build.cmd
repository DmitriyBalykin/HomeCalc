xcopy $(SolutionDir)HomeCalc.Model\\bin\\Debug\\x86 $(TargetDir)x86\ /e /s /y
xcopy $(SolutionDir)HomeCalc.Model\\bin\\Debug\\x64 $(TargetDir)x64\ /e /s /y
xcopy $(SolutionDir)HomeCalc.Model\\bin\\Release\\x86 $(TargetDir)x86\ /e /s /y
xcopy $(SolutionDir)HomeCalc.Model\\bin\\Release\\x64 $(TargetDir)x64\ /e /s /y

del $(SolutionDir)Target\\HomeCalc\\*.* /q

xcopy $(TargetDir)x86 $(SolutionDir)Target\\HomeCalc\x86\ /e /s /y
xcopy $(TargetDir)HomeCalc.View.exe $(SolutionDir)Target\\HomeCalc\ /e /s /y
xcopy $(TargetDir) $(SolutionDir)Target\\HomeCalc\ /e /s /y