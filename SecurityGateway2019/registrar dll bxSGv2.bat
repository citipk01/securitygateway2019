@echo

cmd /c C:\Windows\Microsoft.NET\Framework\v4.0.30319\regasm D:\inetpub\wwwroot\buxis\lib\Seguridad\BxSecurityGateway2018v2.dll /u

cmd /c C:\Windows\Microsoft.NET\Framework\v4.0.30319\regasm D:\inetpub\wwwroot\buxis\lib\Seguridad\BxSecurityGateway2018v2.dll /codebase /tlb

cmd /k


cmd /c C:\Windows\Microsoft.NET\Framework\v4.0.30319\regsvcs /appname:BxSecurityGateway2018v2 /tlb:BxSecurityGateway2018v2.tlb D:\inetpub\wwwroot\buxis\lib\Seguridad\BxSecurityGateway2018v2.dll