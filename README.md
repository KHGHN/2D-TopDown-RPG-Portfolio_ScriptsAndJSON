## 이 깃헙은 회사 지원에 필요한 포트폴리오의 스크립트와 JSON파일 공유용입니다.

### * 프로젝트 - 2D-TopDown-RPG-Portfolio  / 유니티 엔진

### * 게임 장르 Top-Down

### * 구현 했는 기능들
>#### 1. 캐릭터 및 몬스터 이동 , 공격, 상태변경, 애니메이터 상태변경, 죽음 처리 등
>>##### PlayerController.cs / MonsterController.cs / BaseController.cs / CameraController.cs

>#### 2. 캐릭터 스텟 및 몬스터 스텟 및 HP Bar 구현
>>##### PlayerStat.cs / Stat.cs / 

>#### 3. 캐릭터 및 몬스터 전투 시 히트박스로 콜라이더 판단 
>>##### PlayerHitBox.cs / MonsterHitBox.cs

>#### 4. 캐릭터 인벤토리 시스템
>>#### 인벤토리 기능 
>>( 아이템 획득, 아이템 사용, 아이템 삭제, 아이템 중첩, 마우스 드래그 앤 드랍으로 아이템 스왑 및 중첩, 아이템 정보 창 출력 )
>>>##### Inventory 폴더 내 cs 파일들

>#### 5. 싱글톤 매니저들을 이용하여 처리
>>##### Managers 폴더 내 cs 파일들

>#### 6. JSON을 이용한 데이터 입출력 기능 테스트
>>##### Data폴더 내 JSON파일 2가지와 Data.Contents.cs / DataManager.cs

>#### 7. StateMachine을 이용하여 상태변경 
>>##### StateMachine.cs

>#### 8. Enum을 이용하여 가독성 처리
>>##### Define.cs

>#### * 웹사이트 검색 및 인터넷 강의를 참고하여 작성한 코드들도 있음
>>##### StateMachine 관련 / Mangers 관련 / 마우스 드래그 앤 드롭 / JSON / 등등
