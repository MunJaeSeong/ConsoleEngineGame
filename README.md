# ConsoleEngineGame

## 1. 만들 게임
```markdown
콘솔 문자 화면 위에서 돌아가는 무한 웨이브 디펜스 게임입니다.
플레이어가 제한된 자원으로 방어 유닛을 배치해 적의 공격을 막습니다.

자세한 사항은 [docs/design/디펜스-게임기획.md](docs/design/디펜스-게임기획.md)를 확인해주세요.
```

## 2. 문서

| 문서 | 설명 |
|---|---|
| [docs/engine-guide.md](docs/engine-guide.md) | 엔진 v0.2.0 가이드. 루프 순서, API, 좌표계, 프레임 속도, 입력 규칙, 제약 |
| [docs/design/디펜스-게임기획.md](docs/design/디펜스-게임기획.md) | 게임 기획서 및 구체화된 유닛·몬스터 스펙 |
| [docs/game/Implementation.md](docs/game/Implementation.md) | 구현된 조작법과 핵심 게임 흐름 |
| [docs/game/System.md](docs/game/System.md) | 시스템 구현 안내 |


## 3. 실행

```bash
dotnet run --project Program/GridDefense.csproj
```

방향키로 커서를 이동하고, 숫자 키로 유닛을 선택한 뒤 Enter로 배치합니다.
Space로 웨이브를 시작하고, U는 강화, S는 판매를 수행합니다.

## 4. 구현 상태

- 20×20 그리드 맵과 몬스터 이동 경로
- 다이아 500과 시작 HP 50
- 10종의 소환 유닛과 자동 공격
- 1~10단계 확률형 강화와 실패 시 유닛 소멸
- 유닛 판매와 다이아 회수
- 웨이브별 몬스터 수와 체력 스케일링
- 몬스터 처치 보상과 결승점 도달 HP 감소
- 무한 웨이브와 게임 오버 처리