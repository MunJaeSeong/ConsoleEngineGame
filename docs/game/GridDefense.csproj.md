# GridDefense.csproj

`GridDefense.csproj`는 현재 콘솔 게임 프로젝트의 빌드 및 실행 설정을 정의하는 MSBuild 프로젝트 파일입니다.

## 프로젝트 기본 설정

| 항목 | 설정 | 설명 |
|---|---|---|
| SDK | `Microsoft.NET.Sdk` | 일반적인 콘솔 및 라이브러리 프로젝트 빌드 도구 |
| `OutputType` | `Exe` | 실행 파일 형식의 콘솔 프로그램 |
| `TargetFramework` | `net10.0` | .NET 10 런타임을 대상으로 빌드 |
| `ImplicitUsings` | `enable` | 자주 사용하는 네임스페이스를 자동으로 가져옴 |
| `Nullable` | `enable` | null 가능성 관련 컴파일 검사 활성화 |

## ConsoleEngine 프로젝트 참조

```xml
<ProjectReference Include="..\ConsoleEngine\ConsoleEngine.csproj" />
```

현재 프로젝트가 상위 폴더에 있는 `ConsoleEngine` 프로젝트를 참조하도록 설정합니다.

이 참조 덕분에 `GridDefense` 프로젝트에서 다음 형식의 사용이 가능합니다.

```csharp
using ConsoleEngine;
```

주요 사용 클래스는 다음과 같습니다.

- `GameBase`
- `Screen`

## 빌드 구조

```text
ConsoleEngine 프로젝트
  └─ ConsoleEngine.dll
       ↑ 프로젝트 참조
GridDefense 프로젝트
  ├─ Program.cs
  ├─ DefenseGame.cs
  ├─ Enemy.cs
  ├─ GridMap.cs
  └─ 실행 파일 생성
```

## 실행 방법

솔루션 루트에서 다음 명령을 사용할 수 있습니다.

```powershell
dotnet run --project .\Program\GridDefense.csproj
```

## 관련 프로젝트

- `ConsoleEngine/ConsoleEngine.csproj`: 콘솔 게임 공통 엔진 프로젝트
- `Program/GridDefense.csproj`: 타워 디펜스 게임 실행 프로젝트