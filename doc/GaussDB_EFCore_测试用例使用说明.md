# GaussDB EFCore 测试用例使用说明

## 1. 说明

本文档说明当前仓库的常用测试方式，包括 provider 单测、functional 测试、分布式 functional 全量测试、单用例过滤、TRX 解析和测试库清理。

文档中不包含真实数据库地址、账号、密码、本机路径等敏感信息。命令中的 `<...>` 均为占位符，请在本机临时替换，不要提交到仓库。

## 2. 前置条件

1. 安装 .NET 9 SDK。
2. 在仓库根目录执行命令，本文统一写作 `<repo-root>`。
3. 如需跑 functional 测试，需要准备可用的 GaussDB/openGauss 数据库。
4. 如需清理远端测试库，需要准备 `gsql.exe` 或其他可执行 SQL 的客户端。

检查 SDK：

```powershell
dotnet --info
```

检查当前目标框架和依赖版本：

```powershell
Get-Content .\Directory.Build.props
Get-Content .\Directory.Packages.props
Get-Content .\global.json
```

## 3. 测试项目

| 项目 | 路径 | 用途 |
| --- | --- | --- |
| Provider 单测 | `test/EFCore.GaussDB.Tests/EFCore.GaussDB.Tests.csproj` | 验证 provider 内部服务、类型映射、SQL 生成、降级控制等 |
| Functional 测试 | `test/EFCore.GaussDB.FunctionalTests/EFCore.GaussDB.FunctionalTests.csproj` | 基于真实数据库验证 EF Core relational 行为 |

## 4. Provider 单测

在 `<repo-root>` 执行：

```powershell
dotnet test .\test\EFCore.GaussDB.Tests\EFCore.GaussDB.Tests.csproj -f net9.0
```

该测试通常不需要真实数据库连接，适合作为最先执行的快速验证。

## 5. 集中式 Functional 测试

### 5.1 设置连接串

使用环境变量传入测试连接。示例：

```powershell
$env:Test__GaussDB__DefaultConnection='Server=<centralized-host>;Username=<db-user>;Password=<db-password>;Port=<db-port>;SSL Mode=disable;Timeout=15;Include Error Detail=true'
$env:Test__GaussDB__EnableExtensionSessionParameter='false'
Remove-Item Env:\Test__GaussDB__IsDistributed -ErrorAction SilentlyContinue
```

说明：

- `<centralized-host>` 替换为集中式数据库地址。
- `<db-user>`、`<db-password>` 替换为临时测试账号信息。
- 集中式测试不要设置 `Test__GaussDB__IsDistributed=true`。
- 不要把真实连接串写入仓库文件。

### 5.2 构建并测试

```powershell
dotnet build .\test\EFCore.GaussDB.FunctionalTests\EFCore.GaussDB.FunctionalTests.csproj -f net9.0

dotnet test .\test\EFCore.GaussDB.FunctionalTests\EFCore.GaussDB.FunctionalTests.csproj `
  -f net9.0 `
  --no-build `
  --logger "trx;LogFileName=functional-centralized-local.trx" `
  --results-directory .\test\EFCore.GaussDB.FunctionalTests\TestResults
```

## 6. 分布式 Functional 全量测试

### 6.1 设置连接串

分布式测试必须显式打开分布式开关：

```powershell
$env:Test__GaussDB__DefaultConnection='Server=<distributed-host-1>,<distributed-host-2>,<distributed-host-3>;Username=<db-user>;Password=<db-password>;Port=<db-port>;SSL Mode=disable;Timeout=15;Include Error Detail=true'
$env:Test__GaussDB__EnableExtensionSessionParameter='false'
$env:Test__GaussDB__IsDistributed='true'
```

说明：

- `Server` 可以写多个分布式节点，使用逗号分隔。
- `Test__GaussDB__IsDistributed=true` 会启用测试侧分布式适配，包括移除 FK DDL、必要时追加复制分布、补充稳定排序等。
- 该开关只应该用于分布式库；集中式测试不要设置。

### 6.2 构建并测试

```powershell
dotnet build .\test\EFCore.GaussDB.FunctionalTests\EFCore.GaussDB.FunctionalTests.csproj -f net9.0

dotnet test .\test\EFCore.GaussDB.FunctionalTests\EFCore.GaussDB.FunctionalTests.csproj `
  -f net9.0 `
  --no-build `
  --logger "trx;LogFileName=functional-distributed-local.trx" `
  --results-directory .\test\EFCore.GaussDB.FunctionalTests\TestResults
```

本分支最近一次分布式全量参考结果：

```text
total=14553
executed=13826
passed=13826
failed=0
skipped=727
```

以你本机最新 TRX 结果为准。

## 7. 跑单个测试或一类测试

使用 `--filter` 可以降低验证成本。

按类名过滤：

```powershell
dotnet test .\test\EFCore.GaussDB.FunctionalTests\EFCore.GaussDB.FunctionalTests.csproj `
  -f net9.0 `
  --no-build `
  --filter "FullyQualifiedName~NorthwindIncludeQueryGaussDBTest"
```

按方法名过滤：

```powershell
dotnet test .\test\EFCore.GaussDB.FunctionalTests\EFCore.GaussDB.FunctionalTests.csproj `
  -f net9.0 `
  --no-build `
  --filter "FullyQualifiedName~Include_collection_skip_no_order_by"
```

按 migrations 类过滤：

```powershell
dotnet test .\test\EFCore.GaussDB.FunctionalTests\EFCore.GaussDB.FunctionalTests.csproj `
  -f net9.0 `
  --no-build `
  --filter "FullyQualifiedName~MigrationsGaussDBTest"
```

## 8. 解析 TRX 结果

测试完成后，TRX 位于：

```text
<repo-root>\test\EFCore.GaussDB.FunctionalTests\TestResults
```

解析 counters：

```powershell
$trx='.\test\EFCore.GaussDB.FunctionalTests\TestResults\functional-distributed-local.trx'
[xml]$x=Get-Content $trx
$ns=New-Object System.Xml.XmlNamespaceManager($x.NameTable)
$ns.AddNamespace('t','http://microsoft.com/schemas/VisualStudio/TeamTest/2010')
$x.SelectSingleNode('//t:ResultSummary/t:Counters',$ns).Attributes | ForEach-Object {
  "$($_.Name)=$($_.Value)"
}
```

列出失败用例：

```powershell
$x.SelectNodes('//t:UnitTestResult[@outcome="Failed"]',$ns) | ForEach-Object {
  $messageNode=$_.SelectSingleNode('t:Output/t:ErrorInfo/t:Message',$ns)
  [pscustomobject]@{
    Test=$_.testName
    Message=if ($messageNode) { $messageNode.InnerText } else { '' }
  }
} | Format-List
```

## 9. 测试库清理

Functional 测试会创建很多临时数据库。全量测试完成、测试中断或网络断开后，都建议清理测试库，避免远端数据库空间持续上涨。

以下命令只保留白名单数据库。请把 `<gsql-path>`、`<admin-host>`、`<db-user>`、`<db-password>`、`<db-port>` 替换为本机临时值。

```powershell
$gsql='<gsql-path>'
$adminHost='<admin-host>'
$dbUser='<db-user>'
$dbPassword='<db-password>'
$dbPort='<db-port>'

$keep = @('postgres','template0','template1','mytest','templatea','templatem')
$keepSql = ($keep | ForEach-Object { "'$_'" }) -join ','

$dbs = & $gsql -d postgres -h $adminHost -U $dbUser -p $dbPort -W $dbPassword -At -c "SELECT datname FROM pg_database WHERE datname NOT IN ($keepSql) ORDER BY datname;"
$dbs = @($dbs | Where-Object { $_ -and $_.Trim().Length -gt 0 })

foreach ($db in $dbs) {
  $escaped = $db.Trim().Replace('"','""')
  "DROP DATABASE IF EXISTS `"$escaped`";" | & $gsql -d postgres -h $adminHost -U $dbUser -p $dbPort -W $dbPassword
}

& $gsql -d postgres -h $adminHost -U $dbUser -p $dbPort -W $dbPassword -At -c "SELECT count(*) FROM pg_database WHERE datname NOT IN ($keepSql);"
```

期望最后输出 `0`。

如果某些数据库因为连接未释放无法删除，可以先结束测试进程，再执行清理；必要时使用数据库提供的会话终止命令，但不要把真实会话信息写入文档或提交。

## 10. 常见问题

### 10.1 `FOREIGN KEY ... REFERENCES constraint is not yet supported`

这是分布式库不支持 FK DDL 的错误。分布式测试需要设置：

```powershell
$env:Test__GaussDB__IsDistributed='true'
```

该开关会让测试建表路径移除 FK DDL。集中式测试不应开启该开关。

### 10.2 `Column ... is not a hash distributable data type`

这是分布式表的分布键限制。测试侧会在必要时使用 `DISTRIBUTE BY REPLICATION` 让测试表绕开 hash 分布键限制。

如果仍出现该错误，优先检查：

- 是否使用了最新测试代码。
- 是否设置了 `Test__GaussDB__IsDistributed=true`。
- 失败 SQL 是否来自测试建表路径以外的手写 SQL。

### 10.3 断言顺序不一致

分布式数据库没有稳定的物理返回顺序。遇到 `Skip`、`Take`、`FirstOrDefault`、`Include` 相关断言差异时，先确认查询是否有明确 `OrderBy`。

如果测试语义需要固定结果集，应在分布式分支补充稳定排序，而不是依赖数据库物理顺序。

### 10.4 测试结束后数据库空间上涨

通常是临时测试库残留。执行第 9 节的清理脚本，并确认剩余数量为 `0`。

## 11. 敏感信息规则

不要提交以下内容：

- 真实 IP、域名、账号、密码。
- 完整连接串。
- 个人本机路径或用户名。
- 测试生成的 TRX、log、临时脚本。

推荐只在当前 PowerShell 会话中设置环境变量。测试结束后可以清理：

```powershell
Remove-Item Env:\Test__GaussDB__DefaultConnection -ErrorAction SilentlyContinue
Remove-Item Env:\Test__GaussDB__EnableExtensionSessionParameter -ErrorAction SilentlyContinue
Remove-Item Env:\Test__GaussDB__IsDistributed -ErrorAction SilentlyContinue
```
